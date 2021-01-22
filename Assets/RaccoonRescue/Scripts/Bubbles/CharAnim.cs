using UnityEngine;
using System.Collections;

public class CharAnim : MonoBehaviour {
	Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}

	void OnEnable () {
		Ball.OnThrew += Throw;
		GameEvent.OnStatus += OnStatusChange;
	}

	void OnDisable () {
		Ball.OnThrew -= Throw;
		GameEvent.OnStatus -= OnStatusChange;
	}

	void OnStatusChange (GameState status) {
		if (status == GameState.WinBanner)
			anim.SetTrigger ("Win");
		if (status == GameState.OutOfMoves) {
			anim.SetBool ("Idle", false);
			anim.SetTrigger ("Lose");
		}
		if (status == GameState.Playing) {
			if (!anim.GetBool ("Idle")) {
				anim.SetBool ("Idle", true);
			}
		}
//

	}

	void Throw () {
		anim.SetTrigger ("Throw");
		// if (Random.Range(0, 5) == 1)
		// SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.character[Random.Range(0, SoundBase.Instance.character.Length)]);

	}

	// Update is called once per frame
	public void Idle () {

	}
}
