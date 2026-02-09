using UnityEngine;

public class GlobalUIService : IGlobalUIService
{
    private readonly GlobalUIView _view;

    // Inject View lewat constructor
    public GlobalUIService(GlobalUIView view)
    {
        _view = view;
        HideAll();
    }

    // --- TRANSIENT UI (Bisa muncul/hilang selama game jalan) ---
    public void ShowLoading(string message = "") => _view.SetLoading(true, message);
    public void HideLoading() => _view.SetLoading(false);
    public void ShowAlert(string title, string message) => _view.DisplayAlert(title, message);
    public void HideAlert() => _view.HideAlert();

    

    // --- TERMINAL UI (Mengunci alur game) ---
    public void ShowNetworkError()
    {
        ShowAlert("Network Error", "Please check your internet connection and try again.");
    }

    public void ShowMaintenance()
    {
        ShowAlert("Maintenance", "The system is currently under maintenance. Please try again later.");
    }

    public void ShowUpdateRequired()
    {
        ShowAlert("Update Required", "A new version of the application is available. Please update to continue.");
    }

    public void ShowServerError()
    {
        ShowAlert("Server Error", "An unexpected server error occurred. Please try again later.");
    }

    // --- LAYER MANAGEMENT ---
    public Transform GetLayer(NavType type)
    {
        return _view.GetLayer(type);
    }

    public void HideAll()
    {
        HideLoading();
        HideAlert();
    }
}
