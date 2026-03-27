using Newtonsoft.Json;

public class RefreshTokenDto
{
    [JsonProperty("accessToken")]
    public string AccessToken { get; set; }
}