using System;

public interface IGameStateService
{
    int BaseHealth { get; }
    int Score { get; }
    bool IsGameOver { get; }

    event Action<int> OnBaseHealthChanged;
    event Action<int> OnScoreChanged;
    event Action OnGameOver;

    void AddScore(int amount);
    void DamageBase(int amount);
}
