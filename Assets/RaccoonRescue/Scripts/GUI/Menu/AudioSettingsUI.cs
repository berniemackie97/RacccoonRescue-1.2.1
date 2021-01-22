using UnityEngine;
using System.Collections;
using InitScriptName;

public class AudioSettingsUI : MonoBehaviour
{

	// Use this for initialization
	void OnEnable()
	{
		GameObject Off = transform.GetChild(0).gameObject;
		if (name == "MusicOn") {
			if (PlayerPrefs.GetFloat("Music") == 0f) {
				Off.SetActive(true);
			} else {

				Off.SetActive(false);

			}
		} else if (name == "SoundOn") {
			if (PlayerPrefs.GetInt("Sound") == 0) {
				SoundBase.Instance.mixer.SetFloat("soundVolume", -80);
				Off.SetActive(true);
			} else {
				SoundBase.Instance.mixer.SetFloat("soundVolume", 1);
				Off.SetActive(false);

			}
		}

	}

	// Update is called once per frame
	void Update()
	{

	}
}
