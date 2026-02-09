using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

public class GameInitializer : IStartable
{
    private readonly INavigationService _navigation;
    private readonly IAuthApiService _authService;
    private readonly IAuthStore _authStore;

    // Constructor Injection bekerja di sini!
    public GameInitializer(INavigationService navigation, IAuthApiService authService, IAuthStore authStore)
    {
        _navigation = navigation;
        _authService = authService;
        _authStore = authStore;
    }

    public void Start()
    {
        StartCheckToken().Forget();
    }

    /// <summary>
    /// Attempts to refresh the access token using the stored refresh token.
    /// </summary>
    private async UniTaskVoid StartCheckToken()
    {
        LoggerService.LogDebug("Checking for existing session token...");
        await UniTask.WaitUntil(() => _navigation != null && _authStore != null);

        //load refresh token from PlayerPrefs for automatic login
        var sessionToken = PlayerPrefs.GetString("auth-token");
        if (string.IsNullOrEmpty(sessionToken))
        {
            _navigation.NavigateTo(AppRoute.Login).Forget();
            return;
        }

        // High-performance call to refresh endpoint
        var response = await _authService.RefreshTokenAsync(sessionToken);

        if (response.HandledByGlobalUI)
            return;

        if (response.IsSuccess)
        {
            _authStore.SetSession(response.Data.AccessToken, sessionToken);
            _navigation.NavigateTo(AppRoute.Lobby).Forget();
            return;
        }
        else
        {
            LoggerService.Warning("Session expired permanently.");
            _navigation.NavigateTo(AppRoute.Login).Forget();
        }
    }
}
