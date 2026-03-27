
public interface IApiConfig
{
    string API_BASE_URL { get; }
    string API_BEARER { get; }
    string API_SALT_PASSWORD { get; }
    string API_VERSION { get; }
    string DEVICE_PLATFORM { get; }
    string GOOGLE_WEB_CLIENT_ID { get; }
    string WEBSOCKET_URL { get; }
    string API_LANDING_URL { get; }

    void Reload();
}