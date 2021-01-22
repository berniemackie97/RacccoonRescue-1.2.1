using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Background : MonoBehaviour {
	public Sprite[] pictures;

	// Use this for initialization
	void OnEnable () {
		if (mainscript.Instance != null)
		if (pictures.Length > (int)((float)mainscript.Instance.currentLevel / 20f - 0.01f))
			GetComponent<Image> ().sprite = pictures [(int)((float)mainscript.Instance.currentLevel / 20f - 0.01f)];


	}
	

}
