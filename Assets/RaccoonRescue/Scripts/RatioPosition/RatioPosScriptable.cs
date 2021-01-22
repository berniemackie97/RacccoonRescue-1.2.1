using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RatioPosScriptable : ScriptableObject
{
	public List<RatioPos> ratioList = new List<RatioPos>();

	public void Save(RatioPos ratioPos)
	{
		var obj = ratioList.Where(i => i.gameObject.Equals(ratioPos.gameObject) || i.name == ratioPos.name).FirstOrDefault();
		if (obj == null)
		{
			ratioList.Add(ratioPos);
			obj = ratioList.Last();
		}
		obj.ratioList = ratioPos.ratioList.Clone();

	}

	public RatioPos Load(GameObject gameObject)
	{
		var obj = ratioList.Where(i => i.gameObject.Equals(gameObject)).FirstOrDefault();
		return obj;
	}

}
