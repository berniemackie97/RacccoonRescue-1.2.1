using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class FriendAvatar : MonoBehaviour, IAvatarLoader
{
    public Image image;
    private FriendData friendData;

    public FriendData FriendData
    {
        get
        {
            return friendData;
        }

        set
        {
            friendData = value;
            if (friendData != null)
                ShowPicture();
        }
    }

    void OnEnable()
    {
        Hide();
    }

    public void ShowPicture()
    {
        StartCoroutine(WaitForPicture());
    }

    IEnumerator WaitForPicture()
    {
        yield return new WaitUntil(() => FriendData.picture != null);
        //		print ("picture on map");//TODO: check player's picture
        GetComponent<SpriteRenderer>().enabled = true;
        image.sprite = FriendData.picture;
        image.enabled = true;
        SetPosition();
    }

    void SetPosition()
    {
        MapLevel level = LevelsMap._instance.GetLevel(FriendData.level);
        if (level != null)
            transform.position = level.transform.position - new Vector3(1, 1, 0);

    }

    void Hide()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        image.enabled = false;
    }

    void OnDisable()
    {
        Hide();
    }

}
