using System;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using VContainer.Unity;

public class WebSocketService : IWebSocketService, ITickable, IDisposable
{
    public event Action<string> OnMessageReceived;
    public event Action OnConnected;
    public event Action OnDisconnected;

    private WebSocket ws;

    public void Tick() // Pengganti Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        ws?.DispatchMessageQueue();
#endif
    }

    public async UniTask ConnectServer(string url, string userUid, string accessToken, string refreshToken)
    {
        await CleanUpSocket();

        string uriFull = $"{url}?userUid={userUid}&accessToken={accessToken}&refreshToken={refreshToken}";
        LoggerService.LogDebug($"[WebSocketService] Connecting to {uriFull}");

        ws = new WebSocket(uriFull);

        ws.OnOpen += () =>
        {
            LoggerService.LogDebug("[WebSocketService] Connection opened!");
            OnConnected?.Invoke();
        };

        ws.OnError += (e) =>
        {
            LoggerService.Error($"[WebSocketService] Error: {e}");
        };

        ws.OnClose += (e) =>
        {
            LoggerService.LogDebug($"[WebSocketService] Connection closed with code {e}");
            OnDisconnected?.Invoke();
        };

        ws.OnMessage += (bytes) =>
        {
            LoggerService.LogDebug($"[WebSocketService] Msg : {bytes.Length} bytes");

            var message = System.Text.Encoding.UTF8.GetString(bytes);
            OnMessageReceived?.Invoke(message); // Teruskan pesan ke Presenter
        };

        await ws.Connect();
    }

    public async void SendMessage(string message)
    {
        if (ws != null && ws.State == WebSocketState.Open)
        {
            await ws.SendText(message);
        }
    }

    private async UniTask CleanUpSocket()
    {
        if (ws == null) return;

        // Hapus semua listener agar tidak terjadi double-trigger saat reconnect
        try
        {
            await ws.Close();
            ws = null;
        }
        catch (Exception) { /* ignore */ }
    }

    public void Disconnect()
    {
        _ = CleanUpSocket();
    }

    public void Dispose()
    {
        _ = CleanUpSocket();
    }
}