//#if GAMESPARKS
//
//using UnityEngine;
//using System.Collections;
//using GameSparks.Api.Requests;
//using UnityEngine.UI;
//
//public class UserManager : MonoBehaviour
//{
//
//    public static UserManager instance;
//
//    //These are the account details we want to pull in
//    public string userName;
//    public string userId;
//    private string facebookId;
//
//    public Text userNameLabel;
//    public Sprite profilePicture;
//
//    // Use this for initialization
//    void Start()
//    {
//        instance = this;
//    }
//
//    public void UpdateInformation()
//    {
//        //We send an AccountDetailsRequest
//        new AccountDetailsRequest().Send((response) =>
//        {
//            //We pass the details we want from our response to the function which will update our information
//            UpdateGUI(response.DisplayName, response.UserId, response.ExternalIds.GetString("FB").ToString());
//        });
//    }
//
//    void RequestGems()
//    {
//        GameSparks.getPlayer().getBalance1()
//    }
//
//    public void UpdateGUI(string name, string uid, string fbId)
//    {
//        userName = name;
//        //userNameLabel.text = userName;
//        userId = uid;
//        facebookId = fbId;
//        //StartCoroutine(getFBPicture());
//    }
//
//    public IEnumerator getFBPicture()
//    {
//        //To get our facebook picture we use this address which we pass our facebookId into
//        var www = new WWW("http://graph.facebook.com/" + facebookId + "/picture?width=210&height=210");
//
//        yield return www;
//
//        Sprite sprite = new Sprite();
//        sprite = Sprite.Create(www.texture, new Rect(0, 0, 200, 200), new Vector2(0, 0), 1f);
//        profilePicture = sprite;
//        //GameObject.Find("Character").GetComponent<SpriteRenderer>().sprite = profilePicture;
//
//    }
//}
//
//#endif