using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth = 1f;
    [SerializeField] private bool _resetToMaxOnEnable = true;

    private float _currentHealth;
    private bool _isDead;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public bool IsDead => _isDead;

    public event Action<Health> OnDied;

    private void Awake()
    {
        ResetToMaxHealth();
    }

    private void OnEnable()
    {
        if (_resetToMaxOnEnable)
        {
            ResetToMaxHealth();
        }
    }

    public bool TakeDamage(float damage)
    {
        if (_isDead || damage <= 0f)
        {
            return false;
        }

        _currentHealth = Mathf.Max(0f, _currentHealth - damage);

        if (_currentHealth <= 0f)
        {
            _isDead = true;
            OnDied?.Invoke(this);
        }

        return true;
    }

    public void Heal(float amount)
    {
        if (_isDead || amount <= 0f)
        {
            return;
        }

        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
    }

    public void ResetToMaxHealth()
    {
        _maxHealth = Mathf.Max(1f, _maxHealth);
        _currentHealth = _maxHealth;
        _isDead = false;
    }
}
