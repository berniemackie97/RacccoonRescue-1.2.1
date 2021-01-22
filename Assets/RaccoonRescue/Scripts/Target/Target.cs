using UnityEngine;
using System.Collections;

public abstract class Target: ITargetCheck {
	public int target_count = 0;
	public int total_target_count = 0;

	public virtual void AddTargetCount (int inc) {
		target_count += inc;
	}

	#region ITargetCheck implementation

	public virtual bool CheckTargetComplete () {
		return target_count >= total_target_count;
	}

	#endregion
}
