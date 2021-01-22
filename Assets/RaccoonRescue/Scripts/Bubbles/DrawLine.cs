using UnityEngine;
using System.Collections;

public class DrawLine : MonoBehaviour
{
	LineRenderer line;
	public bool draw = false;
	Color col;

	public static Vector2[] waypoints = new Vector2[3];
	float addAngle = 185;
	public GameObject pointer;
	public GameObject[,] pointers = new GameObject[3, 15];
	GameObject[,] pointers2 = new GameObject[3, 25];
	GameObject[,] pointers3 = new GameObject[3, 15];

	Vector3 lastMousePos;
	private bool startAnim;

	// Use this for initialization
	void Start()
	{
		line = GetComponent<LineRenderer>();
		waypoints[0] = GameObject.Find("boxCatapult").transform.position;
		waypoints[1] = waypoints[0] + Vector2.up * 5;
		for (int i = 0; i < 3; i++) {
			GeneratePoints(i);
			GeneratePositionsPoints(waypoints, i);
			HidePoints(i);
		}

	}

	int GetAimPoints()
	{
		if (!BoostVariables.AimBoost)
			return 3;
		else
			return pointers2.GetLength(1);
	}

	bool pointsHidden;
	void HidePoints(int num = 0)
	{
		//if (pointsHidden) return;
		for (int i = 0; i < pointers.GetLength(1); i++) {
			pointers[num, i].GetComponent<SpriteRenderer>().enabled = false;
			pointers[num, i].GetComponent<LinePoint>().light.SetActive(false);
		}

		for (int i = 0; i < pointers2.GetLength(1); i++) {
			pointers2[num, i].GetComponent<SpriteRenderer>().enabled = false;
			pointers2[num, i].GetComponent<LinePoint>().light.SetActive(false);
		}
		pointsHidden = true;
		//		for (int i = 0; i < pointers3.GetLength (1); i++) {
		//			pointers3 [num, i].GetComponent<SpriteRenderer> ().enabled = false;
		//		}

	}

	void HideAllPoints()
	{
		for (int i = 0; i < 3; i++) {
			HidePoints(i);
		}

	}

	void EnableBoostLight()
	{
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < pointers.GetLength(1); j++) {
				pointers[i, j].GetComponent<LinePoint>().light.SetActive(true);
			}

