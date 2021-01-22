
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#if GAMESPARKS
using GameSparks.Platforms;
using GameSparks.Platforms.WebGL;

#endif


#if PLAYFAB
using PlayFab.ClientModels;
using PlayFab;
#endif

//using PlayFab.AdminModels;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{


	public delegate void NetworkEvents();



	public static event NetworkEvents OnLoginEvent;
	public static event NetworkEvents OnLogoutEvent;
	public static event NetworkEvents OnFriendsOnMapLoaded;
	public static event NetworkEvents OnPlayerPictureLoaded;
	public static event NetworkEvents OnLevelLeadboardLoaded;

#if PLAYFAB || GAMESPARKS
    public static NetworkManager THIS;
    public static NetworkCurrencyManager currencyManager;
    public static NetworkDataManager dataManager;
    public static NetworkFriendsManager friendsManager;
    public static ILoginManager loginManger;
    [HideInInspector]
    private static string userID;

    public static string UserID
    {
        get
        {
            return userID;
        }
        set
        {
	if (value != PlayerPrefs.GetString("UserID") && PlayerPrefs.GetString("UserID") != ""&& userID != "" && userID != null)//1.2
            {
                PlayerPrefs.DeleteAll();
                if (LevelsMap._instance != null)
                    LevelsMap._instance.Reset();
            }

            userID = value;
            PlayerPrefs.SetString("UserID", userID);
            PlayerPrefs.Save();
        }
    }

    public string titleId;
    //public string DeveloperSecretKey;
    private bool isLoggedIn;

    public bool IsLoggedIn
    {
        get
        {
            return isLoggedIn;
        }

        set
        {
            isLoggedIn = value;
            if (value && OnLoginEvent != null)
                OnLoginEvent();
            else if (!value && OnLogoutEvent != null)
                OnLogoutEvent();
        }
    }

    public static List<LeadboardPlayerData> leadboardList = new List<LeadboardPlayerData>();
    public static string facebookUserID;

    private void Awake()
    {
        if (THIS != null && THIS != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            THIS = this;
        }
        DontDestroyOnLoad(this);
    }
    // Use this for initialization
    void Start()
    {
        //#if ((UNITY_PS4 || UNITY_XBOXONE) && !UNITY_EDITOR) || GS_FORCE_NATIVE_PLATFORM





#if GS_FORCE_NATIVE_PLATFORM
	this.gameObject.AddComponent<NativePlatform>();





#elif UNITY_WEBGL && !UNITY_EDITOR
	this.gameObject.AddComponent<WebGLPlatform>();





// #elif !PLAYFAB
// 		this.gameObject.AddComponent<DefaultPlatform>();//1.1
#endif



#if PLAYFAB
		PlayFabSettings.TitleId = titleId;
		loginManger = new PlayFabManager ();





#elif GAMESPARKS
        loginManger = new GamesparksLogin();
#endif
        currencyManager = new NetworkCurrencyManager();
        friendsManager = new NetworkFriendsManager();
        dataManager = new NetworkDataManager();


    }










	#region AUTHORIZATION





    public void LoginWithFB(string accessToken)
    {
        loginManger.LoginWithFB(accessToken, titleId);
    }



    public void UpdateName(string userName)
    {
        loginManger.UpdateName(userName);
    }

    public bool IsYou(string playFabId)
    {
        return loginManger.IsYou(playFabId);
    }











	#endregion














	#region EVENTS





    public static void LevelLeadboardLoaded()
    {
        //		OnLevelLeadboardLoaded();
    }

    public static void PlayerPictureLoaded()
    {
        if (OnPlayerPictureLoaded != null)
            OnPlayerPictureLoaded();
    }

    public static void FriendsOnMapLoaded()
    {
        if (SceneManager.GetActiveScene().name == "map")
            OnFriendsOnMapLoaded();
    }










	#endregion




#endif
}

public class LeadboardPlayerData
{
	public string Name;
	public string userID;
	public int position;
	public int score;
	public Sprite picture;
	public FriendData friendData;
	// 1.3.3
}

