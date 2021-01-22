using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using InitScriptName;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    public Sprite[] boosts;
    public bool isTarget;

    Vector2 speed = // Star movement speed / second
        new Vector2(250, 250);

    [SerializeField]
    private ItemKind ItemKind;

    public ItemKind itemKind
    {
        get
        {
            return ItemKind;
        }
        set
        {
            if (value != null)
            {
                //                DestroyPrefabs();//TODO: powerUp chain
                SetOnDestroyEffectPrefab();
                GetComponent<SpriteRenderer>().sprite = value.sprite;
                if (!GetComponent<SpriteRenderer>().enabled && value.prefab == null)
                    GetComponent<SpriteRenderer>().enabled = true;
                ItemKind = value;
                if (ItemKind.itemType == ItemTypes.Cub)
                    StartCoroutine(Cub_idle());
                AddPrefab();
                Destroyed = false;
            }
        }
    }

    public Vector3 target;
    Vector2 worldPos;
    Vector3 forceVect;
    public bool setTarget;
    public float startTime;
    float duration = 1.0f;
    public Square square;
    Vector2[] meshArray;
    public bool findMesh;
    Vector3 dropTarget;
    float row;
    string str;
    public bool newBall;
    float mTouchOffsetX;
    float mTouchOffsetY;
    float xOffset;
    float yOffset;
    public Vector3 targetPosition;
    bool stopedBall;
    private bool destroyed;
    public bool NotSorting;
    public int score = 10;
    public Sprite cubGui;
    List<Ball> fireballArray = new List<Ball>();

    public bool Destroyed
    {
        get { return destroyed; }
        set
        {
            if (value)
            {
                GetComponent<BoxCollider2D>().enabled = false;
                GetComponent<SpriteRenderer>().enabled = false;
                DestroyPrefabs();
            }
            else
            {
                GetComponent<BoxCollider2D>().enabled = true;
                if (prefabList.Count > 0 && itemKind.applyingPrefab != ApplyingPrefabTypes.Replace)
                    GetComponent<SpriteRenderer>().enabled = true;
            }

            destroyed = value;
        }
    }

    public List<Ball> nearBalls = new List<Ball>();
    GameObject Meshes;
    public int countNEarBalls;
    float bottomBorder;
    float topBorder;
    float leftBorder;
    float rightBorder;
    float gameOverBorder;
    bool gameOver;
    bool isPaused;
    public AudioClip swish;
    public AudioClip pops;
    public AudioClip join;
    Vector3 meshPos;
    bool dropedDown;
    bool rayTarget;
    RaycastHit2D[] bugHits;
    RaycastHit2D[] bugHits2;
    RaycastHit2D[] bugHits3;
    public bool falling;
    Animation character;
    private int HitBug;
    private bool fireBall;
    private static int fireworks;
    private bool touchedTop;
    private bool touchedSide;
    private int fireBallLimit = 10;
    private bool launched;
    private bool animStarted;
    public GameObject splashPrefab;
    Ball targetBall;

    public delegate void ThrowAction();

    public static event ThrowAction OnThrew;

    Rigidbody2D rb;
    public Animation anim;
    // Use this for initialization
    void Start()
    {
        character = GameObject.Find("Character").gameObject.GetComponent<Animation>();
        meshPos = new Vector3(-1000, -1000, -10);
        //  sprite = GetComponent<OTSprite>();
        //sprite.passive = true;
        //	sprite.onCollision = OnCollision;
        dropTarget = transform.position;
        //		spriteBatch = GameObject.Find("SpriteBatch").GetComponent<OTSpriteBatch>();
        Meshes = GameObject.Find("-Ball");
        // Add the custom tile action controller to this tile
        //      sprite.AddController(new MyActions(this));

        bottomBorder = Camera.main.GetComponent<mainscript>().bottomBorder;
        topBorder = Camera.main.GetComponent<mainscript>().topBorder;
        leftBorder = Camera.main.GetComponent<mainscript>().leftBorder;
        rightBorder = Camera.main.GetComponent<mainscript>().rightBorder;
        gameOverBorder = Camera.main.GetComponent<mainscript>().gameOverBorder;
        gameOver = Camera.main.GetComponent<mainscript>().gameOver;
        isPaused = Camera.main.GetComponent<mainscript>().isPaused;
        dropedDown = Camera.main.GetComponent<mainscript>().dropingDown;
        rb = GetComponent<Rigidbody2D>();
        m_lastFixedUpdateTimes = new float[2];
        m_newTimeIndex = 0;
    }

    IEnumerator AllowLaunchBall()
    {
        yield return new WaitForSeconds(2);
        mainscript.StopControl = false;

    }

    public void PushBallAFterWin()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.bubble_cannon);

        GetComponent<BoxCollider2D>().offset = Vector2.zero;
        GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.5f);
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        // Destroyed = true;
        setTarget = true;
        startTime = Time.time;
        target = new Vector2(UnityEngine.Random.Range(-3, 3), 2);
        if (rb != null)
            rb.AddForce(target - dropTarget, ForceMode2D.Force);
        tag = "Destroy";
        StartCoroutine(WaitDestroy());
    }

    IEnumerator WaitDestroy()
    {
        yield return new WaitForSeconds(0.6f);
        destroy();
    }

    IEnumerator Cub_idle()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(10, 30));
            if (Destroyed)
                yield break;
            anim.Play("animal_idle");
        }
    }

    Vector3 prevPos;
    private float[] m_lastFixedUpdateTimes;
    private int m_newTimeIndex;

    private static float m_interpolationFactor;

    private int OldTimeIndex()
    {
        return (m_newTimeIndex == 0 ? 1 : 0);
    }
    Vector3 startPos;

    bool touchBegin;
    void Update()
    {
        bool touch = false;
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    return;
                touchBegin = true;
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && touchBegin)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    return;
                touch = true;
                touchBegin = false;
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
                touch = true;
            }
        }

        if (touch)
        {
            Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Vector3.back * 10;
            GameObject ball = gameObject;
            if (!launched && !ball.GetComponent<Ball>().setTarget && mainscript.Instance.newBall2 == null /*&& mainscript.Instance.newBall == null*/ && newBall && !Camera.main.GetComponent<mainscript>().gameOver && (GameEvent.Instance.GameStatus == GameState.Playing || GameEvent.Instance.GameStatus == GameState.WaitForTarget2))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPos = pos;
                if (worldPos.y > mainscript.Instance.lowerLineAngle && !mainscript.StopControl && worldPos.y < 6f)
                {
                    StartBall(worldPos, mainscript.Instance.ballOnLine[0], mainscript.Instance.touchTheBorder[0], true);
                }
            }
        }
        if (transform.position.y < -7f)
            Destroy(gameObject);
        if (transform.position.y > 10)
        {
            if (mainscript.Instance.Balls.transform.GetChild(0) != null)
            {
                if (transform.position.y > mainscript.Instance.Balls.transform.GetChild(0).position.y)
                {
                    StopBall();
                    //					destroy ();
                }
            }
            else
            {
                destroy();
            }

        }

        if (setTarget)
        {
            //float newerTime = m_lastFixedUpdateTimes[m_newTimeIndex];
            //float olderTime = m_lastFixedUpdateTimes[OldTimeIndex()];
            //Debug.Log(newerTime - olderTime);
            //if (newerTime != olderTime) {
            //	m_interpolationFactor = (Time.time - newerTime) / (newerTime - olderTime);
            //} else {
            //	m_interpolationFactor = 1;
            //}
            //transform.position = Vector2.Lerp(transform.position, target, Time.deltaTime);
        }
        else
        {

        }
    }

    void LateUpdate()
    {
        //Debug.Log(transform.position - prevPos);
        prevPos = transform.position;
    }

    void FixedUpdate()
    {
        m_newTimeIndex = OldTimeIndex();
        m_lastFixedUpdateTimes[m_newTimeIndex] = Time.fixedTime;

        if (GameEvent.Instance.GameStatus == GameState.GameOver)
            return;

        if (stopedBall)
        {

            transform.position = meshPos;
            stopedBall = false;
            if (newBall)
            {

                gameObject.layer = 9;

                mainscript.Instance.checkBall = gameObject;
                this.enabled = false;
            }

        }

        if (transform.position != target && setTarget && !stopedBall && !isPaused && Camera.main.GetComponent<mainscript>().dropDownTime < Time.time)
        {
            float totalVelocity = Vector3.Magnitude(rb.velocity);
            if (totalVelocity > 20)
            {
                float tooHard = totalVelocity / (20);
                rb.velocity /= tooHard;

            }
            else if (totalVelocity < 15)
            {
                float tooSlowRate = totalVelocity / (15);
                if (tooSlowRate != 0)
                    rb.velocity /= tooSlowRate;
            }
            if (rb.velocity.y < 1.5f && rb.velocity.y > 0)
                rb.velocity = new Vector2(rb.velocity.x, 1.7f);

        }
        if (setTarget)
            triggerEnter();

        if ((transform.position.y <= -10 || transform.position.y >= 5) && fireBall && !Destroyed)
        {
            setTarget = false;
            launched = false;
            mainscript.Instance.DestroySingle(gameObject, 0.00001f);

            mainscript.Instance.checkBall = gameObject;
        }

    }

    bool touchTheBorder;
    public int num = 0;

    public void StartBall(Vector3 worldPos, Ball targetBall_ = null, bool touchTheBorder = false, bool loseMove = false)
    {
        if (GameEvent.Instance.GameStatus != GameState.WinProccess)
            GameEvent.Instance.GameStatus = GameState.BlockedGame;
        if (loseMove)
            LevelData.LimitAmount--;
        mainscript.Instance.boxCatapult.GetComponent<Square>().aim_boost_effect.SetActive(false);
        mainscript.Instance.destroyingBalls.Clear();
        GameObject ball = gameObject;
        this.touchTheBorder = touchTheBorder;
        targetBall = targetBall_;
        NotSorting = false;
        OnThrew();
        if (PowerUp == Powerups.TRIPLE && this == mainscript.Instance.lauchingBall)
        {
            for (int i = 0; i < 2; i++)
            {
                //				GameObject b = Instantiate (creatorBall.Instance.ball_hd, transform.position, transform.rotation) as GameObject;
                Ball b = creatorBall.Instance.createBall(transform.position);
                NotSorting = false;
                b.num = i + 1;
                b.SetupBall(true, 1, ItemKind);
                b.GetComponent<Animation>().Stop();
                b.SetPower(PowerUp);
                b.GetComponent<CircleCollider2D>().enabled = false;
                Vector3 newDir = Vector3.zero;
                newDir = mainscript.Instance.collidingPoints[i + 1];

                b.StartBall(newDir, mainscript.Instance.ballOnLine[i + 1], mainscript.Instance.touchTheBorder[i + 1]);
            }
        }
        if (targetBall != null && !touchTheBorder)
            worldPos = targetBall.transform.position;

        launched = true;
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.swish[0]);
        mTouchOffsetX = (worldPos.x - ball.transform.position.x); //+ MathUtils.random(-10, 10);
        mTouchOffsetY = (worldPos.y - ball.transform.position.y);
        xOffset = (float)Mathf.Cos(Mathf.Atan2(mTouchOffsetY, mTouchOffsetX));
        yOffset = (float)Mathf.Sin(Mathf.Atan2(mTouchOffsetY, mTouchOffsetX));
        speed = new Vector2(xOffset, yOffset);
        //		if (!fireBall && this == mainscript.Instance.lauchingBall)
        GetComponent<CircleCollider2D>().enabled = true;
        target = worldPos;

        setTarget = true;
        startTime = Time.time;
        dropTarget = transform.position;
        InitScript.Instance.BoostActivated = false;
        mainscript.Instance.newBall = gameObject;
        mainscript.Instance.newBall2 = gameObject;
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;

        startPos = transform.position;
        rb.AddForce((target - dropTarget) * 10, ForceMode2D.Force);
        StartCoroutine(DetectSquare(speed));
        //Debug.DrawLine( DrawLine.waypoints[0], target );
        //Debug.Break();
    }

    Square targetSquare;

    IEnumerator DetectSquare(Vector2 dir)
    {

        while (target != Vector3.zero)
        {
            if (rb != null)
            {
                if (rb.velocity != Vector2.zero)
                    dir = rb.velocity;
            }
            Ball b = null;
            Vector3[] positions = new Vector3[] {
                transform.position,
                    transform.position + Vector3.right * 0.3f,
                    transform.position + Vector3.left * 0.3f
            };
            RaycastHit2D[] hit;
            List<RaycastHit2D> hitList = new List<RaycastHit2D>();
            for (int i = 0; i < 3; i++)
            {
                hit = Physics2D.RaycastAll(positions[i], dir, 10000, LayerMask.GetMask("Ball"));

                hitList.InsertRange(hitList.Count, hit);
            }
            hitList.Sort(delegate (RaycastHit2D x, RaycastHit2D y)
            {
                if (Vector2.Distance(transform.position, x.transform.position) < Vector2.Distance(transform.position, y.transform.position))
                    return -1;
                else
                    return 1;
            });

            foreach (var item in hitList)
            {
                if (item.collider.gameObject.layer == LayerMask.NameToLayer("Ball"))
                {
                    b = item.collider.gameObject.GetComponent<Ball>();
                    break;
                }

            }
            if (b != null)
            {
                b.square.EnableMeshesAround();
                //				yield return new WaitForSeconds (0.2f);
                for (int i = 0; i < 3; i++)
                {
                    hit = Physics2D.RaycastAll(positions[i], dir, 10000, LayerMask.GetMask("Mesh"));
                    foreach (var item in hit)
                    {
                        if (item.collider.GetComponent<Square>() != null)
                        {
                            if (item.collider.GetComponent<Square>().Busy == null)
                            {
                                targetSquare = item.collider.gameObject.GetComponent<Square>();
                                break;
                            }
                        }
                    }
                    if (targetSquare != null)
                        break;
                }
            }
            yield return new WaitForSeconds(0.2f);
        }

    }

    public void SetupBall(bool newball = false, int row = 1, ItemKind itemKind = null)
    {
        GameObject b = gameObject;
        if (newball)
        {
            b.gameObject.layer = 17;
            b.GetComponent<Ball>().enabled = true;
            b.transform.parent = Camera.main.transform;
            Rigidbody2D rig = b.AddComponent<Rigidbody2D>();
            rig.interpolation = RigidbodyInterpolation2D.Interpolate;
            b.GetComponent<CircleCollider2D>().enabled = false;
            rig.gravityScale = 0;
            //			if (GamePlay.Instance.GameStatus == GameState.Playing)
            //				b.GetComponent<Animation> ().Play ();
        }
        else
        {
            b.GetComponent<Ball>().enabled = false;
            if ((LevelData.GetTarget() == TargetType.Top && row == 0) || itemKind.itemType == ItemTypes.Cub)
                b.GetComponent<Ball>().isTarget = true;
            b.GetComponent<BoxCollider2D>().offset = Vector2.zero;
            b.GetComponent<BoxCollider2D>().size = new Vector2(0.65f, 0.65f);
        }

    }

    public bool colorBoost;

    public void SetBoost(BoostType boostType)
    {
        tag = "Ball";

        if (boostType == BoostType.ColorBallBoost)
        {
            GameObject rainbow = Instantiate(Resources.Load("Particles/Rainbow")) as GameObject;
            colorBoost = true;
            rainbow.transform.parent = gameObject.transform;
            rainbow.transform.localPosition = Vector3.zero + Vector3.back * 10;
            GetComponent<SpriteRenderer>().sprite = boosts[(int)boostType - 1];
            transform.localScale = Vector3.one * 0.5f;
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

    public List<Ball> addFrom(List<Ball> b, List<Ball> b2)
    {
        foreach (Ball obj in b)
        {
            if (!findInArray(b2, obj))
            {
                b2.Add(obj);
            }
        }
        return b2;
    }

    public void changeNearestColor()
    {
        GameObject gm = GameObject.Find("Creator");
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, 0.5f, 1 << 9);
        foreach (Collider2D obj in fixedBalls)
        {
            gm.GetComponent<creatorBall>().createBall(obj.transform.position);
            Destroy(obj.gameObject);
        }

    }

    public void checkNextNearestColor(List<Ball> b, int counter)
    {
        //		Debug.Log(b.Count);
        Vector3 distEtalon = transform.localScale;
        //		GameObject[] meshes = GameObject.FindGameObjectsWithTag(tag);
        //		foreach(GameObject obj in meshes) {
        int layerMask = 1 << LayerMask.NameToLayer("Ball");
        Collider2D[] meshes = Physics2D.OverlapCircleAll(transform.position, 1.0f, layerMask);
        foreach (Collider2D obj1 in meshes)
        {
            if (obj1.GetComponent<Ball>().itemKind.color == gameObject.GetComponent<Ball>().itemKind.color && (obj1.GetComponent<Ball>().itemKind.itemType == ItemTypes.Simple || obj1.GetComponent<Ball>().itemKind.itemType == ItemTypes.Cub))
            {
                Ball obj = obj1.GetComponent<Ball>();
                float distTemp = Vector3.Distance(transform.position, obj.transform.position);
                if (distTemp <= 1.0f)
                {
                    if (!findInArray(b, obj))
                    {
                        counter++;
                        b.Add(obj);
                        obj.GetComponent<bouncer>().checkNextNearestColor(b, counter);
                        //		destroy();
                        //obj.GetComponent<mesh>().checkNextNearestColor();
                        //		obj.GetComponent<mesh>().destroy();
                    }
                }
            }
        }
    }

    List<GameObject> FindBallsByColor(ItemColor color)
    {
        List<GameObject> list = new List<GameObject>();
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (var ball in balls)
        {
            if (ColorComparision(ball.GetComponent<Ball>(), color) && (ball.GetComponent<Ball>().itemKind.itemType == ItemTypes.Simple || ball.GetComponent<Ball>().itemKind.itemType == ItemTypes.Cub))
            {
                list.Add(ball);
            }
        }
        return list;
    }

    bool ColorComparision(Ball ball, ItemColor color)
    {
        if (!colorBoost && ball.itemKind.color == color)
            return true;
        else if (colorBoost)
            return true;
        else
            return false;
    }

    public void checkNearestColor(System.Action callback = null)
    {
        int counter = 0;
        if (Destroyed)
        {
            if (callback != null)
                callback();
            return;
        }

        List<Ball> b = new List<Ball>();
        b.Add(this);
        Vector3 distEtalon = transform.localScale;
        List<GameObject> balls = FindBallsByColor(itemKind.color); //GameObject.FindGameObjectsWithTag (tag);
        foreach (GameObject obj in balls)
        { // detect the same color balls
            float distTemp = Vector3.Distance(transform.position, obj.transform.position);
            if (distTemp <= 0.9f && distTemp > 0)
            {
                b.Add(obj.GetComponent<Ball>());
                obj.GetComponent<bouncer>().checkNextNearestColor(b, counter);
                //if (mainscript.Instance.destroyingBalls.IndexOf(obj.GetComponent<Ball>()) < 0)
                //mainscript.Instance.destroyingBalls.Add(obj.GetComponent<Ball>());

            }
        }
        mainscript.Instance.countOfPreparedToDestroy = b.Count;
        int matchColors = 3;
        if (colorBoost)
            matchColors = 2;
        if (b.Count < matchColors)
        {
            Camera.main.GetComponent<mainscript>().bounceCounter++;
            mainscript.Instance.ComboCount = 0;
        }

        Camera.main.GetComponent<mainscript>().dropingDown = false;
        FindLight(gameObject);

        if (b.Count >= matchColors)
        {
            mainscript.Instance.ComboCount++;
            mainscript.Instance.destroyBallsArray(b, 0.00001f, false, () =>
            {
                if (callback != null)
                    callback();
            });
        }
        else if (callback != null)
            callback();

        b.Clear();
    }

    public void StartFall()
    {
        if (falling)
            return;
        tag = "Destroy";
        enabled = false;

        if (square != null)
            square.Busy = null;
        if (gameObject == null)
            return;
        if (LevelData.GetTarget() == TargetType.Top && isTarget)
        {
            Instantiate(Resources.Load("Prefabs/TargetStar"), gameObject.transform.position, Quaternion.identity);
        }
        else if (LevelData.GetTarget() == TargetType.RescuePets && isTarget)
        {
            StartCoroutine(FlyToTarget());
        }
        setTarget = false;
        transform.SetParent(null);
        gameObject.layer = 13;
        //		gameObject.tag = "Ball";
        if (gameObject.GetComponent<Rigidbody2D>() == null)
            gameObject.AddComponent<Rigidbody2D>();
        gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        gameObject.GetComponent<Rigidbody2D>().velocity = gameObject.GetComponent<Rigidbody2D>().velocity + new Vector2(UnityEngine.Random.Range(-2, 2), 0);
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
        gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
        gameObject.GetComponent<CircleCollider2D>().radius = 0.3f;
        falling = true;

    }

    IEnumerator FlyToTarget()
    {
        if (itemKind.itemType == ItemTypes.Cub)
        {
            Instantiate(mainscript.Instance.cubPrefab, transform.position, Quaternion.identity);
            GetComponent<SpriteRenderer>().sprite = cubGui;
        }
        Vector3 targetPos = GameObject.Find("Target_Icon").transform.position;
        if (!LevelData.CheckTargetComplete())
            LevelData.AddTargetCount(1);

        AnimationCurve curveX = new AnimationCurve(new Keyframe(0, transform.position.x), new Keyframe(0.5f, targetPos.x));
        AnimationCurve curveY = new AnimationCurve(new Keyframe(0, transform.position.y), new Keyframe(0.5f, targetPos.y));
        curveY.AddKey(0.2f, transform.position.y - 1);
        float startTime = Time.time;
        Vector3 startPos = transform.position;
        float speed = 0.2f;
        float distCovered = 0;
        while (distCovered < 0.6f)
        {
            distCovered = (Time.time - startTime);
            transform.position = new Vector3(curveX.Evaluate(distCovered), curveY.Evaluate(distCovered), 0);
            transform.Rotate(Vector3.back * 10);
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);

    }

    public bool checkNearestBall(List<Ball> b)
    {
        if (mainscript.Instance.TopBorder.transform.position.y - transform.position.y <= 0.5f)
        {
            mainscript.Instance.controlArray = addFrom(b, mainscript.Instance.controlArray);
            b.Clear();
            return true; /// don't destroy
		}
        if (findInArray(mainscript.Instance.controlArray, this))
        {
            mainscript.Instance.controlArray = addFrom(b, mainscript.Instance.controlArray);
            b.Clear();
            return true;
        } /// don't destroy

        b.Add(this);
        foreach (Ball obj in nearBalls)
        {
            if (obj != gameObject && obj != null)
            {
                if (obj.gameObject.layer == 9)
                {
                    //					if (findInArray (mainscript.Instance.controlArray, obj)) {
                    //						b.Clear ();
                    //						return true;
                    //					} /// don't destroy
                    //					else {
                    float distTemp = Vector3.Distance(transform.position, obj.transform.position);
                    if (distTemp <= 0.9f && distTemp > 0)
                    {
                        if (!findInArray(b, obj))
                        {
                            mainscript.Instance.arraycounter++;
                            if (obj.GetComponent<Ball>().checkNearestBall(b))
                                return true;
                        }
                    }
                }
                //				}
            }
        }
        return false;

    }

    public void connectNearBalls()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Ball");
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, 0.5f, layerMask);
        int oldCount = nearBalls.Count;
        nearBalls.Clear();

        foreach (Collider2D obj in fixedBalls)
        {
            if (nearBalls.Count <= 7 && obj.gameObject != gameObject)
            { //note: check fall alone obj.gameObject != gameObject
                nearBalls.Add(obj.GetComponent<Ball>());
            }
        }
        countNEarBalls = nearBalls.Count;
    }

    IEnumerator pullToMesh(Transform otherBall = null, System.Action callback = null)
    {
        //	AudioSource.PlayClipAtPoint(join, new Vector3(5, 1, 2));
        Square busySquare = null;
        float searchRadius = 0.2f;
        transform.parent = Meshes.transform;
        Destroy(rb);
        //  rigidbody2D.isKinematic = true;
        yield return new WaitForFixedUpdate();
        // StopCoroutine( "pullToMesh" );
        dropTarget = transform.position;

        if (mainscript.Instance.reverseMesh[num] == null)
        {
            while (findMesh)
            {
                Vector3 centerPoint = transform.position;
                Collider2D[] _fixedBalls1 = Physics2D.OverlapCircleAll(centerPoint, 0.1f, 1 << 10); //meshes
                var fixedBalls1 = _fixedBalls1.OrderBy(i => Vector3.Distance(i.transform.position, transform.position)).ToArray(); //1.1 aiming problem fix
                foreach (Collider2D obj1 in fixedBalls1)
                {
                    busySquare = SetMesh(obj1, busySquare);
                }
                if (findMesh)
                {
                    Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(centerPoint, searchRadius, 1 << 10); //meshes
                    foreach (Collider2D obj in fixedBalls)
                    {
                        busySquare = SetMesh(obj, busySquare);
                    }
                }
                if (busySquare != null)
                {
                    busySquare.GetComponent<Square>().Busy = this;
                    gameObject.GetComponent<bouncer>().offset = busySquare.GetComponent<Square>().offset;
                    if (LevelData.GetTarget() == TargetType.Round)
                        LockLevelRounded.Instance.Rotate(target, transform.position);

                }

                if (findMesh)
                    searchRadius += 0.2f;

                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            SetMesh(mainscript.Instance.reverseMesh[num].GetComponent<Collider2D>(), mainscript.Instance.reverseMesh[num]);
        }
        mainscript.Instance.connectNearBallsGlobal();
        //   FindLight( gameObject );

        if (busySquare != null)
        {
            Hashtable animTable = mainscript.Instance.animTable;
            animTable.Clear();
            PlayHitAnim(transform.position, animTable);
        }
        AfterStopBall();
        yield return new WaitForSeconds(0.5f);
        creatorBall.Instance.OffGridColliders();

        callback();

        // StartCoroutine( mainscript.Instance.destroyAloneBall() );
    }

    Square SetMesh(Collider2D obj, Square busyMesh)
    {
        if (obj.gameObject.GetComponent<Square>() == null)
        {
            mainscript.Instance.DestroySingle(gameObject, 0.00001f);
        }
        else if (obj.gameObject.GetComponent<Square>().Busy == null)
        {
            findMesh = false;
            stopedBall = true;

            if (meshPos.y <= obj.gameObject.transform.position.y)
            {
                meshPos = obj.gameObject.transform.position;
                busyMesh = obj.gameObject.GetComponent<Square>();

            }

            //yield return new WaitForSeconds(1f/10f);
        }
        return busyMesh;
    }

    public void PlayHitAnim(Vector3 newBallPos, Hashtable animTable)
    {

        int layerMask = 1 << LayerMask.NameToLayer("Ball");
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, 0.5f, layerMask);
        float force = 0.15f;
        foreach (Collider2D obj in fixedBalls)
        {
            if (!animTable.ContainsKey(obj.gameObject) && obj.gameObject != gameObject && animTable.Count < 50)
                obj.GetComponent<Ball>().PlayHitAnimCorStart(newBallPos, force, animTable);
        }
        if (fixedBalls.Length > 0 && !animTable.ContainsKey(gameObject))
            PlayHitAnimCorStart(fixedBalls[0].gameObject.transform.position, 0, animTable);
    }

    public void PlayHitAnimCorStart(Vector3 newBallPos, float force, Hashtable animTable)
    {
        if (!animStarted)
        {
            StartCoroutine(PlayHitAnimCor(newBallPos, force, animTable));
            PlayHitAnim(newBallPos, animTable);
        }
    }

    public IEnumerator PlayHitAnimCor(Vector3 newBallPos, float force, Hashtable animTable)
    {
        animStarted = true;
        animTable.Add(gameObject, gameObject);
        if (tag == "centerball")
            yield break;
        yield return new WaitForFixedUpdate();
        float dist = Vector3.Distance(transform.position, newBallPos);
        force = 1 / dist + force;
        newBallPos = transform.position - newBallPos;
        if (transform.parent == null)
        {
            animStarted = false;
            yield break;
        }
        newBallPos = Quaternion.AngleAxis(transform.parent.parent.rotation.eulerAngles.z, Vector3.back) * newBallPos;
        newBallPos = newBallPos.normalized;
        newBallPos = transform.localPosition + (newBallPos * force / 10);

        float startTime = Time.time;
        Vector3 startPos = transform.localPosition;
        float speed = force * 5;
        float distCovered = 0;
        while (distCovered < 1 && !float.IsNaN(newBallPos.x))
        {
            distCovered = (Time.time - startTime) * speed;
            if (this == null)
                yield break;
            //   if( destroyed ) yield break;
            if (falling)
            {
                //           transform.localPosition = startPos;
                yield break;
            }
            transform.localPosition = Vector3.Lerp(startPos, newBallPos, distCovered);
            yield return new WaitForEndOfFrame();
        }
        Vector3 lastPos = transform.localPosition;
        startTime = Time.time;
        distCovered = 0;
        while (distCovered < 1 && !float.IsNaN(newBallPos.x))
        {
            distCovered = (Time.time - startTime) * speed;
            if (this == null)
                yield break;
            if (falling)
            {
                //      transform.localPosition = startPos;
                yield break;
            }
            transform.localPosition = Vector3.Lerp(lastPos, startPos, distCovered);
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = startPos;
        animStarted = false;
    }

    public void FindLight(GameObject activatedByBall)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Ball");
        Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, 0.5f, layerMask);
        int i = 0;
        foreach (Collider2D obj in fixedBalls)
        {
            i++;
            if (i <= 10)
            {
                if ((obj.gameObject.tag == "light") && GameEvent.Instance.GameStatus == GameState.Playing)
                {
                    mainscript.Instance.DestroySingle(obj.gameObject);
                    mainscript.Instance.DestroySingle(activatedByBall);
                }
                else if ((obj.gameObject.tag == "cloud") && GameEvent.Instance.GameStatus == GameState.Playing)
                {
                    obj.GetComponent<ColorManager>().ChangeRandomColor();
                }

            }
        }
    }

    void StopBall(bool pulltoMesh = true, Transform otherBall = null)
    {
        launched = true;
        mainscript.Instance.latestLaunchedBall = this;
        mainscript.lastBall = gameObject.transform.position;
        if (mainscript.Instance.ballOnLine[num] == null)
        {
            if (otherBall != null)
                creatorBall.Instance.EnableGridCollidersAroundSquare(otherBall.GetComponent<Ball>().square);
            //			else
            //				creatorBall.Instance.EnableGridColliders ();

        }
        else
        {
            creatorBall.Instance.EnableGridCollidersAroundSquare(mainscript.Instance.ballOnLine[num].square);
        }
        target = Vector2.zero;
        setTarget = false;
        if (rb != null)
            rb.velocity = Vector2.zero;
        findMesh = true;
        GetComponent<BoxCollider2D>().offset = Vector2.zero;
        GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
        //      transform.eulerAngles = Vector3.zero;

        if (pulltoMesh)
        {
            StartCoroutine(pullToMesh(otherBall, () => { }));
        }
        mainscript.Instance.reverseMesh[num] = null;
    }

    void AfterStopBall()
    {
        DestroyMatchColors();

        ExecutPowerupsAround(() =>
        {
            DestroyMatchColors();
        });
        if (itemKind.itemType == ItemTypes.Simple)
            CheckNearBreakable();
        mainscript.Instance.ballOnLine[num] = null;
        newBall = false;
    }

    #region COLLISION

    void OnTriggerStay2D(Collider2D other)
    {
        if (findMesh && other.gameObject.layer == 9)
        {
            //  StartCoroutine(pullToMesh());
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        OnTriggerEnter2D(coll.collider);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // stop
        if (other.gameObject.name.Contains("ball") && setTarget && name.IndexOf("bug") < 0 && mainscript.Instance.reverseMesh[0] == null)
        {
            if (!other.gameObject.GetComponent<Ball>().enabled)
            {
                if ((other.gameObject.tag == "black_hole") && GameEvent.Instance.GameStatus == GameState.Playing)
                {
                    SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.black_hole);
                    mainscript.Instance.DestroySingle(gameObject);
                }

                if (!fireBall)
                {
                    if (targetBall != null)
                    {
                        if (targetBall.gameObject == other.gameObject)
                        {
                            StopBall(true, other.transform);
                        }
                    }
                    else
                        StopBall(true, other.transform);
                }
                else
                {
                    if (other.gameObject.tag.Contains("animal") || other.gameObject.tag.Contains("empty") || other.gameObject.tag.Contains("centerball"))
                        return;
                    fireBallLimit--;
                    if (fireBallLimit > 0)
                        mainscript.Instance.DestroySingle(other.gameObject, 0.000000000001f);
                    else
                    {
                        StopBall();
                        mainscript.Instance.destroyBallsArray(fireballArray, 0.000000000001f);

                    }

                }
                //           FindLight(gameObject);
            }
            //          }
        }
        else if (other.gameObject.name.IndexOf("ball") == 0 && setTarget && name.IndexOf("bug") == 0 && mainscript.Instance.reverseMesh[0] == null)
        {
            if (other.GetComponent<Ball>().itemKind.color == itemKind.color)
            {
                Destroy(other.gameObject);
                //                Score.Instance.addScore(3);
            }
        }
        else if (other.gameObject.name == "TopBorder" && setTarget)
        {
            if (LevelData.GetTarget() == TargetType.Top || LevelData.GetTarget() == TargetType.RescuePets)
            {
                if (!findMesh)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    StopBall();

                    if (fireBall)
                    {
                        mainscript.Instance.destroyBallsArray(fireballArray, 0.000000000001f);
                    }
                }

            }

            //      } else if (targetSquare != null && !stopedBall) {
            //          //          print (other.gameObject + " " + targetSquare + " " + (other.gameObject == targetSquare.gameObject));
            //          if (other.gameObject == targetSquare.gameObject) {
            //              StopBall ();
            //          }

        }

    }

    public void CheckBallCrossedBorder()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.1f, 1 << 14) != null || Physics2D.OverlapCircle(transform.position, 0.1f, 1 << 17) != null)
        {
            mainscript.Instance.DestroySingle(gameObject, 0.00001f);
        }

    }

    void triggerEnter()
    {

        // check if we collided with a bottom block and adjust our speed and rotation accordingly
        if (transform.position.y <= bottomBorder && target.y < 0)
        {
            ExplEffect();
            StopBall(false);
            //target = new Vector2( target.x, target.y * -1 );
        }
        else
        {

            //// check if we collided with a left block and adjust our speed and rotation accordingly
            if (transform.position.x <= leftBorder && target.x < 0 && !touchedSide)
            {

                //  touchedSide = true;
                Invoke("CanceltouchedSide", 0.1f);
                SwitchVelocity();
                //				print ("trigger " +rb.velocity);
            }
            // check if we collided with a right block and adjust our speed and rotation accordingly
            if (transform.position.x >= rightBorder && target.x > 0 && !touchedSide)
            {
                //  touchedSide = true;
                Invoke("CanceltouchedSide", 0.1f);
                SwitchVelocity();
            }
            //             check if we collided with a right block and adjust our speed and rotation accordingly
            //if (transform.position.y >= topBorder && target.y > 0 && LevelData.GetTarget () == TargetType.Round && !touchedTop) {
            //	touchedTop = true;
            //	// target = new Vector2( target.x, -target.y );
            //	rb.velocity = new Vector2 (rb.velocity.x,rb.velocity.y * -1);
            //	//         print( target.y );
            //}

        }

    }

    void SwitchVelocity()
    {
        target = new Vector2(target.x * -1, target.y);
        rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
        if (touchTheBorder && targetBall != null)
        {
            target = targetBall.transform.position;
            rb.velocity = (target - transform.position).normalized * 10;
        }
    }

    void CanceltouchedSide()
    {
        touchedSide = false;

    }

    public void SetupCollider()
    {
        if (square.col >= creatorBall.columns - 1 && square.offset == 0)
        {
            BoxCollider2D box = gameObject.GetComponent<BoxCollider2D>();
            box.offset = new Vector2(0.2f, 0);
            box.size = new Vector2(1, box.size.y);
        }
        else if (square.col == 0 && square.offset > 0)
        {
            BoxCollider2D box = gameObject.GetComponent<BoxCollider2D>();
            box.offset = new Vector2(-0.2f, 0);
            box.size = new Vector2(1, box.size.y);
        }
    }

    #endregion

    #region Additional_Prefab

    public List<GameObject> prefabList = new List<GameObject>();

    public void AddPrefab()
    {
        if (itemKind.prefab != null)
        {
            if (!IsPrefabExist(itemKind.prefab.name))
            {
                GameObject gm = Instantiate(itemKind.prefab) as GameObject;
                prefabList.Add(gm);
                gm.name = itemKind.prefab.name;
                gm.transform.SetParent(transform);
                gm.transform.localPosition = Vector3.zero;
                gm.transform.localScale = Vector3.one;
                if (itemKind.applyingPrefab == ApplyingPrefabTypes.Replace)
                {
                    GetComponent<SpriteRenderer>().enabled = false;
                }
                else if (itemKind.applyingPrefab == ApplyingPrefabTypes.Apply)
                {
                    if (gm.GetComponent<SpriteRenderer>() != null)
                    {
                        gm.GetComponent<SpriteRenderer>().sortingLayerID = gameObject.GetComponent<SpriteRenderer>().sortingLayerID;
                        gm.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + 20;
                    }
                }
                else if (itemKind.applyingPrefab == ApplyingPrefabTypes.Behind)
                {
                    if (gm.GetComponent<SpriteRenderer>() != null)
                    {
                        gm.GetComponent<SpriteRenderer>().sortingLayerID = gameObject.GetComponent<SpriteRenderer>().sortingLayerID;
                        gm.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder - 20;
                    }
                }
            }
        }
    }

    public void DestroyPrefabs()
    {
        foreach (GameObject item in prefabList)
        {
            Destroy(item);
        }
    }

    public bool IsPrefabExist(string prefabName)
    {
        foreach (GameObject item in prefabList)
        {
            if (item != null)
            {
                if (item.name == prefabName)
                    return true;
            }
        }
        return false;
    }

    public void SetOrderPrefabs()
    {
        foreach (GameObject item in prefabList)
        {
            if (item != null)
            {
                if (item.GetComponent<SpriteRenderer>() != null)
                {
                    item.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + 20;
                }
                foreach (Transform item2 in item.transform)
                {
                    if (item2.GetComponent<SpriteRenderer>() != null)
                    {
                        item2.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + 20;
                    }
                }
            }
        }
    }

    public void HideBallAndPrefabs()
    {
        if (this != null)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            foreach (var item in GetComponentsInChildren<SpriteRenderer>())
            {
                item.enabled = false;
            }
        }
    }

    #endregion

    #region Powerups

    [SerializeField]
    Powerups powerUp;

    public Powerups PowerUp
    {
        get
        {
            return powerUp;
        }
        set
        {
            powerUp = value;
            itemKind = LevelEditorBase.THIS.items.Find((e) => e.itemType == ItemTypes.Extra && e.powerUp == powerUp);
            AddPrefab();
        }
    }

    public void SetPower(Powerups powerup)
    {
        PowerUp = powerup;
    }

    public GameObject LeafEffect;
    public GameObject fireEffect;
    public GameObject waterEffect;

    public bool executed;

    private void ExecutePowerup(System.Action callback = null)
    {
        //if (Destroyed)//
        //{
        //    if (callback != null)
        //        callback();
        //    return;
        //}
        if (!executed && square != null)
        {
            executed = true;
            if (PowerUp == Powerups.FIRE)
            {
                DestroyPrefabs();
                mainscript.Instance.DestroyAround(this, 2, () =>
                {
                    if (callback != null)
                        callback();

                });
            }
            else if (PowerUp == Powerups.TRIPLE)
            {
                mainscript.Instance.DestroyAround(this, 1, () =>
                {
                    if (callback != null)
                        callback();

                });
            }
            else if (PowerUp == Powerups.WATER)
            { // powerup water

                Square sq = creatorBall.Instance.GetSquare(square.row - 1, square.col);
                if (sq == null)
                    sq = creatorBall.Instance.GetSquare(square.row - 1, square.col - 1);
                else if (sq.Busy == null)
                    sq = creatorBall.Instance.GetSquare(square.row - 1, square.col + 1);
                if (sq.Busy == null)//1.2
                    sq = creatorBall.Instance.GetSquare(square.row - 1, square.col + 2);
                if (sq.Busy == null)//1.2
                    sq = creatorBall.Instance.GetSquare(square.row - 1, square.col - 2);
                if (sq.Busy == null)//1.2
                    sq = creatorBall.Instance.GetSquare(square.row, square.col + 1);
                if (sq.Busy == null)//1.2
                    sq = creatorBall.Instance.GetSquare(square.row, square.col - 1);
                if (!newBall || this.square.row == 0)
                    sq = creatorBall.Instance.GetSquare(square.row, square.col);
                if (mainscript.Instance.ballOnLine[0] != null && mainscript.Instance.latestLaunchedBall == gameObject)
                    sq = mainscript.Instance.ballOnLine[0].square;
                if (sq.Busy == null && mainscript.Instance.latestLaunchedBall == gameObject)
                {
                    sq = square;
                }
                mainscript.Instance.DestroyLine(sq.Busy, true, () =>
                {
                    if (callback != null)
                        callback();

                    this.destroy();
                });
            }
            else if (PowerUp == Powerups.GROW)
            { // powerup grow
                mainscript.Instance.GrowEffectPub(this, () =>
                {
                    if (callback != null)
                        callback();
                    this.destroy();
                });
            }
            else
            {
                if (callback != null)
                {
                    callback();
                }
            }
        }
        else
        {
            if (callback != null)
            {
                callback();
            }
        }
    }

    public void ExecutPowerupsAround(System.Action callback)
    {
        List<Ball> extraBallsAround = new List<Ball>();
        ExecutePowerup(() =>
        {
            if (itemKind.itemType == ItemTypes.Simple)
            {
                foreach (Ball item in nearBalls)
                {
                    if (item.itemKind.itemType == ItemTypes.Extra && item != this && !item.Destroyed && itemKind.itemType != ItemTypes.Extra)
                        extraBallsAround.Add(item);
                }
                if (extraBallsAround.Count == 0)
                {
                    callback();
                }
                else
                {
                    foreach (Ball item in extraBallsAround)
                    {
                        item.gameObject.AddComponent<WaitForDestroyComponent>(); //1.2 Fix delayed win
                        item.ExecutePowerup(() =>
                        {
                            callback();
                        });
                    }
                }
            }
            else
            {
                callback();
            }

        });
    }

    #endregion

    #region DESTROYING

    public delegate void OnDestroyAction(ItemColor color);

    public static event OnDestroyAction OnDestroy;

    //Latest destroying function
    public int scoreCombo = 1;
    bool destroyStarted;

    public void destroy(bool destroyAnyway = false, Powerups[] disablePower = null /*PowerUps disabling some effects */ , System.Action callback = null)
    {
        if (!destroyStarted)
        {
            //			Debug.Log (gameObject);
            // GameEvent.OnStatus -= OpenHiddenBall;
            if (itemKind.color == ItemColor.Unbreakable && !destroyAnyway)
            {
                Destroy(GetComponent<WaitForDestroyComponent>()); //1.2 Leaf ball and unbreakable

                GetComponent<SpriteRenderer>().enabled = true;
                if (callback != null)
                    callback();
                return;
            }
            if (!destroyAnyway)
            {
                if (itemKind.itemType == ItemTypes.Breakable && itemKind.appearBallAfterDestroyNum > 0)
                {
                    Destroyed = false;
                    PrepareOpenBreakableBall();
                    if (callback != null)
                        callback();

                    return;
                }
            }
            if (destroyStarted)
                return;
            destroyStarted = true;
            Destroyed = true;

            if (LevelData.GetTarget() == TargetType.RescuePets && isTarget)
            {
                //				DestroyPrefabs ();
                GetComponent<SpriteRenderer>().enabled = true;
                StartCoroutine(FlyToTarget());
                if (callback != null)
                    callback();

                return;
            }
            GetComponent<Collider2D>().enabled = false;
            if (gameObject.name.IndexOf("ball") == 0)
                gameObject.layer = 0;
            Camera.main.GetComponent<mainscript>().bounceCounter = 0;
            if (OnDestroy != null)
                OnDestroy(itemKind.color);
            if (!disableBasicExplosion)
                ExplEffect();
            GetComponent<CircleCollider2D>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            //if (itemKind.itemType != ItemTypes.Breakable)
            //    CheckNearBreakable();
            if (mainscript.Instance.destroyingBalls.IndexOf(this) > -1)
                mainscript.Instance.destroyingBalls.Remove(this);
            if (itemKind.itemType == ItemTypes.Breakable && itemKind.appearBallAfterDestroyNum > 0)
            {
                if (!falling)
                {
                    SetOnDestroyEffectPrefab(); //special effect while destroying the gameobject
                }
            }
            else
            {
                SetOnDestroyEffectPrefab(); //special effect while destroying the gameobject
            }

            if (LevelData.GetTarget() == TargetType.Top && isTarget)
            {
                Instantiate(Resources.Load("Prefabs/TargetStar"), gameObject.transform.position, Quaternion.identity);
            }
            else if (LevelData.GetTarget() == TargetType.RescuePets && isTarget)
            {
                // Instantiate( Resources.Load( "Prefabs/TargetStar" ), gameObject.transform.position, Quaternion.identity );
            }
            PopupScore(itemKind.score * scoreCombo, transform.position); //1.2

            if (disablePower != null)
            {
                if (Array.FindIndex(disablePower, (e) => e == PowerUp) < 0)
                {
                    ExecutePowerup(() =>
                    {
                        if (callback != null)
                            callback();
                        if (PowerUp != Powerups.NONE)
                            DestroyMatchColors();
                        Destroy(gameObject, 0);
                    });
                }
                else
                {
                    if (callback != null)
                        callback();
                    Destroy(gameObject, 0);
                }
            }
            else
            {
                ExecutePowerup(() =>
                {
                    if (callback != null)
                        callback();
                    if (PowerUp != Powerups.NONE)
                        DestroyMatchColors();
                    Destroy(gameObject, 0);
                });
            }
        }
    }

    public void PopupScore(int value, Vector3 pos)
    {
        mainscript.Score += value;
        if (LevelEditorBase.THIS.showPopupScores)
        {
            Transform parent = GameObject.Find("CanvasScore").transform;
            GameObject poptxt = Instantiate(mainscript.Instance.popupScore, pos, Quaternion.identity) as GameObject;
            poptxt.transform.GetComponentInChildren<Text>().text = "" + value;
            poptxt.transform.GetComponentInChildren<Text>().color = GetScoreColor();
            poptxt.transform.SetParent(parent);
            poptxt.transform.localScale = Vector3.one;
            Destroy(poptxt, 1);
        }

    }

    Color GetScoreColor()
    {
        Color col;
        if (itemKind.color == ItemColor.RED || itemKind.itemType == ItemTypes.Extra)
            col = new Color(255f / 255f, 61f / 255f, 130f / 255f);
        else if (itemKind.color == ItemColor.YELLOW || itemKind.powerUp == Powerups.TRIPLE)
            col = new Color(254f / 255f, 205f / 255f, 36f / 255f);
        else if (itemKind.color == ItemColor.GREEN || itemKind.powerUp == Powerups.GROW)
            col = new Color(25f / 255f, 201f / 255f, 27f / 255f);
        else if (itemKind.color == ItemColor.BLUE || itemKind.powerUp == Powerups.WATER || itemKind.itemType == ItemTypes.Breakable)
            col = new Color(41f / 255f, 178f / 255f, 255f / 255f);
        else if (itemKind.color == ItemColor.VIOLET)
            col = new Color(193f / 255f, 58f / 255f, 196f / 255f);
        else if (itemKind.color == ItemColor.ORANGE || itemKind.powerUp == Powerups.FIRE)
            col = new Color(255f / 255f, 90f / 255f, 4f / 255f);
        else
            col = new Color(255f / 255f, 0f / 255f, 0f / 255f);

        return col;
    }

    public bool disableBasicExplosion;

    public void ExplEffect()
    {
        GameObject prefab = Resources.Load("Particles/BubbleExplosion") as GameObject;
        GameObject explosion = (GameObject)Instantiate(prefab, gameObject.transform.position + Vector3.back * 20f, Quaternion.identity);

        //		StartCoroutine (explode ());
    }

    IEnumerator explode()
    {
        float startTime = Time.time;
        float endTime = Time.time + 0.1f;
        Vector3 tempPosition = transform.localScale;
        Vector3 targetPrepare = transform.localScale * 1.2f;

        while (!isPaused && endTime > Time.time)
        {
            //transform.position  += targetPrepare * Time.deltaTime;
            transform.localScale = Vector3.Lerp(tempPosition, targetPrepare, (Time.time - startTime) * 10);
            //	transform.position  = targetPrepare ;
            yield return new WaitForEndOfFrame();
        }
        //      yield return new WaitForSeconds(0.01f );
        GameObject prefab = Resources.Load("Particles/BubbleExplosion") as GameObject;
        GameObject explosion = (GameObject)Instantiate(prefab, gameObject.transform.position + Vector3.back * 20f, Quaternion.identity);
        Debug.Log(explosion);
        if (square != null)
            explosion.transform.parent = square.transform;
        //   if( !isPaused )

    }

    void DestroyMatchColors()
    {
        checkNearestColor(() =>
        {
            mainscript.Instance.StartCoroutine(mainscript.Instance.SeekAndDestroyAloneBall());
        });
        if (this != null)
            Destroy(GetComponent<Rigidbody>());
        mainscript.Instance.checkBall = null;
    }

    void SetOnDestroyEffectPrefab()
    {
        if (itemKind.onDestroyEffect != null)
            Instantiate(itemKind.onDestroyEffect, transform.position, transform.rotation);
    }

    #endregion

    #region BreakableBalls

    void CheckNearBreakable()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Ball");
        Collider2D[] meshes = Physics2D.OverlapCircleAll(transform.position, 1f, layerMask);
        ArrayList c = new ArrayList();
        foreach (Collider2D obj1 in meshes)
        {
            if (obj1.gameObject.GetComponent<Ball>().itemKind.itemType == ItemTypes.Breakable)
            {
                if (c.IndexOf(obj1.gameObject) > -1)
                    continue;
                c.Add(obj1.gameObject);

                GameObject obj = obj1.gameObject;
                float distTemp = Vector3.Distance(transform.position, obj.transform.position);
                if (distTemp <= 1f)
                {
                    obj.GetComponent<Ball>().PrepareOpenBreakableBall();
                }
            }
        }
        if (colorBoost)
            destroy();
    }

    public void PrepareOpenBreakableBall()
    {
        OpenBreakableBall();
    }

    public void OpenBreakableBall( /*GameState st*/ )
    {
        if (this != null)
        {
            if (itemKind.appearBallAfterDestroyNum > 0)
            {
                if (itemKind.GetNextBallAfterDestroy().color == ItemColor.random)
                {
                    DestroyPrefabs();
                    GetComponent<ColorManager>().ChangeRandomColor();
                    Destroy(GetComponent<WaitForDestroyComponent>()); //1.2 Leaf ball and hidden ball issue
                }
                else
                {
                    itemKind = itemKind.GetNextBallAfterDestroy();
                    Destroy(GetComponent<WaitForDestroyComponent>()); //1.2 Leaf ball and hidden ball issue
                }
            }
            else
            {
                destroy();
            }
        }
    }

    #endregion
}