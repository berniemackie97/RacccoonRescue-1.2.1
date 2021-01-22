using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaitWhileDestroy : CustomYieldInstruction
{ //1.2 Fix delayed win
	private bool currentDestroyFinished;
	private WaitForDestroyComponent[] items;

	public override bool keepWaiting
	{
		get
		{
			return GameObject.FindObjectsOfType<WaitForDestroyComponent>().Count() > 0;//!items.AllNull();
		}
	}

	public WaitWhileDestroy()
	{
		items = GameObject.FindObjectsOfType<WaitForDestroyComponent>();
	}
}