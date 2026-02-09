using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public class UserRepository
{
    // Data disimpan di sini (In-Memory)
    private UserData _cachedUser;

    private readonly IUserApiService _userApiService;
    public UserData CurrentUser => _cachedUser;

    public UserRepository(IUserApiService userApiService)
    {
        _userApiService = userApiService;
    }

    public async UniTask<UserData> GetUserData(bool forceRefresh = false, CancellationToken ct = default)
    {
        // Jika data sudah ada di gudang dan tidak minta refresh, berikan yang ada
        if (_cachedUser != null && !forceRefresh)
            return _cachedUser;

        // Jika belum ada, ambil dari API Service
        var result = await _userApiService.GetUserAsync(ct);
        if (result.IsSuccess)
        {
            _cachedUser = result.Data;
            return _cachedUser;
        }

        // Jika gagal, kamu bisa melempar Exception atau menggunakan pola Result
        throw new Exception(result.ErrorMessage ?? "Failed to fetch user data");
    }

    public async UniTask<UserData> UpdateUsername(string username, CancellationToken ct = default)
    {
        var result = await _userApiService.UpdateUsernameAsync(username, ct);
        if (result.IsSuccess)
        {
            _cachedUser.Username = username;
            return _cachedUser;
        }

        // Jika gagal, kamu bisa melempar Exception atau menggunakan pola Result
        throw new Exception(result.ErrorMessage ?? "Failed to update username");
    }

    public async UniTask<UserData> UpdateAvatar(string avatarId, CancellationToken ct = default)
    {
        var result = await _userApiService.UpdateAvatarAsync(avatarId, ct);
        if (result.IsSuccess)
        {
            _cachedUser.AvatarId = avatarId;
            return _cachedUser;
        }

        // Jika gagal, kamu bisa melempar Exception atau menggunakan pola Result
        throw new Exception(result.ErrorMessage ?? "Failed to update avatar");
    }

    public async UniTask<ApiResponseDto<object>> DeactivateUser(string reason, CancellationToken ct = default)
    {
        var result = await _userApiService.DeactivateUserAsync(reason, ct);

        // 2. Jika sukses di server, bersihkan cache lokal
        if (result.IsSuccess)
        {
            _cachedUser = null; // Data dihapus dari "Gudang"
            LoggerService.Info("User session cleared from repository cache.");
        }

        return result;
    }


}