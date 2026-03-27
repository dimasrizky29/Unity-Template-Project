using System;
using Cysharp.Threading.Tasks;

public interface IWebSocketService
{
    event Action OnConnected;
    event Action OnDisconnected;
    event Action<string> OnMessageReceived;

    UniTask ConnectServer(string url, string userUid, string accessToken, string refreshToken);
    void Disconnect();
    void SendMessage(string message);
}