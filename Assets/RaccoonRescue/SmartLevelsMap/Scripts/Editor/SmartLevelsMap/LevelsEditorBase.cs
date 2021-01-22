using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public abstract class LevelsEditorBase : UnityEditor.Editor
{
    protected List<MapLevel> GetMapLevels()
    {
        return FindObjectsOfType<MapLevel>().OrderBy(ml => ml.Number).ToList();
    }

    protected MapLevel CreateMapLevel(Vector3 position, int number, MapLevel mapLevelPrefab)
    {
        MapLevel mapLevel = PrefabUtility.InstantiatePrefab(mapLevelPrefab) as MapLevel;
        mapLevel.transform.position = position;
        return mapLevel;
    }

    protected void UpdateLevelsNumber(List<MapLevel> mapLevels)
    {
        for (int i = 0; i < mapLevels.Count; i++)
        {
            mapLevels[i].Number = i + 1;
            mapLevels[i].name = string.Format("Level{0:00}", i + 1);
        }
    }

    protected void UpdatePathWaypoints(List<MapLevel> mapLevels)
    {
        Assets.Plugins.SmartLevelsMap.Scripts.Path path = FindObjectOfType<Assets.Plugins.SmartLevelsMap.Scripts.Path>();
        path.Waypoints.Clear();
        foreach (MapLevel mapLevel in mapLevels)
            path.Waypoints.Add(mapLevel.PathPivot);
    }

    protected void SetAllMapLevelsAsDirty()
    {
        GetMapLevels().ForEach(EditorUtility.SetDirty);
    }

    protected void SetStarsEnabled(LevelsMap levelsMap, bool isEnabled)
    {
        levelsMap.SetStarsEnabled(isEnabled);
        if (isEnabled)
            levelsMap.SetStarsType(levelsMap.StarsType);
        EditorUtility.SetDirty(levelsMap);
        SetAllMapLevelsAsDirty();
    }
}
