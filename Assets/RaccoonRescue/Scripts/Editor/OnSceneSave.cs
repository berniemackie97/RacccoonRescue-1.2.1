using UnityEngine;
using UnityEditor;
using System.Collections;

public class OnSceneSave : SaveAssetsProcessor {
	static string[] OnWillSaveAssets (string[] paths) {

		foreach (string path in paths) {
			if (path.ToString ().Contains ("game.unity") || path.ToString ().Contains ("map.unity"))
				SaveCanvasMenuPrefab ();
		}
		return paths;
	}

	static void SaveCanvasMenuPrefab () {
		GameObject canvasMenu = GameObject.Find ("CanvasMenu");
		Object canvasMenuPrefab = PrefabUtility.GetPrefabParent (canvasMenu);
		if (canvasMenu != null && canvasMenuPrefab != null)
			PrefabUtility.ReplacePrefab (canvasMenu, canvasMenuPrefab, ReplacePrefabOptions.ConnectToPrefab);

	}
}