using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using InitScriptName;
using System.Linq;

#if FACEBOOK
using Facebook.Unity;
#endif


public class FacebookManager : MonoBehaviour
{
    private bool LoginEnable;
    public GameObject facebookButton;
    public string facebookPlayerID;
    private string lastResponse = string.Empty;
    public static string userID;
    public static string userName;
    public static List<FriendData> Friends = new List<FriendData>();
    public static Sprite profilePic;

    protected string LastResponse
    {
        get
        {
            return this.lastResponse;
        }

        set
        {
            this.lastResponse = value;
        }
    }

    private string status = "Ready";

    protected string Status
    {
        get
        {
            return this.status;
        }

        set
        {
            this.status = value;
        }
    }

    bool loginForSharing;
    public static FacebookManager THIS;
    bool FacebookEnable;
    bool loginOnce;

    void Awake()
    {
        if (THIS == null)
            THIS = this;
        else if (THIS != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
#if FACEBOOK
        if (facebookButton == null)
            facebookButton = GameObject.Find("FacebookButton");
        //		if (FB.IsLoggedIn)
        //			facebookButton.SetActive (false);
        FacebookEnable = true;
        if (FacebookEnable)
            CallFBInit();

#else
		FacebookEnable = false;

#endif

    }

    public void AddFriend(FriendData friend)
    {
        FriendData friendIndex = FacebookManager.Friends.Find(delegate (FriendData bk)
        {
            return bk.userID == friend.userID;
        });
        if (friendIndex == null)
            Friends.Add(friend);
    }

    public void SetPicture(string userID, Sprite sprite)
    {
        FriendData friendIndex = FacebookManager.Friends.Find(delegate (FriendData bk)
        {
            return bk.userID == userID;
        });
        if (friendIndex != null)
            friendIndex.picture = sprite;
    }

#if PLAYFAB || GAMESPARKS
    public FriendData GetCurrentUserAsFriend()
    {
        FriendData friend = new FriendData()
        {
            FacebookID = NetworkManager.facebookUserID,
            userID = NetworkManager.UserID,
            picture = profilePic
        };
        //		print ("playefab id: " + friend.PlayFabID);
        return friend;
    }
#endif

    #region FaceBook_stuff

#if FACEBOOK
    public void CallFBInit()
    {
        Debug.Log("init facebook");
        FB.Init(OnInitComplete, OnHideUnity);

    }

    private void OnInitComplete()
    {
        Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
        if (FB.IsLoggedIn)
        {
            LoggedSuccefull();
        }
#if UNITY_FACEBOOK
		CallFBLogin();
#endif
    }

    private void OnHideUnity(bool isGameShown)
    {
        Debug.Log("Is game showing? " + isGameShown);
    }

    void OnGUI()
    {
        if (LoginEnable)
        {
            CallFBLogin();
            LoginEnable = false;
        }

    }


    public void CallFBLogin()
    {
        if (!loginOnce)
        {
            loginOnce = true;
            Debug.Log("login");
            FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, this.HandleResult);
        }
    }

    public void CallFBLoginForPublish()
    {
        // It is generally good behavior to split asking for read and publish
        // permissions rather than ask for them all at once.
        //
        // In your own game, consider postponing this call until the moment
        // you actually need it.
        FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, this.HandleResult);
    }

    public void CallFBLogout()
    {
        FB.LogOut();
        facebookButton.SetActive(true);


#if PLAYFAB || GAMESPARKS
        NetworkManager.THIS.IsLoggedIn = false;
        Destroy(NetworkManager.THIS.gameObject);
        FacebookManager.profilePic = null;
#endif
        if (MenuManager.Instance != null)
            MenuManager.Instance.Loading.SetActive(true);
        SceneManager.LoadScene("map");
    }

    public void Share()
    {
        if (!FB.IsLoggedIn)
        {
            loginForSharing = true;
            LoginEnable = true;
            Debug.Log("not logged, logging");

#if PLAYFAB || GAMESPARKS
            NetworkManager.OnLoginEvent += Share;
#endif
        }
        else
        {//1.1
            var path = LevelEditorBase.THIS.androidSharingPath;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                path = LevelEditorBase.THIS.iosSharingPath;

            FB.FeedShare("",
                new Uri(path),
                "Raccoon Rescue",
                "I just scored " + mainscript.Score + " points! Try to beat me!",
                "Raccoon Rescue",
                new Uri("http://candy-smith.com/wp-content/uploads/2017/02/1.jpg"));
        }
    }

    protected void HandleResult(IResult result)
    {
        loginOnce = false;
        if (result == null)
        {
            this.LastResponse = "Null Response\n";
            Debug.Log(this.LastResponse);
            return;
        }

        //     this.LastResponseTexture = null;

        // Some platforms return the empty string instead of null.
        if (!string.IsNullOrEmpty(result.Error))
        {
            this.Status = "Error - Check log for details";
            this.LastResponse = "Error Response:\n" + result.Error;
            Debug.Log(result.Error);
        }
        else if (result.Cancelled)
        {
            this.Status = "Cancelled - Check log for details";
            this.LastResponse = "Cancelled Response:\n" + result.RawResult;
            Debug.Log(result.RawResult);
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            this.Status = "Success - Check log for details";
            this.LastResponse = "Success Response:\n" + result.RawResult;
            // Debug.Log(result.RawResult);
            LoggedSuccefull();
        }
        else
        {
            this.LastResponse = "Empty Response\n";
            Debug.Log(this.LastResponse);
        }
    }

    public void LoggedSuccefull()
    {
        PlayerPrefs.SetInt("Facebook_Logged", 1);
        PlayerPrefs.Save();

        if (facebookButton != null)
            facebookButton.SetActive(false);

        //Debug.Log(result.RawResult);

        facebookPlayerID = AccessToken.CurrentAccessToken.UserId;
        userID = AccessToken.CurrentAccessToken.UserId;
        GetPicture(AccessToken.CurrentAccessToken.UserId);
        GetUserName();

#if PLAYFAB || GAMESPARKS
        NetworkManager.facebookUserID = AccessToken.CurrentAccessToken.UserId;
        NetworkManager.THIS.LoginWithFB(AccessToken.CurrentAccessToken.TokenString);
#endif
    }

    void GetUserName()
    {
        FB.API("/me?fields=first_name", HttpMethod.GET, GettingNameCallback);
    }

    private void GettingNameCallback(IGraphResult result)
    {
        if (string.IsNullOrEmpty(result.Error))
        {
            IDictionary dict = result.ResultDictionary as IDictionary;
            string fbname = dict["first_name"].ToString();
            userName = fbname;

            //
            //#if PLAYFAB || GAMESPARKS
            //			NetworkManager.THIS.UpdateName (fbname);
            //			#endif

        }

    }
    IEnumerator loadPicture(string url)//1.1
    {
        WWW www = new WWW(url);
        yield return www;

        var texture = www.texture;

        Sprite sprite = new Sprite();
        sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0, 0), 1f);
        profilePic = sprite;

