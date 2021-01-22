using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Lamp : MonoBehaviour
{
	public Image fillRect;
	public ItemColor colorLamp;
	public GameObject light;
	public Powerups powerup;
	public Animator anim;
	public GameObject lightningEffect;

	void Start()
	{
		fillRect.fillAmount = 0;
		light.SetActive(false);
		lightningEffect.SetActive(false);
		if (colorLamp == 0) {
			Debug.LogError("lamp's color not defined");
			return;
		}

	}

	// Use this for initialization
	void OnEnable()
	{
		//LightContainerLamp.OnFinished += ApplyPower;
		Ball.OnDestroy += Fill;
	}

	void OnDisable()
	{
		//LightContainerLamp.OnFinished -= ApplyPower;
		Ball.OnDestroy -= Fill;
	}


	void Fill(ItemColor color)
	{
		if (colorLamp == color && fillRect.fillAmount < 1 && (GameEvent.Instance.GameStatus == GameState.BlockedGame || GameEvent.Instance.GameStatus == GameState.Playing)) {
			fillRect.fillAmount += 0.066666666666667f;
			if (fillRect.fillAmount == 1) {
				SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.powerup_fill);
				anim.SetTrigger("Play");
				light.SetActive(true);
			}
		}
	}

	public Ball ball;
	ItemColor clickedLampColor;

	public void OnClick()
	{
		GameObject catapult = GameObject.Find("boxCatapult");
		ball = catapult.GetComponent<Square>().Busy;
		clickedLampColor = colorLamp;
		if (ball != null) {
			if (fillRect.fillAmount == 1 && GameEvent.Instance.GameStatus == GameState.Playing && ball.PowerUp == Powerups.NONE && !mainscript.Instance.lauchingBall.colorBoost) {
				light.SetActive(false);
				SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.powerup_click[Random.Range(0, SoundBase.Instance.powerup_click.Length)]);

				// fillRect.fillAmount = 0;
				lightningEffect.SetActive(true);
				StartCoroutine(FlyToTarget());
				ApplyPower();
			}
		}
	}

	void ApplyPower()
	{
		if (ball != null) {
			if (fillRect.fillAmount == 1 && colorLamp == clickedLampColor) {
				mainscript.Instance.SetPower(powerup);
				fillRect.fillAmount = 0;

			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (/*!LevelData.colorsDict.ContainsValue(colorLamp) ||*/ LevelData.powerups[(int)powerup - 1] == 0)
			gameObject.SetActive(false);
		if (fillRect.fillAmount == 1 && Random.Range(0, 100) == 1)
			anim.SetTrigger("Play");

	}

	IEnumerator FlyToTarget()
	{
		Vector3 targetPos = GameObject.Find("boxCatapult").transform.position;

		AnimationCurve curveX = new AnimationCurve(new Keyframe(0, fillRect.transform.position.x), new Keyframe(0.5f, targetPos.x));
		AnimationCurve curveY = new AnimationCurve(new Keyframe(0, fillRect.transform.position.y), new Keyframe(0.5f, targetPos.y));
		curveY.AddKey(0.2f, fillRect.transform.position.y + 1);
		float startTime = Time.time;
		Vector3 startPos = transform.position;
		float speed = 0.2f;
		float distCovered = 0;
		// while (distCovered < 0.5f)
		// {
		//     distCovered = (Time.time - startTime);
		//     fillRect.transform.position = new Vector3(curveX.Evaluate(distCovered), curveY.Evaluate(distCovered), 0);
		//     fillRect.transform.Rotate(Vector3.back * 10);
		//     yield return new WaitForEndOfFrame();
		// }
		yield return new WaitForSeconds(1.5f);

		fillRect.rectTransform.localPosition = Vector3.zero;

	}

}
