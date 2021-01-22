using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using InitScriptName;

public enum BoostType
{
	ExtraSwitchBallsBoost = 0,
	ColorBallBoost,
	AimBoost
}

public class BoostShop : MonoBehaviour
{
	public Sprite[] icons;
	public string[] titles;
	public string[] descriptions;
	public int[] prices;
	public Image icon;
	public Text title;
	public Text description;
	public Text price;

	BoostType boostType;
	System.Action<int> buySuccess;
	// Use this for initialization
	void Start()
	{

	}

	void Update()
	{
		//		GameEvent.Instance.GameStatus = GameState.Pause;
	}

	// Update is called once per frame
	public void SetBoost(BoostType _boostType, System.Action<int> buyCallback)
	{
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
		boostType = _boostType;
		gameObject.SetActive(true);
		icon.sprite = icons[(int)_boostType];
		icon.SetNativeSize();
		title.text = titles[(int)_boostType];
		description.text = descriptions[(int)_boostType];
		int pr = prices[(int)_boostType];
		price.text = "" + pr;
		buySuccess = buyCallback;

	}

	public void BuyBoost()
	{
		//		GetComponent<AnimationManager> ().BuyBoost (boostType, prices [(int)boostType]);
		SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
		if (InitScript.Gems >= prices[(int)boostType]) {
			//			InitScript.Instance.BuyBoost (boostType, 1, prices [(int)boostType]);
			//			InitScript.Instance.SpendBoost (boostType);
			buySuccess((prices[(int)boostType]));
			GetComponent<AnimationManager>().CloseMenu();
		} else {
			MenuManager.Instance.ShowCurrencyShop();
		}
	}

}
