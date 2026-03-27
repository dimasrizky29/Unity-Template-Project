using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class TopBarView : MonoBehaviour
{
    private INavigationService _navigation;
    private IUserRepository _userRepository;
    private IAudioService _audioService;

    [Header("UI Reference")]
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI balanceGameText;
    public TextMeshProUGUI balanceShopText;
    public Image avatarImage;

    [Header("UI Button")]
    public Button profileBtn;
    public Button[] depositBtn;
    public Button withdrawBtn;
    public Button inboxBtn;
    public Button settingBtn;
    public Button referralBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        InitializeButton();
    }

    [Inject]
    public void Construct(INavigationService navigation, IUserRepository userRepository, IAudioService audioService)
    {
        _navigation = navigation;
        _userRepository = userRepository;
        _audioService = audioService;

        // Subscribe ke event update data dari Repository
        _userRepository.OnUserDataUpdated += UpdateView;

        // Inisialisasi tampilan dengan data saat ini
        UpdateView(_userRepository.CurrentUser);
    }

    private void OnDestroy()
    {
        // Unsubscribe dari event untuk mencegah memory leak
        if (_userRepository != null)
            _userRepository.OnUserDataUpdated -= UpdateView;
    }

    private void UpdateView(UserData user)
    {
        if (user != null)
        {
            usernameText.text = user.Username;
            balanceGameText.text = FormatBalance(user.BalanceGame.CurrentBalance);
            balanceShopText.text = FormatBalance(user.BalanceShop.CurrentBalance);
        }
    }

    private void InitializeButton()
    {
        profileBtn.onClick.AddListener(OnProfileClicked);
        foreach (var btn in depositBtn)
            btn.onClick.AddListener(OnDepositClicked);
        withdrawBtn.onClick.AddListener(OnWithdrawClicked);
        inboxBtn.onClick.AddListener(OnInboxClicked);
        settingBtn.onClick.AddListener(OnSettingClicked);
        referralBtn.onClick.AddListener(OnReferralClicked);
    }

    // --- UI Event Handlers ---
    public void OnProfileClicked()
    {
        _navigation.NavigateTo(AppRoute.Profile);

        // play sound
        _audioService.PlayOneShot("ButtonGeneral");
    }

    public void OnDepositClicked()
    {
        _navigation.NavigateTo(AppRoute.Deposit);

        // play sound
        _audioService.PlayOneShot("ButtonGeneral");
    }

    public void OnWithdrawClicked()
    {
        _navigation.NavigateTo(AppRoute.Withdraw);

        // play sound
        _audioService.PlayOneShot("ButtonGeneral");
    }

    public void OnInboxClicked()
    {
        _navigation.NavigateTo(AppRoute.Inbox);

        // play sound
        _audioService.PlayOneShot("ButtonGeneral");
    }

    public void OnSettingClicked()
    {
        _navigation.NavigateTo(AppRoute.Settings);

        // play sound
        _audioService.PlayOneShot("ButtonGeneral");
    }

    public void OnReferralClicked()
    {
        _navigation.NavigateTo(AppRoute.Referral);

        // play sound
        _audioService.PlayOneShot("ButtonGeneral");
    }

    // --- Utility Methods ---
    public string FormatBalance(double balance)
    {
        return balance.ToString("#,##0.##");
    }
}
