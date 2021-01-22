using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PurchasedMenu : MonoBehaviour {
	public Sprite[] sprites;
	public Image icon;
	public Text text;
	public string[] strings;
	// Use this for initialization
	void Start () {

	}

	public void SetIconSprite (BoostType bType) {
		int i = 0;
		if (bType == BoostType.ColorBallBoost)
			i = 0;
		else if (bType == BoostType.AimBoost)
			i = 1;
		else if (bType == BoostType.ExtraSwitchBallsBoost)
			i = 2;
		icon.sprite = sprites [i];
		icon.SetNativeSize ();
		icon.transform.localScale = Vector3.one * 2f;
		text.text = strings [i];
	}
}
