using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIAdmin : MonoBehaviour
{

	void OnGUI()
	{
		GUILayout.Space(50);
		if (GUILayout.Button("Add 100 coins")) {
			InitScriptName.InitScript.Instance.AddGems(100);
		}

		GUILayout.Space(50);

		if (SceneManager.GetActiveScene().name == "game") {

			if (GUILayout.Button("Lose")) {
				LevelData.LimitAmount = 3;

			}
			GUILayout.Space(50);
		}
		if (SceneManager.GetActiveScene().name == "map") {
			if (GUILayout.Button("All levels")) {
				for (int i = 1; i < GameObject.Find("Levels").transform.childCount; i++) {
					// LevelsMap.CompleteLevel(i, _starsCount);
					SaveLevelStarsCount(i, 3);
				}
			}

			GUILayout.Space(50);


			if (GUILayout.Button("Clear all")) {
				PlayerPrefs.DeleteAll();
				PlayerPrefs.Save();
			}
		}

	}

	public void SaveLevelStarsCount(int level, int starsCount)
	{
		Debug.Log(string.Format("Stars count {0} of level {1} saved.", starsCount, level));
		PlayerPrefs.SetInt(GetLevelKey(level), starsCount);

	}

	private string GetLevelKey(int number)
	{
		return string.Format("Level.{0:000}.StarsCount", number);
	}

	public int LoadLevelStarsCount(int level)
	{
		return level > 10 ? 0 : (level % 3 + 1);
	}
}
