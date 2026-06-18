#nullable enable

using System;
using System.Collections.Generic;

/*
DI wiring (composition root)

Inject all spawners into WaveController as IReadOnlyList<IEnemySpawnSource>.
Inject same spawners into EnemyEventBridge so it can subscribe once at startup.
Inject IGameStateService / IScoreService / IBaseHealthService into GameServicesAdapter.
Keep WaveController unaware of game services and keep services unaware of concrete spawners.
*/

public class EnemyEventBridge : IDisposable
{
    private readonly IReadOnlyList<IEnemySpawnSource> _enemySpawners;
    private readonly HashSet<Enemy> _boundEnemies = new();
    private bool _isDisposed;

    public event Action<Enemy>? OnEnemyDied;
    public event Action<Enemy>? OnEnemyCompletedRoute;


    public EnemyEventBridge(IReadOnlyList<IEnemySpawnSource> enemySpawners)
    {
        _enemySpawners = enemySpawners;

        foreach (IEnemySpawnSource spawner in _enemySpawners)
        {
            spawner.OnEnemySpawned += BridgeEnemy;
        }
    }

    // Dispose of all subscriptions and references
    public void Dispose()
    {
        if (_isDisposed) return;

        foreach (IEnemySpawnSource spawner in _enemySpawners)
        {
            spawner.OnEnemySpawned -= BridgeEnemy;
        }

        foreach (Enemy enemy in _boundEnemies) // in case disposed of while enemies are active
        {
            enemy.OnDied -= EnemyDied;
            enemy.OnCompletedRoute -= EnemyCompletedRoute;
        }

        _boundEnemies.Clear();
        _isDisposed = true;
    }

    private void BridgeEnemy(Enemy enemy)
    {
        if (_isDisposed) return;
        if (_boundEnemies.Contains(enemy)) return;

        enemy.OnDied += EnemyDied;
        enemy.OnCompletedRoute += EnemyCompletedRoute;
        _boundEnemies.Add(enemy);
    }

    private void EnemyDied(Enemy enemy)
    {
        OnEnemyDied?.Invoke(enemy);
        enemy.OnDied -= EnemyDied;
        enemy.OnCompletedRoute -= EnemyCompletedRoute;
        _boundEnemies.Remove(enemy);
    }

    private void EnemyCompletedRoute(Enemy enemy)
    {
        OnEnemyCompletedRoute?.Invoke(enemy);
        enemy.OnDied -= EnemyDied;
        enemy.OnCompletedRoute -= EnemyCompletedRoute;
        _boundEnemies.Remove(enemy);
    }

}