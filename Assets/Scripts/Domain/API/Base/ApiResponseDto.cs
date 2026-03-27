using Newtonsoft.Json;

public class ApiResponseDto<T>
{
    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("data")]
    public T Data { get; set; }

    [JsonProperty("meta")]
    public MetaData Meta { get; set; } //for pagination, optional

    // --- Metadata Internal (Bukan dari JSON) ---
    [JsonIgnore]
    public int Code { get; set; }

    [JsonIgnore] 
    public bool HandledByGlobalUI { get; private set; }

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
                                 Code == 0;     // network Physical Error
    #endregion

    // Method Factory untuk mempermudah pembuatan Error DTO secara internal
    public static ApiResponseDto<T> CreateError(string msg, int code, bool handled) => new()
    {
        Status = "error",
        Message = msg,
        Code = code,
        HandledByGlobalUI = handled
    };

    public void SetHandledByGlobalUI(bool handled)
    {
        HandledByGlobalUI = handled;
    }
}

#region --- DTO Tambahan untuk API ---

public class MetaData
{
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public int Total { get; set; }
    public int Limit { get; set; }
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

#endregion
