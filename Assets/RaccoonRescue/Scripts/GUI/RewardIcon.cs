using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RewardIcon : MonoBehaviour {
	public Sprite[] sprites;
	public string[] strings;
	public Image icon;
	public Text text;
	// Use this for initialization
	void Start () {

	}

	public void SetIconSprite (int i) {
		icon.sprite = sprites [i];
		text.text = strings [i];
	}
}
