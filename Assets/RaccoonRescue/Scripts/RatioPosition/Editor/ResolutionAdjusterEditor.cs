using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(ResolutionAdjuster))]
public class ResolutionAdjusterEditor : Editor
{
	ResolutionAdjuster obj;
	public List<RatioPosElement> ratioListCopy = new List<RatioPosElement>();
	private bool quit;
	public RatioPosScriptable storage;

	private void OnEnable()
	{
		obj = (ResolutionAdjuster)target;
		storage = AssetDatabase.LoadAssetAtPath("Assets/RaccoonRescue/RatioPosition/RatioPosScriptable.asset", typeof(RatioPosScriptable)) as RatioPosScriptable;

	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if (GUILayout.Button("Set position"))
		{
			float orthographicSize = Camera.main.orthographicSize;
			var v = obj.ratioList.Find(i => i.cameraSize == orthographicSize);
			if (v == null)
			{
				obj.ratioList.Add(new RatioPosElement() { cameraSize = orthographicSize, pos = obj.transform.position });
				v = obj.ratioList.Last();
			}
			else
			{
				v.pos = obj.transform.position;
			}
			Save();

		}
	}

	public void Save()
	{
		storage.Save(new RatioPos() { gameObject = obj.gameObject, name = obj.gameObject.name, ratioList = obj.ratioList.Clone() });
		EditorUtility.SetDirty(storage);
		AssetDatabase.SaveAssets();
	}

	public RatioPos Load()
	{
		return storage.Load(obj.gameObject);
	}


}