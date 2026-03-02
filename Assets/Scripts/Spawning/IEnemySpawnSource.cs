#nullable enable

using System;

public interface IEnemySpawnSource
{
    public event Action<Enemy>? OnEnemySpawned;
    public Enemy SpawnEnemy();
}
