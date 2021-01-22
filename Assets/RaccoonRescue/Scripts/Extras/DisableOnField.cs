using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnField : MonoBehaviour {
	public Object[] objects;
	// Use this for initialization
	void Start () {
		if (transform.parent.GetComponent<Ball> () != null) {
			if (!transform.parent.GetComponent<Ball> ().enabled) {
				foreach (Object item in objects) {
					Destroy (item);
				}
			}
		}
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
