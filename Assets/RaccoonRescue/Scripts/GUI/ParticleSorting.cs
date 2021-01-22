using UnityEngine;
using System.Collections;

public class ParticleSorting : MonoBehaviour {
	public int sortingOrder = 1;
	// Use this for initialization
	void Start () {
		GetComponent<ParticleSystem> ().GetComponent<Renderer> ().sortingLayerID = SortingLayer.NameToID ("New Layer 1");
		GetComponent<ParticleSystem> ().GetComponent<Renderer> ().sortingOrder = sortingOrder;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
