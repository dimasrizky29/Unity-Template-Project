using System;
using UnityEngine;

/// <summary>
/// Ultra-lightweight session data structure. 
/// Using a struct for zero-allocation when passing session info around.
/// </summary>
public struct UserSession
{
    public string AccessToken;
    public string RefreshToken;
    public DateTime Expiry;
    public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);
}

/// <summary>
/// Global high-performance Auth Store.
/// Acts as the single source of truth for user sessions.
/// </summary>
public class AuthStore : IAuthStore
{
    // High-performance event for UI/Router updates
    public event Action<UserSession> OnSessionChanged;
    private UserSession _currentSession;

    public UserSession Session => _currentSession;
    public bool IsLoggedIn => _currentSession.IsAuthenticated;

    /// <summary>
    /// Sets the session and notifies listeners. O(1) performance.
    /// </summary>
    public void SetSession(string accessToken, string refreshToken, int expiresInSeconds = 3600)
    {
        //save to PlayerPrefs or secure storage
        if (refreshToken != PlayerPrefs.GetString("auth-token"))
            PlayerPrefs.SetString("auth-token", refreshToken);

        _currentSession = new UserSession
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiry = DateTime.UtcNow.AddSeconds(expiresInSeconds)
        };

        OnSessionChanged?.Invoke(_currentSession);
    }

    /// <summary>
    /// Clears the session and triggers a logout event.
    /// </summary>
    public void ClearSession()
    {
        _currentSession = default; // Reset to empty struct
        OnSessionChanged?.Invoke(_currentSession);

        //save to PlayerPrefs or secure storage
        PlayerPrefs.DeleteKey("auth-token");
    }

    /// <summary>
    /// Lightweight check to see if the token needs refreshing.
    /// </summary>
    public bool IsTokenExpired()
    {
        if (!IsLoggedIn) return true;
        return DateTime.UtcNow >= _currentSession.Expiry;
    }
}