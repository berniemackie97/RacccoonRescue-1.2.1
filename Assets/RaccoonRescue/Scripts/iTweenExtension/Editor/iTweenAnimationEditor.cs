using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(iTweenAnimation))]
public class iTweenAnimationEditor : Editor {

	public override void OnInspectorGUI () {
		iTweenAnimation instance = (iTweenAnimation)target;
		DrawDefaultInspector ();
		if (GUILayout.Button ("Default")) {
//			instance.Init (); 
		}

		if (GUILayout.Button ("Reload")) {
			instance.Reload ();
		}

	}
}
