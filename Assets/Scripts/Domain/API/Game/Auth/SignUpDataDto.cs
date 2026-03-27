using System;
using Newtonsoft.Json;

public class SignUpDataDto
{
    [JsonProperty("uid")]
    public Guid? Uid { get; set; }

    [JsonProperty("googleId")]
    public string GoogleId { get; set; }

    [JsonProperty("appleId")]
    public string AppleId { get; set; }

    [JsonProperty("emailVerifiedAt")]
    public DateTime? EmailVerifiedAt { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("deactivatedAt")]
    public DateTime? DeactivatedAt { get; set; }

    [JsonProperty("bankPin")]
    public string BankPin { get; set; }

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("gameId")]
    public string GameId { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("referrer")]
    public string Referrer { get; set; }

    [JsonProperty("refCode")]
    public string RefCode { get; set; }
}