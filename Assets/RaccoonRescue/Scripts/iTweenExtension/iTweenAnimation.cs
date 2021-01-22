using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

//[ExecuteInEditMode]
[System.Serializable]
public class iTweenAnimation : MonoBehaviour {
	public Hashtable tt	= new Hashtable () {
		{ "position", Vector3.zero }
	};
	//	public MyHashObject[] table;

	iTweenAnimation nextAnimation;
	//	GameObject gameObject;

	public Tweens animation;
	public float x;
	public float y;
	public float z;
	public float time;
	//	public float speed;
	public iTween.EaseType easetype;
	public iTween.LoopType looptype;
	public float delay;

	//	[HideInInspector]
	//	public List<MyHashObject> table;

	public iTweenAnimation () {
	}

	void Start () {
		StartAnimation (gameObject);
	}

	//	public iTweenAnimation Init () {
	////		if (table == null ) {
	//		Debug.Log ("create anim");
	//		table = new List<MyHashObject> () {
	//			new MyHashObject ("x", "" + 0),
	//			new MyHashObject ("y", "" + 0),
	//			new MyHashObject ("z", "" + 0),
	//			new MyHashObject ("time", "" + 0.3f),
	//			new MyHashObject ("easetype", iTween.EaseType.linear.ToString ()),
	//			new MyHashObject ("looptype", iTween.LoopType.none.ToString ()),
	//			new MyHashObject ("delay", "" + 0f),
	//		};
	////		}
	//		return this;
	//	}

	public void SetOnComplete (iTweenAnimation _nextAnimation) {
		nextAnimation = _nextAnimation;
//		table.Add (new MyHashObject ("oncomplete", "StartNext"));
	}

	void StartNext () {
		if (nextAnimation != null)
			nextAnimation.StartAnimation (gameObject);
	}

	public void StartAnimation (GameObject _gameObject) {
//		if (gameObject == null)
//			gameObject = _gameObject;
		if (gameObject.GetComponent<iTween> () == null) {
			iTween.MoveTo (gameObject, gameObject.transform.position, 0);
		}
		Type mytype = typeof(iTweenAnimation);
		FieldInfo[] myFields = mytype.GetFields ();
		iTween itween = gameObject.GetComponent<iTween> ();
		Hashtable hash = new Hashtable ();
//		foreach (MyHashObject item in table) {
//			hash.Add (item.Key, getValue (item));
//		}

		for (int i = 0; i < myFields.Length; i++) {
			hash.Add (myFields [i].Name, myFields [i].GetValue (this));
		}

		object[] parameters = new object[2];
		parameters [0] = gameObject;
		parameters [1] = hash;
		MethodInfo theMethod = itween.GetType ().GetMethod (animation.ToString (), new Type[] {
			typeof(GameObject),
			typeof(Hashtable)
		});
//		Debug.Log (theMethod);
		theMethod.Invoke (this, parameters);

	}

	object getValue (MyHashObject item) {
		if (item.Key == "position") {
			string sVector = item.Value;
			if (sVector.StartsWith ("(") && sVector.EndsWith (")")) {
				sVector = sVector.Substring (1, sVector.Length - 2);
			}

			// split the items
			string[] sArray = sVector.Split (',');

			// store as a Vector3
			Vector3 result = new Vector3 (
				                 float.Parse (sArray [0]),
				                 float.Parse (sArray [1]),
				                 float.Parse (sArray [2]));
			return result;
		} else if (item.Key == "time" || item.Key == "delay" || item.Key == "x" || item.Key == "y" || item.Key == "z") {
			return float.Parse (item.Value);
		} else if (item.Key == "easetype") {
			return System.Enum.Parse (typeof(iTween.EaseType), item.Value);
		} else if (item.Key == "looptype") {
			return System.Enum.Parse (typeof(iTween.LoopType), item.Value);
		}
		return null;
	}

	public void Reload () {
		Start ();
	}
}
//iTween.MoveTo(gameObject, iTween.Hash("position", vector3, "time", 0.3, "easetype", iTween.EaseType.linear, "onComplete", "newBall"));
public enum Tweens {
	MoveAdd,
	MoveBy,
	MoveFrom,
	MoveTo,
	MoveUpdate,
	RotateAdd,
	RorateBy,
	RoatateFrom,
	RotateTo,
	RotateUpdate,
	ScaleAdd,
	ScaleBy,
	ScaleFrom,
	ScaleTo,
	ScaleUpdate
}

[System.Serializable]
public class MyHashObject {
	public string Key;
	public string Value;

	public MyHashObject (string key, string value) {
		Key = key;
		Value = value;
	}
}

[Serializable]
public class GenericClass<T> {
	public T someField;
}