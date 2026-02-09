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

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("googleId")]
    public string GoogleId { get; set; }

    [JsonProperty("bankPin")]
    public string BankPin { get; set; }
}