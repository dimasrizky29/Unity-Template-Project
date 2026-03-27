using System.Threading;

public interface IGoogleSignInService
{
    void OnSignInGoogleClicked(string referralCode, CancellationToken ct = default);
}