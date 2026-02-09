using System.Threading;
using Cysharp.Threading.Tasks;

public interface IAuthApiService
{
    UniTask<ApiResponseDto<SignInDataDto>> LoginAsync(string email, string password, CancellationToken ct = default);
    UniTask<ApiResponseDto<RefreshTokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    UniTask<ApiResponseDto<SignUpDataDto>> RegisterAsync(string email, string password, string referralCode, CancellationToken ct = default);
}