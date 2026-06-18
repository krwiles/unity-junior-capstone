
using System;

public class CurrencyService : ICurrencyService
{
    private int _current;

    public int Current => _current;

    public event Action<int> OnCurrencyChanged;

    public void Add(int amount)
    {
        if (amount < 1) return;

        _current += amount;
        OnCurrencyChanged.Invoke(_current);
    }

    public bool TrySpend(int amount)
    {
        throw new NotImplementedException();
    }
}