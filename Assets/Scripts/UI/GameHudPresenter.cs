using TMPro;
using UnityEngine;

public class GameHudPresenter : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _currencyText;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _gameOverText;

    private IGameStateService _gameStateService;
    private ICurrencyService _currencyService;
    private bool _isInitialized;
    private bool _isSubscribed;

    public void Initialize(
        IGameStateService gameStateService, 
        ICurrencyService currencyService
        )
    {
        _gameStateService = gameStateService;
        _currencyService = currencyService;
        _isInitialized = true;

        if (isActiveAndEnabled)
        {
            TrySubscribeAndRefresh();
        }
    }

    private void OnEnable()
    {
        TrySubscribeAndRefresh();
    }

    private void OnDisable()
    {
        TryUnsubscribe();
    }

    private void TrySubscribeAndRefresh()
    {
        if (!_isInitialized || _isSubscribed)
        {
            return;
        }

        if (_gameStateService == null || _currencyService == null)
        {
            return;
        }

        _gameStateService.OnScoreChanged += UpdateScoreText;
        _gameStateService.OnBaseHealthChanged += UpdateHealthText;
        _gameStateService.OnGameOver += ShowGameOver;
        _currencyService.OnCurrencyChanged += UpdateCurrencyText;

        _isSubscribed = true;
        RefreshFromState();
    }

    private void TryUnsubscribe()
    {
        if (!_isSubscribed)
        {
            return;
        }

        _gameStateService.OnScoreChanged -= UpdateScoreText;
        _gameStateService.OnBaseHealthChanged -= UpdateHealthText;
        _gameStateService.OnGameOver -= ShowGameOver;
        _currencyService.OnCurrencyChanged -= UpdateCurrencyText;

        _isSubscribed = false;
    }

    private void RefreshFromState()
    {
        UpdateScoreText(_gameStateService.Score);
        UpdateCurrencyText(_currencyService.Current);
        UpdateHealthText(_gameStateService.BaseHealth);

        if (_gameStateService.IsGameOver)
        {
            ShowGameOver();
        }
        else if (_gameOverText != null)
        {
            _gameOverText.gameObject.SetActive(false);
        }
    }

    private void UpdateScoreText(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    private void UpdateCurrencyText(int current)
    {
        _currencyText.text = "Money: $" + current;
    }

    private void UpdateHealthText(int health)
    {
        _healthText.text = "Health: " + health; 
    }

    private void ShowGameOver()
    {
        if (_gameOverText != null)
        {
            _gameOverText.gameObject.SetActive(true);
            _gameOverText.text = "GAME OVER";
        }
    }

}
