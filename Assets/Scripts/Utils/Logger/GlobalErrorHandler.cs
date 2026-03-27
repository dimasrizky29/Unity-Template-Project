using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helper component for creating the error overlay UI programmatically.
/// This can be used to automatically set up the error UI if not manually created in the scene.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class GlobalErrorHandlerUI : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetupUI = true;

    private void Awake()
    {
        if (autoSetupUI)
        {
            SetupErrorUI();
        }
    }

    /// <summary>
    /// Automatically creates the error overlay UI structure.
    /// </summary>
    private void SetupErrorUI()
    {
        // Create error overlay GameObject
        GameObject overlay = new GameObject("ErrorOverlay");
        overlay.transform.SetParent(transform, false);

        // Add CanvasGroup for fade effects
        var canvasGroup = overlay.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        // Add Image for background
        var bgImage = overlay.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.85f); // Semi-transparent black

        // Stretch to fill screen
        var rectTransform = overlay.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;

        // Create content panel
        GameObject contentPanel = new GameObject("ContentPanel");
        contentPanel.transform.SetParent(overlay.transform, false);

        var contentRect = contentPanel.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
        contentRect.sizeDelta = new Vector2(600, 400);
        contentRect.anchoredPosition = Vector2.zero;

        var contentImage = contentPanel.AddComponent<Image>();
        contentImage.color = new Color(0.2f, 0.2f, 0.2f, 1f); // Dark gray background

        // Create title text
        GameObject titleObj = CreateText("TitleText", "An Error Occurred", 24, FontStyle.Bold);
        titleObj.transform.SetParent(contentPanel.transform, false);
        var titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(580, 40);
        titleRect.anchoredPosition = new Vector2(0, -20);

        // Create message text
        GameObject messageObj = CreateText("MessageText", "Error message will appear here", 18, FontStyle.Normal);
        messageObj.transform.SetParent(contentPanel.transform, false);
        var messageRect = messageObj.GetComponent<RectTransform>();
        messageRect.anchorMin = new Vector2(0.5f, 0.5f);
        messageRect.anchorMax = new Vector2(0.5f, 0.5f);
        messageRect.sizeDelta = new Vector2(580, 200);
        messageRect.anchoredPosition = new Vector2(0, 20);

        var messageText = messageObj.GetComponent<Text>();
        messageText.alignment = TextAnchor.MiddleCenter;
        messageText.supportRichText = true;

        // Create details text (scrollable)
        GameObject detailsObj = CreateText("DetailsText", "Details will appear here", 12, FontStyle.Normal);
        detailsObj.transform.SetParent(contentPanel.transform, false);
        var detailsRect = detailsObj.GetComponent<RectTransform>();
        detailsRect.anchorMin = new Vector2(0.5f, 0f);
        detailsRect.anchorMax = new Vector2(0.5f, 0f);
        detailsRect.sizeDelta = new Vector2(580, 80);
        detailsRect.anchoredPosition = new Vector2(0, 40);

        var detailsText = detailsObj.GetComponent<Text>();
        detailsText.alignment = TextAnchor.UpperLeft;
        detailsText.fontSize = 10;
        detailsText.color = new Color(0.8f, 0.8f, 0.8f, 1f);

        // Create button container
        GameObject buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(contentPanel.transform, false);
        var buttonRect = buttonContainer.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0f);
        buttonRect.anchorMax = new Vector2(0.5f, 0f);
        buttonRect.sizeDelta = new Vector2(580, 50);
        buttonRect.anchoredPosition = new Vector2(0, 10);

        // Create buttons
        var retryButton = CreateButton("RetryButton", "Retry", new Vector2(-150, 0));
        retryButton.transform.SetParent(buttonContainer.transform, false);

        var goToLoginButton = CreateButton("GoToLoginButton", "Go to Login", new Vector2(0, 0));
        goToLoginButton.transform.SetParent(buttonContainer.transform, false);

        var dismissButton = CreateButton("DismissButton", "Dismiss", new Vector2(150, 0));
        dismissButton.transform.SetParent(buttonContainer.transform, false);

        // Assign references to GlobalErrorHandler using reflection or manual assignment
        // Note: In a real scenario, you might want to use a different approach
        // For now, the user should manually assign these in the inspector

        LoggerService.Info("[GlobalErrorHandlerUI] Error UI structure created. Please assign references in GlobalErrorHandler inspector.");
    }

    private GameObject CreateText(string name, string text, int fontSize, FontStyle style)
    {
        GameObject textObj = new GameObject(name);
        var textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = fontSize;
        textComponent.fontStyle = style;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleCenter;

        return textObj;
    }

    private GameObject CreateButton(string name, string buttonText, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name);
        var image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.3f, 0.3f, 0.3f, 1f);

        var button = buttonObj.AddComponent<Button>();
        var colors = button.colors;
        colors.normalColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        colors.highlightedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        button.colors = colors;

        var rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(120, 40);
        rectTransform.anchoredPosition = position;

        // Add text to button
        GameObject textObj = CreateText("Text", buttonText, 14, FontStyle.Normal);
        textObj.transform.SetParent(buttonObj.transform, false);
        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        return buttonObj;
    }
}
