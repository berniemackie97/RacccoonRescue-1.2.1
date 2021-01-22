using UnityEngine;
using System.Collections;

public class Cub : MonoBehaviour
{
	public GameObject shadow;
	public GameObject parachute;
	public Animator anim;
	Vector3[] randomPos;
	Vector3 targetPos;
	// Use this for initialization
	void Start()
	{
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.pops);
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.baby[Random.Range(0, SoundBase.Instance.baby.Length)]);
		randomPos = new Vector3[2];
		randomPos[0] = new Vector3(0.9f, -3.5f, 0);
		randomPos[1] = new Vector3(3.08f, -2.7f, 0);
		StartCoroutine(Fall());

	}


	IEnumerator Fall()
	{

		float startTime = Time.time;
		Vector3 startPos = transform.position;
		//		randomPos = Random.insideUnitCircle + (Vector2)randomPos;
		float x = Random.Range(randomPos[0].x, randomPos[1].x);
		float y = Random.Range(randomPos[0].y, randomPos[1].y);
		targetPos = new Vector3(x, y, 0);
		foreach (Transform item in transform.GetChild(0).transform) {
			SpriteRenderer spr = item.GetComponent<SpriteRenderer>();
			spr.sortingOrder = (int)((Mathf.Abs(targetPos.y * 10) + 2 + targetPos.x * 10));

		}

		float speed = 2.1f;
		float distCovered = 0;
		while (distCovered < 1) {
			//			speed += Time.deltaTime;
			distCovered = (Time.time - startTime) / speed;
			transform.position = Vector3.Lerp(startPos, targetPos, distCovered);
			yield return new WaitForFixedUpdate();
		}
		Landing();
	}

	void Landing()
	{
		anim.SetTrigger("idle");
		shadow.SetActive(true);
		parachute.SetActive(false);
		//		anim.Stop ();
		transform.position = targetPos;
		transform.rotation = Quaternion.identity;

	}
}
