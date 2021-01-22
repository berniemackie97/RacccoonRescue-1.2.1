using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using InitScriptName;
using UnityEngine.SceneManagement;

public class Counter : MonoBehaviour
{
	//  UILabel label;
	Text label;
	bool dispMsg;
	// Use this for initialization
	void Start()
	{
		label = GetComponent<Text>();
	}

	// Update is called once per frame
	void Update()
	{
		if (name == "Moves") {
			label.text = "" + LevelData.LimitAmount;
			if (LevelData.LimitAmount <= 5 && GameEvent.Instance.GameStatus == GameState.Playing) {
				label.color = Color.red;
				label.GetComponent<CustomOutline>().enabled = true;
				if (!GetComponent<Animation>().isPlaying) {
					GetComponent<Animation>().Play();
					SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.alert);
				}
			}
		}
		if (name == "Scores" || name == "Score") {
			label.text = "" + mainscript.Score;
		}
		if (name == "Level") {
			label.text = "" + PlayerPrefs.GetInt("OpenLevel");
		}
		if (name == "Target") {
			//			if (LevelData.GetTarget () == TargetType.Top)
			//				label.text = "" + Mathf.Clamp (LevelData.GetTargetCount (), 0, 6) + "/6";
			//			else if (LevelData.GetTarget () == TargetType.Round)
			//				label.text = "" + Mathf.Clamp (LevelData.GetTargetCount (), 0, 1) + "/1";
			//			else if (LevelData.GetTarget () == TargetType.Animals)
			if (LevelData.IsTargetCubs())
				label.text = "" + LevelData.GetTargetCount() + "/" + LevelData.GetTotalTargetCount();
			else
				dispMsg = true;
		}

		if (name == "Lifes") {
			label.text = "" + InitScript.Instance.GetLife();
		}

		if (name == "Gems") {
			label.text = "" + InitScript.Gems;
		}

		if (name == "PriceRefill") {
			label.text = "" + LevelEditorBase.THIS.CostIfRefill;
		}

		if (name == "FailedExtraMoves") {
			label.text = "+" + LevelEditorBase.THIS.ExtraFailedMoves;
		}
	}

	void OnGUI()
	{ // only display message 

		if (dispMsg)
			GUI.Box(new Rect(5, Screen.height / 2 - 5, Screen.width - 10, 30), "Please add some cubs to the level!");
	}


	string GetPlus(int boostCount)
	{
		if (boostCount > 0)
			return "" + boostCount;
		else
			return "+";
	}


}
