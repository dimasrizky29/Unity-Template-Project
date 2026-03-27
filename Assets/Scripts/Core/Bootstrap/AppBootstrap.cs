using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class AppBootstrap : IInitializable, IDisposable
{
    private readonly IAuthStore _authStore;
    private readonly IAuthApiService _authService;
    private readonly IUserRepository _userRepository;
    private readonly IWebSocketService _webSocketService;
    private readonly IDeepLinkService _deepLinkService;
    private readonly IApiConfig _apiConfig;
    private readonly INavigationService _navigation;
    private readonly IAudioService _audioService;

    private int _reconnectAttempts = 0;
    private const int MAX_RECONNECT_ATTEMPTS = 3;
    private bool isConnected = false;

    [Inject] // VContainer akan menyuntikkan (inject) ketiga komponen ini
    public AppBootstrap(
        IAuthStore authStore,
        IAuthApiService authApiService,
        IUserRepository userRepository,
        IWebSocketService webSocketService,
        IApiConfig apiConfig,
        INavigationService navigation,
        IDeepLinkService deepLinkService,
        IAudioService audioService)
    {
        _authStore = authStore;
        _authService = authApiService;
        _userRepository = userRepository;
        _webSocketService = webSocketService;
        _apiConfig = apiConfig;
        _navigation = navigation;
        _deepLinkService = deepLinkService;
        _audioService = audioService;
    }

    public void Initialize()
    {
        // screen terus menyala
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        _authStore.OnSessionChanged += HandleSessionChanged;
        _webSocketService.OnConnected += HandleConnectionOpen;
        _webSocketService.OnDisconnected += HandleConnectionLost;

        // Ini menangani user yang sebelumnya sudah login (Auto-login)
        BootSequenceAsync().Forget();

        // Play BGM General
        _audioService.PlayBGM("BGMGeneral");

#if UNITY_ANDROID
        if (IsAndroidVersionAtLeast(33))
        {
            RequestNotificationPermission();
        }
#endif
    }

    public void Dispose()
    {
        _authStore.OnSessionChanged -= HandleSessionChanged;
        _webSocketService.OnConnected -= HandleConnectionOpen;
        _webSocketService.OnDisconnected -= HandleConnectionLost;
    }

    /// <summary>
    /// Attempts to refresh the access token using the stored refresh token. (Auto Login)
    /// </summary>
    private async UniTaskVoid BootSequenceAsync()
    {
        LoggerService.LogDebug("Checking for existing session token...");
        
        // deeplink ready
        _deepLinkService.BootstrapIsReady = true;

        //load refresh token from PlayerPrefs for automatic login
        var sessionToken = PlayerPrefs.GetString("auth-token");
        bool autoLoginSuccess = false;

        if (!string.IsNullOrEmpty(sessionToken))
        {
            var response = await _authService.RefreshTokenAsync();
            if (response.IsSuccess)
            {
                // SetSession akan mengubah IsLoggedIn menjadi true
                _authStore.SetSession(response.Data.AccessToken, sessionToken);
                autoLoginSuccess = true;
            }
        }

        // Deeplink Proocess
        var coldStartUrl = _deepLinkService.ColdStartUrl;
        if (!string.IsNullOrEmpty(coldStartUrl))
        {
            // Jika link butuh auth & autoLoginSuccess true -> Langsung masuk
            // Jika link butuh auth & autoLoginSuccess false -> Dicegat Auth Guard & masuk ke _pendingState
            _deepLinkService.ProcessDeepLink(coldStartUrl);
            return;
        }

        // JIKA TIDAK ADA DEEP LINK, BARU TENTUKAN SCENE AWAL
        if (autoLoginSuccess)
            _navigation.NavigateTo(AppRoute.MainMenu).Forget();
        else
            _navigation.NavigateTo(AppRoute.Login).Forget();
    }

    private void HandleSessionChanged(UserSession session)
    {
        if (session.IsAuthenticated)
        {
            // --- PENANGANAN USER BARU LOGIN ---
            // Jika event mendeteksi login sukses, jalankan alur persiapan data & socket
            // Kita gunakan .Forget() karena ini adalah Fire-and-Forget dari event
            SetupSessionAsync().Forget();
        }
        else
        {
            // --- PENANGANAN LOGOUT ---
            _webSocketService.Disconnect();

            // Play BGM General
            _audioService.PlayBGM("BGMGeneral");
        }
    }

    private void HandleConnectionOpen()
    {
        LoggerService.LogDebug("[Bootstrap] Connection established successfully.");
        _reconnectAttempts = 0; // Reset counter saat koneksi berhasil
        isConnected = true;
    }

    private void HandleConnectionLost()
    {
        LoggerService.LogDebug("[Bootstrap] Connection lost. Attempting to refresh token...");
        isConnected = false;
        // Gunakan UniTaskVoid untuk Fire-and-Forget dari event
        ReconnectFlow().Forget();
    }

    private async UniTask SetupSessionAsync()
    {
        try
        {
            // 1. Pastikan Data User ada (Ambil dari API jika belum ada)
            await EnsureUserDataLoadedAsync();

            // 2. Konek WebSocket
            ConnectWebSocket();
        }
        catch (Exception ex)
        {
            LoggerService.Exception(ex, "[Bootstrap] Failed to setup session after login");
        }
    }

    private async UniTask EnsureUserDataLoadedAsync()
    {
        LoggerService.LogDebug("[Bootstrap] Fetching user data for the first time...");
        await _userRepository.GetUserData();
    }

    private void ConnectWebSocket()
    {
        if (!_authStore.IsLoggedIn)
        {
            LoggerService.Warning("[Bootstrap] Cannot connect: User is not logged in.");
            return;
        }

        var session = _authStore.Session;
        var userData = _userRepository.CurrentUser;
        if (userData == null)
        {
            LoggerService.Warning("[Bootstrap] User data is null. Cannot connect to WebSocket.");
            return;
        }
        string uid = userData.Uid.ToString();
        string accessToken = session.AccessToken;
        string refreshToken = session.RefreshToken;
        _webSocketService.ConnectServer(_apiConfig.WEBSOCKET_URL, uid, accessToken, refreshToken).Forget();
    }

    private async UniTask ReconnectFlow()
    {
        // Jika sudah mencapai batas, jangan coba lagi, lempar ke Login
        if (_reconnectAttempts >= MAX_RECONNECT_ATTEMPTS)
        {
            LoggerService.Warning("[Bootstrap] Max reconnect reached. Clearing session.");
            _authStore.ClearSession();
            return;
        }

        _reconnectAttempts++;
        LoggerService.LogDebug($"[Bootstrap] Connection lost. Attempt {_reconnectAttempts}...");

        // Jeda sebelum mencoba (Simple exponential backoff)
        await UniTask.Delay(TimeSpan.FromSeconds(6 * _reconnectAttempts));

        if(isConnected) return;

        try
        {
            var result = await _authService.RefreshTokenAsync();

            if (result.HandledByGlobalUI)
                return;

            if (result.IsSuccess)
            {
                // Update token di store
                _authStore.SetSession(result.Data.AccessToken, _authStore.Session.RefreshToken);
            }
            else
            {
                LoggerService.Warning("[Bootstrap] Refresh token failed. Clear the session");
                _authStore.ClearSession();
            }
        }
        catch (Exception ex)
        {
            LoggerService.Exception(ex, "[Bootstrap] Exception during reconnect flow");
            _authStore.ClearSession();
        }
    }

    #region Permission
    private void RequestNotificationPermission()
    {
        using AndroidJavaObject activity = GetUnityActivity();
        string permission = "android.permission.POST_NOTIFICATIONS";
        int permissionStatus = activity.Call<int>("checkSelfPermission", permission);

        if (permissionStatus != 0) // PERMISSION_GRANTED != 0
        {
            activity.Call("requestPermissions", new string[] { permission }, 0);
        }
        else
        {
            Debug.Log("Notification permission already granted.");
        }
    }

    private bool IsAndroidVersionAtLeast(int apiLevel)
    {
        using AndroidJavaClass versionClass = new("android.os.Build$VERSION");
        int sdkInt = versionClass.GetStatic<int>("SDK_INT");
        return sdkInt >= apiLevel;
    }

    private AndroidJavaObject GetUnityActivity()
    {
        using AndroidJavaClass unityPlayer = new("com.unity3d.player.UnityPlayer");
        return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    }
    #endregion
}