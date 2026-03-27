using System.Threading;
using Cysharp.Threading.Tasks;

public interface IAuthApiService
{
    UniTask<ApiResponseDto<SignInDataDto>> LoginAsync(string email, string password, CancellationToken ct = default);
    UniTask<ApiResponseDto<SignUpDataDto>> RegisterAsync(string email, string password, string referralCode, CancellationToken ct = default);
    UniTask<ApiResponseDto<SignInDataDto>> SignInOAuthAsync(string oauth, string referralCode = null, CancellationToken ct = default);
    UniTask<ApiResponseDto<object>> SignOutAsync(CancellationToken ct = default);
    UniTask<ApiResponseDto<RefreshTokenDto>> RefreshTokenAsync(string refreshToken = null, CancellationToken ct = default);
}