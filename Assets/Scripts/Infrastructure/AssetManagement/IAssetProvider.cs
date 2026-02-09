using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IAssetProvider
{
    // Memuat aset ke memori (tanpa spawn), misal: Audio, ScriptableObject
    UniTask<T> LoadAssetAsync<T>(string key);

    // Memuat PREFAB dan langsung menjadikannya GameObject di scene
    UniTask<GameObject> InstantiateAsync(string key, Vector3 position, Quaternion rotation, Transform parent = null);

    // Membersihkan memori
    void ReleaseAsset<T>(T asset);
    void ReleaseInstance(GameObject instance);
}