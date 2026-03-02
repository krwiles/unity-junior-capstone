#nullable enable

using System;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, IEnemySpawnSource
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private PathNode _nodeHead;
    
    private GenericPool<Enemy> _pool;

    public event Action<Enemy>? OnEnemySpawned;


    private void Start()
    {
        _pool = new GenericPool<Enemy>(_enemyPrefab, gameObject.transform);
        _pool.Warm(100);

        _nodeHead.CalculateDistanceToGoal(); // Initialize nodes
        
        InvokeRepeating(nameof(SpawnEnemy), 1f, 0.5f);
    }

    public Enemy SpawnEnemy()
    {
        return SpawnEnemy(_nodeHead.gameObject.transform.position);
    }

    public Enemy SpawnEnemy(Vector3 spawnLocation)
    {
        Enemy enemy = _pool.Get();
        enemy.InitializeRoute(_nodeHead);
        enemy.transform.position = spawnLocation;
        
        OnEnemySpawned?.Invoke(enemy);
        return enemy;
    }
}
