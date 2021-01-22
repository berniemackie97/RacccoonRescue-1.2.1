using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class UnityAdsInit : MonoBehaviour {
	public string gameIDAndroid;
	public string gameIDiOS;
	// Use this for initialization
	void Start () {
		string gameID = gameIDAndroid;
#if UNITY_IOS
        gameID = gameIDiOS;
#endif
		#if UNITY_ADS
		Debug.Log ("initialize Unity ads");
		Advertisement.Initialize (gameID, false);
		#endif
	}

	// Update is called once per frame
	void Update () {

	}
}
