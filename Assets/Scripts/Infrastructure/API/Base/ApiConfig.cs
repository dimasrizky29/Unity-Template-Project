using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VContainer;

/// <summary>
/// Globalized API configuration that reads from .env files.
/// Automatically loads configuration based on environment (development/production).
/// </summary>
public class ApiConfig : IApiConfig
{
    private Dictionary<string, string> _config = new();
    private bool _isInitialized = false;
    private readonly object _lockObject = new ();

    // Constructor: Load config immediately when VContainer creates this instance
    [Inject]
    public ApiConfig()
    {
        EnsureInitialized();
    }

    /// <summary>
    /// API Base URL from environment configuration.
    /// </summary>
    public string API_BASE_URL => GetConfigValue("API_BASE_URL", "https://your-api.com/api");

    /// <summary>
    /// WebSocket URL from environment configuration.
    /// </summary>
    public string WEBSOCKET_URL => GetConfigValue("WEBSOCKET_URL", "wss://your-api.com/ws");

    /// <summary>
    /// API Bearer token from environment configuration.
    /// </summary>
    public string API_BEARER => GetConfigValue("API_BEARER", string.Empty);

    /// <summary>
    /// API Salt Password from environment configuration.
    /// </summary>
    public string API_SALT_PASSWORD => GetConfigValue("API_SALT_PASSWORD", string.Empty);

    /// <summary>
    /// API Version from environment configuration.
    /// </summary>
    public string API_VERSION => GetConfigValue("API_VERSION", Application.version);

    /// <summary>
    /// landing URL from environment configuration.
    /// </summary>
    public string API_LANDING_URL => GetConfigValue("API_LANDING_URL", "https://your-api.com/api");

    /// <summary>
    /// Device Platform
    /// </summary>
    public string DEVICE_PLATFORM => GetPlatformRun();
    
    /// <summary>
    /// Gets the Google web client identifier used for authentication with Google services.
    /// </summary>
    public string GOOGLE_WEB_CLIENT_ID => GetConfigValue("GOOGLE_WEB_CLIENT_ID", string.Empty);

    /// <summary>
    /// Gets a configuration value by key, with optional default value.
    /// </summary>
    private string GetConfigValue(string key, string defaultValue = "")
    {
        return _config.TryGetValue(key, out string value) ? value : defaultValue;
    }

    /// <summary>
    /// Manually reloads the configuration. Useful for runtime updates.
    /// </summary>
    public void Reload()
    {
        lock (_lockObject)
        {
            _config.Clear();
            _isInitialized = false;
            EnsureInitialized();
        }
    }

    /// <summary>
    /// Ensures the configuration is loaded. Thread-safe initialization.
    /// </summary>
    private void EnsureInitialized()
    {
        if (_isInitialized) return;

        lock (_lockObject)
        {
            if (_isInitialized) return;

            LoadEnvironmentConfig();
            _isInitialized = true;
        }
    }

