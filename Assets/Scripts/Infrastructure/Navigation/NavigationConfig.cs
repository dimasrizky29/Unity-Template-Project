using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable]
public class RouteDefinition
{
    public AppRoute Route;                  // Enum tujuan (misal: AppRoute.Shop)
    public RouteFlags Flags;                // Flag, aturan (misal: RequiresAuth, IsAdditive)
    public NavType Type;                    // Tipe navigasi
    public string TargetScene;              // Nama scene (jika butuh pindah scene)
    public AssetReference Prefab;           // Referensi Addressable (jika tipe Window/Popup)

    // Helper method agar kode di Service tetap enak dibaca
    public bool HasFlag(RouteFlags flag) => (Flags & flag) != 0;
}

[CreateAssetMenu(fileName = "NavigationConfig", menuName = "Service/Configs/Navigation")]
public class NavigationConfig : ScriptableObject
{
    public List<RouteDefinition> Routes;

    // Helper untuk mengubah List jadi Dictionary saat Runtime biar cepat (O(1))
    public Dictionary<AppRoute, RouteDefinition> ToDictionary()
    {
        var dict = new Dictionary<AppRoute, RouteDefinition>();
        foreach (var r in Routes) dict[r.Route] = r;
        return dict;
    }
}