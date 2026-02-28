using System;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GenericPool<Enemy> _pool;
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private GameObject _poolContainer;
    [SerializeField] private PathNode _nodeHead;


    private void Start()
    {
        _pool = new GenericPool<Enemy>(_enemyPrefab, _poolContainer.transform);
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
        enemy.Construct(_pool, _nodeHead);
        enemy.transform.position = spawnLocation;
        return enemy;
    }
}
