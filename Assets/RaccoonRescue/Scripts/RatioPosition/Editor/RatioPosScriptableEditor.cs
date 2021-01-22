using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(RatioPosScriptable))]
public class RatioPosScriptableEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		if (GUILayout.Button("Fill objects"))
		{
			RatioPosScriptable obj = (RatioPosScriptable)target;
			foreach (var item in obj.ratioList)
			{
				GameObject go = GameObject.Find(item.name);
				Debug.Log(go);
				if (go != null)
				{
					var adj = go.GetComponent<ResolutionAdjuster>();
					if (adj == null)
						adj = go.AddComponent<ResolutionAdjuster>();
					adj.ratioList = item.ratioList.Clone();
					EditorUtility.SetDirty(go);
				}
			}
			EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
		}
	}

}