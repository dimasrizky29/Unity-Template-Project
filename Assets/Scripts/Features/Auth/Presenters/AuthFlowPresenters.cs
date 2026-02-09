using System;
using System.Threading;
using Cysharp.Threading.Tasks;

/// <summary>
/// 
/// </summary>
public class AuthFlowPresenters : IDisposable
{
    private readonly INavigationService _navigation;
    private readonly IAuthApiService _authApiService;
    private readonly IAuthStore _authStore;
    public IGlobalUIService _globalUIService;
    private readonly CancellationTokenSource _cts;

    // VContainer will automatically inject these
    public AuthFlowPresenters(IAuthApiService authApiService, IAuthStore authStore, INavigationService navigation, IGlobalUIService globalUIService)
    {
        _authApiService = authApiService;
        _authStore = authStore;
        _navigation = navigation;
        _globalUIService = globalUIService;

        _cts = new CancellationTokenSource();
    }

    void IDisposable.Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    #region Public Routes and Actions
    // --- PUBLIC ROUTES ---
    public void GoToLogin() => _navigation.NavigateTo(AppRoute.Login).Forget();
    public void GoToRegister() => _navigation.NavigateTo(AppRoute.Register).Forget();

    public async void OnLoginButtonClicked(string email, string password)
    {
        // Show loading indicator
        _globalUIService.ShowLoading("Logging in...");

        var response = await _authApiService.LoginAsync(email, password, ct: _cts.Token);

        // Hide loading indicator
        _globalUIService.HideLoading();

        if (response.HandledByGlobalUI)
            return;

        if (response.IsSuccess) // Successful login
        {
            _authStore.SetSession(response.Data.AccessToken, response.Data.RefreshToken);
            _navigation.NavigateTo(AppRoute.Profile).Forget();
        }
        else
        {
            LoggerService.Warning($"Login Failed: {response.ErrorMessage}");
            _globalUIService.ShowAlert("Login Failed", response.ErrorMessage);
        }
    }

    public async void OnRegisterButtonClicked(string email, string password, string referralCode)
    {
        // Show loading indicator
        _globalUIService.ShowLoading("Registering...");

        var response = await _authApiService.RegisterAsync(email, password, referralCode, ct: _cts.Token);
        
        // Hide loading indicator
        _globalUIService.HideLoading();

        if (response.HandledByGlobalUI)
            return;

        if (response.IsSuccess) // Successful login
        {
            _globalUIService.ShowAlert("Registration successful", " Redirecting...");

            // Small delay to show success message
            await UniTask.Delay(1000, cancellationToken: _cts.Token);

            _navigation.NavigateTo(AppRoute.Login).Forget();

            _globalUIService.HideAlert();
        }
        else
        {
            LoggerService.Warning($"Registration Failed: {response.ErrorMessage}");
            _globalUIService.ShowAlert("Registration Failed", response.ErrorMessage);
        }
    }
    #endregion

    #region Private Routes and Actions

    // --- PRIVATE ROUTES ---
    public void GoToProfile() => ExecuteAuthenticatedAction(AppRoute.Profile);

    /// <summary>
    /// Wrapper for authenticated navigation or actions that handles token refresh logic.
    /// </summary>
    private async void ExecuteAuthenticatedAction(AppRoute targetRoute)
    {
        // 1. Check if token is locally expired before even trying
        if (_authStore.IsTokenExpired())
        {
            bool refreshed = await TryRefreshToken();
            if (!refreshed) return;
        }

        // In a real scenario, you'd perform your API call here. 
        // We simulate a request to check for "Access token expired" or 440 errors.
        await _navigation.NavigateTo(targetRoute);
    }

    /// <summary>
    /// Attempts to refresh the access token using the stored refresh token.
    /// </summary>
    private async UniTask<bool> TryRefreshToken()
    {
        var session = _authStore.Session;
        if (string.IsNullOrEmpty(session.RefreshToken))
        {
            Logout();
            return false;
        }

        // High-performance call to refresh endpoint
        var response = await _authApiService.RefreshTokenAsync(session.RefreshToken, ct: _cts.Token);

        if (response.HandledByGlobalUI)
            return false;

        if ( response.IsSuccess)
        {
            _authStore.SetSession(response.Data.AccessToken, session.RefreshToken);
            return true;
        }
        else
        {
            LoggerService.Warning("Session expired permanently. Logging out.");
            _globalUIService.ShowAlert("Session Expired", "Please log in again.");
            Logout();
        }

        return false;
    }

    public void Logout()
    {
        _authStore.ClearSession();
        // AppRouter automatically handles the redirect because it listens to AuthStore.OnSessionChanged
    }
    #endregion

    #region Reference Implementations

    public AppRoute GetCurrentRoute()
    {
        LoggerService.Info($"Current Route: {_navigation.CurrentRoute}");
        return _navigation.CurrentRoute;
    }

    public bool IsLoggedIn()
    {
        LoggerService.Info($"Is Logged In: {_authStore.IsLoggedIn}");
        return _authStore.IsLoggedIn;
    }

    #endregion

}
