using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class ResolutionAdjuster : MonoBehaviour
{
	[SerializeField]
	public List<RatioPosElement> ratioList = new List<RatioPosElement>();
	public RatioPosScriptable storage;
	void Start()
	{
		if (ratioList.Any(i => i.cameraSize == Camera.main.orthographicSize))
			transform.position = ratioList.Where(i => i.cameraSize == Camera.main.orthographicSize).First().pos;
	}


}


[System.Serializable]
public class RatioPosElement
{
	public float cameraSize;
	public Vector3 pos;

}

[System.Serializable]
public class RatioPos
{
	public GameObject gameObject;
	public string name;
	public List<RatioPosElement> ratioList = new List<RatioPosElement>();


}

public static class CollectionExtensions
{
	public static List<T> Clone<T>(this List<T> listToClone)
	{
		var array = new T[listToClone.Count];
		listToClone.CopyTo(array, 0);
		return array.ToList();
	}
}


