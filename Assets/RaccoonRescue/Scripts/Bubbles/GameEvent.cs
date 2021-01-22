using UnityEngine;
using System.Collections;
using InitScriptName;
using UnityEngine.SceneManagement;


public enum GameState
{
	Loading,
	Map,
	PlayMenu,
	Playing,
	Highscore,
	OutOfMoves,
	GameOver,
	PreFailed,
	Pause,
	WinProccess,
	WinBanner,
	WinMenu,
	WaitForPopup,
	WaitAfterClose,
	BlockedGame,
	Tutorial,
	PrePlayBanner,
	WaitForTarget2
}


public class GameEvent : MonoBehaviour
{
	public static GameEvent Instance;
	[SerializeField]
	private GameState gameStatus;
	bool winStarted;

	public delegate void OnStatusChanged(GameState status);

	public static event OnStatusChanged OnStatus;

	public delegate void GameStateEvents();

	public static event GameStateEvents OnMapState;
	public static event GameStateEvents OnEnterGame;

	public GameState GameStatus {
		get {
			return GameEvent.Instance.gameStatus;
		}
		set {
			if (GameEvent.Instance.gameStatus != value) {
				if (value == GameState.WinProccess) {
					BoostVariables.ResetBoosts();

					if (!winStarted)
						StartCoroutine(WinAction());
				} else if (value == GameState.OutOfMoves) {
					StartCoroutine(LoseAction());
				} else if (value == GameState.Tutorial) {
					value = GameState.Playing;
					gameStatus = value;
					if (mainscript.Instance.currentLevel == 1)
						ShowTutorial();
					//				} else if (value == GameState.PrePlayBanner && gameStatus != GameState.Playing) {
					//					ShowPreTutorial ();
				} else if (value == GameState.Map) {
					if (PlayerPrefs.GetInt("Won") == 1) {
						PlayerPrefs.SetInt("Won", 0);
						if (PlayerPrefs.GetInt("OpenLevel") + 1 <= LevelsMap._instance.MapLevels.Count) {
							Debug.Log("");
							//LevelsMap.OnLevelSelected(PlayerPrefs.GetInt("OpenLevel") + 1); //auto open menu play
						} else
							MenuManager.Instance.CongratulationsMenu.SetActive((true));
					}

					if (OnMapState != null)
						OnMapState();

				} else if (value == GameState.GameOver) {

					BoostVariables.ResetBoosts();
				}
				print(value);
				OnStatus(value);
			}
			if (value == GameState.WaitAfterClose) {
				if (this != null)
					StartCoroutine(WaitAfterClose());
			}
			//			if (value == GameState.Tutorial) {
			//				if (gameStatus != GameState.Playing)
			//					GameEvent.Instance.gameStatus = value;
			//			}

			GameEvent.Instance.gameStatus = value;

		}
	}


	// Use this for initialization
	void Awake()
	{

		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy(gameObject);
		//		DontDestroyOnLoad (this);
	}

	void OnEnable()
	{
		//Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		//Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "map")
			GameStatus = GameState.Map;
		else if (scene.name == "game") {
			if (OnEnterGame != null)
				OnEnterGame();
		}
	}

	void Update()
	{
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {
			if (Input.GetKey(KeyCode.W))
				GameEvent.Instance.GameStatus = GameState.WinProccess;
			if (Input.GetKey(KeyCode.L)) {
				LevelData.LimitAmount = 0;
				GameEvent.Instance.GameStatus = GameState.OutOfMoves;
			}
			if (Input.GetKey(KeyCode.D))
				mainscript.Instance.destroyAllballs();
			if (Input.GetKey(KeyCode.M))
				LevelData.LimitAmount = 1;

		}
	}

	// Update is called once per frame
	IEnumerator WinAction()
	{
		winStarted = true;
		InitScript.Instance.AddLife(1);
		GameObject.Find("Canvas").transform.Find("LevelCleared").gameObject.SetActive(true);

		//       yield return new WaitForSeconds( 1f );
		if (GameObject.Find("Music") != null)
			GameObject.Find("Music").SetActive(false);
		//    GameObject.Find( "CanvasPots" ).transform.Find( "Black" ).gameObject.SetActive( true );
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.winSound);
		yield return new WaitForSeconds(1f);
		foreach (GameObject item in GameObject.FindGameObjectsWithTag("Ball")) {
			if (!item.GetComponent<Ball>().enabled)
				item.GetComponent<Ball>().StartFall();

		}

