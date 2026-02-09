using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace Infrastructure.Networking
{
    public interface IHttpClient
    {
        UniTask<string> SendAsync(UnityWebRequest request, CancellationToken ct);
    }

    public class BaseHttpClient : IHttpClient
    {
        public async UniTask<string> SendAsync(UnityWebRequest request, CancellationToken ct)
        {
            try
            {
                await request.SendWebRequest().WithCancellation(ct);

                // Jika sukses atau error protocol (401, 400), kita tetap ambil teksnya
                // agar bisa diparsing JSON error-nya di layer atas.
                return request.downloadHandler.text;
            }
            catch (UnityWebRequestException)
            {
                // Jika error koneksi (RTO, No Internet, DNS)
                if (request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.DataProcessingError)
                {
                    throw new Exception("NETWORK_ERROR");
                }

                // Jika errornya 4xx/5xx, UniTask melempar exception, 
                // tapi kita tetap butuh body JSON-nya.
                return request.downloadHandler?.text;
            }
            catch (OperationCanceledException)
            {
                throw; // Biarkan cancel berjalan normal
            }
            catch (Exception)
            {
                throw; // Error lain(coding error dll)
            }
        }
    }
}
