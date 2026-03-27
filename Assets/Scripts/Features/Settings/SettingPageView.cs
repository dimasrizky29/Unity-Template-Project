using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class SettingPageView : BasePagePanel
{
    private IAuthApiService _authApi;
    private IUserRepository _repository;
    private IGlobalUIService _globalUI;
    
    private readonly CancellationTokenSource _cts = new();

    [Header("UI Setting")]
    // --- AUDIO ---
    public GameObject[] musicOn;
    public GameObject[] musicOff;
    public GameObject[] soundOn;
    public GameObject[] soundOff;

    public TMP_Dropdown reasonDeleteAcc;

    public TextMeshProUGUI appVersion;

    [Inject]
    public void Construct(IAuthApiService authApi, IUserRepository repository, IGlobalUIService globalUI)
    {
        _authApi = authApi;
        _repository = repository;
        _globalUI = globalUI;
    }

    private void Start()
    {
        // Display audio 
        bool isMusicOn = !_audioService.IsBgmMuted;
        ChangeMusicActive(isMusicOn);

        bool isSoundOn = !_audioService.IsSfxMuted;
        ChangeSoundActive(isSoundOn);

        appVersion.text = "Version " + Application.version;
    }

    #region Audio View

    public void Btn_ChangeMusic(bool on)
    {
        // play sound
        _audioService.PlayOneShot("ButtonGeneral");

        bool isMute = !on;
        _audioService.SetMute(AudioType.BGM, isMute);

        ChangeMusicActive(on);
    }

    public void Btn_ChangeSound(bool on)
    {
        // play sound
        _audioService.PlayOneShot("ButtonGeneral");

        bool isMute = !on;
        _audioService.SetMute(AudioType.UI, isMute);
        _audioService.SetMute(AudioType.SFX, isMute);

        ChangeSoundActive(on);
    }

    private void ChangeMusicActive(bool on)
    {
        foreach (var item in musicOn)
            item.SetActive(on);

        foreach (var item in musicOff)
            item.SetActive(!on);
    }

    private void ChangeSoundActive(bool on)
    {
        foreach (var item in soundOn)
            item.SetActive(on);

        foreach (var item in soundOff)
            item.SetActive(!on);
    }   

    #endregion

    #region Button Reference
    public void Btn_OpenGraphicsControl()
    {
        // play sound
        _audioService.PlayOneShot("ButtonGeneral");

        SwitchSubPanel("Graphics_Control");
    }

    public void Btn_OpenConfirmLogout()
    {
        // play sound
        _audioService.PlayOneShot("ButtonGeneral");

        SwitchSubPanel("Confirm_Logout");
    }

    public void Btn_OpenConfirmDeleteAcc()
    {
        // play sound
        _audioService.PlayOneShot("ButtonGeneral");

        SwitchSubPanel("Confirm_DeleteAcc");
    }

    public void Btn_ConfirmLogout()
    {
        // play sound
        _audioService.PlayOneShot("ButtonGeneral");

        SignOutUser().Forget();
    }

    public void Btn_ConfirmDeleteAcc()
    {
        // play sound
        _audioService.PlayOneShot("ButtonGeneral");

        var reason = reasonDeleteAcc.options[reasonDeleteAcc.value].text;
        DeleteUserProfile(reason).Forget();
    }
    #endregion

    #region Handling Request
    public async UniTaskVoid SignOutUser()
    {
        try
        {
            // Tampilkan loading
            _globalUI.ShowLoading("Sign out account...");

            var result = await _authApi.SignOutAsync(_cts.Token);

            // Switch to main thread for UI updates (required for Unity)
            await UniTask.SwitchToMainThread();

            if (result.IsSuccess)
            {

                LoggerService.Info($"User signed out successfully");

                // Logika setelah hapus akun: Biasanya tendang ke layar Login
                _navigationService.NavigateTo(AppRoute.Login).Forget();
            }
            else
            {
                InvokeError("Failed to sign out user");
                LoggerService.Error("Sign out operation failed");
            }
        }
        catch (Exception e)
        {
            await UniTask.SwitchToMainThread();

            InvokeError("Network error occurred while signing out");
            LoggerService.Exception(e, "Sign Out Crash");
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
                _navigationService.NavigateTo(AppRoute.Login).Forget();
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

    #endregion

    /// <summary>
    /// Invokes OnError event with null check (zero-allocation).
    /// </summary>
    private void InvokeError(string errorMessage)
    {
        //OnError?.Invoke(errorMessage);
        _globalUI.ShowAlert("Failed", errorMessage);
    }
}