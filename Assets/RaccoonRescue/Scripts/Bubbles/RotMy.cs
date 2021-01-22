using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotMy : MonoBehaviour {
	public float speed = 500;

	// Use this for initialization
	void Update () {
		transform.Rotate (new Vector3 (0, 0, Time.deltaTime * speed));
	}
	

}
