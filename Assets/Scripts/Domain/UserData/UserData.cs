using System;
using Newtonsoft.Json;

public class UserData
{
    [JsonProperty("uid")]
    public Guid? Uid { get; set; }

    [JsonProperty("gameId")]
    public string GameId { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("refCode")]
    public string RefCode { get; set; }

    [JsonProperty("avatarId")]
    public string AvatarId { get; set; }

    [JsonProperty("mail")]
    public Mail MailData { get; set; }

    [JsonProperty("balance")]
    public Balance BalanceShop { get; set; }

    [JsonProperty("balanceEarn")]
    public BalanceEarn BalanceGame { get; set; }

    public class Mail
    {
        [JsonProperty("haveUnreadInbox")]
        public bool HaveUnreadInbox { get; set; }

        [JsonProperty("unreadCount")]
        public int UnreadCount { get; set; }
    }

    public class Balance
    {
        [JsonProperty("currentBalance")]
        public double CurrentBalance { get; set; }
    }

    public class BalanceEarn
    {
        [JsonProperty("currentBalance")]
        public double CurrentBalance { get; set; }
    }
}

