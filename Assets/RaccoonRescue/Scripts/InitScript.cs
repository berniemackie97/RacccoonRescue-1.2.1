using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
using Assets.Plugins.SmartLevelsMap.Scripts;
using UnityEngine.EventSystems;
using System.Xml;
using UnityEngine.SceneManagement;
using System.Linq;

public enum RewardedAdsType
{
    GetLifes,
    GetGems,
    GetGoOn
}


namespace InitScriptName
{


    public class InitScript : MonoBehaviour
    {
        public static InitScript Instance;
        private int _levelNumber = 1;
        private int _starsCount = 1;
        private bool _isShow;
        public static int openLevel;

        public static bool boostJelly;
        public static bool boostMix;
        public static bool boostChoco;

        public static bool sound = false;
        public static bool music = false;

        public static int waitedPurchaseGems;

        public static List<string> selectedFriends;
        public static bool Lauched;
        public static int scoresForLeadboardSharing;
        public static int lastPlace;
        public static int savelastPlace;
        public static bool beaten;
        public static List<string> Beatedfriends;
        int messCount;
        public static bool loggedIn;
        //	public GameObject LoginButton;
        //	public GameObject InviteButton;
        public GameObject EMAIL;
        public GameObject MessagesBox;


        public static bool FirstTime;
        public static int Lifes;
        public static int CapOfLife = 5;
        public static int Gems;

        public static float RestLifeTimer;
        public static string DateOfExit;
        public static DateTime today;
        public static DateTime DateOfRestLife;
        public static string timeForReps;
        public float TotalTimeForRestLifeHours = 0;
        public float TotalTimeForRestLifeMin = 15;
        public float TotalTimeForRestLifeSec = 60;


        public static bool openNext;
        public static bool openAgain;

        public bool BoostActivated;

        Hashtable mapFriends = new Hashtable();



        private string admobUIDAndroid;
        private string admobUIDIOS;
        public string rewardedVideoZone;
        public string nonRewardedVideoZone;


        public void Awake()
        {
            Instance = this;
            if (LevelEditorBase.THIS == null)
            {
                GameObject gm = Resources.Load("LevelEditorBase") as GameObject;
                GameObject lb = Instantiate(gm) as GameObject;
                lb.name = "LevelEditorBase";
            }

#if UNITY_INAPPS

			gameObject.AddComponent<UnityInAppsIntegration>();
#endif
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("map"))
            {
                //				if (GameObject.Find ("Canvas").transform.Find ("MenuPlay").gameObject.activeSelf)
                //					GameObject.Find ("Canvas").transform.Find ("MenuPlay").gameObject.SetActive (false);
                //
            }
            RestLifeTimer = PlayerPrefs.GetFloat("RestLifeTimer");

            //			if(InitScript.DateOfExit == "")
            //			print(InitScript.DateOfExit );
            DateOfExit = PlayerPrefs.GetString("DateOfExit", "");

            Gems = PlayerPrefs.GetInt("Gems");
            Lifes = PlayerPrefs.GetInt("Lifes");
            CapOfLife = LevelEditorBase.THIS.CapOfLife;
            TotalTimeForRestLifeHours = LevelEditorBase.THIS.TotalTimeForRestLifeHours;
            TotalTimeForRestLifeMin = LevelEditorBase.THIS.TotalTimeForRestLifeMin;
            TotalTimeForRestLifeSec = LevelEditorBase.THIS.TotalTimeForRestLifeSec;

            if (PlayerPrefs.GetInt("Lauched") == 0)
            {    //First lauching
                FirstTime = true;
                Lifes = CapOfLife;
                Gems = LevelEditorBase.THIS.FirstGems;
                PlayerPrefs.SetInt("Gems", Gems);
                PlayerPrefs.SetInt("Lifes", Lifes);
                PlayerPrefs.SetInt("Lauched", 1);
                PlayerPrefs.SetFloat("Music", 1);
                PlayerPrefs.SetInt("Sound", 1);
                PlayerPrefs.Save();
            }

            GameObject.Find("Music").GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Music");
            SoundBase.Instance.GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("Sound");

            //			ReloadBoosts ();

            boostPurchased = false;

        }

        void Start()
        {
            Application.targetFrameRate = 60;
        }

        List<MapLevel> MapLevels = new List<MapLevel>();

        public List<MapLevel> GetMapLevels()
        {
            if (MapLevels.Count == 0)//1.4.4
                MapLevels = FindObjectsOfType<MapLevel>().OrderBy(ml => ml.Number).ToList();

            return MapLevels;
        }

        public int GetLatestReachedLevel()
        {
            if (SceneManager.GetActiveScene().name == "map")
                return GetMapLevels().Where(l => !l.IsLocked).Select(l => l.Number).Max();
            else
                return mainscript.Instance.currentLevel;
        }



