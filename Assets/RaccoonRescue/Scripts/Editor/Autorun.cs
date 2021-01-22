using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

[InitializeOnLoad]
public class Autorun {
	static Autorun () {
		EditorApplication.update += InitProject;

	}

	static void InitProject () {
		EditorApplication.update -= InitProject;
		if (EditorApplication.timeSinceStartup < 10 || !EditorPrefs.GetBool ("AlreadyOpened")) {
			if (EditorSceneManager.GetActiveScene ().name != "game" && Directory.Exists ("Assets/RaccoonRescue/Scenes")) {
				EditorSceneManager.OpenScene ("Assets/RaccoonRescue/Scenes/map.unity");

			}
			LevelEditor.Init ();
			LevelEditor.ShowHelp ();
			EditorPrefs.SetBool ("AlreadyOpened", true);
		}

	}
}