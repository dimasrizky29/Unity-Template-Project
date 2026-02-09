using System.Threading;
using Cysharp.Threading.Tasks;

public interface IGameApiService
{
    UniTask<ApiResponseDto<T>> GetAsync<T>(string url, CancellationToken ct = default);
    UniTask<ApiResponseDto<T>> PostAsync<T>(string url, object data, CancellationToken ct = default);
    UniTask<ApiResponseDto<T>> PutAsync<T>(string url, object data, CancellationToken ct = default);
    UniTask<bool> DeleteAsync(string url, CancellationToken ct = default);
}