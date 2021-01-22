using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(Ball))]
public class BallEditor : Editor {
	public override void OnInspectorGUI () {
		DrawDefaultInspector ();

		Ball myScript = (Ball)target;
		if (GUILayout.Button ("Add Fire powerup")) {
			myScript.SetPower (Powerups.FIRE);
		}
		if (GUILayout.Button ("Add Grow powerup")) {
			myScript.SetPower (Powerups.GROW);
		}
		if (GUILayout.Button ("Add Water powerup")) {
			myScript.SetPower (Powerups.WATER);
		}
		if (GUILayout.Button ("Add Triple powerup")) {
			myScript.SetPower (Powerups.TRIPLE);
		}


	}
}