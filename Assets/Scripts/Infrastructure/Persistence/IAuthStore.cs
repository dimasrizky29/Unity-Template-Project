using System;

public interface IAuthStore
{
    UserSession Session { get; }
    bool IsLoggedIn { get; }
    event Action<UserSession> OnSessionChanged;
    void SetSession(string accessToken, string refreshToken, int expiresInSeconds = 3600);
    void ClearSession();
    bool IsTokenExpired();
}
