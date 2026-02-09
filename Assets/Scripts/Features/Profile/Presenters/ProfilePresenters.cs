using Cysharp.Threading.Tasks;
using System;
using System.Threading;

/// <summary>
/// High-performance controller for user profile operations.
/// Bridges the UserApiService and UI components to handle business logic.
/// Features:
/// - Event-based architecture with main thread safety
/// - Zero-allocation event invocations
/// - Proper async/await with UniTask
/// - Thread-safe UI updates
/// </summary>
public class ProfilePresenters : IDisposable
{
    private readonly UserRepository _repository;
    private readonly INavigationService _navigation;
    public IGlobalUIService _globalUI;

    private readonly CancellationTokenSource _cts = new();

    // High-performance events for UI updates (cached delegates for zero-allocation)
    public event Action<UserData> OnDataUpdated;
    //public event Action<string> OnError;

    public ProfilePresenters(UserRepository repository, INavigationService navigation, IGlobalUIService globalUIService)
    {
        _repository = repository;
        _navigation = navigation;
        _globalUI = globalUIService;

    }

    void IDisposable.Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    public async UniTaskVoid Initialize()
    {
        try
        {
            // Ambil data dari Repository
            var data = await _repository.GetUserData(ct: _cts.Token);

            await UniTask.SwitchToMainThread();
            OnDataUpdated?.Invoke(data);
        }
        catch (Exception e)
        {
            InvokeError(e.Message);
            LoggerService.Exception(e, "Failed to Get Data");
        }
    }

    public async UniTaskVoid UpdateUsername(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            InvokeError("Username cannot be empty");
            LoggerService.Warning("Update attempted with empty username");
            return;
        }

        try
        {
            // Tampilkan loading
            _globalUI.ShowLoading("");

            var result = await _repository.UpdateUsername(username, _cts.Token);

            // Switch to main thread for UI updates (required for Unity)
            await UniTask.SwitchToMainThread();
            OnDataUpdated?.Invoke(result);
        }
        catch (Exception e)
        {
            InvokeError(e.Message);
            LoggerService.Exception(e, "Failed to update username");
        }
        finally { _globalUI.HideLoading(); }
    }

    public async UniTaskVoid UpdateAvatar(string avatarId)
    {
        if (string.IsNullOrEmpty(avatarId))
        {
            InvokeError("Failed to update user profile");
            LoggerService.Warning("Update attempted with empty avatarId");
            return;
        }

        try
        {
            // Tampilkan loading
            _globalUI.ShowLoading("");

            var result = await _repository.UpdateAvatar(avatarId);

            // Switch to main thread for UI updates (required for Unity)
            await UniTask.SwitchToMainThread();
            OnDataUpdated?.Invoke(result);
        }
        catch (Exception e)
        {
            InvokeError(e.Message);
            LoggerService.Exception(e, "Failed to update avatar");
        }
        finally { _globalUI.HideLoading(); }
    }

    public async UniTaskVoid DeleteUserProfile(string reason)
    {
        if (string.IsNullOrEmpty(reason))
        {
            InvokeError("Reason for deletion cannot be empty");
            return;
        }

        try
        {
            // Tampilkan loading
            _globalUI.ShowLoading("Deleting account...");

            var result = await _repository.DeactivateUser(reason, _cts.Token);

            // Switch to main thread for UI updates (required for Unity)
            await UniTask.SwitchToMainThread();

            if (result.IsSuccess)
            {
                LoggerService.Info($"User profile deactivated successfully");

                // Logika setelah hapus akun: Biasanya tendang ke layar Login
                _navigation.NavigateTo(AppRoute.Login).Forget();
            }
            else
            {
                InvokeError("Failed to delete user profile");
                LoggerService.Error("Delete operation failed");
            }
        }
        catch (Exception e)
        {
            await UniTask.SwitchToMainThread();
            InvokeError("Network error occurred while deleting profile");
            LoggerService.Exception(e, "Delete User Crash");
        }
        finally { _globalUI.HideLoading(); }
    }

    /// <summary>
    /// Invokes OnError event with null check (zero-allocation).
    /// </summary>
    private void InvokeError(string errorMessage)
    {
        //OnError?.Invoke(errorMessage);
        _globalUI.ShowAlert("Failed", errorMessage);
    }

    public void BackRoutePanel()
    {
        _navigation.GoBack();
    }
}
