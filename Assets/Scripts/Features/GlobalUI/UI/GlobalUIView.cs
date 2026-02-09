using UnityEngine;
using TMPro;

public class GlobalUIView : MonoBehaviour
{
    [Header("Canvas Layer")]
    [SerializeField] private Transform _windowLayer; // 
    [SerializeField] private Transform _popupLayer;  // 

    [Header("System UI")]
    // System UI Components
    [Header("Loading Component")]
    [SerializeField] private GameObject loadingOverlay;
    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("Alert Component")]
    [SerializeField] private GameObject alertPanel;
    [SerializeField] private TextMeshProUGUI alertTitle;
    [SerializeField] private TextMeshProUGUI alertMessage;

    public void SetLoading(bool active, string msg = "")
    {
        loadingOverlay.SetActive(active);
        loadingText.text = msg;
    }

    public void DisplayAlert(string title, string msg)
    {
        alertPanel.SetActive(true);
        alertTitle.text = title;
        alertMessage.text = msg;
    }

    public void HideAlert()
    {
        alertPanel.SetActive(false);
    }

    public Transform GetLayer(NavType type)
    {
        return type == NavType.Popup ? _popupLayer : _windowLayer;
    }
}
