using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class ItemsEditorScriptable : ScriptableObject {
	const string assetName = "ItemsEditor";
	const string itemEditorPath = "Assets/RaccoonRescue/Resources/";
	[SerializeField]
	public List<ItemKind> items;
	[SerializeField]
	public ItemKind selectedItem;
	private static ItemsEditorScriptable instance;

	public static ItemsEditorScriptable Instance {
		get {
			if (instance == null) {
				instance = Resources.Load (assetName) as ItemsEditorScriptable;
				if (instance == null) {
					
					// If not found, autocreate the asset object.
					instance = CreateInstance<ItemsEditorScriptable> ();

					AssetDatabase.CreateAsset (instance, itemEditorPath + "ItemsEditor.asset");
					AssetDatabase.SaveAssets ();
					AssetDatabase.Refresh ();
					EditorUtility.FocusProjectWindow ();
					Selection.activeObject = instance;


				}
			}
			return instance;
		}
	}

	//	#if UNITY_EDITOR
	//	[MenuItem ("Window/Items Editor")]
	//	public static void Edit () {
	//		Selection.activeObject = Instance;
	//	}
	//
	//	#endif

	//	public void CreateAsset () {
	//		instance = CreateInstance<ItemsEditor> ();
	//		items = new List<ItemKind> ();
	//		AssetDatabase.CreateAsset (instance, itemEditorPath + "ItemsEditor.asset");
	//		AssetDatabase.SaveAssets ();
	//
	//
	//	}

	public void OnGUI () {
		if (Instance == null)
			return;
		if (items == null)
			items = new List<ItemKind> ();
		
		if (selectedItem != null) {
			selectedItem.sprite = (Sprite)EditorGUILayout.ObjectField ("sprite", selectedItem.sprite, typeof(Sprite), new GUILayoutOption[] {
				GUILayout.Width (200),
			});
		}
		GUILayout.BeginHorizontal ();
		foreach (var item in items) {
			Debug.Log (item);
			if (item.sprite != null) {
				Texture2D tex = item.sprite.texture;
				if (GUILayout.Button (tex, new GUILayoutOption[] {	GUILayout.Width (50),	GUILayout.Height (50)	})) {
					selectedItem = item;
				}
			}
		}
		if (GUILayout.Button ("+", new GUILayoutOption[] {	GUILayout.Width (50),	GUILayout.Height (50)	})) {
			selectedItem = new ItemKind ();
			items.Add (selectedItem);
			EditorUtility.SetDirty (Instance);
		}
		GUILayout.EndHorizontal ();
//		if (GUI.changed) {
//			Debug.Log ("changed");
//			EditorUtility.SetDirty (this);
//		}
	}

}