#if PLAYFAB || GAMESPARKS
        SetPicture(NetworkManager.UserID, profilePic);
        NetworkManager.PlayerPictureLoaded();

#endif
    }

    void GetPicture(string id)
    {
        print("get pic");
        FB.API("/" + id + "/picture?g&width=128&height=128&redirect=false", HttpMethod.GET, this.ProfilePhotoCallback);//1.1
    }

    private void ProfilePhotoCallback(IGraphResult result)
    {
        if (string.IsNullOrEmpty(result.Error))//1.1
        {
            // Debug.Log(result.ResultDictionary.ContainsKey("data"));
            var dic = result.ResultDictionary["data"] as Dictionary<string, object>;
            string url = dic.Where(i => i.Key == "url").First().Value as string;
            StartCoroutine(loadPicture(url));
        }

        //         if (string.IsNullOrEmpty(result.Error) && result.Texture != null)
        //         {
        //             print("got pic");
        //             Sprite sprite = new Sprite();
        //             sprite = Sprite.Create(result.Texture, new Rect(0, 0, 50, 50), new Vector2(0, 0), 1f);
        //             profilePic = sprite;





        // #if PLAYFAB || GAMESPARKS
        //             SetPicture(NetworkManager.UserID, profilePic);
        //             NetworkManager.PlayerPictureLoaded();

        // #endif
        //         }

    }



    public void SaveScores()
    {
        int score = 10000;

        var scoreData =
            new Dictionary<string, string>() { { "score", score.ToString() } };

        IDictionary<string, string> dic =
            new Dictionary<string, string>();
        //dic.Add("stat_type", "http://samples.ogp.me/768772476466403");
        //dic.Add("object1", "{\"fb:app_id\":\"1040909022611487\",\"og:type\":\"object\",\"og:title\":\"111\"}");
        //FB.API("/me/scores", HttpMethod.POST, APICallBack, scoreData);
        //FB.API("me/objects/object1", HttpMethod.POST, APICallBack, dic);
        //"object": "{\"fb:app_id\":\"302184056577324\",\"og:type\":\"object\",\"og:url\":\"Put your own URL to the object here\",\"og:title\":\"Sample Object\",\"og:image\":\"https:\\\/\\\/s-static.ak.fbcdn.net\\\/images\\\/devsite\\\/attachment_blank.png\"}"

    }

    public void GetFriendsPicture()
    {
        FB.API("me/friends?fields=picture.width(128).height(128)", HttpMethod.GET, RequestFriendsCallback);
    }


    private void RequestFriendsCallback(IGraphResult result)
    {
        if (!string.IsNullOrEmpty(result.RawResult))
        {
            var resultDictionary = result.ResultDictionary;
            if (resultDictionary.ContainsKey("data"))//1.1
            {
                var dataArray = (List<object>)resultDictionary["data"];
                // Debug.Log(dataArray.Count);
                var dic = dataArray.Select(x => x as Dictionary<string, object>).ToArray();
                // Debug.Log(dic.Length);

                foreach (var item in dic)
                {
                    string id = (string)item["id"];
                    // Debug.Log(id);
                    var url = item.Where(x => x.Key == "picture").SelectMany(x => x.Value as Dictionary<string, object>).Where(x => x.Key == "data").SelectMany(x => x.Value as Dictionary<string, object>).Where(i => i.Key == "url").First().Value;
                    // Debug.Log(url);
                    FriendData friend = Friends.Where(x => x.FacebookID == id).FirstOrDefault();
                    if (friend != null)
                        GetPictureByURL("" + url, friend);
                }
            }

            if (!string.IsNullOrEmpty(result.Error))
            {
                Debug.Log(result.Error);
            }
        }
    }

    public void GetPictureByURL(string url, FriendData friend)
    {
        StartCoroutine(GetPictureCor(url, friend));
    }

    IEnumerator GetPictureCor(string url, FriendData friend)
    {

        Sprite sprite = new Sprite();
        WWW www = new WWW(url);
        yield return www;
        sprite = Sprite.Create(www.texture, new Rect(0, 0, 128, 128), new Vector2(0, 0), 1f);
        friend.picture = sprite;
        //		print ("get picture for " + url);
    }

    public void APICallBack(IGraphResult result)
    {
        Debug.Log(result);
    }

#endif
    #endregion

}

public class FriendData
{
    public string userID;
    public string FacebookID;
    public Sprite picture;
    public int level;
    public GameObject avatar;
}