    /// <summary>
    /// Determines the current environment and loads the appropriate .env file.
    /// </summary>
    private void LoadEnvironmentConfig()
    {
        // Determine environment: development or production
        bool isDevelopment = IsDevelopmentEnvironment();
        string envFileName = isDevelopment ? ".env.dev" : ".env.prod";

        LoggerService.Info($"Loading API config from {envFileName} (Environment: {(isDevelopment ? "Development" : "Production")})");

        // DEKLARASI DI LUAR BLOK agar bisa diakses oleh platform manapun
        string pathOrContent = null;

#if UNITY_EDITOR
        // Try multiple locations for the .env file
        string[] possiblePaths = {
            Path.Combine(Application.dataPath, "..", envFileName), // Project root
            Path.Combine(Application.streamingAssetsPath, envFileName), // StreamingAssets
            Path.Combine(Application.dataPath, envFileName), // Data path
            Path.Combine(Directory.GetCurrentDirectory(), envFileName) // Current directory
        };

        foreach (string path in possiblePaths)
        {
            if (File.Exists(path))
            {
                pathOrContent = path;
                break;
            }
        }

        if (string.IsNullOrEmpty(pathOrContent))
        {
            LoggerService.Warning($"Environment file {envFileName} not found. Using default values.");
            return;
        }
#elif UNITY_ANDROID || UNITY_IOS
        envFileName = isDevelopment ? "env_dev" : "env_prod";
        TextAsset envAsset = Resources.Load<TextAsset>(envFileName);

        if (envAsset == null)
        {
            LoggerService.Warning($"Environment file {envFileName} not found in Resources. Using default values.");
            return;
        }

        // Di mobile, kita simpan ISINYA, bukan path-nya
        pathOrContent = envAsset.text; 
#endif

        try
        {
            ParseEnvFile(pathOrContent);
            // GUNAKAN envFileName untuk log agar isi password/API Key tidak bocor ke log
            LoggerService.Info($"Successfully loaded API config: {envFileName}");
        }
        catch (Exception ex)
        {
            LoggerService.Exception(ex, $"Failed to load environment file: {envFileName}");
        }
    }

    /// <summary>
    /// Determines if the current environment is development.
    /// Checks Debug.isDebugBuild and custom define symbols.
    /// </summary>
    private bool IsDevelopmentEnvironment()
    {
        // Check Unity's debug build flag
        //if (Debug.isDebugBuild) return true;

        // Check for custom define symbols
#if DEVELOPMENT
            return true;
#endif

#if PRODUCTION
            return false;
#endif

        // Default to development if not explicitly set
        return true;
    }

    /// <summary>
    /// Call to get platform run now
    /// </summary>
    /// <returns>android/ios</returns>
	public string GetPlatformRun()
    {
        return Application.platform switch
        {
            RuntimePlatform.Android => "android",
            RuntimePlatform.IPhonePlayer => "ios",
            RuntimePlatform.OSXEditor => "android",
            RuntimePlatform.WindowsEditor => "android",
            RuntimePlatform.WindowsPlayer => "android",
            RuntimePlatform.LinuxServer => "server",
            _ => "not detected",
        };
    }

    /// <summary>
    /// Parses a .env file and populates the configuration dictionary.
    /// Supports KEY=VALUE format, ignores comments and empty lines.
    /// </summary>
    private void ParseEnvFile(string pathOrContent)
    {
        if (string.IsNullOrEmpty(pathOrContent)) return;

#if UNITY_EDITOR
        // Di Editor, pathOrContent adalah ALAMAT FILE
        string[] lines = File.ReadAllLines(pathOrContent);
#elif UNITY_ANDROID || UNITY_IOS
        // Di Mobile, pathOrContent adalah ISI TEKS FILE
        string[] lines = pathOrContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
#else
        // Fallback kosong untuk platform lain jika ada
        string[] lines = new string[0]; 
#endif

        foreach (string line in lines)
        {
            // Trim whitespace
            string trimmedLine = line.Trim();

            // Skip empty lines and comments
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                continue;

            // Parse KEY=VALUE
            int equalsIndex = trimmedLine.IndexOf('=');
            if (equalsIndex <= 0)
                continue;

            string key = trimmedLine.Substring(0, equalsIndex).Trim();
            string value = trimmedLine.Substring(equalsIndex + 1).Trim();

            // Remove quotes if present
            if (value.Length >= 2 &&
                ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                 (value.StartsWith("'") && value.EndsWith("'"))))
            {
                value = value.Substring(1, value.Length - 2);
            }

            if (!string.IsNullOrEmpty(key))
            {
                _config[key] = value;
            }
        }
    }

    /// <summary>
    /// Gets all loaded configuration values. Useful for debugging.
    /// </summary>
    public Dictionary<string, string> GetAllConfig()
    {
        EnsureInitialized();
        return new Dictionary<string, string>(_config);
    }
}
