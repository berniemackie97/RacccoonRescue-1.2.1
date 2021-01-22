using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Square : MonoBehaviour
{
	[SerializeField]
	private Ball busy;

	public Ball Busy
	{
		get { return busy; }
		set
		{
			if (value != null)
			{
				if (thisName == "boxCatapult")
				{
					mainscript.Instance.lauchingBall = value;
				}
				if (!value.NotSorting)
				{
					//					value.GetComponent<SpriteRenderer> ().sortingOrder = Mathf.FloorToInt (1 / (transform.position.y + 10) * 100);// disabled for sorting ice balls on long level
					value.SetOrderPrefabs();
					value.square = this;
					value.SetupCollider();
					if (value.tag == "centerball")
						value.GetComponent<SpriteRenderer>().sortingOrder = 100;
				}
			}

			busy = value;
		}
	}

	GameObject[] meshes;
	bool destroyed;
	public float offset;
	bool triggerball;
	public GameObject boxFirst;
	public GameObject boxSecond;
	public static bool waitForAnim;
	public int row;
	public int col;
	public GameObject aim_boost_effect;
	[HideInInspector]
	public BoxCollider2D boxCollider;
	string thisName;
	// Use this for initialization
	void Awake()
	{
		boxCollider = GetComponent<BoxCollider2D>();
		thisName = name;
	}

	bool enabledWheel;
	// Update is called once per frame
	void Update()
	{
		if (busy == null)
		{
			GameObject box = null;
			Ball ball = null;
			if (thisName == "boxCatapult" && !Square.waitForAnim /*&& !mainscript.Instance.block*/)
			{
				box = boxSecond;
				ball = box.GetComponent<Square>().busy;
				if (ball != null && GameEvent.Instance.GameStatus == GameState.Playing)
				{
					ball.GetComponent<bouncer>().bounceToCatapult(transform.position);
					//ball.GetComponent<ball>().newBall = true;
					mainscript.Instance.lauchingBall = ball;
					ball.GetComponent<SpriteRenderer>().sortingOrder += 10;
					busy = ball;
					aim_boost_effect.SetActive(false);
					box.GetComponent<Square>().Busy = null;
					//busy.GetComponent<SpriteRenderer>().sortingOrder = 1;
					mainscript.Instance.SetColorsForNewBall();
					if (mainscript.Instance.currentLevel == 101)
					{
						//busy.SetPower(Powerups.TRIPLE);
						//Debug.LogError("comment it after test"); //TODO: comment it after test
					}
				}
			}
			else if (thisName == "boxSecond" && !Square.waitForAnim)
			{
				if (BoostVariables.ExtraSwitchBallsBoost)
				{
					box = boxFirst;
					ball = box.GetComponent<Square>().busy;
					if (ball != null)
					{
						ball.GetComponent<bouncer>().bounceTo(transform.position); //1.1
						busy = ball;
						box.GetComponent<Square>().Busy = null;
						busy.GetComponent<SpriteRenderer>().sortingOrder = 1;
					}
				}
				else
				{
					if ((GameEvent.Instance.GameStatus == GameState.Playing || GameEvent.Instance.GameStatus == GameState.WaitForTarget2) && LevelData.LimitAmount > 1)
					{
						busy = Camera.main.GetComponent<mainscript>().createFirstBall(transform.position);
						busy.NotSorting = true;
					}
					else if ((GameEvent.Instance.GameStatus == GameState.WinProccess || GameEvent.Instance.GameStatus == GameState.PrePlayBanner) && LevelData.LimitAmount > 0)
					{
						busy = Camera.main.GetComponent<mainscript>().createFirstBall(transform.position);
						busy.NotSorting = true;
					}
				}

			}
			else if (thisName == "boxFirst" && !Square.waitForAnim && BoostVariables.ExtraSwitchBallsBoost)
			{
				if (BoostVariables.ExtraSwitchBallsBoost && !enabledWheel)
				{
					enabledWheel = true;
					transform.GetChild(0).gameObject.SetActive(true);
				}
				if ((GameEvent.Instance.GameStatus == GameState.Playing || GameEvent.Instance.GameStatus == GameState.WaitForTarget2) && LevelData.LimitAmount > 2)
				{
					busy = Camera.main.GetComponent<mainscript>().createFirstBall(transform.position);
					busy.NotSorting = true;
					busy.GetComponent<SpriteRenderer>().sortingOrder = 2;
				}
				else if (GameEvent.Instance.GameStatus == GameState.WinProccess && LevelData.LimitAmount > 0)
				{
					busy = Camera.main.GetComponent<mainscript>().createFirstBall(transform.position);
					busy.GetComponent<SpriteRenderer>().sortingOrder = 2;
					busy.NotSorting = true;
				}

			}

		}

		if (busy != null && !Square.waitForAnim)
		{
			if (thisName == "boxCatapult")
			{
				//if(Vector3.Distance(transform.position, busy.transform.position) > 1.6 )
				if (busy.GetComponent<Ball>().setTarget)
					busy = null;
			}
			else if (thisName == "boxFirst")
			{
				if (Vector3.Distance(transform.position, busy.transform.position) > 2)
					busy = null;
			}
			else if (thisName == "boxSecond")
			{
				if (Vector3.Distance(transform.position, busy.transform.position) > 0.9f)
				{
					busy = null;
				}
			}
		}
	}

	public void CheckAimBoost()
	{
		if (thisName == "boxCatapult")
		{
			if (BoostVariables.AimBoost)
				aim_boost_effect.SetActive(true);
		}
	}

	public void BounceFrom(GameObject box)
	{
		Ball ball = box.GetComponent<Square>().busy;
		if (ball != null && busy != null)
		{
			//		ball.GetComponent<bouncer>().bounceToCatapult(transform.position);
			busy.GetComponent<bouncer>().bounceTo(box.transform.position);
			box.GetComponent<Square>().busy = busy;
			busy = ball;
		}
	}

	void setColorTag(GameObject ball)
	{
		if (ball.name.IndexOf("Orange") > -1)
		{
			ball.tag = "Fixed";
			//	tag = "Orange";
		}
		else if (ball.name.IndexOf("Red") > -1)
		{
			ball.tag = "Fixed";
			//	tag = "Red";
		}
		else if (ball.name.IndexOf("Yellow") > -1)
		{
			ball.tag = "Fixed";
			//	tag = "Yellow";
		}
	}

	void OnCollisionStay2D(Collision2D other)
	{
		if (other.gameObject.name.IndexOf("ball") > -1 && busy == null)
		{

			busy = other.gameObject.GetComponent<Ball>();
		}
	}

	void OnTriggerExit(Collider other)
	{
		//busy = null;
	}

	public void EnableMeshesAround()
	{
		List<Square> squares = creatorBall.Instance.GetSquaresAround(this.GetComponent<Square>());
		foreach (var item in squares)
		{
			item.GetComponent<BoxCollider2D>().enabled = true;
			//			print (item + " c: " + item.col + " r: " + item.row);
		}
	}

	public void destroy()
	{
		tag = "Mesh";
		Destroy(busy);
		busy = null;
	}







}
