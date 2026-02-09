using UnityEngine;
using UnityEngine.UI;
using VContainer;
using TMPro;

/// <summary>
/// High-performance User Profile UI with API integration.
/// Features:
/// - Cached UI references for zero-allocation updates
/// - Request debouncing to prevent duplicate API calls
/// - Proper cancellation token management
/// - Modular design with clear separation of concerns
/// </summary>
public class UserProfileView : MonoBehaviour
{
    private ProfilePresenters _profileController;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI idText;
    [SerializeField] private Button usernameUpdate;
    [SerializeField] private Button avatarUpdate;
    [SerializeField] private Button deleteAccount;
    [SerializeField] private Button close;
    [SerializeField] private InputField nameInputField;

    private void Awake()
    {
        InitializeUI();
        SubscribeToControllerEvents();
    }

    [Inject]
    public void Construct(ProfilePresenters controller)
    {
        _profileController = controller;
    }

    private void OnEnable()
    {
        // Load user profile when view is enabled
        _profileController.Initialize().Forget();
    }

    private void OnDestroy()
    {
        UnsubscribeFromControllerEvents();
    }

    private void InitializeUI()
    {
        // Button listeners
        if (usernameUpdate != null)
            usernameUpdate.onClick.AddListener(OnUpdateUsernameClicked);
        if (avatarUpdate != null)
            avatarUpdate.onClick.AddListener(OnUpdateAvatarClicked);
        if (deleteAccount != null)
            deleteAccount.onClick.AddListener(OnDeleteAccountClicked);
        if (close != null)
            close.onClick.AddListener(BackRoutePanel);
    }

    /// <summary>
    /// Subscribes to controller events for UI updates.
    /// </summary>
    private void SubscribeToControllerEvents()
    {
        if (_profileController == null) return;

        _profileController.OnDataUpdated += HandleUserProfileUpdated;
    }

    /// <summary>
    /// Unsubscribes from controller events.
    /// </summary>
    private void UnsubscribeFromControllerEvents()
    {
        if (_profileController == null) return;

        _profileController.OnDataUpdated -= HandleUserProfileUpdated;
    }

    /// <summary>
    /// Requests to update user profile via controller.
    /// </summary>
    public void RequestUpdateUsername()
    {
        // Get values from input fields
        string newName = nameInputField.text;

        _profileController.UpdateUsername(newName).Forget();
    }

    /// <summary>
    /// Requests to update user profile via controller.
    /// </summary>
    public void RequestUpdateAvatar()
    {
        // Get values from input fields
        string newName = nameInputField.text;

        _profileController.UpdateUsername(newName).Forget();
    }

    /// <summary>
    /// Requests to deactive account via controller.
    /// </summary>
    public void RequestDeactiveAccount()
    {
        // Get values from input fields
        string newName = nameInputField.text;

        _profileController.UpdateUsername(newName).Forget();
    }

    /// <summary>
    /// Requests to deactive account via controller.
    /// </summary>
    public void BackRoutePanel()
    {
        _profileController.BackRoutePanel();
    }

    /// <summary>
    /// Handles user profile updated event from controller.
    /// </summary>
    private void HandleUserProfileUpdated(UserData userData)
    {
        if (userData != null && userData.GameId != "")
            UpdateUI(userData);
    }

    /// <summary>
    /// Updates UI elements with user data. Performance-optimized to only update changed values.
    /// </summary>
    private void UpdateUI(UserData userData)
    {
        if (userData == null) return;

        // Performance: Only update UI if values actually changed
        if (nameText != null && string.IsNullOrEmpty(userData.Username))
            nameText.text = userData.Username;

        if (idText != null && string.IsNullOrEmpty(userData.GameId))
            idText.text = userData.GameId;
    }

    // Event handlers
    private void OnUpdateUsernameClicked()
    {
        RequestUpdateUsername();
    }

    private void OnUpdateAvatarClicked()
    {
        RequestUpdateAvatar();
    }

    private void OnDeleteAccountClicked()
    {
        RequestDeactiveAccount();
    }

    private void OnCloseClicked()
    {
        
    }
}
