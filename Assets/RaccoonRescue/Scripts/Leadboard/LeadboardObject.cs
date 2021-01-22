using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LeadboardObject : MonoBehaviour
{
    public Image icon;
    public Text place;
    public Text playerName;
    public Text score;
#if PLAYFAB || GAMESPARKS
    private LeadboardPlayerData playerData;

    public LeadboardPlayerData PlayerData
    {
        get
        {
            return playerData;
        }

        set
        {
            playerData = value;
            SetupIcon();
        }
    }

    void SetupIcon()
    {
        StartCoroutine(WaitForPicture());
    }

    IEnumerator WaitForPicture()
    {
        yield return new WaitUntil(() => PlayerData != null);
        yield return new WaitUntil(() => PlayerData.friendData != null);

        if (PlayerData.friendData.picture == null)
        {//1.4.4
            FacebookManager.THIS.LoggedSuccefull();
            FacebookManager.THIS.GetFriendsPicture();
        }
        yield return new WaitUntil(() => PlayerData.friendData.picture != null);

        PlayerData.picture = PlayerData.friendData.picture;
        icon.sprite = PlayerData.picture;
        place.text = "" + PlayerData.position;
        playerName.text = PlayerData.Name;
        score.text = "" + PlayerData.score;
        if (NetworkManager.THIS.IsYou(PlayerData.userID))
        {
            playerName.text = "YOU";
            playerName.color = Color.yellow;
            //if (mainscript.Instance.gameStatus == GameState.Win) {
            //    score.text = "" + PlayerPrefs.GetInt("Score" + mainscript.Instance.currentLevel);
            //   }
        }

    }

#endif

}
