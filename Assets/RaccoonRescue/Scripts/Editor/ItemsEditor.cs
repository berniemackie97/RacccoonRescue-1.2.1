using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(ItemsEditorScriptable))]
public class ItemsEditor : Editor {

	public override void OnInspectorGUI () {
		ItemsEditorScriptable instance = (ItemsEditorScriptable)target;
//		ItemKind k = (ItemKind)EditorGUILayout.ObjectField (instance.selectedItem, typeof(ItemKind));
		foreach (var item in instance.items) {
			EditorGUILayout.LabelField (item.sprite.name);
		}
	}

}
