using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private Transform _turret;
    private readonly HashSet<Enemy> _enemiesInRange = new();
    [SerializeField] private int _enemyLayer = 6;
    private Enemy _target;
    [SerializeField] private float _turnSpeed = 700f;
    private Shooter _shooterComponent;

    [SerializeField] private float _angleTolerance = 5f;

    
    public void Awake()
    {
        if (_turret == null)
        {
            Debug.LogWarning("Turret child not found.");
        }
        
        _shooterComponent = GetComponent<Shooter>();
    }

    public void Update()
    {        
        UpdateTarget();
        if (_target != null)
        {
            bool isFacingTarget = FaceTarget();
            if (isFacingTarget)
            {
                _shooterComponent.TryFire();
            }
        }
    }

    private void UpdateTarget()
    {
        _enemiesInRange.RemoveWhere(enemy => enemy == null || !enemy.gameObject.activeInHierarchy);

        float closestDistanceToGoal = float.MaxValue;
        _target = null;

        foreach (Enemy enemy in _enemiesInRange)
        {
            if (enemy.DistanceToGoal < closestDistanceToGoal)
            {
                closestDistanceToGoal = enemy.DistanceToGoal;
                _target = enemy;
            }
        }
    }

    private bool FaceTarget()
    {
        Vector2 dir = _target.transform.position - _turret.position;

        float targetZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        float currentZ = _turret.eulerAngles.z;
        float newZ = Mathf.MoveTowardsAngle(currentZ, targetZ, _turnSpeed * Time.deltaTime);

        _turret.rotation = Quaternion.Euler(0f, 0f, newZ);

        bool isFacingTarget = Mathf.Abs(Mathf.DeltaAngle(targetZ, newZ)) < _angleTolerance;
        return isFacingTarget;
    }


    public void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.layer != _enemyLayer) return;
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            _enemiesInRange.Add(enemy);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject.layer != _enemyLayer) return;
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            _enemiesInRange.Remove(enemy);
        }
    }

}
