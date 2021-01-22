using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if FACEBOOK
using Facebook.Unity;
#endif

public class FBButton : MonoBehaviour {
	public bool showIfLogged;
	public GameObject button;
	#if FACEBOOK
	void OnEnable () {
		if (button == null)
			button = gameObject;
		NetworkManager.OnLoginEvent += Login;
		NetworkManager.OnLogoutEvent += LogOut;
		SwitchButton ();
	}

	void OnDisable () {
		NetworkManager.OnLoginEvent -= Login;
		NetworkManager.OnLogoutEvent -= LogOut;
	}

	void SwitchButton () {
		if (FB.IsLoggedIn)
			button.SetActive (showIfLogged);
		else
			button.SetActive (!showIfLogged);
		
	}

	void Login () {
		SwitchButton ();
	}

	void LogOut () {
		SwitchButton ();
	}

	#endif

}
