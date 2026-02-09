using System.Threading;
using Cysharp.Threading.Tasks;

/// <summary>
/// High-performance, modular Auth API service.
/// Provides clean, reusable methods for authentication-related API operations.
/// Uses UniTask for zero-allocation async operations.
/// </summary>
public class AuthApiService : IAuthApiService
{
    // Dependencies (Inject lewat Constructor)
    private readonly IGameApiService _apiService;
    private readonly IApiConfig _config;

    // VContainer akan otomatis mengisi ini
    public AuthApiService(IGameApiService apiService, IApiConfig config)
    {
        _apiService = apiService;
        _config = config;
    }

    /// <summary>
    /// Logs in a user with email and password.
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="password">User password</param>
    /// <param name="baseUrl">Optional base URL (uses ApiConfig if not provided)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>LoginResponse if successful, null otherwise</returns>
    public async UniTask<ApiResponseDto<SignInDataDto>> LoginAsync(
        string email,
        string password,
        CancellationToken ct = default)
    {
        string passwordSalt = password + _config.API_SALT_PASSWORD;
        var loginData = new { email, passwordSalt };
        string url = $"{_config.API_BASE_URL}/auth/signin/email";
        return await _apiService.PostAsync<SignInDataDto>(url, loginData, ct);
    }

    /// <summary>
    /// Registers a new user with email and password.
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="password">User password</param>
    /// <param name="baseUrl">Optional base URL (uses ApiConfig if not provided)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>LoginResponse if successful, null otherwise</returns>
    public async UniTask<ApiResponseDto<SignUpDataDto>> RegisterAsync(
        string email,
        string password, string referralCode,
        CancellationToken ct = default)
    {
        var registerData = new { email, password, referralCode };
        string url = $"{_config.API_BASE_URL}/auth/sign-up";
        return await _apiService.PostAsync<SignUpDataDto>(url, registerData, ct);
    }

    /// <summary>
    /// Refreshes the access token using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to use</param>
    /// <param name="baseUrl">Optional base URL (uses ApiConfig if not provided)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>LoginResponse with new tokens if successful, null otherwise</returns>
    public async UniTask<ApiResponseDto<RefreshTokenDto>> RefreshTokenAsync(
        string refreshToken,
        CancellationToken ct = default)
    {
        var refreshData = new { refreshToken };
        string url = $"{_config.API_BASE_URL}/refresh";
        return await _apiService.PostAsync<RefreshTokenDto>(url, refreshData, ct);
    }
}
