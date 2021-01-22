using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class creatorBall : MonoBehaviour
{
    public static creatorBall Instance;
    public GameObject ball_hd;
    public GameObject ball_ld;
    public GameObject bug_hd;
    public GameObject bug_ld;
    public GameObject thePrefab;
    GameObject ball;
    public static int columns = 11;
    public static int rows = 70;
    public static List<Vector2> grid = new List<Vector2>();
    int lastRow;
    float offsetStep = 0.33f;
    //private OTSpriteBatch spriteBatch = null;
    GameObject Meshes;
    [HideInInspector]
    public List<Square> squares = new List<Square>();
    int[] map;
    private int maxCols;
    private int maxRows;


    // Use this for initialization
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        ball = ball_hd;
        thePrefab.transform.localScale = new Vector3(0.67f, 0.58f, 1);
        Meshes = GameObject.Find("-Ball");
        //LevelData.LoadDataFromXML(mainscript.Instance.currentLevel);
        LevelData.LoadLevel((maxrows, maxcols) =>
        {
            this.maxCols = maxcols;
            this.maxRows = maxrows;
        }
        );

        //LevelData.LoadDataFromLocal(mainscript.Instance.currentLevel);
        if (LevelData.GetTarget() == TargetType.Top || LevelData.GetTarget() == TargetType.RescuePets)
            MoveLevelDown();
        else
        {
            // GameObject.Find( "TopBorder" ).transform.position += Vector3.down * 3.5f;
            GameObject.Find("TopBorder").transform.parent = null;
            GameObject.Find("TopBorder").GetComponent<SpriteRenderer>().enabled = false;
            GameObject ob = GameObject.Find("-Meshes");
            ob.transform.position += Vector3.up * 3f;
            ob.AddComponent<LockLevelRounded>();
            GameEvent.Instance.GameStatus = GameState.PrePlayBanner;
        }
        createMesh();
        LoadMap(LevelData.map);
        Camera.main.GetComponent<mainscript>().connectNearBallsGlobal();
        StartCoroutine(getBallsForMesh());
        //        ShowBugs();
    }

    public void LoadMap(int[] pMap)
    {
        map = pMap;
        int roww = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int mapValue = map[i * columns + j];
                if (mapValue > 0 && mapValue < LevelEditorBase.THIS.items.Count)
                {
                    roww = i;
                    if (LevelData.GetTarget() == TargetType.Round)
                        roww = i + 6;
                    ItemKind itemKind = LevelEditorBase.THIS.items[mapValue];
                    createBall(GetSquare(roww, j).transform.position, itemKind.color, false, i, itemKind);

                    //					if (!LevelData.colorsDict.ContainsValue (b.itemKind.color) && (b.itemKind.itemType == ItemTypes.Simple || b.itemKind.itemType == ItemTypes.Pet)) {
                    //						LevelData.colorsDict.Add (key, b.itemKind.color);
                    //						key++;
                    //					}
                }
                else if (mapValue == 0 && LevelData.GetTarget() == TargetType.Top && i == 0)
                {
                    Instantiate(Resources.Load("Prefabs/TargetStar"), GetSquare(i, j).transform.position, Quaternion.identity);
                }
            }
        }
    }


    private void MoveLevelUp()
    {
        Debug.Log("move up");
        StartCoroutine(MoveUpDownCor());
    }

    public bool levelMoving;

    IEnumerator MoveUpDownCor(bool inGameCheck = false, Action callback = null)
    {
        yield return new WaitForSeconds(0.1f);
        if (!inGameCheck)
            GameEvent.Instance.GameStatus = GameState.WaitForPopup;
        bool up = false;
        levelMoving = true;
        mainscript.Instance.BlockGame(ItemColor.RED);
        List<float> table = new List<float>();
        float lineY = -1.3f;//GameObject.Find( "GameOverBorder" ).transform.position.y;
        Transform bubbles = GameObject.Find("-Ball").transform;
        int i = 0;
        foreach (Transform item in bubbles)
        {
            if (item == null)
                continue;
            if (!inGameCheck)
            {
                if (item.position.y < lineY)
                {
                    table.Add(item.position.y);
                }
            }
            else if (!item.GetComponent<Ball>().Destroyed)
            {
                if (item.position.y > lineY && mainscript.Instance.TopBorder.transform.position.y > 5f)
                {
                    table.Add(item.position.y);
                }
                else if (item.position.y < lineY + 1f)
                {
                    table.Add(item.position.y);
                    up = true;
                }
            }
            i++;
        }


        if (table.Count > 0)
        {
            if (up)
                AddMesh();

            float targetY = 0;
            table.Sort();
            if (!inGameCheck)
                targetY = lineY - table[0] + 2.5f;
            else
                targetY = lineY - table[0] + 1.5f;
            GameObject Meshes = GameObject.Find("-Meshes");
            Vector3 targetPos = Meshes.transform.position + Vector3.up * targetY;
            float startTime = Time.time;
            Vector3 startPos = Meshes.transform.position;
            float speed = 0.5f;
            float distCovered = 0;
            while (distCovered < 1)
            {
                //				print (table.Count);
                speed += Time.deltaTime / 1.5f;
                distCovered = (Time.time - startTime) / speed;
                Meshes.transform.position = Vector3.Lerp(startPos, targetPos, distCovered);
                yield return new WaitForEndOfFrame();
                if (startPos.y > targetPos.y)
                {
                    if (mainscript.Instance.TopBorder.transform.position.y <= 6.0f && inGameCheck)
                        break;
                }
            }
        }

        levelMoving = false;
        //        Debug.Log("lift finished");
        if (GameEvent.Instance.GameStatus == GameState.WaitForPopup)
            GameEvent.Instance.GameStatus = GameState.PrePlayBanner;
        //		else if (GameEvent.Instance.GameStatus != GameState.GameOver && GameEvent.Instance.GameStatus != GameState.WinProccess)
        //			GameEvent.Instance.GameStatus = GameState.Playing;


    }

    public void MoveLevelDown()
    {
        StartCoroutine(MoveUpDownCor(true));
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator getBallsForMesh()
    {
        GameObject[] meshes = GameObject.FindGameObjectsWithTag("Mesh");
        foreach (GameObject obj1 in meshes)
        {
            Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(obj1.transform.position, 0.2f, 1 << 9);  //balls
            foreach (Collider2D obj in fixedBalls)
            {
                obj1.GetComponent<Square>().Busy = obj.gameObject.GetComponent<Ball>();
                //	obj.GetComponent<bouncer>().offset = obj1.GetComponent<Grid>().offset;
            }
        }
        yield return new WaitForSeconds(0.5f);
    }

    public void EnableGridColliders()
    {
        foreach (Square item in squares)
        {
            item.boxCollider.enabled = true;
        }
    }

    public void EnableGridCollidersAroundSquare(Square sq)
    {
        List<Square> squaresAround = GetSquaresAround(sq);
        foreach (Square item in squaresAround)
        {
            item.boxCollider.enabled = true;
        }
    }


    public void OffGridColliders()
    {
        foreach (Square item in squares)
        {
            if (item.boxCollider.enabled && (LevelData.GetTarget() != TargetType.Round && item.row > 0))
                item.boxCollider.enabled = false;
        }
    }

    public void createRow(int j)
    {
        float offset = 0;
        for (int i = 0; i < columns; i++)
        {
            if (j % 2 == 0)
                offset = 0;
            else
                offset = offsetStep;
            Vector3 v = new Vector3(transform.position.x + i * thePrefab.transform.localScale.x + offset, transform.position.y - j * thePrefab.transform.localScale.y, transform.position.z);
            createBall(v);
        }
    }

    public void CreateItem(Vector3 pos, ItemKind kind)
    {
        createBall(pos, kind.color);
    }

    public Ball createBall(Vector3 vec, ItemColor color = ItemColor.random, bool newball = false, int row = 1, ItemKind itemKind = null)
    {
        GameObject b = null;

        if (color == ItemColor.random && !newball)
            color = (ItemColor)LevelData.colorsDict[UnityEngine.Random.Range(0, LevelData.colorsDict.Count)];

        if (newball && mainscript.colorsDict.Count > 0)
        {
            //			if (GameEvent.Instance.GameStatus == GameState.Playing) {
            color = (ItemColor)mainscript.colorsDict[UnityEngine.Random.Range(0, mainscript.colorsDict.Count)];
            //			}
        }
        else if (newball)
        {
            color = (ItemColor)LevelData.colorsDict[UnityEngine.Random.Range(0, LevelData.colorsDict.Count)];

        }

        b = Instantiate(ball, transform.position, transform.rotation) as GameObject;
        b.transform.position = new Vector3(vec.x, vec.y, ball.transform.position.z);
        //		b.GetComponent<ColorManager> ().SetColor ((int)color);
        b.transform.parent = Meshes.transform;
        ItemTypes itemType = ItemTypes.Simple;
        if (itemKind != null)
        {
            itemType = itemKind.itemType;
            if (itemKind.color == ItemColor.random && itemType == ItemTypes.Simple)
                itemKind = LevelEditorBase.THIS.items.Find((e) => e.color == color && e.itemType == itemType);
        }
        else
            itemKind = LevelEditorBase.THIS.items.Find((e) => e.color == color && e.itemType == itemType);

        b.GetComponent<Ball>().itemKind = itemKind;
        if (itemKind.itemType == ItemTypes.Extra)
            b.GetComponent<Ball>().SetPower(itemKind.powerUp);

        if (itemKind.itemType == ItemTypes.Rotation)
            b.tag = "centerball";

        b.GetComponent<Ball>().AddPrefab();

        GameObject[] fixedBalls = GameObject.FindGameObjectsWithTag("Ball");
        ;
        b.name = b.name + fixedBalls.Length.ToString();
        b.gameObject.GetComponent<Ball>().SetupBall(newball, row, itemKind);
        return b.gameObject.GetComponent<Ball>();
    }


    public ItemColor GetRandomColor()
    {
        ItemColor color = ItemColor.BLUE;
        color = (ItemColor)LevelData.colorsDict[UnityEngine.Random.Range(0, LevelData.colorsDict.Count)];
        return color;
    }

    public ItemKind GetItemKindByColor(ItemColor color)
    {
        ItemKind itemKind = LevelEditorBase.THIS.items.Find((e) => e.color == color && e.itemType == ItemTypes.Simple);
        return itemKind;
    }

    public void createMesh()
    {

        GameObject Meshes = GameObject.Find("-Meshes");
        float offset = 0;

        for (int j = 0; j < rows + 1; j++)
        {
            for (int i = 0; i < columns; i++)
            {
                if (j % 2 == 0)
                    offset = 0;
                else
                    offset = offsetStep;
                GameObject b = Instantiate(thePrefab, transform.position, transform.rotation) as GameObject;
                Vector3 v = new Vector3(transform.position.x + i * b.transform.localScale.x + offset, transform.position.y - j * b.transform.localScale.y, transform.position.z);
                b.transform.parent = Meshes.transform;
                b.transform.localPosition = v;
                b.tag = "Mesh";
                GameObject[] fixedBalls = GameObject.FindGameObjectsWithTag("Mesh");
                b.name = b.name + fixedBalls.Length.ToString();
                Square sq = b.GetComponent<Square>();
                sq.offset = offset;
                sq.row = j;
                sq.col = i;
                squares.Add(sq);
                lastRow = j;
            }
        }
        creatorBall.Instance.OffGridColliders();

    }

    public void AddMesh()
    {
        GameObject Meshes = GameObject.Find("-Meshes");
        float offset = 0;
        int j = lastRow + 1;
        for (int i = 0; i < columns; i++)
        {
            if (j % 2 == 0)
                offset = 0;
            else
                offset = offsetStep;
            GameObject b = Instantiate(thePrefab, transform.position, transform.rotation) as GameObject;
            Vector3 v = new Vector3(transform.position.x + i * b.transform.localScale.x + offset, transform.position.y - j * b.transform.localScale.y, transform.position.z);
            b.transform.parent = Meshes.transform;
            b.transform.position = v;
            b.tag = "Mesh";
            GameObject[] fixedBalls = GameObject.FindGameObjectsWithTag("Mesh");
            b.name = b.name + fixedBalls.Length.ToString();
            Square sq = b.GetComponent<Square>();
            sq.offset = offset;
            sq.row = j;
            sq.col = i;

            squares.Add(sq);
        }
        lastRow = j;

    }


    //TODO move next methods to mainscript
    //	public List<Square> GetSquaresAround (Square sq) {
    //		int row = sq.row;
    //		int col = sq.col;
    //		List<Square> squaresAround = new List<Square> ();
    //		for (int i = row - 2; i < row + 2; i++) {
    //			for (int j = col - 2; j < col + 2; j++) {
    //				if (i >= 0 && i < maxRows && j >= 0 && j < maxCols)
    //					squaresAround.Add (GetSquare (i, j));
    //			}
    //		}
    //		return squaresAround;
    //	}

    public List<Square> GetSquaresAround(Square sq, int radius = 1)
    {
        int row = sq.row;
        int col = sq.col;
        List<Square> squaresAround = new List<Square>();
        int r = 1;
        InitRanges(sq, r);
        for (int i = row - r; i <= row + r; i++)
        {
            if ((sq.offset == 0 && GetSquare(i, 0).offset == 0) || (sq.offset > 0 && GetSquare(i, 0).offset > 0))
            {
                firstCol = col - r;
                lastCol = col + r;
            }
            else
            {
                InitRanges(sq, r);
            }
            for (int j = firstCol; j <= lastCol; j++)
            {
                if (i >= 0 && i < maxRows && j >= 0 && j < maxCols)
                    squaresAround.Add(GetSquare(i, j));
            }

        }
        for (int i = 1; i < radius; i++)
        {
            List<Square> listCopy = new List<Square>();
            foreach (var item in squaresAround)
            {
                listCopy.Add(item);
            }
            foreach (Square item in listCopy)
            {
                List<Square> extraListSquares = GetSquaresAround(item, 1);
                foreach (Square s in extraListSquares)
                {
                    if (listCopy.IndexOf(s) < 0)
                        squaresAround.Add(s);
                }
            }
        }

        return squaresAround;
    }

    int firstCol;
    int lastCol;

    void InitRanges(Square sq, int radius = 1)
    {
        int row = sq.row;
        int col = sq.col;

        firstCol = col - radius;
        lastCol = col;
        if (sq.offset > 0)
        {
            firstCol = col;
            lastCol = col + radius;
        }

    }

    public Square GetSquare(int row, int col)
    {
        row = Mathf.Clamp(row, 0, maxRows - 1);
        col = Mathf.Clamp(col, 0, maxCols - 1);
        return squares[row * columns + col];
    }

    public Square[] GetSquares(Vector2 section, Vector2 origin, bool withEmpty = false)
    {
        section.x = Mathf.Clamp(section.x, 0, maxCols);
        section.y = Mathf.Clamp(section.y, 0, maxRows);
        List<Square> sqList = new List<Square>();

        GetSquaresParameters rowParam;
        rowParam = new GetSquaresParameters((int)origin.y, (int)section.y, 1);
        if (section.y - origin.y < 0)
            rowParam = new GetSquaresParameters((int)origin.y, (int)section.y, -1);

        GetSquaresParameters colParam;
        colParam = new GetSquaresParameters((int)origin.x, (int)section.x, 1);
        if (section.x - origin.x < 0)
            colParam = new GetSquaresParameters((int)origin.x, (int)section.x, -1);

        for (int r = rowParam.start; rowParam.GetEnd(r, rowParam.max); r += rowParam.step)
        {
            for (int c = colParam.start; colParam.GetEnd(c, colParam.max); c += colParam.step)
            {
                Square sq = GetSquare(r, c);
                if (sq.Busy == null && !withEmpty)
                {
                    for (int i = -1; i < 1; i++)
                    {
                        sq = GetSquare(r, c + i);
                        if (sq.Busy != null)
                            break;
                    }
                }
                sqList.Add(sq);
            }
        }

        return sqList.ToArray();
    }
}

public class GetSquaresParameters
{
    public int start, max, step;

    public GetSquaresParameters(int _start, int _max, int _step)
    {
        start = _start;
        max = _max;
        step = _step;
    }

    public bool GetEnd(int a1, int a2)
    {
        if (step > 0)
            return a1 <= a2;
        else
            return a1 >= a2;
    }
}
