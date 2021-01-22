using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuComplete : MonoBehaviour {
	public GameObject[] stars;
	public Text score;

	public void OnEnable () {
		for (int i = 0; i < 3; i++) {
			stars [i].SetActive (false);
		}
	}

	public void OnAnimationFinished () {
		StartCoroutine (MenuCompleteCor ());
		StartCoroutine (MenuCompleteScoring ());

	}

	IEnumerator MenuCompleteCor () {
		for (int i = 0; i < mainscript.Instance.stars; i++) {
			//  SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.scoringStar );
			stars [i].SetActive (true);
			yield return new WaitForSeconds (0.5f);
			SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.hit);
		}
	}

	IEnumerator MenuCompleteScoring () {
		
		for (int i = 0; i <= mainscript.Score; i += 500) {
			score.text = "" + i;
			// SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.scoring );
			yield return new WaitForSeconds (0.00001f);
		}
		score.text = "" + mainscript.Score;
	}

}
