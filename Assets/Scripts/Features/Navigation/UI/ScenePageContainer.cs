using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public class ScenePageContainer : MonoBehaviour
{
    [Serializable]
    public struct RoutePanelMap
    {
        public AppRoute route;
        public PagePanel panel;
    }

    [SerializeField] private List<RoutePanelMap> panels;

    // Dictionary menyimpan IViewPanel agar lebih fleksibel
    private Dictionary<AppRoute, IViewPanel> _map;
    private INavigationService _navService;

    // Awake jalan duluan: Siapkan data internal
    private void Awake()
    {
        // Konversi List ke Dictionary agar akses O(1)
        _map = new Dictionary<AppRoute, IViewPanel>();
        foreach (var item in panels)
        {
            if (item.panel != null)
            {
                _map[item.route] = item.panel;
                // Matikan semua panel saat start (Clean slate)
                item.panel.SetVisibilityImmediate(false);
            }
        }
    }

    [Inject]
    public void Construct(INavigationService navService)
    {
        _navService = navService;
        _navService.RegisterView(this);
    }

    private void Start()
    {
        _navService.RegisterView(this);
    }

    public void HideAllPanels()
    {
        foreach (var p in _map.Values)
            p.SetVisibilityImmediate(false);
    }

    public IViewPanel GetPanel(AppRoute route)
    {
        return _map.GetValueOrDefault(route);
    }
}