using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Greetings : MonoBehaviour {

	#if PLAYFAB || GAMESPARKS
	void OnEnable () {
		StartCoroutine (WaitForName ());
	}

	IEnumerator WaitForName () {
		yield return new WaitUntil (() => FacebookManager.userName != "");
		GetComponent<Text> ().text = "Hello, " + FacebookManager.userName + "!";
	}
	#endif

}
