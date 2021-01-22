using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelCounter : MonoBehaviour {
	public static LevelCounter THIS;
	public static int levelsCounted;
	// Use this for initialization
	void Start () {
		if (THIS == null)
			THIS = this;
		else
			Destroy (gameObject);


		DontDestroyOnLoad (gameObject);
	}

	// Update is called once per frame
	void Update () {

	}
}