        public static bool boostPurchased;

        void Update()
        {

        }



        public void SetGems(int count)
        {
            Gems = count;
            PlayerPrefs.SetInt("Gems", Gems);
            PlayerPrefs.Save();
        }


        public void AddGems(int count)
        {
            Gems += count;
            PlayerPrefs.SetInt("Gems", Gems);
            PlayerPrefs.Save();
#if PLAYFAB || GAMESPARKS //1.2
            NetworkManager.currencyManager.IncBalance(count);
#endif
        }

        public void SpendGems(int count)
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.coins);
            Gems -= count;
            PlayerPrefs.SetInt("Gems", Gems);
            PlayerPrefs.Save();
#if PLAYFAB || GAMESPARKS//1.2
            NetworkManager.currencyManager.DecBalance(count);
#endif
        }

        public void RestoreLifes()
        {
            Lifes = CapOfLife;
            PlayerPrefs.SetInt("Lifes", Lifes);
            PlayerPrefs.Save();
        }


        public void AddLife(int count)
        {
            Lifes += count;
            if (Lifes > CapOfLife)
                Lifes = CapOfLife;
            PlayerPrefs.SetInt("Lifes", Lifes);
            PlayerPrefs.Save();
        }

        public int GetLife()
        {
            if (Lifes > CapOfLife)
            {
                Lifes = CapOfLife;
                PlayerPrefs.SetInt("Lifes", Lifes);
                PlayerPrefs.Save();
            }
            return Lifes;
        }

        public void PurchaseSucceded()
        {
            AddGems(waitedPurchaseGems);
            waitedPurchaseGems = 0;
        }

        public void SpendLife(int count)
        {
            if (Lifes > 0)
            {
                Lifes -= count;
                PlayerPrefs.SetInt("Lifes", Lifes);
                PlayerPrefs.Save();
            }
            else
            {

            }
        }


        #region selectlevel

        public int LoadLevelStarsCount(int level)
        {
            return level > 10 ? 0 : (level % 3 + 1);
        }

        public void SaveLevelStarsCount(int level, int starsCount)
        {
            Debug.Log(string.Format("Stars count {0} of level {1} saved.", starsCount, level));
        }

        public void ClearLevelProgress(int level)
        {

        }

        void OnApplicationFocus(bool focusStatus)
        {//2.1
            GameObject music = GameObject.Find("Music");
            if (music)
            {
                music.GetComponent<AudioSource>().Play();
            }
        }


        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                if (RestLifeTimer > 0)
                {
                    PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
                }
                PlayerPrefs.SetInt("Lifes", Lifes);
                PlayerPrefs.SetString("DateOfExit", DateTime.Now.ToString());
                PlayerPrefs.SetInt("Gems", Gems);
                PlayerPrefs.Save();
            }
        }

        public void OnLevelClicked(object sender, LevelReachedEventArgs args)
        {
            if (EventSystem.current.IsPointerOverGameObject(-1))
                return;
            if (!MenuManager.Instance.MenuPlay.activeSelf && !MenuManager.Instance.MenuCurrencyShop.activeSelf && !MenuManager.Instance.MenuLifeShop.activeSelf)
            {
                PlayerPrefs.SetInt("OpenLevel", args.Number);
                PlayerPrefs.Save();
                openLevel = args.Number;
                LevelData.GetTargetOnLevel(args.Number);
                GameEvent.Instance.GameStatus = GameState.PlayMenu;
            }
        }

        TargetType GetTargetFromFile(int n)
        {
            TextAsset textReader = Resources.Load("Levels/" + n) as TextAsset;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(textReader.text);
            XmlNodeList elemList = doc.GetElementsByTagName("property");
            foreach (XmlElement element in elemList)
            {
                if (element.GetAttribute("name") == "GM")
                {
                    return (TargetType)int.Parse(element.GetAttribute("value"));
                }
            }



            return (TargetType)0;
        }

        void OnEnable()
        {
            LevelsMap.LevelSelected += OnLevelClicked;

        }

        void OnDisable()
        {
            LevelsMap.LevelSelected -= OnLevelClicked;

            //		if(RestLifeTimer>0){
            PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
            //		}
            PlayerPrefs.SetInt("Lifes", Lifes);
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("game"))
                PlayerPrefs.SetString("DateOfExit", DateTime.Now.ToString());
            PlayerPrefs.SetInt("Gems", Gems);
            PlayerPrefs.Save();

            //		FacebookSNSAgent.OnUserInfoArrived -= OnUserInfoArrived;
            //		FacebookSNSAgent.OnUserFriendsArrived -= OnUserFriendsArrived;
        }


        #endregion


    }

}