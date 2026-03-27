using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

public class DeepLinkService : IDeepLinkService, IInitializable, IDisposable
{
    private readonly INavigationService _navigation;
    private readonly IAuthStore _authStore;

    private string _lastProcessedUrl;
    // Simpan link cold-start jika aplikasi masih booting
    public string ColdStartUrl { get; private set; }
    public bool BootstrapIsReady { get; set; }

    public DeepLinkService(INavigationService navigation, IAuthStore authStore)
    {
        _navigation = navigation;
        _authStore = authStore;
    }

    public void Initialize()
    {
        // Subscribe untuk Hot Start (Aplikasi sedang berjalan di background)
        Application.deepLinkActivated += OnDeepLinkActivated;
        Debug.Log("DeepLinkService initialized");
        // Tangkap Cold Start (Aplikasi dibuka dari link)
        if (!string.IsNullOrEmpty(Application.absoluteURL))
        {
            ColdStartUrl = Application.absoluteURL;
            LoggerService.LogDebug($"[DeepLink] Cold Start URL captured: {ColdStartUrl}");
        }
    }

    private void OnDeepLinkActivated(string url)
    {
        // Jika aplikasi sudah "Ready" (sudah lewat bootstrap), langsung proses.
        // Jika belum, biarkan Bootstrap yang memanggilnya nanti.
        if (BootstrapIsReady)
        {
            ProcessDeepLink(url);
        }
    }

    public void Dispose()
    {
        Application.deepLinkActivated -= OnDeepLinkActivated;
    }

    // Fungsi ini bisa dipanggil otomatis (Hot Start) atau dipanggil oleh AppBootstrap (Cold Start)
    public void ProcessDeepLink(string url)
    {
        if (string.IsNullOrEmpty(url) || url == _lastProcessedUrl) return;

        _lastProcessedUrl = url; // Cegah pemrosesan ganda untuk link yang sama

        LoggerService.LogDebug($"[DeepLink] Processing: {url}");

        try
        {
            Uri uri = new(url);
            string path = uri.AbsolutePath; // example: "/register"
            string query = uri.Query;       // example: "?ref=1234"

            if (path.Contains("/register"))
            {
                string refCode = ExtractQueryParam(query, "ref");

                // --- ROUTING ---
                // Kirim refCode sebagai Payload

                if (_authStore.IsLoggedIn)
                {
                    LoggerService.Warning("[DeepLink] User already logged in, ignoring register link.");
                    _navigation.NavigateTo(AppRoute.MainMenu).Forget();
                    return; // Atau bawa ke menu profile
                }

                LoggerService.LogDebug($"[DeepLink] referral code {refCode}");
                _navigation.NavigateTo(AppRoute.Register, payload: refCode).Forget();
            }
        }
        catch (Exception ex)
        {
            LoggerService.Exception(ex, "[DeepLink] Failed to parse URL");
        }
    }

    // Helper sederhana untuk mengambil value dari query string
    private string ExtractQueryParam(string query, string paramName)
    {
        if (string.IsNullOrEmpty(query)) return null;

        query = query.TrimStart('?');
        string[] pairs = query.Split('&');
        foreach (var pair in pairs)
        {
            string[] keyValue = pair.Split('=');
            if (keyValue.Length == 2 && keyValue[0] == paramName)
                return keyValue[1];
        }
        return null;
    }
}