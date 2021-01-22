using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ButtonMy : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{

	}
	bool touchBegin;
	void OnMouseDown()
	{
		bool touch = false;
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) {
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
				if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
					return;
				touch = true;

			}
		} else {
			if (Input.GetMouseButtonDown(0)) {
				if (EventSystem.current.IsPointerOverGameObject()) {
					Debug.Log(EventSystem.current.IsPointerOverGameObject());
					return;
				}
				touch = true;
			}
		}

		if (touch) {
			if (name == "Change" && GameEvent.Instance.GameStatus == GameState.Playing) {
				mainscript.Instance.ChangeBoost();
			}

		}

	}
	// Update is called once per frame
	void OnPress(bool press)
	{
		if (press)
			return;
	}
}
