using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[RequireComponent(typeof(CanvasGroup))]
public class BaseSubPanel : MonoBehaviour, ISubPanel
{
    [Header("Sub Panel Settings")]
    [SerializeField] private string _subPanelId;
    [SerializeField] protected float fadeDuration = 0.1f; // Tab biasanya lebih cepat dari Page

    public string SubPanelId => _subPanelId;

    protected IPagePanel _parent;
    protected CanvasGroup _canvasGroup;

    public bool IsActive => gameObject.activeSelf && _canvasGroup != null && _canvasGroup.alpha > 0.9f;

    public void Initialize(IPagePanel parent)
    {
        _parent = parent;
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public async UniTask Show(CancellationToken token = default)
    {
        gameObject.SetActive(true);
        _canvasGroup.blocksRaycasts = true;

        await Fade(1, token);

        _canvasGroup.interactable = true;
    }

    public async UniTask Hide(CancellationToken token = default)
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        await Fade(0, token);

        gameObject.SetActive(false);
    }

    public void SetVisibilityImmediate(bool isVisible)
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = isVisible ? 1 : 0;
        _canvasGroup.blocksRaycasts = isVisible;
        _canvasGroup.interactable = isVisible;
        gameObject.SetActive(isVisible);
    }

    // Logic Fade yang sama dengan BasePagePanel agar konsisten
    protected async UniTask Fade(float targetAlpha, CancellationToken token)
    {
        if (_canvasGroup == null) return;

        float startAlpha = _canvasGroup.alpha;
        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            if (token.IsCancellationRequested) return;

            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
        _canvasGroup.alpha = targetAlpha;
    }

    public void SetPayload(object payload)
    {
        // Bisa di-override oleh class child jika butuh data spesifik
    }
}