using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domain.Api.Base;
using Infrastructure.Networking;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// ApiService is a high-performance, lightweight API Service optimized for Unity.
/// Uses UniTask for zero-allocation async/await and UnityWebRequest for native networking.
/// </summary>
public class GameApiService : BaseApiService, IGameApiService
{
    private const int DefaultTimeout = 30;
    private bool _isRefreshing = false;

    // Dependencies (Inject lewat Constructor)
    private readonly IHttpClient _httpClient;
    private readonly IAuthStore _authStore;
    private readonly IApiConfig _config;

    public GameApiService(IHttpClient httpClient,IAuthStore authStore,
            IApiConfig config, IGlobalUIService globalUI) : base(globalUI)
    {
        _httpClient = httpClient;
        _authStore = authStore;
        _config = config;
    }

    #region Public Methods
    public async UniTask<ApiResponseDto<T>> GetAsync<T>(string url, CancellationToken ct = default)
    {
        return await SendRequestWithInterceptAsync<T>(() =>
        {
            var req = UnityWebRequest.Get(url);
            req.downloadHandler = new DownloadHandlerBuffer();
            return req;
        }, ct);
    }

    public async UniTask<ApiResponseDto<T>> PostAsync<T>(string url, object data, CancellationToken ct = default)
    {
        return await SendRequestWithInterceptAsync<T>(() =>
        {
            var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            SetupJsonBody(req, data);
            return req;
        }, ct);
    }

