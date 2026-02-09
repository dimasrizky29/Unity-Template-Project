using UnityEngine;

public interface IGlobalUIService
{
    // --- Layer Management (Baru) ---
    Transform GetLayer(NavType type);

    // --- Loading & Alerts ---
    void ShowLoading(string message = "");
    void HideLoading();
    void ShowAlert(string title = "Alert", string message = "");
    void HideAlert();

    // Error handlers
    void ShowNetworkError();
    void ShowMaintenance();
    void ShowUpdateRequired();
    void ShowServerError();
}
