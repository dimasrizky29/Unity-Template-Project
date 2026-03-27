using System;
using Newtonsoft.Json;
using VContainer.Unity;

public class SocketMessage
{
    public string status; // code
    public string type; // contoh: "update_balance", "new_chat"
    public object data; // JSON string di dalam string
}

public class SocketMessageDispatcher : IInitializable, IDisposable
{
    private readonly IWebSocketService _socketService;
    private readonly IUserRepository _userRepository;
    private readonly IGlobalUIService _globalUIService;

    public SocketMessageDispatcher(
        IWebSocketService socketService,
        IUserRepository userRepository,
        IGlobalUIService globalUIService)
    {
        _socketService = socketService;
        _userRepository = userRepository;
        _globalUIService = globalUIService;
    }

    public void Initialize()
    {
        _socketService.OnMessageReceived += HandleMessage;
    }

    public void Dispose()
    {
        _socketService.OnMessageReceived -= HandleMessage;
    }

    private void HandleMessage(string rawMessage)
    {
        LoggerService.LogDebug($"Handle Msg : {rawMessage}");

        // 1. Parsing awal untuk tahu tipe pesannya
        var envelope = JsonConvert.DeserializeObject<SocketMessage>(rawMessage);

        // 2. Distribusikan ke Service/Repository yang tepat
        switch (envelope.type)
        {
            case "deposit":
                var balanceData = JsonConvert.DeserializeObject<Balance>(envelope.data.ToString());
                _userRepository.UpdateCachedUser(
                    (data) => data.BalanceShop.CurrentBalance = balanceData.CurrentBalance );

                //_globalUIService.ShowDepositSuccess(true);
                break;
        }
    }
}