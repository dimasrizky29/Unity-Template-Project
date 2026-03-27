using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public class UserRepository : IUserRepository
{
    #region User Data 
    // Data disimpan di sini (In-Memory)
    private UserData _cachedUser;
    private UserData ChachedUser
    {
        get => _cachedUser;
        set
        {
            _cachedUser = value;
            OnUserDataUpdated?.Invoke(_cachedUser); // Beritahu yang subscribe kalau data berubah
        }
    }

    private readonly IUserApiService _userApiService;

    public event Action<UserData> OnUserDataUpdated;

    public UserData CurrentUser => _cachedUser;

    public UserRepository(IUserApiService userApiService)
    {
        _userApiService = userApiService;
    }

    public async UniTask<UserData> GetUserData(bool forceRefresh = false, CancellationToken ct = default)
    {
        // Jika data sudah ada di gudang dan tidak minta refresh, berikan yang ada
        if (ChachedUser != null && !forceRefresh)
            return ChachedUser;

        // Jika belum ada, ambil dari API Service
        var result = await _userApiService.GetUserAsync(ct);
        if (result.IsSuccess)
        {
            ChachedUser = result.Data;
            return ChachedUser;
        }

        // Jika gagal, kamu bisa melempar Exception atau menggunakan pola Result
        throw new Exception(result.ErrorMessage ?? "Failed to fetch user data");
    }

    public async UniTask<UserData> UpdateUsername(string username, CancellationToken ct = default)
    {
        var result = await _userApiService.UpdateUsernameAsync(username, ct);
        if (result.IsSuccess)
        {
            ChachedUser.Username = username;
            ForceRefreshUI();
            return ChachedUser;
        }

        // Jika gagal, kamu bisa melempar Exception atau menggunakan pola Result
        throw new Exception(result.ErrorMessage ?? "Failed to update username");
    }

    public async UniTask<UserData> UpdateAvatar(string avatarId, CancellationToken ct = default)
    {
        var result = await _userApiService.UpdateAvatarAsync(avatarId, ct);
        if (result.IsSuccess)
        {
            ChachedUser.AvatarId = avatarId;
            ForceRefreshUI();
            return ChachedUser;
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
            ChachedUser = null; // Data dihapus dari "Gudang"
            LoggerService.Info("User session cleared from repository cache.");
        }

        return result;
    }

    public void UpdateCachedUser(Action<UserData> updateAction)
    {
        if (_cachedUser == null)
            return;

        updateAction.Invoke(_cachedUser);

        ForceRefreshUI(); // Trigger event untuk update UI

        LoggerService.LogDebug("Cached user data updated and UI refresh triggered.");
    }

    private void ForceRefreshUI()
    {
        OnUserDataUpdated?.Invoke(_cachedUser);
    }
    #endregion

    #region User Place Data

    // Data disimpan di sini (In-Memory) global untuk tempat yang sedang aktif
    private UserPlaceData _cachedPlaceData;
    public UserPlaceData CurrentUserPlace => _cachedPlaceData;

    public void SetCachedUserPlace(UserPlaceData placeData)
    {
        if (placeData == null)
        {
            LoggerService.LogDebug("Received null place data. Cache will not be updated.");
            return;
        }

        _cachedPlaceData = placeData;
        LoggerService.LogDebug(_cachedPlaceData is null ? "null" : _cachedPlaceData.Id);
        LoggerService.LogDebug("Cached place data set.");
    }

    public void UpdateCachedUserPlace(Action<UserPlaceData> updatePlace)
    {
        updatePlace.Invoke(_cachedPlaceData);
        LoggerService.LogDebug("Cached place data updated.");
    }
    #endregion
}