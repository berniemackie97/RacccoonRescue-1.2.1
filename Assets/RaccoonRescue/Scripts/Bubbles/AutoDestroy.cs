using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {
	public float sec = 1;
	// Use this for initialization
	void OnEnable () {
		Destroy (gameObject, sec);
	}

	void Hide () {
		gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
