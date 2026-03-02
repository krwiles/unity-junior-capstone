using System;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolable<Projectile>
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _lifetime = 3f;
    
    private GenericPool<Projectile>? _pool;
    private float _expire;


    public void SetPool(GenericPool<Projectile> pool)
    {
        _pool = pool;
    }

    public void OnEnable()
    {
        _expire = Time.time + _lifetime;
    }

    void Update()
    {
        if (Time.time > _expire)
        {
            _pool?.Return(this);
        }
        
        transform.Translate(_speed * Time.deltaTime * Vector3.up);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(_damage);
            _pool?.Return(this);
        }
    }

    public void ResetState()
    {
        // nothing to reset on bullet for now
    }

}
