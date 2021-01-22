#if PLAYFAB || GAMESPARKS
using UnityEngine;
using System.Collections;
using InitScriptName;

#if PLAYFAB
using PlayFab.ClientModels;
using PlayFab;
#endif
using System.Collections.Generic;

public class NetworkCurrencyManager {
	public static int currentBalance;
	ICurrencyManager currencyMananager;

	public NetworkCurrencyManager () {
		NetworkManager.OnLoginEvent += GetBalance;
		NetworkManager.OnLogoutEvent += Logout;
#if PLAYFAB
		currencyMananager = new PlayFabCurrencyManager ();
#elif GAMESPARKS 
		currencyMananager = new GamesparksCurrencyManager ();
#endif
	}

	void Logout () {
		NetworkManager.OnLoginEvent -= GetBalance;
		NetworkManager.OnLogoutEvent -= Logout;
	}

	public  void IncBalance (int amount) {
		if (!NetworkManager.THIS.IsLoggedIn)
			return;


		if (currencyMananager != null)
			currencyMananager.IncBalance (amount);
	}

	public  void DecBalance (int amount) {
		if (!NetworkManager.THIS.IsLoggedIn)
			return;


		if (currencyMananager != null)
			currencyMananager.DecBalance (amount);
	}

	public  void SetBalance (int newbalance) {
		if (!NetworkManager.THIS.IsLoggedIn)
			return;

		//		GetBalance ();

		if (currencyMananager != null)
			currencyMananager.SetBalance (newbalance);
	}

	public  void GetBalance () {
		if (!NetworkManager.THIS.IsLoggedIn)
			return;

		if (currencyMananager != null) {
			currencyMananager.GetBalance ((balance) => {
				Debug.Log (balance);
				currentBalance = balance;
				if (currentBalance >= InitScript.Gems)
					InitScript.Instance.SetGems (balance);
				else
					SetBalance (InitScript.Gems);
				
			});

		}
	}

}

#endif