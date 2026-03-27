using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(CanvasGroup))]
public abstract class BasePagePanel : MonoBehaviour, IPagePanel
{
    [Header("Transition Settings")]
    [SerializeField] protected float fadeDuration = 0.15f;

    protected string _defaultSubPanelId = "Main";

    protected CanvasGroup _canvasGroup;
    protected Dictionary<string, ISubPanel> _subPanelCache = new();
    protected Stack<string> _history = new();
    protected string _currentSubId;

    [Inject] protected INavigationService _navigationService;
    [Inject] protected IAudioService _audioService;

    public bool IsActive => gameObject.activeSelf && _canvasGroup != null && _canvasGroup.alpha > 0.9f;

    protected virtual void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        // --- AUTO DISCOVERY ---
        EnsureSubPanelsInitialized();
    }

    protected void EnsureSubPanelsInitialized()
    {
        if (_subPanelCache.Count > 0) return;

        var children = GetComponentsInChildren<ISubPanel>(true);
        foreach (var sub in children)
        {
            if (!_subPanelCache.ContainsKey(sub.SubPanelId))
            {
                sub.Initialize(this);
                _subPanelCache.Add(sub.SubPanelId, sub);
                sub.SetVisibilityImmediate(false);
            }
        }
    }

    public virtual void SetPayload(object payload)
    {
        EnsureSubPanelsInitialized();
        LoggerService.LogDebug($"[BasePagePanel] Payload: {payload}");

        // Jika tidak ada sub-panel, abaikan payload ini
        if (_subPanelCache.Count == 0) return;

        string targetId = (payload is string s) ? s : _defaultSubPanelId;
        if (!string.IsNullOrEmpty(targetId))
        {
            SwitchSubPanel(targetId, true);
        }
    }

    public virtual async UniTask Show(CancellationToken token = default)
    {
        gameObject.SetActive(true);
        _canvasGroup.blocksRaycasts = true;

        // Jika ada sub-panel default, pastikan dia siap sebelum fade
        if (!string.IsNullOrEmpty(_defaultSubPanelId) && string.IsNullOrEmpty(_currentSubId))
        {
            SwitchSubPanel(_defaultSubPanelId, false);
        }

        await Fade(1, token);
        _canvasGroup.interactable = true;
    }

    public virtual async UniTask Hide(CancellationToken token = default)
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        await Fade(0, token);

        _history.Clear();
        _currentSubId = null;
        gameObject.SetActive(false);
    }

    public virtual void SetVisibilityImmediate(bool isVisible)
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = isVisible ? 1 : 0;
        _canvasGroup.blocksRaycasts = isVisible;
        _canvasGroup.interactable = isVisible;
        gameObject.SetActive(isVisible);
    }

    public virtual void SwitchSubPanel(string subPanelId, bool addToHistory = true)
    {
        // Jika ID adalah "Main", kita anggap ini kembali ke tampilan awal (tanpa sub-panel)
        if (subPanelId == _defaultSubPanelId)
        {
            if (addToHistory && !string.IsNullOrEmpty(_currentSubId))
            {
                _history.Push(_currentSubId);
            }

            // Matikan semua sub-panel yang terdaftar
            foreach (var sub in _subPanelCache.Values) sub.Hide().Forget();
            _currentSubId = _defaultSubPanelId;
            return;
        }

        if (!_subPanelCache.ContainsKey(subPanelId))
        {
            LoggerService.Warning($"SubPanel ID '{subPanelId}' tidak ditemukan di cache!");
            return;
        }

        if (addToHistory && _currentSubId != subPanelId)
        {
            // Jika sebelumnya null atau Main, push "Main" ke history
            _history.Push(string.IsNullOrEmpty(_currentSubId) ? _defaultSubPanelId : _currentSubId);
        }

        foreach (var sub in _subPanelCache.Values)
        {
            if (sub.SubPanelId == subPanelId) sub.Show().Forget();
            else sub.Hide().Forget();
        }

        _currentSubId = subPanelId;
    }

    public virtual void Back()
    {
        // sound close
        _audioService.PlayOneShot("ButtonClose");

        LoggerService.LogDebug($"Back Pressed. Current History Count: {_history.Count}");

        if (_history.Count > 0)
        {
            string previousId = _history.Pop();
            SwitchSubPanel(previousId, false);
        }
        else
        {
            // Jika history benar-benar habis, baru kembali ke Route sebelumnya
            LoggerService.LogDebug("History empty, navigating back to previous route.");
            _navigationService.GoBack().Forget();
        }
    }

    public virtual bool TryBackSubPanel()
    {
        if (_history.Count > 0)
        {
            // sound close
            _audioService.PlayOneShot("ButtonClose");

            string previousId = _history.Pop();
            SwitchSubPanel(previousId, false);
            return true; // Berhasil menutup sub-panel
        }

        // Jika history sub-panel habis, kembalikan false.
        // Kita TIDAK memanggil GoBack() di sini. Biarkan layer di atasnya yang urus.
        return false;
    }

    private async UniTask Fade(float targetAlpha, CancellationToken token)
    {
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
}