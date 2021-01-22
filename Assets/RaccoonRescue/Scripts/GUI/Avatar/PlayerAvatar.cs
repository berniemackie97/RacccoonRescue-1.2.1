using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using InitScriptName;

public class PlayerAvatar : MonoBehaviour, IAvatarLoader {
	public Image image;

	#if PLAYFAB || GAMESPARKS
	void OnEnable () {
		image.enabled = false;
		NetworkManager.OnPlayerPictureLoaded += ShowPicture;
		if (FacebookManager.profilePic != null)
			ShowPicture ();
	}

	void OnDisable () {
		NetworkManager.OnPlayerPictureLoaded -= ShowPicture;
		image.enabled = false;
	}


	#endif
	public void ShowPicture () {
		image.sprite = FacebookManager.profilePic;
		image.enabled = true;
	}

}
