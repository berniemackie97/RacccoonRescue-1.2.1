using UnityEngine;
using System.Collections;

public enum TargetType
{
    Top = 0,
    Round,
    RescuePets
}

public class TargetManager
{

    Target target;
    public TargetType targetType;


    public TargetManager(TargetType targetType_)
    {
        targetType = targetType_;
        if (targetType == TargetType.Top)
        {
            Debug.LogError("Please add some cubs to the level!");
        }
        else if (targetType == TargetType.Round)
        {
            Debug.LogError("Please add some cubs to the level!");
        }
        else if (targetType == TargetType.RescuePets)
            target = new CubTarget();

    }

    public void AddTargetCount(int inc)
    {
        target.AddTargetCount(inc);
    }

    public void SetTotalTargetCount(int inc)
    {
        target.total_target_count = inc;
    }


    public TargetType GetTarget()
    {
        return targetType;
    }

    public int GetTargetCount()
    {
        return target.target_count;
    }

    public int GetTotalTargetCount()
    {
        return target.total_target_count;
    }


    public bool CheckTargetComplete()
    {
        return target.CheckTargetComplete();
    }
}

