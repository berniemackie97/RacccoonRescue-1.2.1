using UnityEngine;
using System.Collections;

public enum ItemColor
{
	BLUE = 1,
	GREEN,
	RED,
	VIOLET,
	YELLOW,
	ORANGE,
	random,
	Unbreakable
}


public class ColorManager : MonoBehaviour
{

	public Sprite[] sprites;
	public Sprite[] extraSprites;
	public ItemColor BallType;
	public int color;
	public static ColorManager THIS;
	// Use this for initialization
	void Start()
	{
		THIS = this;
	}

	public void SetColor(ItemColor color)
	{
		Debug.Log("Replace color " + color);
		BallType = color;
		ItemKind itemKind = LevelEditorBase.THIS.items.Find((e) => e.color == color && e.itemType == ItemTypes.Simple);
		// GetComponent<SpriteRenderer> ().sprite = GetComponent<Ball> ().itemKind.sprite;//LevelEditorBase.THIS.items [(int)color - 1].sprite;
		GetComponent<Ball>().itemKind = itemKind;
		//		foreach (Sprite item in sprites) {
		//			if (item.name == "ball_" + color) {
		//				GetComponent<SpriteRenderer> ().sprite = LevelEditorBase.THIS.items [(int)color - 1].sprite;
		//				SetSettings (color);
		//		gameObject.tag = "" + color;  //TODO: color handle
		//			}
		//		}
	}

	private void SetSettings(ItemColor color)
	{
		//		if (color == ItemColor.centerball) {
		//			if (LevelData.GetTarget () == TargetType.Round) {
		//
		//			}
		//		}
	}

	public void SetColor(int color)
	{
		BallType = (ItemColor)color;
		GetComponent<SpriteRenderer>().sprite = GetComponent<Ball>().itemKind.sprite;//LevelEditorBase.THIS.items [color - 1].sprite;
	}

	public void ChangeRandomColor()
	{
		gameObject.GetComponent<Ball>().DestroyPrefabs();
		ItemColor color = creatorBall.Instance.GetRandomColor();
		gameObject.GetComponent<Ball>().itemKind = creatorBall.Instance.GetItemKindByColor(color);
	}

	// Update is called once per frame
	void Update()
	{
		if (transform.position.y <= -16 && transform.parent == null) {
			Destroy(gameObject);
		}
		//if (!GetComponent<ball>().setTarget && GamePlay.Instance.GameStatus == GameState.Playing)
		//    transform.eulerAngles = Vector3.zero;
	}
}
