using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public interface IUserRepository
{
    // --- User Data ---
    UserData CurrentUser { get; }

    UniTask<ApiResponseDto<object>> DeactivateUser(string reason, CancellationToken ct = default);
    UniTask<UserData> GetUserData(bool forceRefresh = false, CancellationToken ct = default);
    UniTask<UserData> UpdateAvatar(string avatarId, CancellationToken ct = default);
    UniTask<UserData> UpdateUsername(string username, CancellationToken ct = default);
    void UpdateCachedUser(Action<UserData> updateAction);

    event Action<UserData> OnUserDataUpdated;

    // --- User Place Global ---

    UserPlaceData CurrentUserPlace { get; }
    void SetCachedUserPlace(UserPlaceData userPlace);

    void UpdateCachedUserPlace(Action<UserPlaceData> updateAction);
}