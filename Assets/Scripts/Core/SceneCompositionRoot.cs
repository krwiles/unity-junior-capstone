#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class SceneCompositionRoot : MonoBehaviour
{
    [SerializeField] private EnemySpawner[] _enemySpawners = Array.Empty<EnemySpawner>();

    private EnemyEventBridge? _enemyEventBridge;

    private void Awake()
    {
        var spawnSources = new List<IEnemySpawnSource>(_enemySpawners.Length);

        foreach (EnemySpawner spawner in _enemySpawners)
        {
            spawnSources.Add(spawner);
        }

        _enemyEventBridge = new EnemyEventBridge(spawnSources);
        _enemyEventBridge.OnEnemyDied += EnemyDied; // Testing
    }

    private void OnDestroy()
    {
        _enemyEventBridge?.Dispose(); // make sure references are cleaned up
        _enemyEventBridge = null;
    }


    // Testing functionality
    private int enemiesDead = 0;
    private void EnemyDied(Enemy enemy) 
    {
        enemiesDead += 1;
        Debug.Log("Enemies dead: " + enemiesDead);
    }
}
