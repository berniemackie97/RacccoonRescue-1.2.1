using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LIMIT
{
    MOVES
}

public class LevelEditorBase : MonoBehaviour
{
    public static LevelEditorBase THIS;
    public Sprite[] sprites;
    public Sprite[] backgrounds;
    public bool enableInApps;
    public bool enableUnityAds;
    public bool enableGoogleMobileAds;
    public string[] InAppIDs;
    public int rewardedGems;
    public List<AdItem> adsEvents = new List<AdItem>();
    public string admobUIDAndroid;
    public string admobUIDIOS;
    public string admobRewardedUIDAndroid;
    public string admobRewardedUIDIOS;
    public bool showPopupScores;
    public string androidSharingPath;
    public string iosSharingPath;

    public int CapOfLife = 5;
    public float TotalTimeForRestLifeHours = 0;
    public float TotalTimeForRestLifeMin = 15;
    public float TotalTimeForRestLifeSec = 60;
    public int CostIfRefill = 12;
    public int FirstGems = 20;
    //cost of continue playing after fail
    public int FailedCost;
    //extra moves that you get to continue game after fail
    public int ExtraFailedMoves = 5;
    //extra seconds that you get to continue game after fail
    public int ExtraFailedSecs = 30;

    public bool enableChartboostAds;
    public List<ItemKind> items = new List<ItemKind>();
    public Texture2D[] powerupTextures = new Texture2D[4];
    public Texture2D[] powerupTexturesOn = new Texture2D[4];

    void Awake()
    {
        DontDestroyOnLoad(this);//1.2
        THIS = this;
    }

    public string[] GetItemsName()
    {
        string[] array = new string[items.Count];
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].sprite != null)
                array[i] = items[i].sprite.name;
        }
        return array;
    }
}

[System.Serializable]
public class ItemKind
{
    public Sprite sprite;
    public ItemColor color;
    public ItemTypes itemType;
    public Powerups powerUp;
    public GameObject prefab;
    public ApplyingPrefabTypes applyingPrefab;
    public GameObject onDestroyEffect;
    public int appearBallAfterDestroyNum;
    public int score = 10;
    //next phase for breakable item

    public ItemKind(Sprite spr, ItemColor col, ItemTypes type)
    {
        sprite = spr;
        color = col;
        itemType = type;
    }

    public ItemKind GetNextBallAfterDestroy()
    {
        return LevelEditorBase.THIS.items[appearBallAfterDestroyNum];
    }

    public ItemKind()
    {
    }
}

public enum ItemTypes
{
    Simple = 0,
    Rotation,
    Cub,
    Extra,
    Breakable
}

public enum ApplyingPrefabTypes
{
    Replace = 0,
    Apply,
    Behind
}