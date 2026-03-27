using System;
using Newtonsoft.Json;

public class SignInDataDto
{
    // Tokens returned alongside user info
    [JsonProperty("accessToken")]
    public string AccessToken { get; set; }

    [JsonProperty("refreshToken")]
    public string RefreshToken { get; set; }
}