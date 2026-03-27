using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

public class MobileInputListener : ITickable
{
    private readonly INavigationService _navService;
    private readonly IAudioService _audioService;

    [Inject]
    public MobileInputListener(INavigationService navService, IAudioService audioService)
    {
        _navService = navService;
        _audioService = audioService;
    }

    public void Tick()
    {
        // KeyCode.Escape mendeteksi tombol Back di Android & tombol Esc di PC
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleBackAction().Forget();
        }
    }

    private async UniTaskVoid HandleBackAction()
    {
        // Kita lempar tanggung jawabnya ke Navigation Service
        bool isHandled = await _navService.HandleHardwareBack();

        // Jika false, berarti user benar-benar di ujung aplikasi (MainMenu / Root)
        if (!isHandled)
        {
            // Tampilkan Popup konfirmasi keluar
            ShowLeaveConfirmation();
        }
    }

    private void ShowLeaveConfirmation()
    {
        _audioService.PlayOneShot("IssuePanel");

        _navService.NavigateTo(AppRoute.Exit);

    }
}