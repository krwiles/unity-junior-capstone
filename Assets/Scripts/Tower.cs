using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private Transform _turret;
    private HashSet<Enemy> _enemiesInRange = new();
    [SerializeField] private int _enemyLayer = 6;
    private Enemy _target;
    [SerializeField] private float _turnSpeed = 500f;
    private Shooter _shooterComponent;

    
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
            FaceTarget();
            _shooterComponent.TryFire();
        }
    }

    private void UpdateTarget()
    {
        if (_enemiesInRange.Count == 0)
        {
            _target = null;
            return;
        }
        else
        {
            float closestDistanceToGoal = float.MaxValue;
            foreach (Enemy enemy in _enemiesInRange)
            {
                if (enemy.DistanceToGoal < closestDistanceToGoal)
                {
                    closestDistanceToGoal = enemy.DistanceToGoal;
                    _target = enemy;
                }
            }
        }
    }

    private void FaceTarget()
    {
        Vector2 dir = _target.transform.position - _turret.position;

        float targetZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        float currentZ = _turret.eulerAngles.z;
        float newZ = Mathf.MoveTowardsAngle(currentZ, targetZ, _turnSpeed * Time.deltaTime);

        _turret.rotation = Quaternion.Euler(0f, 0f, newZ);
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
