#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class SceneCompositionRoot : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] _spawnSourceBehaviours = Array.Empty<MonoBehaviour>();

    private EnemyEventBridge? _enemyEventBridge;


    private void Awake()
    {
        var spawnSources = ValidateAndCastSpawners();
        _enemyEventBridge = new EnemyEventBridge(spawnSources);
        _enemyEventBridge.OnEnemyDied += EnemyDied; // Testing
    }

    private void OnDestroy()
    {
        _enemyEventBridge?.Dispose(); // make sure references are cleaned up
        _enemyEventBridge = null;
    }

    private List<IEnemySpawnSource> ValidateAndCastSpawners() 
    {
        var spawnSources = new List<IEnemySpawnSource>(_spawnSourceBehaviours.Length);

        foreach (MonoBehaviour behaviour in _spawnSourceBehaviours)
        {
            if (behaviour is IEnemySpawnSource spawnSource)
            {
                spawnSources.Add(spawnSource);
                continue;
            }

            if (behaviour != null)
            {
                Debug.LogError($"{nameof(SceneCompositionRoot)}: '{behaviour.name}' does not implement {nameof(IEnemySpawnSource)}.", behaviour);
            }
        }

        return spawnSources;
    }

    // Testing functionality
    private int enemiesDead = 0;
    private void EnemyDied(Enemy enemy) 
    {
        enemiesDead += 1;
        Debug.Log("Enemies dead: " + enemiesDead);
    }
}
