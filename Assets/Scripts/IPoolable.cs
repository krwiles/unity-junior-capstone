using UnityEngine;

/// <summary>
/// Defines behavior required for objects managed by a pool.
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// Resets this object to its initial conditions after it is returned to the pool,
    /// so it is ready for the next reuse cycle.
    /// </summary>
    public void ResetState();
}
