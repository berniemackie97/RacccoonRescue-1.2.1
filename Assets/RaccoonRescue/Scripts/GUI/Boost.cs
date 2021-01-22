using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InitScriptName;

public class Boost : MonoBehaviour
{
	public BoostType boostType;
	public GameObject check;

	void OnEnable()
	{
		DisableCheck();
		CheckBoost();
	}

	public void OnClickBoost()
	{
		SetBoost(boostType);
	}

	void SetBoost(BoostType boostType)
	{
		if (mainscript.Instance != null) {
			if ((GameEvent.Instance.GameStatus != GameState.Playing && GameEvent.Instance.GameStatus != GameState.PlayMenu) || check.activeSelf || mainscript.Instance.lauchingBall.PowerUp != Powerups.NONE)
				return;
		}

		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
		MenuManager.Instance.MenuBoostShop.GetComponent<BoostShop>().SetBoost(boostType, (price) => {
			BuyBoost(boostType, 1, price);
			MenuManager.Instance.ShowPurchased(boostType);
			SpendBoost(boostType);
		});

	}

	public void BuyBoost(BoostType boostType, int count, int price)
	{
		InitScript.Instance.SpendGems(price);
	}

	public void SpendBoost(BoostType boostType)
	{
		InitScript.Instance.BoostActivated = true;
		if (boostType == BoostType.AimBoost) {
			BoostVariables.AimBoost = true;
			if (mainscript.Instance != null)
				mainscript.Instance.boxCatapult.GetComponent<Square>().CheckAimBoost();
		}
		if (boostType == BoostType.ExtraSwitchBallsBoost)
			BoostVariables.ExtraSwitchBallsBoost = true;
		else if (boostType == BoostType.ColorBallBoost) {
			if (mainscript.Instance != null)
				mainscript.Instance.boxCatapult.GetComponent<Square>().Busy.SetBoost(boostType);
		}
		check.SetActive(true);
		if (boostType == BoostType.ColorBallBoost)
			Ball.OnThrew += DisableCheck;

	}

	void CheckBoost()
	{
		if (boostType == BoostType.AimBoost && BoostVariables.AimBoost)
			check.SetActive(true);
		else if (boostType == BoostType.ExtraSwitchBallsBoost && BoostVariables.ExtraSwitchBallsBoost)
			check.SetActive(true);

	}

	void DisableCheck()
	{
		check.SetActive(false);
		Ball.OnThrew -= DisableCheck;
	}
}