    /// <summary>
    /// PUT Request: Updates an existing resource.
    /// </summary>
    public async UniTask<ApiResponseDto<T>> PutAsync<T>(string url, object data, CancellationToken ct = default)
    {
        return await SendRequestWithInterceptAsync<T>(() =>
        {
            var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPUT);
            SetupJsonBody(req, data);
            return req;
        }, ct);
    }

    /// <summary>
    /// DELETE Request.
    /// </summary>
    public async UniTask<bool> DeleteAsync(string url, CancellationToken ct = default)
    {
        var result = await SendRequestWithInterceptAsync<string>(() => UnityWebRequest.Delete(url), ct);
        // For DELETE, we consider it successful if we got a response (even if empty)
        // The interceptor already handles errors, so if we get here with a non-null result, it succeeded
        return result != null;
    }
    #endregion

    #region Core Logic
    /// <summary>
    /// Request handler with interceptor for token refresh.
    /// Takes a factory function to create the request, allowing retry after token refresh.
    /// </summary>
    private async UniTask<ApiResponseDto<T>> SendRequestWithInterceptAsync<T>(
        Func<UnityWebRequest> requestFactory, CancellationToken ct)
    {
        // 1. Wait if a refresh is already in progress (prevents request storms)
        if (_isRefreshing)
            await UniTask.WaitUntil(() => !_isRefreshing, cancellationToken: ct);

        using var request = requestFactory();
        SetupRequestHeaders(request);

        try
        {
            // Send the request and wait for it to finish using UniTask BaseHttpClient
            string json = await _httpClient.SendAsync(request, ct);
            int responseCode = (int)request.responseCode;

            LoggerService.LogDebug($"[API] Response ({responseCode}): {json}");

            // Parsing & Enrichment
            var result = JsonConvert.DeserializeObject<ApiResponseDto<T>>(json) ?? new ApiResponseDto<T>();
            result.Code = responseCode;

            // --- HANDLE ACCESS TOKEN EXPIRED ---
            if (_authStore.IsLoggedIn && IsAccessTokenExpired(request))
            {
                bool refreshed = await HandleTokenRefresh(ct);
                if (refreshed)
                {
                    // Recursive retry: The token is now updated, try again with fresh request
                    using var retryRequest = requestFactory();
                    SetupRequestHeaders(retryRequest);

                    string retryJson = await _httpClient.SendAsync(retryRequest, ct);
                    var retryResult = JsonConvert.DeserializeObject<ApiResponseDto<T>>(retryJson) ?? new ApiResponseDto<T>();
                    retryResult.Code = (int)retryRequest.responseCode;

                    // Check for permanent expiration on retry
                    if (IsSessionPermanentlyExpired(retryRequest) || IsAccessTokenExpired(retryRequest))
                    {
                        LoggerService.Warning("[API] Session invalid after retry. Logging out.");
                        _authStore.ClearSession();
                        retryResult.HandledByGlobalUI = true; // Tandai agar Presenter diam
                        return retryResult;
                    }

                    // Cek Global Error pada hasil retry (Maintenance dll)
                    if (retryResult.IsGlobalError)
                    {
                        HandleInternalApiErrors(retryResult.Code);
                        retryResult.HandledByGlobalUI = true;
                    }

                    return retryResult;
                }

                result.HandledByGlobalUI = true;
                return result;
            }

            // --- HANDLE GLOBAL UI (Maintenance / Force Update / Server Error) ---
            if (result.IsGlobalError)
            {
                HandleInternalApiErrors(result.Code);
                result.HandledByGlobalUI = true;
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                // Logging untuk error non-global (400 Bad Request, dll)
                LoggerService.Warning($"[API] Protocol Error ({result.Code}): {json}");
            }

            return result;
        }
        catch (Exception ex)
        {
            // Handle Technical Error (Internet mati, Timeout, dll)
            HandleTechnicalError(ex);

            if (ex is not OperationCanceledException)
                LoggerService.Exception(ex, $"[API] Exception to {request.url}");

            // Kembalikan DTO error agar Presenter tidak crash & tahu UI sudah dihandle
            return ApiResponseDto<T>.CreateError(ex.Message, 0, true);
        }
    }

    /// <summary>
    /// Logika khusus Game Ronin: Maintenance, Update, Server Error
    /// </summary>
    private void HandleInternalApiErrors(long responseCode)
    {
        switch (responseCode)
        {
            case 426: GlobalUI.ShowUpdateRequired(); break;
            case 503: GlobalUI.ShowMaintenance(); break;
            case 502:
            case 504: GlobalUI.ShowServerError(); break;
        }
    }
    #endregion

    #region Helpers & Token Logic

    /// <summary>
    /// Handles token refresh when access token expires.
    /// Uses a direct request to avoid infinite recursion with the interceptor.
    /// </summary>
    private async UniTask<bool> HandleTokenRefresh(CancellationToken ct)
    {
        if (_isRefreshing) return true; // Already handled by the WaitUntil above
        _isRefreshing = true;

        try
        {
            LoggerService.LogDebug("[API] Access token expired. Attempting refresh...");

            if (!_authStore.IsLoggedIn || string.IsNullOrEmpty(_authStore.Session.RefreshToken))
            {
                LoggerService.Warning("[API] No refresh token available. Clearing session.");
                _authStore.ClearSession();
                return false;
            }

            string refreshUrl = $"{_config.API_BASE_URL}/refresh";
            var refreshData = new { refreshToken = _authStore.Session.RefreshToken };

            // Use direct request to bypass interceptor and avoid infinite recursion
            using var req = new UnityWebRequest(refreshUrl, UnityWebRequest.kHttpVerbPOST);
            SetupJsonBody(req, refreshData);

            // Set bearer token if configured (but don't set access/refresh tokens to avoid recursion)
            if (!string.IsNullOrEmpty(_config.API_BEARER))
                req.SetRequestHeader("Authorization", $"Bearer {_config.API_BEARER}");

            // Add API Version header
            if (!string.IsNullOrEmpty(_config.API_VERSION))
                req.SetRequestHeader("x-api-version", _config.API_VERSION);
            
            req.timeout = DefaultTimeout;

            string json = await _httpClient.SendAsync(req, ct);

            if (req.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<ApiResponseDto<RefreshTokenDto>>(json);

                // Check if refresh token itself expired (440 or "Token expired.")
                if (IsSessionPermanentlyExpired(req) ||
                    (!string.IsNullOrEmpty(response?.Status) && response.Status == "error") ||
                    (req?.responseCode == 401))
                {
                    LoggerService.Warning($"[API] Refresh token invalid/expired (Code: {req.responseCode}). Logging out.");
                    _authStore.ClearSession();
                    return false;
                }

                if (response != null && response.IsSuccess && !string.IsNullOrEmpty(response.Data.AccessToken))
                {
                    _authStore.SetSession(response.Data.AccessToken, _authStore.Session.RefreshToken);
                    LoggerService.LogDebug("[API] Token refresh successful.");
                    return true;
                }
            }
            else
            {
                LoggerService.Error($"[API] Token refresh failed: {req.error} (Status: {req.responseCode})");
            }
        }
        catch (Exception e)
        {
            LoggerService.Exception(e, "Exception during token refresh");
        }
        finally
        {
            _isRefreshing = false;
        }

        _authStore.ClearSession(); // Refresh failed
        return false;
    }

    /// <summary>
    /// Sets up all required headers for the request.
    /// </summary>
    private void SetupRequestHeaders(UnityWebRequest request)
    {
        request.timeout = DefaultTimeout;

        // Add Authorization header if bearer token is configured
        string bearerToken = _config.API_BEARER;
        if (!string.IsNullOrEmpty(bearerToken))
            request.SetRequestHeader("Authorization", $"Bearer {bearerToken}");

        // Add access token and refresh token if the user is authenticated
        if (_authStore.IsLoggedIn)
        {
            string accessToken = _authStore.Session.AccessToken;
            if (!string.IsNullOrEmpty(accessToken))
                request.SetRequestHeader("x-access-token", accessToken);
            
            string refreshToken = _authStore.Session.RefreshToken;
            if (!string.IsNullOrEmpty(refreshToken))
                request.SetRequestHeader("x-refresh-token", refreshToken);
        }

        // Add API Version header
        string apiVersion = _config.API_VERSION;
        if (!string.IsNullOrEmpty(apiVersion))
            request.SetRequestHeader("x-app-version", apiVersion);

        // Add Device Platform header
        string devicePlatform = _config.DEVICE_PLATFORM;
        if (!string.IsNullOrEmpty(devicePlatform))
            request.SetRequestHeader("x-device-platform", devicePlatform);

        // Add device-specific headers
        try
        {
            request.SetRequestHeader("x-device-id", SystemInfo.deviceUniqueIdentifier ?? "");
            request.SetRequestHeader("x-device-type", "game");
            request.SetRequestHeader("x-device-name", Application.isEditor ? "editor" : "game");
        }
        catch (Exception ex)
        {
            // Be defensive: SystemInfo access on some platforms might throw in editor tooling
            LoggerService.Warning($"[API] Failed to set device headers: {ex.Message}");
        }
    }

    private void SetupJsonBody(UnityWebRequest req, object data)
    {
        string json = JsonConvert.SerializeObject(data);
        byte[] body = Encoding.UTF8.GetBytes(json);

        // Log JSON string for debugging, be careful not to log sensitive tokens
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        LoggerService.LogDebug($"[API] Request Body JSON: {json}");
#endif

        req.uploadHandler = new UploadHandlerRaw(body) { contentType = "application/json" };
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
    }

    // --- Helper Methods for Performance ---
    private bool IsAccessTokenExpired(UnityWebRequest req)
    {
        // Triggered by specific backend message or standard 401
        if (req.responseCode == 401) return true;

        string responseText = req.downloadHandler?.text ?? string.Empty;
        return responseText.Contains("Access token expired.");
    }

    private bool IsSessionPermanentlyExpired(UnityWebRequest req)
    {
        // Specific 440 code or "Token expired." message
        if (req.responseCode == 440) return true;

        string responseText = req.downloadHandler?.text ?? string.Empty;
        return responseText.Contains("Token expired.");
    }

    #endregion
}