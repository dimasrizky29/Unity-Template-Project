using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;

/// <summary>
/// High-performance, modular User API service.
/// Provides clean, reusable methods for user-related API operations.
/// Uses UniTask for zero-allocation async operations.
/// </summary>
public class UserApiService : IUserApiService
{
    // Dependencies (Inject lewat Constructor)
    private readonly IGameApiService _apiService;
    private readonly IApiConfig _config;

    // VContainer akan otomatis mengisi ini
    public UserApiService(IGameApiService apiService, IApiConfig config)
    {
        _apiService = apiService;
        _config = config;
    }

    /// <summary>
    /// Gets a user by ID from the API.
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>UserData if successful, null otherwise</returns>
    public async UniTask<ApiResponseDto<UserData>> GetUserAsync(
        CancellationToken ct = default)
    {
        string url = $"{_config.API_BASE_URL}/users";
        return await _apiService.GetAsync<UserData>(url, ct);
    }

    public async UniTask<ApiResponseDto<UserData>> UpdateUsernameAsync(
        string username,
        CancellationToken ct = default)
    {
        var data = new { username };
        string url = $"{_config.API_BASE_URL}/users";
        return await _apiService.PostAsync<UserData>(url, data, ct);
    }

    public async UniTask<ApiResponseDto<UserData>> UpdateAvatarAsync(
        string avatarId,
        CancellationToken ct = default)
    {
        var data = new { avatarId };
        string url = $"{_config.API_BASE_URL}/users";
        return await _apiService.PostAsync<UserData>(url, data, ct);
    }

    public async UniTask<ApiResponseDto<object>> DeactivateUserAsync(
        string reason,
        CancellationToken ct = default)
    {
        var data = new { reason };
        string url = $"{_config.API_BASE_URL}/users";
        return await _apiService.PostAsync<object>(url, data, ct);
    }
}
