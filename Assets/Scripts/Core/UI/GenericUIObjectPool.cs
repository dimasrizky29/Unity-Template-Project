using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GenericUIObjectPool<T> where T : Component
{
    private readonly T _prefab;
    private readonly Transform _container;
    private readonly ObjectPool<T> _pool;
    private readonly List<T> _activeItems = new();

    public List<T> ActiveItems => _activeItems;

    public GenericUIObjectPool(T prefab, Transform container, int defaultCapacity = 10, int maxSize = 20)
    {
        _prefab = prefab;
        _container = container;

        _pool = new ObjectPool<T>(
            createFunc: CreateItem,
            actionOnGet: OnGetItem,
            actionOnRelease: OnReleaseItem,
            actionOnDestroy: OnDestroyItem,
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    private T CreateItem() => Object.Instantiate(_prefab, _container);
    private void OnGetItem(T item) => item.gameObject.SetActive(true);
    private void OnReleaseItem(T item) => item.gameObject.SetActive(false);
    private void OnDestroyItem(T item) => Object.Destroy(item.gameObject);

    public void Prepare(int count)
    {
        // Opsional: Buat item di awal agar tidak ada spike saat pertama kali dibuka
        List<T> temp = new();
        for (int i = 0; i < count; i++) temp.Add(_pool.Get());
        foreach (var item in temp) _pool.Release(item);
    }

    public T Get()
    {
        T item = _pool.Get();
        _activeItems.Add(item);
        return item;
    }

    public void Release(T item)
    {
        if (_activeItems.Remove(item))
        {
            _pool.Release(item);
        }
    }

    public void ReleaseAll()
    {
        foreach (var item in _activeItems)
        {
            _pool.Release(item);
        }
        _activeItems.Clear();
    }
}