		yield return new WaitForSeconds(1f);
		// StartCoroutine( PushRestBalls() );
		Transform b = GameObject.Find("-Ball").transform;
		Ball[] balls = GameObject.Find("-Ball").GetComponentsInChildren<Ball>();
		foreach (Ball item in balls) {
			item.StartFall();
		}
		do {

			if (mainscript.Instance.boxSecond.GetComponent<Square>().Busy != null) {
				yield return new WaitForSeconds(0.1f);
				LevelData.LimitAmount--;
				Ball ball = mainscript.Instance.boxSecond.GetComponent<Square>().Busy;
				mainscript.Instance.boxSecond.GetComponent<Square>().Busy = null;
				ball.transform.parent = mainscript.Instance.Balls;
				ball.tag = "Ball";
				ball.PushBallAFterWin();
			}
			yield return new WaitForEndOfFrame();
		} while (LevelData.LimitAmount > 0);


		foreach (Ball item in balls) {
			if (item != null)
				item.StartFall();
		}

		while (GameObject.FindGameObjectsWithTag("Ball").Length > 0) {
			yield return new WaitForSeconds(0.1f);
			foreach (GameObject item in GameObject.FindGameObjectsWithTag("Ball")) {
				//				item.GetComponent<Ball> ().destroy ();
				if (!item.GetComponent<Ball>().falling) {
					item.tag = "Destroy";
					item.GetComponent<Ball>().PushBallAFterWin();
				}
			}
		}

		yield return new WaitForSeconds(1f);
		GameStatus = GameState.WinBanner;

		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.aplauds);
		PlayerPrefs.SetInt("Won", 1);
		if (PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", mainscript.Instance.currentLevel), 0) < mainscript.Instance.stars)
			PlayerPrefs.SetInt(string.Format("Level.{0:000}.StarsCount", mainscript.Instance.currentLevel), mainscript.Instance.stars);


		if (PlayerPrefs.GetInt("Score" + mainscript.Instance.currentLevel) < mainscript.Score) {
			PlayerPrefs.SetInt("Score" + mainscript.Instance.currentLevel, mainscript.Score);

		}
		GameObject.Find("Canvas").transform.Find("LevelCleared").gameObject.SetActive(false);
#if PLAYFAB || GAMESPARKS
		if (NetworkManager.dataManager != null) {
			NetworkManager.dataManager.SetPlayerScore(mainscript.Instance.currentLevel, mainscript.Score);
			NetworkManager.dataManager.SetPlayerLevel(mainscript.Instance.currentLevel + 1);
			NetworkManager.dataManager.SetStars();
		}
#endif
		GameStatus = GameState.WinMenu;

	}

	//IEnumerator PushRestBalls()
	//{

	//    while( LevelData.limitAmount  > 0)
	//    {
	//        if( mainscript.Instance.boxCatapult.GetComponent<Grid>().Busy != null )
	//        {
	//            LevelData.limitAmount--;
	//            ball b = mainscript.Instance.boxCatapult.GetComponent<Grid>().Busy.GetComponent<ball>();
	//            mainscript.Instance.boxCatapult.GetComponent<Grid>().Busy = null;
	//            b.transform.parent = mainscript.Instance.Balls;
	//            b.tag = "Ball";
	//            b.PushBallAFterWin();

	//        }
	//        yield return new WaitForEndOfFrame();
	//    }

	//}

	void ShowTutorial()
	{
		//GameObject.Find( "Canvas" ).transform.Find( "Tutorial" ).gameObject.SetActive( true );
		MenuManager.Instance.ShowTutorial();

	}

	void ShowPreTutorial()
	{
		GameObject.Find("Canvas").transform.Find("PreTutorial").gameObject.SetActive(true);

	}

	IEnumerator LoseAction()
	{
		//		if (mainscript.Instance.boxSecond.GetComponent<Square> ().Busy != null)
		//			Destroy (mainscript.Instance.boxSecond.GetComponent<Square> ().Busy.gameObject);
		//		if (mainscript.Instance.boxCatapult.GetComponent<Square> ().Busy != null)
		//			Destroy (mainscript.Instance.boxCatapult.GetComponent<Square> ().Busy.gameObject);

		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.OutOfMoves);
		GameObject.Find("Canvas").transform.Find("OutOfMoves").gameObject.SetActive(true);
		yield return new WaitForSeconds(1.5f);
		GameObject.Find("Canvas").transform.Find("OutOfMoves").gameObject.SetActive(false);
		if (LevelData.LimitAmount <= 0) {
			GameStatus = GameState.PreFailed;
		}
		yield return new WaitForSeconds(0.1f);

	}

	IEnumerator WaitAfterClose()
	{
		yield return new WaitForSeconds(1);
		GameStatus = GameState.Playing;
	}
}
