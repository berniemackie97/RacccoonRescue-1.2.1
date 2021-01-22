using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using InitScriptName;
using UnityEngine.SceneManagement;

public class TargetIcon : MonoBehaviour {
	public Sprite[] targets;
	// Use this for initialization
	void OnEnable () {
	    
		StartCoroutine (GetTarget ());
	}

	IEnumerator GetTarget () {
		yield return new WaitUntil (() => LevelData.GetTarget () != null);
		if (LevelData.GetTarget () == TargetType.Top)
			SetIcon (0);
		else if (LevelData.GetTarget () == TargetType.Round)
			SetIcon (1);
		else if (LevelData.GetTarget () == TargetType.RescuePets)
			SetIcon (2);
			

	}

	void SetIcon (int num) {
		GetComponent<Image> ().sprite = targets [num];
//		GetComponent<Image> ().SetNativeSize ();
	}
}
