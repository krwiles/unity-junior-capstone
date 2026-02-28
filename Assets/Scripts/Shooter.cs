using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GenericPool<Projectile> _pool;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private GameObject _poolContainer;

    [SerializeField] private Transform _bulletOrigin;
    [SerializeField] private float _fireRate = 1f;

    private float _nextFireTime;

    void Awake()
    {
        if (_bulletOrigin == null)
        {
            Debug.LogWarning("BulletOrigin not found for shooter: " + gameObject.name);
        }    

        _pool = new GenericPool<Projectile>(_projectilePrefab, _poolContainer.transform);
        _pool.Warm(10);
    }

    public void TryFire()
    {
        if (Time.time < _nextFireTime) return;
        _nextFireTime = Time.time + 1f / _fireRate;
        SpawnProjectile();
    }

    private Projectile SpawnProjectile()
    {
        Projectile projectile = _pool.Get();
        projectile.Construct(_pool);
        projectile.transform.position = _bulletOrigin.position;
        projectile.transform.rotation = _bulletOrigin.rotation;
        return projectile;
    }
}
