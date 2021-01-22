using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using InitScriptName;

public class LIFESAddCounter : MonoBehaviour
{
	Text text;
	static float TimeLeft;
	float TotalTimeForRestLife = 15f * 60;  //8 minutes for restore life
	bool startTimer;
	DateTime templateTime;
	// Use this for initialization
	void Start()
	{
		text = GetComponent<Text>();
		TotalTimeForRestLife = InitScript.Instance.TotalTimeForRestLifeHours * 60 * 60 + InitScript.Instance.TotalTimeForRestLifeMin * 60 + InitScript.Instance.TotalTimeForRestLifeSec;
		//if (TotalTimeForRestLife != InitScript.RestLifeTimer) { InitScript.RestLifeTimer = TotalTimeForRestLife;  }
	}

	bool CheckPassedTime()
	{
		//print(InitScript.DateOfExit );
		if (InitScript.DateOfExit == "" || InitScript.DateOfExit == default(DateTime).ToString())
			InitScript.DateOfExit = DateTime.Now.ToString();

		DateTime dateOfExit = DateTime.Parse(InitScript.DateOfExit);
		if (DateTime.Now.Subtract(dateOfExit).TotalSeconds > TotalTimeForRestLife * (InitScript.CapOfLife - InitScript.Lifes)) {
			//Debug.Log(dateOfExit + " " + InitScript.today);
			InitScript.Instance.RestoreLifes();
			InitScript.RestLifeTimer = 0;
			return false;    ///we dont need lifes
		} else {
			TimeCount((float)DateTime.Now.Subtract(dateOfExit).TotalSeconds);
			// Debug.Log(InitScript.today.Subtract(dateOfExit).TotalSeconds / 60 / 15 + " " + dateOfExit);
			return true;     ///we need lifes
		}
	}

	void TimeCount(float tick)
	{
		if (InitScript.RestLifeTimer <= 0)
			ResetTimer();

		InitScript.RestLifeTimer -= tick;
		if (InitScript.RestLifeTimer <= 1 && InitScript.Lifes < InitScript.CapOfLife) {
			InitScript.Instance.AddLife(1);
			ResetTimer();
		}
		//		}
	}

	void ResetTimer()
	{
		InitScript.RestLifeTimer = TotalTimeForRestLife;
	}

	// Update is called once per frame
	void Update()
	{
		if (!startTimer && DateTime.Now.Subtract(DateTime.Now).Days == 0) {
			InitScript.DateOfRestLife = DateTime.Now;
			if (InitScript.Lifes < InitScript.CapOfLife) {
				if (CheckPassedTime())
					startTimer = true;
				//	StartCoroutine(TimeCount());
			}
		}

		if (startTimer)
			TimeCount(Time.deltaTime);

		if (gameObject.activeSelf) {
			if (InitScript.Lifes < InitScript.CapOfLife) {
				if (InitScript.Instance.TotalTimeForRestLifeHours > 0) {
					int hours = Mathf.FloorToInt(InitScript.RestLifeTimer / 3600);
					int minutes = Mathf.FloorToInt((InitScript.RestLifeTimer - hours * 3600) / 60);
					int seconds = Mathf.FloorToInt((InitScript.RestLifeTimer - hours * 3600) - minutes * 60);

					text.enabled = true;
					text.text = "" + string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
				} else {
					int minutes = Mathf.FloorToInt(InitScript.RestLifeTimer / 60F);
					int seconds = Mathf.FloorToInt(InitScript.RestLifeTimer - minutes * 60);

					text.enabled = true;
					text.text = "" + string.Format("{0:00}:{1:00}", minutes, seconds);

				}
				InitScript.timeForReps = text.text;
				//				//	text.text = "+1 in \n " + Mathf.FloorToInt( MainMenu.RestLifeTimer/60f) + ":" + Mathf.RoundToInt( (MainMenu.RestLifeTimer/60f - Mathf.FloorToInt( MainMenu.RestLifeTimer/60f))*60f);
			} else {
				text.text = "   Full";
			}
		}
	}

	void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus) {
			//	StopCoroutine("TimeCount");
			InitScript.DateOfExit = DateTime.Now.ToString();
			//			PlayerPrefs.SetString("DateOfExit",DateTime.Now.ToString());
			//			PlayerPrefs.Save();
		} else {
			startTimer = false;
			//MainMenu.today = DateTime.Now; 
			//		MainMenu.DateOfExit = PlayerPrefs.GetString("DateOfExit");
		}
	}

	void OnEnable()
	{
		startTimer = false;
	}

	void OnApplicationQuit()  //1.4  
	{
		InitScript.DateOfExit = DateTime.Now.ToString();
		//print(InitScript.DateOfExit);

	}
}
