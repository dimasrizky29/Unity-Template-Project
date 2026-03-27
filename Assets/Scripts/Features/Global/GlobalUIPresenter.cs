using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

public class GlobalUIPresenter : IGlobalUIService
{
    private readonly GlobalUIView _view;

    private UniTaskCompletionSource<bool> _userDecisionTcs;

    [Inject]
    public GlobalUIPresenter(GlobalUIView view)
    {
        _view = view;
        _view.SetService(this);
        HideAll();
    }
    
    public void ShowLoading(string message = "") => _view.SetLoading(true, message);
    public void HideLoading() => _view.SetLoading(false);

    public void ShowLoadingFull(bool active) => _view.SetLoadingFull(active);

    // --- TRANSIENT UI (Bisa muncul/hilang selama game jalan) ---
    public void ShowAlert(string title, string message) => _view.DisplayAlert(title, message);
    public void HideAlert() => _view.HideAlert();

    // --- TERMINAL UI (Mengunci alur game) ---
    public void ShowNetworkError()
    {
        ShowAlert("Network Error", "Please check your internet connection.");
    }

    public void ShowMaintenance()
    {
        ShowAlert("Maintenance", "Please check back later.");
    }

    public void ShowUpdateRequired()
    {
        ShowAlert("Update Required", "Please update your app.");
    }

    public void ShowServerError()
    {
        ShowAlert("Server Error", "Please try again later.");
    }

    public void ShowAccountCollide()
    {
        ShowAlert("Account Collide", "Please try again later.");
    }

    // --- LAYER MANAGEMENT ---
    public Transform GetLayer(NavType type)
    {
        return _view.GetLayer(type);
    }

    public void HideAll()
    {
        _view.HideAll();
    }

    public UniTask<bool> ShowNetworkErrorPromptAsync()
    {
        ShowNetworkError();
        _userDecisionTcs = new UniTaskCompletionSource<bool>();
        return _userDecisionTcs.Task;
    }

    public void ResolvePrompt(bool shouldRetry)
    {
        _userDecisionTcs?.TrySetResult(shouldRetry);
    }
}
