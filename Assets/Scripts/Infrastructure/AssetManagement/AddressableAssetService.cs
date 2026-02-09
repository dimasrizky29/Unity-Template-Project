using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

public class AddressableAssetService : IAssetProvider
{
    // Kita perlu menyimpan Handle untuk setiap instance agar bisa di-release dengan benar nanti
    private readonly Dictionary<GameObject, AsyncOperationHandle> _instantiatedObjects = new();
    private readonly Dictionary<object, AsyncOperationHandle> _loadedAssets = new();

    public async UniTask<T> LoadAssetAsync<T>(string key)
    {
        // Validasi Key
        if (string.IsNullOrEmpty(key))
        {
            LoggerService.Error("[AssetService] Key cannot be null or empty");
            return default;
        }

        try
        {
            var handle = Addressables.LoadAssetAsync<T>(key);
            T result = await handle.ToUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // Cache handle untuk keperluan release nanti
                if (!_loadedAssets.ContainsKey(result))
                {
                    _loadedAssets.Add(result, handle);
                }
                return result;
            }

            LoggerService.Error($"[AssetService] Failed to load asset: {key}");
            return default;
        }
        catch (System.Exception e)
        {
            LoggerService.Error($"[AssetService] Exception: {e.Message}");
            return default;
        }
    }

    public async UniTask<GameObject> InstantiateAsync(string key, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        try
        {
            // InstantiateAsync dari Addressables mengembalikan Handle<GameObject>
            var handle = Addressables.InstantiateAsync(key, position, rotation, parent);
            GameObject instance = await handle.ToUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // PENTING: Simpan mapping antara GameObject dan Handle-nya
                _instantiatedObjects.Add(instance, handle);
                return instance;
            }

            LoggerService.Error($"[AssetService] Failed to instantiate: {key}");
            return null;
        }
        catch (System.Exception e)
        {
            LoggerService.Error($"[AssetService] Exception: {e.Message}");
            return null;
        }
    }

    public void ReleaseAsset<T>(T asset)
    {
        if (asset == null) return;

        if (_loadedAssets.TryGetValue(asset, out var handle))
        {
            Addressables.Release(handle);
            _loadedAssets.Remove(asset);
            LoggerService.LogDebug($"[AssetService] Asset released: {asset}");
        }
    }

    public void ReleaseInstance(GameObject instance)
    {
        if (instance == null) return;

        if (_instantiatedObjects.TryGetValue(instance, out var handle))
        {
            // Addressables.ReleaseInstance otomatis menghancurkan GameObject dan mengurangi Ref Count
            Addressables.ReleaseInstance(instance);
            _instantiatedObjects.Remove(instance);
            LoggerService.LogDebug($"[AssetService] Instance released: {instance.name}");
        }
        else
        {
            // Fallback jika object bukan dari Addressables (misal Destroy biasa)
            Object.Destroy(instance);
            LoggerService.Warning($"[AssetService] Instance {instance.name} not found in cache, destroyed manually.");
        }
    }
}