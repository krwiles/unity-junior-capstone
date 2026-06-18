using System;

public interface ICurrencyService
{
	int Current { get; }

	event Action<int> OnCurrencyChanged;

	bool TrySpend(int amount);
	void Add(int amount);
}
