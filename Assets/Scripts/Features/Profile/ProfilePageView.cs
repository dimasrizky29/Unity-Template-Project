using UnityEngine;
using UnityEngine.UI;
using VContainer;
using TMPro;

public class ProfilePageView : BasePagePanel
{
    private ProfilePresenter _profileController;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI idText;
    [SerializeField] private Image avatarImg;
    [SerializeField] private Button closeButton;

    [Header("Sub-Panel Logic")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button submitUsernameButton;

    [Inject]
    public void Construct(ProfilePresenter controller)
    {
        _profileController = controller;

        // Subscribe ke event update data dari controller
        _profileController.OnDataUpdated += HandleUserProfileUpdated;

        UpdateUI(_profileController.CurrentUser); // Tampilkan data awal jika sudah ada
    }

    protected override void Awake()
    {
        // Panggil base.Awake() agar Auto-Discovery SubPanel berjalan
        base.Awake();

        InitializeButtons();
    }

    private void OnDestroy()
    {
        if (_profileController != null)
            _profileController.OnDataUpdated -= HandleUserProfileUpdated;
    }

    private void InitializeButtons()
    {
        // Tombol Back/Close menggunakan fungsi Back() dari BasePagePanel
        if (closeButton != null)
            closeButton.onClick.AddListener(Back);

        // Tombol submit di dalam sub-panel
        if (submitUsernameButton != null)
            submitUsernameButton.onClick.AddListener(OnSubmitUsernameClicked);
    }

    #region Sub-Panel Navigation
    // Fungsi ini bisa dipanggil dari Button di UI melalui Inspector
    // atau melalui SwitchSubPanel("NAME_ID") di code
    public void UI_OpenUpdateName()
    {
        // play sound
        _audioService.PlayOneShot("ButtonGeneral");

        SwitchSubPanel("Change_Name");
    }

    public void UI_OpenUpdateAvatar()
    {
        // play sound
        _audioService.PlayOneShot("ButtonGeneral");

        SwitchSubPanel("Change_Picture");
    }
    #endregion

    #region Data Handling
    private void HandleUserProfileUpdated(UserData userData)
    {
        if (userData != null)
            UpdateUI(userData);

        LoggerService.LogDebug("User profile updated: " + userData.Username);
    }

    private void UpdateUI(UserData userData)
    {
        nameText.text = userData.Username;
        idText.text = userData.GameId;
    }

    private void OnSubmitUsernameClicked()
    {
        string newName = nameInputField.text;
        _profileController.UpdateUsername(newName).Forget();

        nameInputField.text = ""; // Clear input field after submit

        // Setelah submit, kita bisa kembali ke tampilan profil utama (history kosong)
        Back();
    }

    internal void OnSubmitAvatarClicked(string avatarId)
    {
        // Logic update avatar
        if(avatarId != null)
            _profileController.UpdateAvatar(avatarId).Forget();

        Back();
    }
    #endregion

    #region Overrides
    // Kita tidak perlu lagi menulis ulang Show/Hide secara manual 
    // kecuali ingin menambahkan logic animasi tambahan. 
    // BasePagePanel sudah menangani Fade dan SetActive.

    //public override async UniTask Show(CancellationToken token = default)
    //{
    //    // Opsional: Lakukan refresh data setiap kali panel dibuka
    //    // _profileController.RefreshData();

    //    await base.Show(token);
    //}
    #endregion
}
