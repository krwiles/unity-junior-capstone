using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generic object pool for <see cref="MonoBehaviour"/> instances that implement <see cref="IPoolable"/>.
/// Uses a queue for fast reuse and a set to guard against duplicate returns.
/// </summary>
/// <typeparam name="T">Pooled component type.</typeparam>
public class GenericPool<T> where T : MonoBehaviour, IPoolable
{
    private readonly T _prefab;
    private readonly Transform _poolParent;
    private readonly bool _activateOnGet;
    private readonly Queue<T> _pool = new();
    private readonly HashSet<T> _inPool = new();

    /// <summary>
    /// Creates a new pool.
    /// </summary>
    /// <param name="prefab">Prefab used to create pooled instances.</param>
    /// <param name="poolParent">Parent transform used to organize pooled instances.</param>
    /// <param name="activateOnGet">When true, objects are activated before being returned by <see cref="Get"/>.</param>
    public GenericPool(T prefab, Transform poolParent, bool activateOnGet = true)
    {
        if (prefab == null)
        {
            throw new System.ArgumentNullException(nameof(prefab));
        }

        if (poolParent == null)
        {
            throw new System.ArgumentNullException(nameof(poolParent));
        }

        _prefab = prefab;
        _poolParent = poolParent;
        _activateOnGet = activateOnGet;
    }

    /// <summary>
    /// Pre-allocates pooled instances.
    /// </summary>
    /// <param name="count">Number of instances to create.</param>
    public void Warm(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreateNew();
        }
    }

    private T CreateNew()
    {
        var obj = Object.Instantiate(_prefab, _poolParent);
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
        _inPool.Add(obj);
        return obj;
    }

    /// <summary>
    /// Gets an instance from the pool, creating one if needed.
    /// </summary>
    /// <returns>A pooled instance.</returns>
    public T Get()
    {
        if (_pool.Count == 0)
        {
            CreateNew();
        }

        var obj = _pool.Dequeue();
        _inPool.Remove(obj);

        if (_activateOnGet)
        {
            obj.gameObject.SetActive(true);
        }

        return obj;
    }

    /// <summary>
    /// Returns an instance to the pool after resetting its state.
    /// Invalid or duplicate returns are ignored.
    /// </summary>
    /// <param name="obj">Instance to return.</param>
    public void Return(T obj)
    {
        if (obj == null || _inPool.Contains(obj))
        {
            return;
        }

        obj.ResetState();
        obj.transform.SetParent(_poolParent);
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
        _inPool.Add(obj);
    }
}
