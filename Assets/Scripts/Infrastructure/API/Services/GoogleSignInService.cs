using System;
using System.Threading;
using Cysharp.Threading.Tasks;
//using Firebase.Auth;
//using Google;

public class GoogleSignInService : IGoogleSignInService
{
    private readonly IApiConfig _config;
    private readonly IAuthApiService _authApi;
    private readonly IAuthStore _authStore;
    private readonly INavigationService _navigation;
    private readonly IGlobalUIService _globalUI;
    //private readonly FirebaseAuth _firebaseAuth;

    // Configuration cached
    //private readonly GoogleSignInConfiguration _googleConfiguration;

    private string _referralCode;

    public GoogleSignInService(IApiConfig config, IAuthApiService authApi,
       INavigationService navigation, IGlobalUIService globalUI, IAuthStore authStore)
    {
        _config = config;
        _authApi = authApi;
        _navigation = navigation;
        _authStore = authStore;
        _globalUI = globalUI;

        string webClientId = _config.GOOGLE_WEB_CLIENT_ID;
        LoggerService.LogDebug($"Log Client Id : {webClientId}");

#if !UNITY_EDITOR
        _googleConfiguration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestEmail = true,
            RequestIdToken = true,
            UseGameSignIn = false
        };

        _firebaseAuth = FirebaseAuth.DefaultInstance;
#else
        LoggerService.LogDebug("[Auth] GoogleSignInService initialized in EDITOR MODE (Mock Ready).");
#endif
    }

    /// <summary>
    /// Entry point utama. Mengembalikan UniTask void dipanggil dari UI Button
    /// </summary>
    public async void OnSignInGoogleClicked(string referralCode, CancellationToken ct = default)
    {
        _referralCode = referralCode;
        // Panggil Logic utama dan handle jika ada error yang lolos
        //await SignInProcessAsync(ct);
    }

    /// <summary>
    /// Proses Login Linear: Google -> Firebase -> Backend API
    /// </summary>
    /*private async UniTask SignInProcessAsync(CancellationToken ct)
    {
#if UNITY_EDITOR
        await HandleEditorDummyLogin(ct);
        return;
#endif
        // ---------------------------------------------------------
        // DEVICE (REAL LOGIN)
        // ---------------------------------------------------------

        await UniTask.SwitchToMainThread(cancellationToken: ct);

        _globalUI.ShowLoading("Connecting to Google...");

        try
        {
            // ---------------------------------------------------------
            //  Google Sign-In
            // ---------------------------------------------------------
            GoogleSignIn.Configuration = _googleConfiguration;

            // Konversi Task Google ke UniTask
            GoogleSignInUser googleUser = await GoogleSignIn.DefaultInstance.SignIn()
                .AsUniTask().AttachExternalCancellation(ct);

            if (googleUser == null || string.IsNullOrEmpty(googleUser.IdToken))
            {
                LoggerService.Warning("[Auth] Google Sign-In returned null user or empty token.");
                _globalUI.ShowAlert("Login Failed", "Google authentication failed.");
                return;
            }

            LoggerService.LogDebug($"[Auth] Google User Signed In: {googleUser.DisplayName}");

            // ---------------------------------------------------------
            //  Firebase Authentication
            // ---------------------------------------------------------
            _globalUI.ShowLoading("Authenticating...");

            Credential credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);

            // Login ke Firebase (AsUniTask handling thread switch otomatis)
            var authResult = await _firebaseAuth.SignInWithCredentialAsync(credential)
                .AsUniTask().AttachExternalCancellation(ct);
            FirebaseUser firebaseUser = authResult;

            if (firebaseUser == null)
                throw new Exception("Firebase User is null after sign in.");

            // Ambil Firebase Token (Force Refresh = true)
            string firebaseToken = await firebaseUser.TokenAsync(true)
                .AsUniTask().AttachExternalCancellation(ct);

            LoggerService.LogDebug($"[Auth] Firebase Signed In. User ID: {firebaseUser.UserId}");

            // Backend Verification
            await VerifyWithBackend(firebaseToken);
        }
        catch (GoogleSignIn.SignInException ex)
        {
            // Error spesifik Google
            LoggerService.Error($"[Auth] Google Error: {ex.Status} - {ex.Message}");
            _globalUI.ShowAlert("Google Sign-In Error", $"Status: {ex.Status}");
        }
        catch (OperationCanceledException)
        {
            // User membatalkan login atau menutup aplikasi
            LoggerService.LogDebug("[Auth] Login canceled by user.");
            _globalUI.ShowAlert("Canceled", "Login was canceled.");
        }
        catch (Exception ex)
        {
            // Error umum lainnya
            LoggerService.Exception(ex, "[Auth] Unexpected error during login flow");
            _globalUI.ShowAlert("Error", $"An unexpected error occurred.");
        }
        finally
        {
            // Apapun yang terjadi (Sukses/Gagal/Cancel), Loading harus hilang.
            _globalUI.HideLoading();
        }
    }
    */
#if UNITY_EDITOR
    private async UniTask HandleEditorDummyLogin(CancellationToken ct)
    {
        _globalUI.ShowLoading("Simulating Editor Login...");

        // Simulasi delay network
        await UniTask.Delay(1000, cancellationToken: ct);

        try
        {
            LoggerService.Warning("[Auth] Running in EDITOR MODE. Using Fake/Dev Login.");
            await VerifyWithBackend("devToken3");
        }
        finally
        {
            _globalUI.HideLoading();
        }
    }
#endif

    /// <summary>
    /// Verifikasi token Firebase dengan backend server
    /// </summary>
    /// <param name="firebaseToken"></param>
    /// <returns></returns>
    private async UniTask VerifyWithBackend(string firebaseToken)
    {
        _globalUI.ShowLoading("Verifying with Server...");

        if (string.IsNullOrEmpty(_referralCode))
            _referralCode = null;

        var response = await _authApi.SignInOAuthAsync(firebaseToken, _referralCode);

        if (response.HandledByGlobalUI)
        {
            SafeSignOut();
            return;
        }

        if (response.IsSuccess)
        {
            _authStore.SetSession(response.Data.AccessToken, response.Data.RefreshToken);
            _navigation.NavigateTo(AppRoute.MainMenu).Forget();
        }
        else
        {
            _globalUI.ShowAlert("Login Failed", response.ErrorMessage);
            SafeSignOut();
        }
    }

    /// <summary>
    /// Helper aman untuk Logout Google. 
    /// Mencegah Crash di Editor karena memanggil Native Code.
    /// </summary>
    private void SafeSignOut()
    {
#if !UNITY_EDITOR
        try 
        {
            GoogleSignIn.DefaultInstance?.SignOut();
            _firebaseAuth?.SignOut();
        }
        catch (Exception e) 
        {
            // Telan error saat logout agar tidak mengganggu flow UI
            LoggerService.Warning($"[Auth] Silent SignOut Error: {e.Message}");
        }
#else
        LoggerService.LogDebug("[Auth] Mock SignOut called in Editor.");
#endif
    }
}
