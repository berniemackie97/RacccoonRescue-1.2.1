using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cub_in_bubble : MonoBehaviour {
	public Animator anim;
	// Use this for initialization
	void Start () {
		StartCoroutine (WaitingAnim ());
	}


	IEnumerator WaitingAnim () {
		while (true) {
			yield return new WaitForSeconds (Random.Range (10, 20));
			anim.SetTrigger ("idle");
		}
	}
}
