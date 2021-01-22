using System;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(ITweenSequencer))]
public class iTweenSequencerEditor: Editor {
	public override void OnInspectorGUI () {
//		ITweenSequencer instance = (ITweenSequencer)target;
//		DrawDefaultInspector ();
//		if (instance.sequence == null)
//			instance.sequence = new System.Collections.Generic.List<iTweenAnimation> ();
//		foreach (var item in instance.sequence) {
//			if (item != null) {
//				GUILayout.Space (5);
//				EditorGUILayout.LabelField (item.animation.ToString ());
//				item.animation = (Tweens)EditorGUILayout.EnumPopup (item.animation, new GUILayoutOption[] { GUILayout.Width (115) });
//				for (int i = 0; i < item.table.Count; i++) {
//					EditorGUILayout.BeginHorizontal ();
//					item.table [i].Key = EditorGUILayout.TextField (item.table [i].Key.ToString (), new GUILayoutOption[] {	GUILayout.Width (100) });
//					var ob = item.table [i].Value;
//					Debug.Log (ob);
//					if (ob != null) {
//						if (ob.GetType () == typeof(Vector3))
//							item.table [i].Value = EditorGUILayout.Vector3Field ("", (Vector3)ob, new GUILayoutOption[] {	GUILayout.Width (100) });
//						else if (ob.GetType () == typeof(float))
//							item.table [i].Value = EditorGUILayout.FloatField ("", (float)ob, new GUILayoutOption[] {	GUILayout.Width (100) });
//						else if (ob.GetType () == typeof(iTween.EaseType))
//							item.table [i].Value = (iTween.EaseType)EditorGUILayout.EnumPopup ("", (iTween.EaseType)ob, new GUILayoutOption[] {	GUILayout.Width (100) });
//						else if (ob.GetType () == typeof(iTween.LoopType))
//							item.table [i].Value = (iTween.LoopType)EditorGUILayout.EnumPopup ("", (iTween.LoopType)ob, new GUILayoutOption[] {	GUILayout.Width (100) });
//						else if (ob.GetType () == typeof(string))
//							item.table [i].Value = EditorGUILayout.TextField ("", (string)ob, new GUILayoutOption[] {	GUILayout.Width (100) });
//						Debug.Log (item.table [i].Value);
//					}
//					EditorGUILayout.EndHorizontal ();
//				}
//
//			}
//		}
//
//		if (GUILayout.Button ("+")) {
//			instance.sequence.Add (new iTweenAnimation ().Init ()); 
//		}
//		if (GUILayout.Button ("-")) {
//			instance.sequence.Remove (instance.sequence [instance.sequence.Count - 1]);
//		}
//		if (GUILayout.Button ("()")) {
//			instance.sequence [0].GetTable ();
//		}
	}
}

