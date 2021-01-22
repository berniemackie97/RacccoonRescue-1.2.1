using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ITweenSequencer : MonoBehaviour {
	public List<iTweenAnimation> sequence;
	// Use this for initialization
	void Start () {
		for (int i = 0; i < sequence.Count; i++) {
			if (sequence.Count > 1 && i < sequence.Count - 1)
				sequence [i].SetOnComplete (sequence [i + 1]);
		}
		if (sequence.Count > 0) {
			if (sequence [0] != null)
				sequence [0].StartAnimation (gameObject);
		}
	}

}

