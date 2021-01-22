using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuPlay : MonoBehaviour {
	public GameObject[] stars;
	public Text score;

	public void OnEnable () {
		for (int i = 0; i < 3; i++) {
			stars [i].SetActive (false);
		}
	}

	void OnDisable () {
		GameEvent.Instance.GameStatus = GameState.Map;
	}


	public void OnAnimationFinished () {
		ShowStars ();
	}

	void ShowStars () {

		int starsCount = PlayerPrefs.GetInt (string.Format ("Level.{0:000}.StarsCount", PlayerPrefs.GetInt ("OpenLevel")), 0);
		if (starsCount > 0) {
			for (int i = 0; i < starsCount; i++) {
				stars [i].SetActive (true);
			}
		}

	}

}
