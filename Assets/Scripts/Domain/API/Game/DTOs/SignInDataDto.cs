using System;
using Newtonsoft.Json;

public class SignInDataDto
{
    [JsonProperty("uid")]
    public Guid? Uid { get; set; }

    [JsonProperty("gameId")]
    public string GameId { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("googleId")]
    public string GoogleId { get; set; }

    [JsonProperty("bankPin")]
    public string BankPin { get; set; }

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    // Tokens returned alongside user info
    [JsonProperty("accessToken")]
    public string AccessToken { get; set; }

    [JsonProperty("refreshToken")]
    public string RefreshToken { get; set; }
}