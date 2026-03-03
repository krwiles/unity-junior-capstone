

using System;
using UnityEngine;

public class GameStateService : IGameStateService
{
    private int _baseHealth;
    private int _score;
    private bool _isGameOver;

    public int BaseHealth => _baseHealth;

    public int Score => _score;

    public bool IsGameOver => _isGameOver;

    public event Action<int> OnBaseHealthChanged;
    public event Action<int> OnScoreChanged;
    public event Action OnGameOver;


    public GameStateService(int BaseHealth = 100)
    {
        _baseHealth = Mathf.Max(BaseHealth, 1);
        _score = 0;
        _isGameOver = false;
    }

    public void AddScore(int amount)
    {
        if (amount < 1 || _isGameOver) return;

        _score += amount;
        OnScoreChanged.Invoke(_score);
    }

    public void DamageBase(int amount)
    {
        if (amount < 1 || _isGameOver) return;

        _baseHealth = Mathf.Max(_baseHealth - amount, 0);
        OnBaseHealthChanged.Invoke(_baseHealth);

        if (_baseHealth == 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        _isGameOver = true;
        OnGameOver.Invoke();
    }
}