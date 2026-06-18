#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class SceneCompositionRoot : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] _spawnSourceBehaviours = Array.Empty<MonoBehaviour>();
    [SerializeField] private GameHudPresenter _gameHudPresenter;

    private EnemyEventBridge? _enemyEventBridge;

    private IGameStateService? _gameStateService;
    private ICurrencyService? _currencyService;


    private void Awake()
    {
        var spawnSources = ValidateAndCastSpawners();
        _enemyEventBridge = new EnemyEventBridge(spawnSources);

        _gameStateService = new GameStateService();
        _currencyService = new CurrencyService();

        // Subscribe services to enemy events
        _enemyEventBridge.OnEnemyCompletedRoute += EnemyCompletedRoute;
        _enemyEventBridge.OnEnemyDied += EnemyDied;

        _gameHudPresenter.Initialize(_gameStateService, _currencyService);
    }

    private void OnDestroy()
    {
        if (_enemyEventBridge != null)
        {
            _enemyEventBridge.OnEnemyCompletedRoute -= EnemyCompletedRoute;
            _enemyEventBridge.OnEnemyDied -= EnemyDied;
            _enemyEventBridge.Dispose(); // make sure references are cleaned up
        }
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

    private void EnemyCompletedRoute(Enemy enemy)
    {
        _gameStateService?.DamageBase(enemy.Damage);
    }

    private void EnemyDied(Enemy enemy) 
    {
        _currencyService?.Add(enemy.Value);
        _gameStateService?.AddScore(enemy.Score);
    }
}
