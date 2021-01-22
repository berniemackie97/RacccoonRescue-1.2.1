using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fire : MonoBehaviour {
	Ball ball;
	Rigidbody2D rb;
	// Use this for initialization
	void Start () {
		ball = transform.parent.parent.GetComponent<Ball> ();
		rb = transform.parent.parent.GetComponent<Rigidbody2D> ();
	}

	Vector3 previousPos;
	// Update is called once per frame
	void Update () {

		if (rb == null)
			return;
		Vector3 targetDir = ((Vector3)rb.velocity - transform.position);
		Vector3 vUp = (transform.position + Vector3.up * 2) - transform.position;

		float angle = Vector3.Angle (targetDir, vUp) - 180;
		Vector3 cross = Vector3.Cross (targetDir, vUp);
		if (cross.z < 0)
			angle = -angle;
		
		float distance = Vector3.Distance (transform.parent.parent.position, transform.position);
		transform.rotation = Quaternion.AngleAxis (angle, Vector3.back);
		transform.position = transform.parent.parent.position + Quaternion.AngleAxis (angle - 90, Vector3.back) * new Vector3 (distance, 0, 0);


	}
}
