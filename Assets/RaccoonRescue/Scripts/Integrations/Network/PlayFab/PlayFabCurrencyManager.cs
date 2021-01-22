#if PLAYFAB
using UnityEngine;
using System.Collections;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;
using System;

public class PlayFabCurrencyManager : ICurrencyManager {

	public PlayFabCurrencyManager () {
	}


	public  void IncBalance (int amount) {
		PlayFab.ClientModels.AddUserVirtualCurrencyRequest request = new PlayFab.ClientModels.AddUserVirtualCurrencyRequest () {
			VirtualCurrency = "GC",
			Amount = amount
		};

		PlayFabClientAPI.AddUserVirtualCurrency (request, (result) => {
			Debug.Log (result.Balance);
		},
			(error) => {
				Debug.Log (error.ErrorMessage);
			});
	}

	public  void DecBalance (int amount) {
		PlayFab.ClientModels.SubtractUserVirtualCurrencyRequest request = new PlayFab.ClientModels.SubtractUserVirtualCurrencyRequest () {
			VirtualCurrency = "GC",
			Amount = amount
		};

		PlayFabClientAPI.SubtractUserVirtualCurrency (request, (result) => {
			Debug.Log (result.Balance);
		},
			(error) => {
				Debug.Log (error.ErrorMessage);
			});
	}

	public  void SetBalance (int newbalance) {
		PlayFab.ClientModels.AddUserVirtualCurrencyRequest request = new PlayFab.ClientModels.AddUserVirtualCurrencyRequest () {
			VirtualCurrency = "GC",
			Amount = newbalance - NetworkCurrencyManager.currentBalance
		};

		PlayFabClientAPI.AddUserVirtualCurrency (request, (result) => {
			Debug.Log (result.Balance);
		},
			(error) => {
				Debug.Log (error.ErrorMessage);
			});

	}

	public  void GetBalance (Action<int> Callback) {
		PlayFab.ClientModels.AddUserVirtualCurrencyRequest request = new PlayFab.ClientModels.AddUserVirtualCurrencyRequest () {
			VirtualCurrency = "GC"
		};

		PlayFabClientAPI.AddUserVirtualCurrency (request, (result) => {
			Callback (result.Balance);
		},
			(error) => {
				Debug.Log (error.ErrorMessage);
				//GetCurrencyList();
			});

	}

}

#endif