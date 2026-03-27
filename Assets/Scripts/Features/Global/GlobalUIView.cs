using TMPro;
using UnityEngine;
using VContainer;

public class GlobalUIView : MonoBehaviour
{
    private IApiConfig _apiConfig;
    private IGlobalUIService _service;
    private IAudioService _audioService;

    [Inject]
    public void Construct(IApiConfig config, IAudioService audioService)
    {
        _apiConfig = config;
        _audioService = audioService;
    }

    [Header("Canvas Layer")]
    [SerializeField] private Transform _windowLayer; // 
    [SerializeField] private Transform _popupLayer;  // 

    [Header("Loading Panel")]
    [SerializeField] private GameObject loadingFullPanel;

    [Header("System UI")]
    // System UI Components
    [Header("Loading Component")]
    [SerializeField] private GameObject loadingOverlay;
    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("Alert Component")]
    [SerializeField] private GameObject alertPanel;
    [SerializeField] private TextMeshProUGUI alertTitle;
    [SerializeField] private TextMeshProUGUI alertMessage;


    public Transform GetLayer(NavType type)
    {
        return type == NavType.Popup ? _popupLayer : _windowLayer;
    }

    public void SetService(IGlobalUIService service)
    {
        _service = service;
    }

    #region Display
    #region Loading
    // --- loading
    public void SetLoading(bool active, string msg = "")
    {
        loadingOverlay.SetActive(active);
        loadingText.text = msg;
    }

    // --- loading full
    public void SetLoadingFull(bool active)
    {
        loadingFullPanel.SetActive(active);
    }
    #endregion

    #region Alert
    // --- Alert
    public void DisplayAlert(string title, string msg)
    {
        // Play sound
        _audioService.PlayOneShot("IssuePanel");

        alertTitle.text = title;
        alertMessage.text = msg;

        alertPanel.SetActive(true);
    }

    public void HideAlert()
    {
        alertPanel.SetActive(false);
    }


    #endregion

    public void HideAll()
    {
        loadingFullPanel.SetActive(false);
        loadingOverlay.SetActive(false);
        alertPanel.SetActive(false);
    }

    #endregion

    #region Button 
    public void Btn_Reload()
    {
        // Play sound
        _audioService.PlayOneShot("ButtonGeneral");

        HideAll();

        if(_service == null) return;

        _service.ResolvePrompt(true);
    }

    public void Btn_Update()
    {
        // Play sound
        _audioService.PlayOneShot("ButtonGeneral");

        Application.OpenURL(_apiConfig.API_LANDING_URL);
    }
    #endregion
}
