using System.Threading;
using Cysharp.Threading.Tasks;

public interface IUserApiService
{
    UniTask<ApiResponseDto<UserData>> GetUserAsync(CancellationToken ct = default);
    UniTask<ApiResponseDto<UserData>> UpdateUsernameAsync(string username, CancellationToken ct = default);
    UniTask<ApiResponseDto<UserData>> UpdateAvatarAsync(string avatarId, CancellationToken ct = default);
    UniTask<ApiResponseDto<object>> DeactivateUserAsync(string reason, CancellationToken ct = default);
}