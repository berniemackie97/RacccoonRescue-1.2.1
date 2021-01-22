using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapLevelNumber : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
//		GetComponent<Renderer> ().sortingLayerName = "New Layer 1";
//		GetComponent<Renderer> ().sortingOrder = 3;
		int num = int.Parse (transform.parent.parent.name.Replace ("Level", ""));
		GetComponent<Text> ().text = "" + num;
//		if (num >= 10)
//			transform.position += Vector3.left * 0.1f;
//		if (num == 1 || num == 11)
//			transform.position += Vector3.right * 0.05f;

	}
	
	// Update is called once per frame
	void Update () {
//		if (transform.parent.Find ("Lock").gameObject.activeSelf)
//			GetComponent<TextMesh> ().color = new Color (126, 67, 42);

	}
}
