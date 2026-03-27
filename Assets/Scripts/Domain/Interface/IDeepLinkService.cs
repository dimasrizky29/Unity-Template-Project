public interface IDeepLinkService
{
    string ColdStartUrl { get; }
    bool BootstrapIsReady { get; set; }

    void ProcessDeepLink(string url);
}