using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pot : MonoBehaviour {
	public int score;
	public GameObject splashPrefab;
	public Text label;
	// Use this for initialization
	void Start () {

	}

	void OnTriggerEnter2D (Collider2D col) {
		if (col.gameObject.name.Contains ("ball")) {
			if (col.gameObject.GetComponent<Ball> ().falling) {
				SoundBase.Instance.PlayLimitSound (SoundBase.Instance.bubble_cannon);
				col.gameObject.GetComponent<Ball> ().PowerUp = Powerups.NONE;
				col.gameObject.GetComponent<Ball> ().destroy (true);
				col.gameObject.GetComponent<Rigidbody2D> ().isKinematic = true;
				//			PlaySplash (col.contacts [0].point);
			}
		}
	}

	//	void PlaySplash (Vector2 pos) {
	//		StartCoroutine (SoundsCounter ());
	//		if (mainscript.Instance.potSounds < 4)
	//			SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.pops);
	//
	//		GameObject splash = (GameObject)Instantiate (splashPrefab, transform.position + Vector3.up * 0.9f + Vector3.left * 0.35f, Quaternion.identity);
	//		Destroy (splash, 2f);
	//
	////        mainscript.Instance.PopupScore( score * mainscript.doubleScore, transform.position + Vector3.up );
	//	}
	//
	//	IEnumerator SoundsCounter () {
	//		mainscript.Instance.potSounds++;
	//		yield return new WaitForSeconds (0.2f);
	//		mainscript.Instance.potSounds--;
	//	}


	// Update is called once per frame
	//	void Update () {
	////        label.text = "" + score * mainscript.doubleScore;
	//	}
}
