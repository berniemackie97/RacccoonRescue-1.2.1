using UnityEngine;
using System.Collections;

public class MenuPause : MonoBehaviour
{

	// Use this for initialization
	void OnEnable()
	{
		//GameEvent.Instance.GameStatus = GameState.Pause;
	}

	// Update is called once per frame
	void OnDisable()
	{
		GameEvent.Instance.GameStatus = GameState.WaitAfterClose;
	}
}
