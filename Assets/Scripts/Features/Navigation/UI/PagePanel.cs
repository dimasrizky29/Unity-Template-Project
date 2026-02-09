using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Optimized UI Panel using CanvasGroup.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class PagePanel : MonoBehaviour, IViewPanel
{
    [SerializeField] private float fadeDuration = 0.15f;
    private CanvasGroup _canvasGroup;

    // Properti Interface
    public bool IsActive => gameObject.activeSelf && _canvasGroup != null && _canvasGroup.alpha > 0.9f;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        // Pastikan kondisi awal konsisten
        if (!gameObject.activeSelf) _canvasGroup.alpha = 0;
    }

    public void SetVisibilityImmediate(bool isVisible)
    {
        if(_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = isVisible ? 1 : 0;
        _canvasGroup.blocksRaycasts = isVisible;
        _canvasGroup.interactable = isVisible;
        gameObject.SetActive(isVisible);
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

    private async UniTask Fade(float targetAlpha, CancellationToken token)
    {
        float startAlpha = _canvasGroup.alpha;
        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            // Cek apakah navigasi dibatalkan/pindah scene?
            if (token.IsCancellationRequested) return;

            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        _canvasGroup.alpha = targetAlpha;
    }
}