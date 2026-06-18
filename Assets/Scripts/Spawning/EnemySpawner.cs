#nullable enable

using System;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, IEnemySpawnSource
{
    public float TEMP_spawnSpeed = 0.5f;

    [SerializeField] private Enemy? _enemyPrefab;
    [SerializeField] private PathNode? _nodeHead;
    
    private GenericPool<Enemy>? _pool;

    public event Action<Enemy>? OnEnemySpawned;


    private void Awake()
    {
        if (_enemyPrefab == null || _nodeHead == null)
        {
            Debug.LogError($"{nameof(EnemySpawner)} on '{name}' is missing required references. Assign {nameof(_enemyPrefab)} and {nameof(_nodeHead)} in the Inspector.", this);
            enabled = false;
            return;
        }

        _pool = new GenericPool<Enemy>(_enemyPrefab, transform);
        _pool.Warm(100);

        _nodeHead.CalculateDistanceToGoal(); // Initialize nodes
        
        InvokeRepeating(nameof(SpawnEnemy), 1f, TEMP_spawnSpeed);
    }

    public Enemy SpawnEnemy()
    {
        if (_nodeHead == null)
        {
            throw new InvalidOperationException($"{nameof(EnemySpawner)} is not initialized. Ensure {nameof(Awake)} has run and serialized fields are assigned.");
        }

        return SpawnEnemy(_nodeHead.gameObject.transform.position);
    }

    public Enemy SpawnEnemy(Vector3 spawnLocation)
    {
        if (_pool == null || _nodeHead == null)
        {
            throw new InvalidOperationException($"{nameof(EnemySpawner)} is not initialized. Ensure {nameof(Awake)} has run and serialized fields are assigned.");
        }

        Enemy enemy = _pool.Get();
        enemy.InitializeRoute(_nodeHead);
        enemy.transform.position = spawnLocation;
        
        OnEnemySpawned?.Invoke(enemy);
        return enemy;
    }
}
