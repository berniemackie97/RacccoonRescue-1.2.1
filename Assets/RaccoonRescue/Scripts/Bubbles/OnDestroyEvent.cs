using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnDestroyEvent : MonoBehaviour
{
	public GameObject[] instantiatePrefabs;

	void OnDisable()
	{
		if (!isQuitting)
		{
			foreach (GameObject item in instantiatePrefabs)
			{
				GameObject g = Instantiate(item, transform.position, transform.rotation);
				g.AddComponent<WaitForDestroyComponent>(); //1.2 Fix delayed win

			}
		}
	}

	bool isQuitting;

	void OnApplicationQuit()
	{
		isQuitting = true;
	}
}

