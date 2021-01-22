using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bouncer : MonoBehaviour
{
	Vector3 tempPosition;
	Vector3 targetPrepare;
	bool isPaused;
	public bool startBounce;
	float startTime;
	public float offset;
	public List<Ball> nearBalls = new List<Ball>();
	//	private OTSpriteBatch spriteBatch = null;
	GameObject Meshes;
	public int countNEarBalls;
	float gameOverBorder;

	// Use this for initialization
	void Start()
	{
		isPaused = Camera.main.GetComponent<mainscript>().isPaused;
		gameOverBorder = Camera.main.GetComponent<mainscript>().gameOverBorder;
		targetPrepare = transform.position;
	}

	// Update is called once per frame
	void Update()
	{
	}

	IEnumerator bonceCoroutine()
	{

		while (Vector3.Distance(transform.position, targetPrepare) > 1 && !isPaused && !GetComponent<Ball>().setTarget)
		{
			//transform.position  += targetPrepare * Time.deltaTime;
			transform.position = Vector3.Lerp(tempPosition, targetPrepare, (Time.time - startTime) * 2f);
			//	transform.position  = targetPrepare ;
			yield return new WaitForSeconds(1f / 30f);
		}

	}

	IEnumerator bonceToCatapultCoroutine()
	{

		/*	while (Vector3.Distance(transform.position, targetPrepare)>1 && !isPaused && !GetComponent<ball>().setTarget ){
                //transform.position  += targetPrepare * Time.deltaTime;
                transform.position = Vector3.Lerp(tempPosition, targetPrepare,  (Time.time - startTime)*2);
                //	transform.position  = targetPrepare ;
                yield return new WaitForSeconds(1f/5f);
            }
            if(!isPaused)*/
		Invoke("delayedBonceToCatapultCoroutine", 0.5f);
		yield return new WaitForSeconds(1f / 5f);
	}

	void delayedBonceToCatapultCoroutine()
	{
		transform.position = targetPrepare;
		GetComponent<Ball>().newBall = true;

	}

	void newBall()
	{
		GetComponent<Ball>().newBall = true;
		Square.waitForAnim = false;
		mainscript.Instance.boxCatapult.GetComponent<Square>().CheckAimBoost();

	}

	public void bounceToCatapult(Vector3 vector3)
	{
		vector3 = new Vector3(vector3.x, vector3.y, gameObject.transform.position.z);
		tempPosition = transform.position;
		targetPrepare = vector3;
		startBounce = true;
		startTime = Time.time;
		iTween.MoveTo(gameObject, iTween.Hash("position", vector3, "time", 0.3, "easetype", iTween.EaseType.linear, "onComplete", "newBall"));
		//		StartCoroutine(bonceToCatapultCoroutine());
		//transform.position = vector3;
		Square.waitForAnim = false;

	}

	public void bounceTo(Vector3 vector3)
	{
		vector3 = new Vector3(vector3.x, vector3.y, gameObject.transform.position.z);
		tempPosition = transform.position;
		targetPrepare = vector3;
		startBounce = true;
		startTime = Time.time;
		if (GameEvent.Instance.GameStatus == GameState.Playing)
			iTween.MoveTo(gameObject, iTween.Hash("position", vector3, "time", 0.3, "easetype", iTween.EaseType.linear));
		else if (GameEvent.Instance.GameStatus == GameState.WinProccess)
			iTween.MoveTo(gameObject, iTween.Hash("position", vector3, "time", 0.00001, "easetype", iTween.EaseType.linear));
		//StartCoroutine(bonceCoroutine());
		//transform.position = vector3;
	}

	public void dropDown()
	{
		Vector3 v;

		//		GameObject[] meshes = GameObject.FindGameObjectsWithTag("Mesh");
		//		foreach(GameObject obj in meshes) {
		int layerMask = 1 << LayerMask.NameToLayer("Mesh");
		Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, 0.5f, layerMask);
		foreach (Collider2D obj in fixedBalls)
		{
			float distTemp = Vector3.Distance(new Vector3(transform.position.x - offset, transform.position.y, transform.position.z), obj.transform.position);
			if (distTemp <= 0.9f && obj.transform.position.y + 0.1f < transform.position.y)
			{
				if (obj.GetComponent<Square>().offset > 0)
				{
					v = new Vector3(transform.position.x + obj.GetComponent<Square>().offset, obj.transform.position.y, transform.position.z);
				}
				else
				{
					v = new Vector3(obj.transform.position.x, obj.transform.position.y, transform.position.z);
				}
				bounceTo(v);
				//	transform.position = v;
				return;
			}
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

	public void connectNearBalls()
	{
		int layerMask = 1 << LayerMask.NameToLayer("Ball");
		Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, 0.5f, layerMask);
		nearBalls.Clear();
		foreach (Collider2D obj in fixedBalls)
		{
			if (nearBalls.Count <= 7)
				nearBalls.Add(obj.GetComponent<Ball>());
		}
		countNEarBalls = nearBalls.Count;
	}

	public void checkNextNearestColor(List<Ball> b, int counter)
	{
		//		Debug.Log (b.Count);
		Vector3 distEtalon = transform.localScale;
		//		GameObject[] meshes = GameObject.FindGameObjectsWithTag(tag);
		//		foreach(GameObject obj in meshes) {
		int layerMask = 1 << LayerMask.NameToLayer("Ball");
		Collider2D[] meshes = Physics2D.OverlapCircleAll(transform.position, 1f, layerMask);
		foreach (Collider2D obj1 in meshes)
		{
			Ball ball = obj1.GetComponent<Ball>();
			if (ball.itemKind.color == gameObject.GetComponent<Ball>().itemKind.color && (ball.itemKind.itemType == ItemTypes.Simple || ball.itemKind.itemType == ItemTypes.Cub))
			{
				Ball obj = obj1.GetComponent<Ball>();
				float distTemp = Vector3.Distance(transform.position, obj.transform.position);
				if (distTemp <= 1f)
				{
					if (!findInArray(b, obj))
					{
						counter++;
						b.Add(obj);
						obj.GetComponent<Ball>().checkNextNearestColor(b, counter);
						//		destroy();
						//obj.GetComponent<mesh>().checkNextNearestColor();
						//		obj.GetComponent<mesh>().destroy();
					}
				}
			}
		}
	}

}
