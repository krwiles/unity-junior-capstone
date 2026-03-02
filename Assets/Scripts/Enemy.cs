#nullable enable
using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour, IPoolable<Enemy>
{
    [SerializeField] private PathNode? _target;
    [SerializeField] private float _speed;
    [SerializeField] private float _reachRadius;

    [SerializeField] private float _distanceToGoal;
    public float DistanceToGoal => _distanceToGoal;

    private Health _health = null!;
    private GenericPool<Enemy>? _pool;

    public event Action<Enemy>? OnDied;
    public event Action<Enemy>? OnCompletedRoute;


    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        _health.OnDied += HandleDied; 
    }

    private void OnDisable()
    {
        _health.OnDied -= HandleDied; // lifecycle management
    }

    public void SetPool(GenericPool<Enemy> pool)
    {
        _pool = pool;
    }

    public void InitializeRoute(PathNode nodeHead)
    {
        _target = nodeHead;
    }

    public void Update()
    {
        // check if end of path
        if (_target == null) 
        {
            OnCompletedRoute?.Invoke(this);
            _pool?.Return(this);
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
            _target = _target.NextNode; // set next node
        }

        _distanceToGoal = _target.DistanceToGoal + distanceToTarget;
    }

    public void ResetState()
    {
        _target = null;
        OnDied = null;
        OnCompletedRoute = null;
    }

    private void HandleDied(Health _)
    {
        OnDied?.Invoke(this);
        _pool?.Return(this);
    }
}
