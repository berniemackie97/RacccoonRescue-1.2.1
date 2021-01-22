using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostVariables {
	public static BoostVariables Instance;
	public static bool AimBoost;
	public static bool ExtraSwitchBallsBoost;

	public static void ResetBoosts () {
		AimBoost = false;
		ExtraSwitchBallsBoost = false;
	}

	// Use this for initialization
	//	void Start () {
	//		if (Instance == null)
	//			Instance = this;
	//	}
	//
	//	// Update is called once per frame
	//	void Update () {
	//
	//	}
}
