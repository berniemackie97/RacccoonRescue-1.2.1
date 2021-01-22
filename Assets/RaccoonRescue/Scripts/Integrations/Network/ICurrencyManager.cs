using System;

public interface ICurrencyManager {

	void IncBalance (int amount);

	void DecBalance (int amount);

	void SetBalance (int newbalance);

	void GetBalance (Action<int> Callback);
}
