using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Threading;
using VContainer.Unity;

/// <summary>
/// High-performance controller for user profile operations.
/// Bridges the UserApiService and UI components to handle business logic.
/// Features:
/// - Event-based architecture with main thread safety
/// - Zero-allocation event invocations
/// - Proper async/await with UniTask
/// - Thread-safe UI updates
/// </summary>
public class ProfilePresenter : IDisposable
{
    private readonly IUserRepository _repository;
    public IGlobalUIService _globalUI;

    private readonly CancellationTokenSource _cts = new();

    // High-performance events for UI updates (cached delegates for zero-allocation)
    public event Action<UserData> OnDataUpdated;
    //public event Action<string> OnError;

    public UserData CurrentUser => _repository?.CurrentUser;

    public ProfilePresenter(IUserRepository repository, IGlobalUIService globalUIService)
    {
        _repository = repository;
        _globalUI = globalUIService;
    }

    void IDisposable.Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
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

        if (avatarId == CurrentUser.AvatarId) return;

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

    /// <summary>
    /// Invokes OnError event with null check (zero-allocation).
    /// </summary>
    private void InvokeError(string errorMessage)
    {
        //OnError?.Invoke(errorMessage);
        _globalUI.ShowAlert("Failed", errorMessage);
    }
}
