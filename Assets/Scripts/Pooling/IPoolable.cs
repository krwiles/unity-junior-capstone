using UnityEngine;

/// <summary>
/// Defines behavior required for objects managed by a pool.
/// </summary>
public interface IPoolable<T> where T : MonoBehaviour, IPoolable<T>
{
    /// <summary>
    /// Injects the owning pool reference once when the object is created.
    /// </summary>
    /// <param name="pool">Owning pool.</param>
    public void SetPool(GenericPool<T> pool);

    /// <summary>
    /// Resets this object to its initial conditions after it is returned to the pool,
    /// so it is ready for the next reuse cycle.
    /// </summary>
    public void ResetState();
}
