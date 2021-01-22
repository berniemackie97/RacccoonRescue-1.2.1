using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreWinBanner : MonoBehaviour {

	public void Stop(){
		GameEvent.Instance.GameStatus = GameState.WinMenu;
	}
}
