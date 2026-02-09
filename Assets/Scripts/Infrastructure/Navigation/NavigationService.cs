using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class NavigationService : INavigationService, IDisposable
{
    private readonly Dictionary<AppRoute, RouteDefinition> _routeConfig;
    private readonly IGlobalUIService _uiManager;
    private readonly IAuthStore _authStore;

    // Cache untuk Scene View (Local)
    private ScenePageContainer _currentSceneContainer;
    private UniTaskCompletionSource<ScenePageContainer> _sceneLoadTcs;
    private AppRoute _currentRoute = AppRoute.Startup; // Track route saat ini

    private CancellationTokenSource _navCts;

    // Cache untuk Window/Popup yang sudah di-spawn (biar gak spawn double)
    private Dictionary<AppRoute, IViewPanel> _activeWindows = new();
    private readonly Stack<AppRoute> _history = new Stack<AppRoute>();

    private AppRoute? _pendingRoute; // Menggunakan nullable agar bisa kosong
    private bool _isGoingBack; // Flag agar saat 'Back', kita tidak menambah histori baru

    public NavigationService(NavigationConfig config, IGlobalUIService uiManager, IAuthStore authStore)
    {
        _routeConfig = config.ToDictionary();
        _uiManager = uiManager;
        _authStore = authStore;

        // Ganti event static dengan inject IAuthStore
        _authStore.OnSessionChanged += HandleSessionChanged;
    }

    // Method untuk Scene lokal mendaftarkan UI-nya
    public void RegisterView(ScenePageContainer container)
    {
        _currentSceneContainer = container;
        _sceneLoadTcs?.TrySetResult(container); // Beritahu NavigateTo bahwa scene sudah siap
    }

    public async UniTask NavigateTo(AppRoute route)
    {
        // Cek Config
        if (!_routeConfig.TryGetValue(route, out var config))
        {
            LoggerService.Error($"[Nav] Route {route} tidak terdaftar di Config!");
            return;
        }

        // --- LOGIC ROOT NAVIGATION ---
        if (config.HasFlag(RouteFlags.ClearHistory))
        {
            LoggerService.LogDebug($"[Nav] {route} is a Root Route. Clearing history stack.");
            _history.Clear();
        }

        // Auth Guard
        if (config.HasFlag(RouteFlags.RequiresAuth) && !_authStore.IsLoggedIn)
        {
            // SIMPAN RUTE TUJUAN: "Oke, nanti kalau sudah login, saya antar kamu ke sini"
            _pendingRoute = route;

            LoggerService.LogDebug($"Access denied to {route}. Redirecting to Login...");
            await NavigateTo(AppRoute.Login);
            return;
        }

        // Logic Histori Management
        // Cek Transient
        if (!_isGoingBack && !config.HasFlag(RouteFlags.IsTransient))
        {
            // Jangan push rute yang sama dua kali berturut-turut
            if (_history.Count == 0 || _history.Peek() != _currentRoute)
                _history.Push(_currentRoute);
        }
        _isGoingBack = false;

        // Cancel navigasi sebelumnya jika ada
        _navCts?.Cancel();
        _navCts = new CancellationTokenSource();
        var token = _navCts.Token;

        try
        {
            // 4. Close previous UI if NOT Additive
            if (!config.HasFlag(RouteFlags.IsAdditive))
            {
                await CloseCurrentState(token);
            }

            // LOGIKA UTAMA
            if (config.Type == NavType.ScenePage) // Scene Page (Local)
            {
                await HandleSceneNavigation(config, token);
            }
            else // Window / Popup (Addressables)
                await HandleWindowNavigation(config, token);

            _currentRoute = route; // Update state

            LoggerService.LogDebug($"[Nav] Navigated to {route}");
        }
        catch (OperationCanceledException)
        {
            // Handle jika navigasi dibatalkan
        }
        catch (Exception e) { LoggerService.Exception(e); }
    }

    private async UniTask HandleSceneNavigation(RouteDefinition config, CancellationToken token)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // A. Pindah Scene jika perlu
        if (currentScene != config.TargetScene)
        {
            _currentSceneContainer = null;
            _sceneLoadTcs = new UniTaskCompletionSource<ScenePageContainer>();

            await SceneManager.LoadSceneAsync(config.TargetScene).ToUniTask(cancellationToken: token);
            await _sceneLoadTcs.Task; // Tunggu RegisterView
        }
        
        // B. Tampilkan Panel Local
        if (_currentSceneContainer != null)
        {
            var targetPanel = _currentSceneContainer.GetPanel(config.Route);
            if (targetPanel != null)
                await targetPanel.Show(token);
            else
                Debug.LogError($"[Nav] Panel untuk route {config.Route} tidak ditemukan di scene {config.TargetScene}");
        }
    }

    private async UniTask HandleWindowNavigation(RouteDefinition config, CancellationToken token)
    {
        // 1. Cek Cache (Apakah sudah pernah dibuka?)
        if (!_activeWindows.TryGetValue(config.Route, out var panel)) // Belum ada di cache
        {
            var prefabObj = await config.Prefab.LoadAssetAsync<GameObject>().Task;
            if (token.IsCancellationRequested) return;

            var layer = _uiManager.GetLayer(config.Type);
            var instance = GameObject.Instantiate(prefabObj, layer);

            if (instance.TryGetComponent<IViewPanel>(out panel))
                _activeWindows.Add(config.Route, panel);
        }

        if (panel != null) await panel.Show(token);
    }

    private async UniTask CloseCurrentState(CancellationToken token)
    {
        // Tutup semua window addressable yang sedang aktif
        foreach (var window in _activeWindows.Values)
        {
            if (window.IsActive) await window.Hide(token);
        }

        // Matikan semua panel lokal di scene
        if (_currentSceneContainer != null)
            _currentSceneContainer.HideAllPanels();
    }

    public async UniTask GoBack()
    {
        if (_history.Count == 0)
        {
            LoggerService.LogDebug("No history to go back to.");
            return;
        }

        // Jika rute saat ini adalah ADDITIVE, kita cukup menutupnya tanpa pindah rute
        if (_routeConfig.TryGetValue(_currentRoute, out var config) && config.HasFlag(RouteFlags.IsAdditive))
        {
            var currentPanel = GetPanelInstance(_currentRoute);
            if (currentPanel != null) await currentPanel.Hide();

            _currentRoute = _history.Pop(); // Balik ke state sebelumnya tanpa NavigateTo
            return;
        }

        _isGoingBack = true;
        await NavigateTo(_history.Pop());
    }

    // Helper untuk mencari instance panel di mana pun dia berada
    private IViewPanel GetPanelInstance(AppRoute route)
    {
        if (_activeWindows.TryGetValue(route, out var window)) return window;
        if (_currentSceneContainer != null) return _currentSceneContainer.GetPanel(route);
        return null;
    }

    private void HandleSessionChanged(UserSession session)
    {
        if (session.IsAuthenticated)
        {
            // LOGIN SUKSES: Cek apakah ada rute yang tertunda?
            if (_pendingRoute.HasValue)
            {
                AppRoute target = _pendingRoute.Value;
                _pendingRoute = null; // Hapus dari memori agar tidak looping

                LoggerService.LogDebug($"[Nav] Resuming pending route: {target}");
                NavigateTo(target).Forget(); // Lanjutkan ke tujuan awal
            }

            // Prioritas 2: Jika tidak ada janji, mungkin user baru login biasa, bawa ke Lobby
            else if (_currentRoute == AppRoute.Login)
            {
                NavigateTo(AppRoute.Lobby).Forget();
            }
        }
        else
        {
            // Logout logic
            _pendingRoute = null; // Bersihkan janji jika user memang sengaja logout
            if (_routeConfig.TryGetValue(_currentRoute, out var config) && config.HasFlag(RouteFlags.RequiresAuth))
            {
                NavigateTo(AppRoute.Login).Forget();
            }
        }
    }

    public void Dispose()
    {
        _authStore.OnSessionChanged -= HandleSessionChanged;
        _navCts?.Dispose();

        // Bersihkan Addressables memory jika perlu
        foreach (var key in _activeWindows.Keys) Addressables.Release(key);
    }

    public AppRoute CurrentRoute => _currentRoute;
}