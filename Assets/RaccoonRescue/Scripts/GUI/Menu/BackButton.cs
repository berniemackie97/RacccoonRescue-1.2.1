using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour
{
	public AnimationManager manager;

	void Update()
	{
		if (Application.platform == RuntimePlatform.Android) {
			if (Input.GetKey(KeyCode.Escape)) {
				manager.CloseMenu();
			}
		}
	}
}