			for (int j = 0; j < pointers2.GetLength(1); j++) {
				pointers2[i, j].GetComponent<LinePoint>().light.SetActive(true);
			}
		}

	}

	private void GeneratePositionsPoints(Vector2[] waypoints, int num = 0)
	{
		if (mainscript.Instance.boxCatapult.GetComponent<Square>().Busy != null) {
			col = mainscript.Instance.boxCatapult.GetComponent<Square>().Busy.GetComponent<SpriteRenderer>().sprite.texture.GetPixelBilinear(0.6f, 0.6f);
			col.a = 1;
		}

		HidePoints(num);

		for (int i = 0; i < pointers.GetLength(1); i++) {
			Vector2 AB = waypoints[1] - waypoints[0];
			AB = AB.normalized;
			float step = i / 1.5f;
			Vector2 newPos = waypoints[0] + (step * AB);
			if (step >= (waypoints[1] - waypoints[0]).magnitude) {
				newPos = waypoints[1];
			}
			pointsHidden = false;
			//			if (step <= (waypoints [1] - waypoints [0]).magnitude) {
			pointers[num, i].GetComponent<SpriteRenderer>().enabled = true;
			if (BoostVariables.AimBoost)
				pointers[num, i].GetComponent<LinePoint>().light.SetActive(true);
			pointers[num, i].transform.position = newPos;
			pointers[num, i].GetComponent<SpriteRenderer>().color = col;
			pointers[num, i].GetComponent<LinePoint>().startPoint = pointers[num, i].transform.position;
			pointers[num, i].GetComponent<LinePoint>().nextPoint = pointers[num, i].transform.position;
			if (i > 0)
				pointers[num, i - 1].GetComponent<LinePoint>().nextPoint = pointers[num, i].transform.position;
			//			print (Vector2.Distance (waypoints [1], pointers [i].transform.position));
			//			}

		}
		for (int i = 0; i < GetAimPoints(); i++) {
			Vector2 AB = waypoints[2] - waypoints[1];
			AB = AB.normalized;
			float step = i / 2f;

			if (step < (waypoints[2] - waypoints[1]).magnitude) {
				pointers2[num, i].GetComponent<SpriteRenderer>().enabled = true;
				if (BoostVariables.AimBoost)
					pointers2[num, i].GetComponent<LinePoint>().light.SetActive(true);
				pointers2[num, i].transform.position = waypoints[1] + (step * AB);
				pointers2[num, i].GetComponent<SpriteRenderer>().color = col;
				pointers2[num, i].GetComponent<LinePoint>().startPoint = pointers2[num, i].transform.position;
				pointers2[num, i].GetComponent<LinePoint>().nextPoint = pointers2[num, i].transform.position;
				if (i > 0)
					pointers2[num, i - 1].GetComponent<LinePoint>().nextPoint = pointers2[num, i].transform.position;
			}
		}
	}

	void GeneratePoints(int num = 0)
	{
		for (int i = 0; i < pointers.GetLength(1); i++) {
			pointers[num, i] = Instantiate(pointer, transform.position, transform.rotation) as GameObject;
			pointers[num, i].transform.parent = transform;
			pointers[num, i].GetComponent<LinePoint>().light.SetActive(false);
			pointers[num, i].GetComponent<LinePoint>().SetDraw(this);
		}
		for (int i = 0; i < pointers2.GetLength(1); i++) {
			pointers2[num, i] = Instantiate(pointer, transform.position, transform.rotation) as GameObject;
			pointers2[num, i].transform.parent = transform;
			pointers2[num, i].GetComponent<LinePoint>().light.SetActive(false);
			pointers2[num, i].GetComponent<LinePoint>().SetDraw(this);

		}
	}

	bool boostEnabled;
	// Update is called once per frame
	void Update()
	{
		if (BoostVariables.AimBoost && !boostEnabled) {
			boostEnabled = true;
			//EnableBoostLight();//commented ro prevent double line
		}
		if (Input.GetMouseButtonDown(0)) {
			draw = true;
		} else if (Input.GetMouseButtonUp(0)) {
			draw = false;
		}

		if (draw && !mainscript.StopControl && GameEvent.Instance.GameStatus == GameState.Playing) {
			Draw(waypoints);
			if (mainscript.Instance.lauchingBall) {
				if (mainscript.Instance.lauchingBall.PowerUp == Powerups.TRIPLE) {
					Draw(waypoints, 1);
					Draw(waypoints, 2);
				}
			}
		} else if (!draw) {
			for (int i = 0; i < 3; i++) {
				HidePoints(i);
			}
		}

	}

	void Draw(Vector2[] waypoints_, int num = 0)
	{
		Vector3 dir = Vector2.zero;

		dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Vector3.back * 10;
		if (dir.y < mainscript.Instance.lowerLineAngle) {
			HideAllPoints();
			return;
		}

		if (num == 1) {
			dir.x += 1.5f;
		}
		if (num == 2) {
			dir.x -= 1.5f;
		}

		//     if( dir.y - 2 < transform.position.y ) { HidePoints(); return; }
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (!mainscript.StopControl) {//dir.y < 15.5 && dir.y > - 2 && 

			dir.z = 0;
			if (num == 0) {
				if (lastMousePos == dir) {
					startAnim = true;
				} else
					startAnim = false;
				lastMousePos = dir;
			}


			//				waypoints [0] = transform.position;
			//int layerMask = ~(1 << LayerMask.NameToLayer("Mesh"));
			//			if (num == 0) {

			RaycastHit2D[] hit = Physics2D.LinecastAll(waypoints_[0], waypoints_[0] + ((Vector2)dir - waypoints_[0]).normalized * 100, ~(1 << LayerMask.NameToLayer("Mesh")));
			foreach (RaycastHit2D item in hit) {
				Vector2 point = item.point;

				addAngle = 180;

				if (waypoints_[1].x < 0)
					addAngle = 10;
				if (item.collider.gameObject.layer == LayerMask.NameToLayer("Border") && item.collider.gameObject.name != "GameOverBorder" && item.collider.gameObject.name != "borderForRoundedLevels") {
					if (item.collider.name == "TopBorder" && LevelData.GetTarget() != TargetType.Round)
						GetReversSquare(waypoints_[1], waypoints_[0], num);
					mainscript.Instance.collidingPoints[num] = point;
					if (num == 0) {
						mainscript.Instance.ballOnLine[0] = null;
						mainscript.Instance.touchTheBorder[0] = false;
					}
					waypoints_[1] = point;
					waypoints_[2] = point;
					float angle = 0;
					angle = Vector2.Angle(waypoints_[0] - waypoints_[1], (point - Vector2.up * 100) - (Vector2)point);
					if (waypoints_[1].x > 0)
						angle = Vector2.Angle(waypoints_[0] - waypoints_[1], (Vector2)point - (point - Vector2.up * 100));
					waypoints_[2] = Quaternion.AngleAxis(angle + addAngle, Vector3.back) * ((Vector2)point - (point - Vector2.up * 100));
					RaycastHit2D hit2 = Physics2D.Raycast(waypoints_[1], waypoints_[2], 1000, (1 << LayerMask.NameToLayer("Ball")));
					//Debug.DrawLine(waypoints_[1], waypoints_[2]);
					//Debug.DrawRay(waypoints_[1], waypoints_[2],Color.green);
					if (hit2.collider != null) {
						mainscript.Instance.ballOnLine[num] = hit2.collider.gameObject.GetComponent<Ball>();
						mainscript.Instance.touchTheBorder[num] = true;

						waypoints_[2] = hit2.point;


					}
					break;

				} else if (item.collider.gameObject.layer == LayerMask.NameToLayer("Ball")) {
					if (num == 0) {
						mainscript.Instance.reverseMesh[0] = null;

					}
					mainscript.Instance.ballOnLine[num] = item.collider.GetComponent<Ball>();
					mainscript.Instance.touchTheBorder[num] = false;
					waypoints_[1] = point;
					waypoints_[2] = point;
					//					}
					break;

				} else {
					mainscript.Instance.ballOnLine[num] = null;
					if (num == 0) {

						mainscript.Instance.reverseMesh[0] = null;
					}
					mainscript.Instance.touchTheBorder[num] = false;
					waypoints_[1] = waypoints_[0] + ((Vector2)dir - waypoints_[0]).normalized * 10;
					waypoints_[2] = waypoints_[0] + ((Vector2)dir - waypoints_[0]).normalized * 10;
				}
			}
			//			}
			if (!startAnim)
				GeneratePositionsPoints(waypoints_, num);
		}
	}

	void GetReversSquare(Vector2 firstPos, Vector2 endPos, int num)
	{
		//		Debug.DrawLine (firstPos, endPos);
		RaycastHit2D[] hit = Physics2D.LinecastAll(firstPos, endPos, 1 << 10);
		foreach (RaycastHit2D item in hit) {
			if (item.collider.gameObject.GetComponent<Square>().Busy == null) {
				mainscript.Instance.reverseMesh[num] = item.collider.GetComponent<Square>();
				//				Debug.DrawLine (firstPos, item.collider.transform.position, Color.green);
				return;
			}

		}
		mainscript.Instance.reverseMesh[num] = null;

	}

}
