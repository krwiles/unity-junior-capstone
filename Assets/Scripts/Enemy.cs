#nullable enable
using System;
using Unity.Properties;
using UnityEngine;

public class Enemy : MonoBehaviour, IPoolable
{
    [SerializeField] private GenericPool<Enemy>? _enemyPool;
    [SerializeField] private PathNode? _target;
    [SerializeField] private float _speed;
    [SerializeField] private float _reachRadius;
    [SerializeField] private int _health;

    [SerializeField]
    private float _distanceToGoal;
    public float DistanceToGoal 
    { 
        get { return _distanceToGoal; } 
    }

    public event Action<Enemy>? OnDied;
    public event Action<Enemy>? OnCompletedRoute;

    public void Construct(GenericPool<Enemy> pool, PathNode nodeHead)
    {
        _enemyPool = pool;
        _target = nodeHead;
    }

    public void Update()
    {
        // check if end of path
        if (_target == null) 
        {
            _enemyPool.Return(this);
        } 
        else
        {
            Move();
        }
    }

    private void Move()
    {
        if (_target == null) return;

        // TODO: calculate movement budget per frame, if bigger than difference to target, 
        // set position at target, change next target, calculate remaining movement, MoveAlong vector to next target
        
        Vector3 differenceToTarget = _target.gameObject.transform.position - transform.position;
        transform.Translate(_speed * Time.deltaTime * differenceToTarget.normalized);
        float distanceToTarget = differenceToTarget.magnitude;
        
        // check if target is reached
        if (distanceToTarget <= _reachRadius)
        {
            _target = _target.GetNextNode(); // set next node
        }

        _distanceToGoal = _target.DistanceToGoal + distanceToTarget;
    }

    public void ResetState()
    {
        _target = null;
        OnDied = null;
        OnCompletedRoute = null;
    }
}
