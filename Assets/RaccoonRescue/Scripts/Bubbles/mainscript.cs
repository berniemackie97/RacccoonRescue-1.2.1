using System.Collections;
using System.Collections.Generic;
using System.IO;
using InitScriptName;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class mainscript : MonoBehaviour
{
    public int currentLevel;

    private static mainscript inst;

    public static mainscript Instance
    {
        get
        {
            return inst;
        }
        set
        {
            inst = value;
        }
    }

    public Ball lauchingBall;
    Vector2 speed = // Star movement speed / second
        new Vector2(250, 250);
    GameObject PauseDialogLD;
    GameObject OverDialogLD;
    GameObject PauseDialogHD;
    GameObject OverDialogHD;
    GameObject UI_LD;
    GameObject UI_HD;
    GameObject PauseDialog;
    GameObject OverDialog;
    GameObject FadeLD;
    GameObject FadeHD;
    GameObject AppearLevel;
    Vector2 target;
    Vector2 worldPos;
    Vector2 startPos;
    float startTime;
    float duration = 1.0f;
    bool setTarget;
    float mTouchOffsetX;
    float mTouchOffsetY;
    float xOffset;
    float yOffset;
    public int bounceCounter = 0;
    GameObject[] fixedBalls;
    public Vector2[][] meshArray;
    int offset;
    public GameObject checkBall;
    public GameObject newBall;
    float waitTime = 0f;
    int revertButterFly = 1;
    private static int score;

    public static int Score
    {
        get
        {
            return mainscript.score;
        }
        set
        {
            mainscript.score = value;
        }
    }

    public static int stage = 1;
    public int arraycounter = 0;
    public List<Ball> controlArray = new List<Ball>();
    bool destringAloneBall;
    public bool dropingDown;
    public float dropDownTime = 0f;
    public bool isPaused;
    public bool noSound;
    public bool gameOver;
    public bool arcadeMode;
    public float bottomBorder;
    public float topBorder;
    public float leftBorder;
    public float rightBorder;
    public float gameOverBorder;
    public float ArcadedropDownTime;
    public bool hd;
    public GameObject Fade;
    public int highScore;
    public AudioClip pops;
    public AudioClip click;
    public AudioClip levelBells;
    float appearLevelTime;
    public GameObject boxCatapult;
    public GameObject boxFirst;
    public GameObject boxSecond;
    public GameObject ElectricLiana;
    public GameObject BonusLiana;
    public GameObject BonusScore;
    public static bool ElectricBoost;
    bool BonusLianaCounter;
    bool gameOverShown;
    public static bool StopControl;
    public GameObject finger;

    public GameObject BoostChanging;

    public creatorBall creatorBall;

    public GameObject GameOverBorderObject;
    public GameObject TopBorder;
    public Transform Balls;
    public Hashtable animTable = new Hashtable();
    public static Vector3 lastBall;
    public GameObject FireEffect;
    public static int doubleScore = 1;
    public Ball latestLaunchedBall;
    public int TotalTargets;

    public int countOfPreparedToDestroy;

    public int bugSounds;
    public int potSounds;
    public float lowerLineAngle = -2.5f;
    public static Dictionary<int, ItemColor> colorsDict = new Dictionary<int, ItemColor>();

    private int _ComboCount;

    public int ComboCount
    {
        get
        {
            return _ComboCount;
        }
        set
        {
            _ComboCount = value;
            if (value > 0)
            {
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.combo[Mathf.Clamp(value - 1, 0, 4)]);
                if (value >= 6)
                {
                    SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.combo[3]);
                    doubleScore = 2;
                }
            }
            else
            {
                doubleScore = 1;
            }
        }
    }

    public GameObject popupScore;

    private int TargetCounter;

    public int TargetCounter1
    {
        get
        {
            return TargetCounter;
        }
        set
        {
            TargetCounter = value;
        }
    }

    public GameObject[] starsObject;
    public int stars = 0;

    public GameObject perfect;

    public GameObject[] boosts;
    public GameObject[] locksBoosts;

    public GameObject cubPrefab;

    public GameObject arrows;
    int stageTemp;
    public GameObject newBall2;
    public bool[] touchTheBorder;
    [SerializeField]
    private Ball[] BallOnLine;

    public Ball[] ballOnLine
    {
        get
        {
            return BallOnLine;
        }
        set
        {
            //			if (value != null) {
            //				creatorBall.EnableGridCollidersAroundSquare (value.square);
            //			} else
            //				creatorBall.OffGridColliders ();
            BallOnLine = value;

        }
    }

    public Square[] reverseMesh = new Square[3];
    public Vector2[] collidingPoints;
    //	public int[][] meshMatrix = new int[15][17];
    // Use this for initialization

    void Awake()
    {

        //if (LevelCounter.THIS == null)
        //{
        //    GameObject gm = new GameObject("LevelCounter");
        //    gm.AddComponent<LevelCounter>();
        //}

        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        Debug.Log(Instance);

        ballOnLine = new Ball[3];
        touchTheBorder = new bool[3];
        LevelCounter.levelsCounted++;

        if (InitScript.Instance == null)
            gameObject.AddComponent<InitScript>();

        currentLevel = PlayerPrefs.GetInt("OpenLevel", 1);
        stage = 1;
        mainscript.StopControl = false;
        animTable.Clear();
        //		arcadeMode = InitScript.Arcade;
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            //SwitchLianaBoost();
            //arcadeMode = true;
        }
        //GameObject.Find("Music").GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("Music");
        SoundBase.Instance.GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("Sound");
        if (PlayerPrefs.GetInt("Sound") == 0)
            SoundBase.Instance.mixer.SetFloat("soundVolume", -80);
        else
            SoundBase.Instance.mixer.SetFloat("soundVolume", 1);
        creatorBall = GameObject.Find("Creator").GetComponent<creatorBall>();

        StartCoroutine(ShowArrows());

        // StartCoroutine (CheckColors ());

    }

    IEnumerator CheckColors()
    {
        while (true)
        {
            GetColorsInGame();
            yield return new WaitForEndOfFrame();
            SetColorsForNewBall();
        }

    }

    IEnumerator ShowArrows()
    {
        while (true)
        {

            yield return new WaitForSeconds(30);
            if (GameEvent.Instance.GameStatus == GameState.Playing)
            {
                arrows.SetActive(true);

            }
            yield return new WaitForSeconds(3);
            arrows.SetActive(false);
        }
    }

    void Start()
    {

        stageTemp = 1;
        RandomizeWaitTime();
        score = 0;
        if (PlayerPrefs.GetInt("noSound") == 1)
            noSound = true;

        GameEvent.Instance.GameStatus = GameState.WaitForPopup;
        Ball.OnDestroy += BlockGame;

        PlayerPrefs.SetInt("Won", 0);
        PlayerPrefs.Save();

        //		GameObject.Find("GUIHighscore").GetComponent<GUIText>().text = "High Score: " + highScore+"";
    }

    float deltaTime = 0.0f;

    void OnGUI()
    {
        //GUILayout.Label("" + destroyingBalls.Count);
        //GUILayout.Label("" + block);

        //int w = Screen.width, h = Screen.height;

        //GUIStyle style = new GUIStyle();

        //Rect rect = new Rect(0, 0, w, h * 2 / 100);
        //style.alignment = TextAnchor.UpperLeft;
        //style.fontSize = h * 2 / 100;
        //style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        //float msec = deltaTime * 1000.0f;
        //float fps = 1.0f / deltaTime;
        //string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        //GUI.Label(rect, text, style);
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        if (noSound)
            GetComponent<AudioSource>().volume = 0;
        if (!noSound)
            GetComponent<AudioSource>().volume = 1f;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "game")
                SceneManager.LoadScene("map");
            else
                Application.Quit();
            // GameObject.Find("PauseButton").GetComponent<clickButton>().OnMouseDown();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            // creatorBall.AddMesh();

        }
        if (gameOver && !gameOverShown)
        {
            gameOverShown = true;

            //	return;
        }

        if (GameEvent.Instance.GameStatus == GameState.WinProccess)
        {

            //   return;
        }

        if (Time.time > dropDownTime && dropDownTime != 0f)
        {
            //	dropingDown = false;
            dropDownTime = 0;
            StartCoroutine(getBallsForMesh());
        }
        if (LevelData.GetTarget() != TargetType.Round && LevelData.CheckTargetComplete() && (GameEvent.Instance.GameStatus == GameState.Playing || GameEvent.Instance.GameStatus == GameState.BlockedGame))
        {
            GameEvent.Instance.GameStatus = GameState.WinProccess;
        }
        else if (GameEvent.Instance.GameStatus == GameState.Playing && !block /* || GameEvent.Instance.GameStatus == GameState.BlockedGame*/ )
        {
            if (LevelData.GetTarget() != TargetType.Round && LevelData.CheckTargetComplete())
            {
                GameEvent.Instance.GameStatus = GameState.WinProccess;
            }
            else if (LevelData.GetTarget() == TargetType.Round && LevelData.CheckTargetComplete() && GameEvent.Instance.GameStatus == GameState.WaitForTarget2)
                GameEvent.Instance.GameStatus = GameState.WinProccess;
            else if (LevelData.LimitAmount <= 0 && newBall == null && !LevelData.CheckTargetComplete())
            {
                GameEvent.Instance.GameStatus = GameState.OutOfMoves;
            }
        }
        ProgressBarScript.Instance.UpdateDisplay((float)score * 100f / ((float)LevelData.star1 / ((LevelData.star1 * 100f / LevelData.star3)) * 100f) / 100f);

        if (score >= LevelData.star1 && stars <= 0)
        {
            stars = 1;
        }
        if (score >= LevelData.star2 && stars <= 1)
        {
            stars = 2;
        }
        if (score >= LevelData.star3 && stars <= 2)
        {
            stars = 3;
        }

        if (score >= LevelData.star1)
        {
            starsObject[0].SetActive(true);
        }
        if (score >= LevelData.star2)
        {
            starsObject[1].SetActive(true);
        }
        if (score >= LevelData.star3)
        {
            starsObject[2].SetActive(true);
        }

    }

    public bool block;
    public bool blockCheckStarted;

    IEnumerator GameBlocker()
    {
        blockCheckStarted = true;
        bool checkArray = true;
        bool levelMoving = creatorBall.Instance.levelMoving;
        while (checkArray || creatorBall.Instance.levelMoving || GameEvent.Instance.GameStatus == GameState.BlockedGame)
        {
            checkArray = false;
            yield return new WaitForEndOfFrame();
            foreach (Ball item in destroyingBalls)
            {
                if (item != null)
                {
                    if (item.itemKind.color != ItemColor.Unbreakable)//1.2 failed not happened after latest powerup ball
                    {
                        if (!item.Destroyed)
                        {
                            checkArray = true;
                            break;
                        }
                    }
                }
            }
        }

        blockCheckStarted = false;

        GameEvent.Instance.GameStatus = GameState.Playing;
        destroyingBalls.Clear();
        //if (levelMoving) {
        //	yield return new WaitForSeconds(1f);
        //	if (blockCheckStarted) yield break;
        //}
        block = false;
    }

    public void BlockGame(ItemColor col)
    {
        block = true;
        if ((destroyingBalls.Count > 0 && !blockCheckStarted) || creatorBall.Instance.levelMoving)
        {
            StartCoroutine(GameBlocker());
        }
    }

    private void OnDestroy()
    {
        Ball.OnDestroy -= BlockGame;

    }

    IEnumerator getBallsForMesh()
    {
        GameObject[] meshes = GameObject.FindGameObjectsWithTag("Mesh");
        foreach (GameObject obj1 in meshes)
        {
            Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(obj1.transform.position, 0.1f, 1 << 9); //balls
            foreach (Collider2D obj in fixedBalls)
            {
                if (obj1.GetComponent<Square>().Busy == null)
                {
                    obj1.GetComponent<Square>().Busy = obj.gameObject.GetComponent<Ball>();
                    obj.GetComponent<bouncer>().offset = obj1.GetComponent<Square>().offset;
                }
            }
        }
        yield return new WaitForSeconds(0.2f);
    }

    public Ball createFirstBall(Vector3 vector3)
    {
        GameObject gm = GameObject.Find("Creator");
        return gm.GetComponent<creatorBall>().createBall(vector3, ItemColor.random, true);
    }

    public void connectNearBallsGlobal()
    {
        ///connect near balls
        fixedBalls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject obj in fixedBalls)
        {
            if (obj.layer == 9)
            {
                obj.GetComponent<Ball>().connectNearBalls();
            }
        }

    }

    public void dropUp()
    {
        if (!dropingDown)
        {
            creatorBall.AddMesh();
            dropingDown = true;
            GameObject Meshes = GameObject.Find("-Meshes");
            iTween.MoveAdd(Meshes, iTween.Hash("y", 0.5f, "time", 0.3, "easetype", iTween.EaseType.linear, "onComplete", "OnMoveFinished"));

        }

    }

    void OnMoveFinished()
    {
        dropingDown = false;
    }

    public void dropDown()
    {

        dropingDown = true;
        fixedBalls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject obj in fixedBalls)
        {
            if (obj.layer == 9)
                obj.GetComponent<bouncer>().dropDown();
        }
        GameObject gm = GameObject.Find("Creator");
        gm.GetComponent<creatorBall>().createRow(0);
        //	Invoke("destroyAloneBall", 1f);
        //	destroyAloneBall();
    }

    void RandomizeWaitTime()
    {
        const float minimumWaitTime = 5f;
        const float maximumWaitTime = 10f;
        waitTime = Time.time + Random.Range(minimumWaitTime, maximumWaitTime);
    }

    public void SetColorsForNewBall()
    {
        GetColorsInGame();
        Ball ball = null;
        if (boxCatapult.GetComponent<Square>().Busy != null && colorsDict.Count > 0)
        {
            ball = boxCatapult.GetComponent<Square>().Busy;
            ItemColor color = ball.GetComponent<Ball>().itemKind.color;
            if (!mainscript.colorsDict.ContainsValue(color))
            {
                ball.GetComponent<ColorManager>().SetColor((ItemColor)mainscript.colorsDict[Random.Range(0, mainscript.colorsDict.Count)]);
            }
        }
    }

    public void GetColorsInGame()
    {
        int i = 0;
        colorsDict.Clear();
        // LevelData.colorsDict.Clear();
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject item in balls)
        {
            if ((item.GetComponent<Ball>().itemKind.itemType == ItemTypes.Simple || item.GetComponent<Ball>().itemKind.itemType == ItemTypes.Cub) &&
                item.GetComponent<Ball>().itemKind.color != ItemColor.Unbreakable && !item.GetComponent<Ball>().Destroyed && item.layer != LayerMask.NameToLayer("NewBall"))
            {
                ItemColor col = item.GetComponent<Ball>().itemKind.color;
                if (!colorsDict.ContainsValue(col))
                {
                    colorsDict.Add(i, col);
                    // LevelData.colorsDict.Add(i,col);
                    i++;

                }
            }
        }

    }

    void CheckBallsBorderCross()
    {
        foreach (Transform item in Balls)
        {
            item.GetComponent<Ball>().CheckBallCrossedBorder();
        }
    }

    public bool findInArray(List<Ball> b, Ball destObj)
    {
        foreach (Ball obj in b)
        {

            if (obj == destObj)
                return true;
        }
        return false;
    }

    void playPop()
    {
        //	if(!Camera.main.GetComponent<mainscript>().noSound) SoundBase.Instance.audio.PlayOneShot(SoundBase.Instance.Pops);
        //AudioSource.PlayClipAtPoint(pops, transform.position);
    }

    public void ChangeBoost()
    {
        if (boxCatapult.GetComponent<Square>().Busy.PowerUp == Powerups.NONE)
        {
            mainscript.Instance.boxCatapult.GetComponent<Square>().aim_boost_effect.SetActive(false);
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.swish[0]);
            Square.waitForAnim = true;
            int sort = boxCatapult.GetComponent<Square>().Busy.GetComponent<SpriteRenderer>().sortingOrder;
            Ball ball1 = boxSecond.GetComponent<Square>().Busy;
            boxCatapult.GetComponent<Square>().Busy.newBall = false;
            boxCatapult.GetComponent<Square>().Busy.GetComponent<SpriteRenderer>().sortingOrder = ball1.GetComponent<SpriteRenderer>().sortingOrder;
            ball1.GetComponent<SpriteRenderer>().sortingOrder = sort;
            iTween.MoveTo(boxSecond.GetComponent<Square>().Busy.gameObject, iTween.Hash("position", boxCatapult.transform.position, "time", 0.3, "easetype", iTween.EaseType.linear, "onComplete", "newBall"));
            iTween.MoveTo(boxCatapult.GetComponent<Square>().Busy.gameObject, iTween.Hash("position", boxSecond.transform.position, "time", 0.3, "easetype", iTween.EaseType.linear));
            boxSecond.GetComponent<Square>().Busy = boxCatapult.GetComponent<Square>().Busy;
            boxCatapult.GetComponent<Square>().Busy = ball1;
            //		boxFirst.GetComponent<Grid>().Busy = boxSecond.GetComponent<Grid>().Busy;
            //boxCatapult.GetComponent<Grid>().BounceFrom(boxFirst);
        }
    }

    #region Powerups

    public void SetPower(Powerups powerup)
    {
        boxCatapult.GetComponent<Square>().Busy.SetPower(powerup);
    }

    public void GrowEffectPub(Ball thisBall, System.Action callback)
    {
        StartCoroutine(GrowEffect(thisBall, callback));
    }

    IEnumerator GrowEffect(Ball thisBall, System.Action callback)
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.leaf);
        int growNum = 6;
        int offsetRow = 1;
        Square square = thisBall.square;
        Vector2 ballPos2 = new Vector2(square.col, square.row - 1);
        if (mainscript.Instance.ballOnLine[0] != null)
        {
            ballPos2 = new Vector2(mainscript.Instance.ballOnLine[0].square.col, mainscript.Instance.ballOnLine[0].square.row);
        }
        Vector2 ballPos = new Vector2(ballPos2.x, ballPos2.y - growNum);
        Square[] sqList = creatorBall.Instance.GetSquares(ballPos, ballPos2, true);
        List<Ball> l = new List<Ball>();
        GameObject[] growList = new GameObject[sqList.Length];
        l.Add(thisBall);
        thisBall.HideBallAndPrefabs();
        thisBall.destroy();
        for (int i = 0; i < sqList.Length;)
        {
            Square item = null;
            if (sqList[i] != null)
            {
                item = sqList[i];
                //				if (item.Busy != null) {
                //if (item.Busy.itemKind.color == ItemColor.Unbreakable)
                //break;
                int nextI = i + 1;
                GameObject g = Instantiate(thisBall.LeafEffect, item.transform.position, Quaternion.identity) as GameObject;
                g.transform.parent = item.transform;
                if (item.Busy != null)
                {
                    item.Busy.gameObject.AddComponent<WaitForDestroyComponent>(); //1.2 Fix delayed win

                    l.Add(item.Busy);
                    g.GetComponent<SpriteRenderer>().sortingOrder = item.Busy.GetComponent<SpriteRenderer>().sortingOrder + 20;
                    //					yield return new WaitForSeconds (0.1f);
                }
                growList[i] = g;
                //					item.Busy.prefabList.Add (g);//TODO: check chain
                g.GetComponent<BeginLeafAnimation>().OnFinished(() =>
                {
                    i++;
                });
                yield return new WaitUntil(() => i == nextI);
                if (item.Busy != null)
                    item.Busy.HideBallAndPrefabs();
                //				} else
                //					i++;
            }

            yield return new WaitForFixedUpdate();
        }
        for (int i = 0; i < growList.Length; i++)
        {
            GameObject item = growList[i];
            if (item == null)
                continue;
            if (item.transform.parent.GetComponent<Square>().Busy != null)
                item.transform.parent.GetComponent<Square>().Busy.destroy();
            Destroy(item);
            yield return new WaitForSeconds(0.05f);
        }
        //		mainscript.Instance.destroyBallsArray (l, 0.1f, true, () => {
        //	
        if (callback != null)
            callback();
        //
        //		});
    }

    #endregion

    #region Destroying

    public List<Ball> destroyingBalls = new List<Ball>();

    public void destroyBallsArray(List<Ball> b, float speed, bool byPower = false, System.Action callback = null)
    {
        StartCoroutine(DestroyCor(b, speed, byPower, callback));
    }

    IEnumerator DestroyCor(List<Ball> b, float speed, bool byPower = false, System.Action callback = null)
    {
        List<Ball> l = new List<Ball>();
        foreach (Ball obj in b)
        {
            if (l.IndexOf(obj) < 0)
                l.Add(obj);
        }

        Camera.main.GetComponent<mainscript>().bounceCounter = 0;
        int scoreCounter = 1;
        int rate = 0;
        int soundPool = 0;
        int destroyBallsCount = l.Count;
        int destroyBallsCounter = 0;
        foreach (Ball obj in l)
        {
            if (obj == null)
            {
                destroyBallsCounter++;
                continue;
            }
            if (destroyingBalls.IndexOf(obj) < 0)
                destroyingBalls.Add(obj);
            soundPool++;
            if (soundPool % 6 == 0)
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.combo[Random.Range(0, 4)]);

            yield return new WaitForFixedUpdate();
            // if (scoreCounter > 3) {
            // rate += 10;
            scoreCounter += 1;
            // }
            //			scoreCounter += 10;
            if (b.Count > 10 && Random.Range(0, 10) > 5)
                mainscript.Instance.perfect.SetActive(true);
            //if (b.Count < 10 || soundPool % 20 == 0)
            //yield return new WaitForSeconds(speed);

            obj.scoreCombo = scoreCounter;
            obj.destroy(byPower, null, () =>
            {
                destroyBallsCounter++;
            });

        }
        if (callback != null)
        {
            callback();
        }
    }

    public void DestroyAround(Ball thisBall, int radius, System.Action callback = null)
    {
        StartCoroutine(DestroyAroundCor(thisBall, radius, callback));
    }

    public GameObject fireEffect;

    IEnumerator DestroyAroundCor(Ball thisBall, int radius, System.Action callback = null)
    {
        List<Ball> balls = new List<Ball>();
        List<Square> squares = creatorBall.Instance.GetSquaresAround(thisBall.GetComponent<Ball>().square, radius);

        if (squares.Count >= 0)
        {
            // mainscript.Instance.ComboCount++;
            List<GameObject> effects = new List<GameObject>();
            List<Square> array = new List<Square>();
            Vector2 ballPos = thisBall.transform.position;
            array = squares.FindAll((x) => x == thisBall.square);
            foreach (Square item in array)
            {
                GameObject gm = Instantiate(fireEffect, item.transform.position, item.transform.rotation) as GameObject;
                if (item.Busy != null)
                {
                    item.Busy.HideBallAndPrefabs();
                    item.Busy.gameObject.AddComponent<WaitForDestroyComponent>(); //1.2 Fix delayed win

                    gm.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = item.Busy.GetComponent<SpriteRenderer>().sortingOrder + 50;
                    foreach (var ps in gm.GetComponentsInChildren<ParticleSystem>())
                    {
                        ps.GetComponent<Renderer>().sortingOrder = item.Busy.GetComponent<SpriteRenderer>().sortingOrder + 50;
                    }
                    balls.Add(item.Busy);
                }
                effects.Add(gm);
                yield return new WaitForEndOfFrame();
            }

            //			Vector2 vPos = new Vector2 (thisBall.square.col, thisBall.square.row);
            array = squares.FindAll((x) => Vector2.Distance(ballPos, x.transform.position) <= 0.9f && Vector2.Distance(ballPos, x.transform.position) > 0);

            foreach (Square item in array)
            {
                GameObject gm = Instantiate(fireEffect, item.transform.position, item.transform.rotation) as GameObject;
                if (item.Busy != null)
                {
                    item.Busy.HideBallAndPrefabs();
                    gm.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = item.Busy.GetComponent<SpriteRenderer>().sortingOrder + 50;
                    foreach (var ps in gm.GetComponentsInChildren<ParticleSystem>())
                    {
                        ps.GetComponent<Renderer>().sortingOrder = item.Busy.GetComponent<SpriteRenderer>().sortingOrder + 50;
                    }
                    balls.Add(item.Busy);
                }
                effects.Add(gm);
                yield return new WaitForSeconds(0.0001f);
            }

            array = squares.FindAll((x) => Vector2.Distance(ballPos, x.transform.position) > 0.9f);
            foreach (Square item in array)
            {
                GameObject gm = Instantiate(fireEffect, item.transform.position, item.transform.rotation) as GameObject;
                if (item.Busy != null)
                {
                    item.Busy.HideBallAndPrefabs();
                    gm.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = item.Busy.GetComponent<SpriteRenderer>().sortingOrder + 50;
                    foreach (var ps in gm.GetComponentsInChildren<ParticleSystem>())
                    {
                        ps.GetComponent<Renderer>().sortingOrder = item.Busy.GetComponent<SpriteRenderer>().sortingOrder + 50;
                    }
                    balls.Add(item.Busy);
                }
                effects.Add(gm);
                yield return new WaitForSeconds(0.0001f);
            }
            //yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.1f);

                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.pops);

            }

            destroyBallsArray(balls, 0.001f, false, () =>
            {
                if (callback != null)
                {
                    foreach (var item in effects)
                    {
                        Destroy(item);
                    }
                    callback();
                }
            });
        }
    }

    public List<Ball> GetLine(Ball ball)
    {
        List<Ball> b = new List<Ball>();
        if (ball == null)
            return b;
        b.Add(ball);
        int layerMask = 1 << LayerMask.NameToLayer("Ball");
        RaycastHit2D[] fixedBalls = Physics2D.LinecastAll(ball.transform.position + Vector3.left * 10, ball.transform.position + Vector3.right * 10, layerMask);
        foreach (RaycastHit2D item in fixedBalls)
        {
            if (b.FindIndex(e => e == item.collider.GetComponent<Ball>()) < 0)
            {
                b.Add(item.collider.GetComponent<Ball>());
            }
        }
        return b;
    }

    public void DestroyLine(Ball ball, bool waterEffect = false, System.Action callback = null)
    {
        List<Ball> b = GetLine(ball);
        Square[] sq = creatorBall.GetSquares(new Vector2(ball.square.col - 10, ball.square.row), new Vector2(ball.square.col + 10, ball.square.row), true);
        if (waterEffect)
        {
            StartCoroutine(WaterEffectCor(ball, sq, () =>
            {
                callback();
            }));
        }
        else
        {
            if (b.Count >= 0)
            {
                mainscript.Instance.ComboCount++;
                destroyBallsArray(b, 0.001f, waterEffect, () =>
                {
                    callback();
                });
            }
        }

    }

    IEnumerator WaterEffectCor(Ball ball, Square[] b, System.Action callback = null)
    {
        List<Square> listCopy = new List<Square>();
        foreach (Square item in b)
        {
            if (item.Busy != null)
                item.Busy.disableBasicExplosion = true;
            listCopy.Add(item);
        }
        listCopy.Sort((x, y) =>
        {
            if (Vector2.Distance(ball.transform.position, x.transform.position) < Vector2.Distance(ball.transform.position, y.transform.position))
                return -1;
            else
                return 1;
        });
        List<GameObject> listEffects = new List<GameObject>();
        GameObject waterPrefab = mainscript.Instance.latestLaunchedBall.waterEffect;
        mainscript.Instance.latestLaunchedBall.destroy();
        int i = 0;
        foreach (Square item in listCopy)
        {
            //			if (item.itemKind.itemType != ItemTypes.Extra /*&& !item.Destroyed*/) {
            GameObject g = Instantiate(waterPrefab, item.transform.position, Quaternion.identity) as GameObject;
            g.AddComponent<WaitForDestroyComponent>(); //1.2 Fix delayed win

            g.transform.parent = item.transform;
            if (item.Busy != null)
            {
                g.GetComponent<SpriteRenderer>().sortingOrder = item.Busy.GetComponent<SpriteRenderer>().sortingOrder + 20;

                item.Busy.HideBallAndPrefabs();
            }
            listEffects.Add(g);
            if (i % 2 == 0)
                yield return new WaitForSeconds(0.02f);
            i++;

            //			}
        }
        SoundBase.Instance.PlaySound(SoundBase.Instance.wave);

        foreach (Square item in listCopy)
        {
            if (item.Busy != null)
            {
                Powerups[] s = { Powerups.WATER };
                item.Busy.gameObject.AddComponent<WaitForDestroyComponent>(); //1.2 Fix delayed win

                item.Busy.destroy(true, s);
            }
            yield return new WaitForSeconds(0.01f);

        }
        foreach (var item in listEffects)
        {
            Destroy(item);
        }
        if (callback != null)
            callback();
    }

    public void DestroySingle(GameObject obj, float speed = 0.1f)
    {
        Camera.main.GetComponent<mainscript>().bounceCounter = 0;
        int scoreCounter = 0;
        int rate = 0;
        int soundPool = 0;
        soundPool++;

        if (obj.tag == "light")
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.spark);
            DestroyLine(obj.GetComponent<Ball>());
        }

        if (scoreCounter > 3)
        {
            rate += score;
            scoreCounter += rate;
        }
        scoreCounter += score;
        obj.GetComponent<Ball>().destroy();

    }

    public IEnumerator SeekAndDestroyAloneBall()
    {
        mainscript.Instance.newBall2 = null;
        yield return new WaitForSeconds(Mathf.Clamp((float)countOfPreparedToDestroy / 50, 0.6f, (float)countOfPreparedToDestroy / 50));
        //       yield return new WaitForSeconds( 0.6f );
        int i;
        //	while(true){
        // yield return new WaitForEndOfFrame();

        connectNearBallsGlobal();
        i = 0;
        int willDestroy = 0;
        destringAloneBall = true;
        Camera.main.GetComponent<mainscript>().arraycounter = 0;
        GameObject[] fixedBalls = GameObject.FindGameObjectsWithTag("Ball"); // detect alone balls

        controlArray.Clear();
        List<Ball> b = new List<Ball>();
        foreach (GameObject obj in fixedBalls)
        {
            if (obj != null)
            {
                if (!findInArray(controlArray, obj.gameObject.GetComponent<Ball>()))
                {
                    if (obj.GetComponent<Ball>().nearBalls.Count < 7 && obj.GetComponent<Ball>().nearBalls.Count >= 0 && !obj.GetComponent<Ball>().falling && !obj.GetComponent<Ball>().enabled)
                    {
                        i++;
                        obj.GetComponent<Ball>().checkNearestBall(b);
                        if (b.Count > 0)
                        {
                            willDestroy++;
                            FallBalls(b);
                        }
                    }
                }
            }
        }
        destringAloneBall = false;
        StartCoroutine(getBallsForMesh());
        dropingDown = false;

        creatorBall.Instance.MoveLevelDown();
        yield return new WaitForEndOfFrame();
        yield return new WaitWhileDestroy(); //1.2 Fix delayed win

        mainscript.Instance.newBall = null;
        if (GameEvent.Instance.GameStatus == GameState.BlockedGame)
            GameEvent.Instance.GameStatus = GameState.Playing;
        SetColorsForNewBall();

    }

    public void FallBalls(List<Ball> b)
    {
        Camera.main.GetComponent<mainscript>().bounceCounter = 0;
        int scoreCounter = 0;
        int rate = 0;
        int soundPool = 0;

        foreach (Ball obj in b)
        {
            if (destroyingBalls.IndexOf(obj) < 0)
                destroyingBalls.Add(obj);
            //			if (obj.name.IndexOf ("ball") == 0)
            //				obj.gameObject.layer = 0;
            if (!obj.GetComponent<Ball>().Destroyed)
            {
                if (scoreCounter > 3)
                {
                    rate += 3;
                    scoreCounter += rate;
                }
                scoreCounter++;
                obj.GetComponent<Ball>().StartFall();
            }
        }
    }

    public void destroyAllballs()
    {
        foreach (Transform item in Balls)
        {
            if (item.tag != "centerball")
            {

                item.GetComponent<Ball>().destroy();
            }
        }
    }

    #endregion

    public void ExtraTimer(float sec, bool dowhile, System.Action callback)
    {
        StartCoroutine(ExtraTimerCor(sec, dowhile, callback));
    }

    IEnumerator ExtraTimerCor(float sec, bool dowhile, System.Action callback)
    {
        if (!dowhile)
            yield return new WaitForSeconds(sec);
        callback();
        if (dowhile)
            yield return new WaitForSeconds(sec);

    }
}

public enum Powerups
{
    NONE,
    FIRE,
    GROW,
    WATER,
    TRIPLE
}