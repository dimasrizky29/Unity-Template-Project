using Newtonsoft.Json;

public class ApiResponseDto<T>
{
    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("data")]
    public T Data { get; set; }

    // --- Metadata Internal (Bukan dari JSON) ---
    [JsonIgnore]
    public int Code { get; set; }

    [JsonIgnore] 
    public bool HandledByGlobalUI { get; internal set; }

    #region Helpers

    [JsonIgnore]
    public bool IsSuccess => !string.IsNullOrEmpty(Status) && Status.ToLower() == "success" || Code == 200;

    [JsonIgnore]
    public string ErrorMessage => !string.IsNullOrEmpty(Message) ? Message : "An unknown error has occurred.";

    [JsonIgnore]
    public bool IsGlobalError => Code == 426 || // Force Update
                                 Code == 503 || // Maintenance
                                 Code == 502 || // Bad Gateway
                                 Code == 504 || // Gateway Timeout
                                 Code == 0;     // Network Physical Error
    #endregion

    // Method Factory untuk mempermudah pembuatan Error DTO secara internal
    internal static ApiResponseDto<T> CreateError(string msg, int code, bool handled) => new()
    {
        Status = "error",
        Message = msg,
        Code = code,
        HandledByGlobalUI = handled
    };
}
