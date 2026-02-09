using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

/// <summary>
/// 
/// </summary>
public class AuthView : MonoBehaviour
{
    // inject via inspector or auto-find
    private AuthFlowPresenters _authController;

    [Inject]
    public void Construct(AuthFlowPresenters authController)
    {
        _authController = authController;
    }

    [Header("UI References - Login")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private TMP_InputField loginEmailInput;
    [SerializeField] private TMP_InputField loginPasswordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button goToRegisterButton;

    [Header("UI References - Register")]
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private TMP_InputField registerEmailInput;
    [SerializeField] private TMP_InputField registerPasswordInput;
    [SerializeField] private TMP_InputField registerConfirmPasswordInput;
    [SerializeField] private TMP_InputField referralCodeInput;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button goToLoginButton;

    [Header("Common UI References")]
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private TextMeshProUGUI successText;

    [Header("Performance Settings")]
    [SerializeField] private float requestDebounceTime = 0.5f;
    private float _lastRequestTime;

    private void Awake()
    {
        InitializeUI();
    }

    private void Start()
    {
        _authController.GoToLogin();
    }

    /// <summary>
    /// Initializes UI components and sets up event listeners.
    /// </summary>
    private void InitializeUI()
    {
        // Login panel buttons
        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginClicked);

        if (goToRegisterButton != null)
            goToRegisterButton.onClick.AddListener(OnGoToRegisterClicked);

        // Register panel buttons
        if (registerButton != null)
            registerButton.onClick.AddListener(OnRegisterClicked);

        if (goToLoginButton != null)
            goToLoginButton.onClick.AddListener(OnGoToLoginClicked);

        // Set initial state based on current route
        SyncWithRouter();
    }

    /// <summary>
    /// Syncs the Auth UI state with the current router route.
    /// </summary>
    private void SyncWithRouter()
    {
        if(_authController == null)
            return;

        // Get current route from router and sync internal panels
        var currentRoute = _authController.GetCurrentRoute();
        
        if (currentRoute == AppRoute.Register)
            ShowRegisterPanel();
        else
            // Default to login for Login route or unknown routes
            ShowLoginPanel();
    }

    /// <summary>
    /// Shows login panel and hides register panel.
    /// </summary>
    private void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    /// <summary>
    /// Shows register panel and hides login panel.
    /// </summary>
    private void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    /// <summary>
    /// Handles login button click with validation and debouncing.
    /// </summary>
    private void OnLoginClicked()
    {
        if (_authController == null)
        {
            LoggerService.Warning("Controller not available");
            return;
        }

        // Debounce: Prevent multiple rapid requests
        float currentTime = Time.time;
        if (currentTime - _lastRequestTime < requestDebounceTime)
        {
            ShowError("Too soon since last request");
            LoggerService.Warning("Login request debounced - too soon since last request");
            return;
        }
        _lastRequestTime = currentTime;

        // Validate inputs
        string email = loginEmailInput != null ? loginEmailInput.text : string.Empty;
        string password = loginPasswordInput != null ? loginPasswordInput.text : string.Empty;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("Email and password are required");
            return;
        }

        if (!IsValidEmail(email))
        {
            ShowError("Please enter a valid email address");
            return;
        }

        ClearMessages();

        // Call controller method
        _authController.OnLoginButtonClicked(email, password);
    }

    /// <summary>
    /// Handles register button click with validation and debouncing.
    /// </summary>
    private void OnRegisterClicked()
    {
        if (_authController == null)
        {
            LoggerService.Warning("Controller not available");
            return;
        }

        // Debounce: Prevent multiple rapid requests
        float currentTime = Time.time;
        if (currentTime - _lastRequestTime < requestDebounceTime)
        {
            ShowError("Too soon since last request");
            LoggerService.Warning("Register request debounced - too soon since last request");
            return;
        }
        _lastRequestTime = currentTime;

        // Validate inputs
        string email = registerEmailInput != null ? registerEmailInput.text : string.Empty;
        string password = registerPasswordInput != null ? registerPasswordInput.text : string.Empty;
        string confirmPassword = registerConfirmPasswordInput != null ? registerConfirmPasswordInput.text : string.Empty;
        string referralCode = referralCodeInput != null ? referralCodeInput.text : string.Empty;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            ShowError("All fields are required");
            return;
        }

        if (!IsValidEmail(email))
        {
            ShowError("Please enter a valid email address");
            return;
        }

        if (password.Length < 6)
        {
            ShowError("Password must be at least 6 characters long");
            return;
        }

        if (password != confirmPassword)
        {
            ShowError("Passwords do not match");
            return;
        }

        ClearMessages();

        // Call controller method
        _authController.OnRegisterButtonClicked(email, password, referralCode);
    }

    /// <summary>
    /// Handles navigation to register panel.
    /// </summary>
    private void OnGoToRegisterClicked()
    {
        // Update internal state immediately for responsive UI
        ShowRegisterPanel();
        
        // Navigate via router for proper route management
        _authController.GoToRegister();
    }

    /// <summary>
    /// Handles navigation to login panel.
    /// </summary>
    private void OnGoToLoginClicked()
    {
        // Update internal state immediately for responsive UI
        ShowLoginPanel();
        
        // Navigate via router for proper route management
        _authController.GoToLogin();
    }

    /// <summary>
    /// Displays error message to user.
    /// </summary>
    public void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.gameObject.SetActive(true);
        }
        if (successText != null)
            successText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Displays success message to user.
    /// </summary>
    public void ShowSuccess(string message)
    {
        if (successText != null)
        {
            successText.text = message;
            successText.gameObject.SetActive(true);
        }
        if (errorText != null)
            errorText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Clears all messages.
    /// </summary>
    private void ClearMessages()
    {
        if (errorText != null)
            errorText.gameObject.SetActive(false);
        if (successText != null)
            successText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Simple email validation using regex pattern.
    /// </summary>

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;
        // Simple email validation pattern
        // Checks for: text@text.text format
        try
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern);
        }
        catch
        {
            return false;
        }
    }
}
