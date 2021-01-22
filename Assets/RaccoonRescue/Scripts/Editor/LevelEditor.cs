using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class LevelEditor : EditorWindow
{
    private static LevelEditor window;
    private int maxRows;
    private int maxCols = 11;
    public static int[] levelSquares = new int[81];
    private Texture[] ballTex;
    int levelNumber = 1;
    private Vector2 scrollViewVector;
    private TargetType target;
    private LIMIT limitType;
    private int limit;
    private int colorLimit;
    private int star1;
    private int star2;
    private int star3;
    private string fileName = "1.txt";
    private int brush;
    private static int selectedTab;
    string[] toolbarStrings = new string[] { "Editor", "Items", "Settings", "In-apps", "Ads", "Help" };
    LevelEditorBase lm;
    private bool enableGoogleAdsProcessing;
    List<AdItem> oldList;
    public ItemKind selectedItem;
    const string itemEditorPath = "Assets/RaccoonRescue/Resources/ItemsEditor.asset";
    public ItemsEditorScriptable itemsEditor;
    int[] powerups = new int[4];

    [MenuItem("Window/Game editor")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one:
        window = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
        window.Show();

    }

    public static void ShowHelp()
    {
        selectedTab = 3;
    }

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LevelEditor));

    }

    void OnFocus()
    {
        if (maxRows <= 0)
            maxRows = 50;
        if (maxCols <= 0)
            maxCols = 11;

        Initialize();
        LoadDataFromLocal(levelNumber);
        ballTex = new Texture[lm.sprites.Length];
        for (int i = 0; i < lm.sprites.Length; i++)
        {
            ballTex[i] = lm.sprites[i].texture;
        }
        if (oldList == null)
        {
            oldList = new List<AdItem>();
            oldList.Clear();
            for (int i = 0; i < lm.adsEvents.Count; i++)
            {
                oldList.Add(new AdItem());
                oldList[i].adType = lm.adsEvents[i].adType;
                oldList[i].callsTreshold = lm.adsEvents[i].callsTreshold;
                oldList[i].gameEvent = lm.adsEvents[i].gameEvent;
            }
        }
    }

    void Initialize()
    {
        maxRows = 70;
        maxCols = 11;
        //GameObject gm = Resources.Load("LevelEditorBase") as GameObject;
        GameObject gm = AssetDatabase.LoadAssetAtPath("Assets/RaccoonRescue/Resources/LevelEditorBase.prefab",
                            typeof(GameObject)) as GameObject;
        lm = gm.GetComponent<LevelEditorBase>();
        if (lm.items.Count > 0)
        {
            if (lm.items[0].sprite != null)
                lm.items.Insert(0, new ItemKind());
        }
        levelSquares = new int[maxCols * maxRows];
        for (int i = 0; i < levelSquares.Length; i++)
        {
            levelSquares[i] = 0;
        }


        //		itemsEditor = AssetDatabase.LoadAssetAtPath (itemEditorPath, typeof(ItemsEditor)) as ItemsEditor;
        //		if (itemsEditor.items == null)
        //			itemsEditor.CreateAsset ();
        //		Debug.Log (itemsEditor.items.Count);
    }


    void OnGUI()
    {
        GUI.changed = false;


        if (levelNumber < 1)
            levelNumber = 1;

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        int oldSelected = selectedTab;
        selectedTab = GUILayout.Toolbar(selectedTab, toolbarStrings, new GUILayoutOption[] { GUILayout.Width(350) });
        GUILayout.EndHorizontal();


        scrollViewVector = GUI.BeginScrollView(new Rect(25, 45, position.width - 30, position.height), scrollViewVector, new Rect(0, 0, 400, 4200));
        GUILayout.Space(-30);


        if (selectedTab == 0)
        {

            GUILevelSelector();
            GUILayout.Space(10);

            GUILimit();
            GUILayout.Space(10);


            //GUIColorLimit();
            //GUILayout.Space(10);

            GUIStars();
            GUILayout.Space(10);

            //GUITarget();
            //GUILayout.Space(10);

            GUIBlocks();
            GUILayout.Space(20);


            GUIGameField();
        }
        else if (selectedTab == 1)
        {
            //			ItemsEditorScriptable.Instance.OnGUI ();
            GUIItemEditor();

        }
        else if (selectedTab == 2)
        {
            GUISettings();


        }
        else if (selectedTab == 3)
        {
            GUIInappSettings();

        }
        else if (selectedTab == 4)
        {
            GUIAds();

        }
        else if (selectedTab == 5)
        {
            GUIHelp();
        }

        if (GUI.changed)
        {
            if (!EditorApplication.isPlaying)
                EditorSceneManager.MarkAllScenesDirty();
            //			EditorUtility.SetDirty (ItemsEditor.Instance);
        }


        if (enableGoogleAdsProcessing)
            RunOnceGoogle();

        GUI.EndScrollView();
    }

    void RunOnceGoogle()
    {
        if (Directory.Exists("Assets/PlayServicesResolver"))
        {
            Debug.Log("assets try reimport");
#if GOOGLE_MOBILE_ADS && UNITY_ANDROID
            //            GooglePlayServices.PlayServicesResolver.MenuResolve();
            Debug.Log("assets reimorted");
            enableGoogleAdsProcessing = false;
#endif
        }


    }

    //	void SetScriptingDefineSymbols () {
    //		string defines = "";
    //		if (lm.enableUnityAds)
    //			defines = defines + "; UNITY_ADS";
    //		if (lm.enableGoogleMobileAds)
    //			defines = defines + "; GOOGLE_MOBILE_ADS";
    //		if (lm.enableInApps)
    //			defines = defines + "; UNITY_INAPPS";
    //		if (lm.enableChartboostAds)
    //			defines = defines + "; CHARTBOOST_ADS";
    //
    //		PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android, defines);
    //		PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.iOS, defines);
    //		PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.WSA, defines);
    //
    //	}

    #region ItemEditor

    void GUIItemEditor()
    {
        if (lm.items == null)
            lm.items = new List<ItemKind>();

        if (selectedItem != null)
        {
            EditorGUILayout.BeginHorizontal();
            var save_value = selectedItem.sprite;
            selectedItem.sprite = (Sprite)EditorGUILayout.ObjectField(selectedItem.sprite, typeof(Sprite), new GUILayoutOption[] {
                GUILayout.Width (75), GUILayout.Height (75)
            });
            if (save_value != selectedItem.sprite)
                SaveItem();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {
                GUILayout.Width (200)
            });
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("type", new GUILayoutOption[] {
                GUILayout.Width (50)
            });

            var save_value2 = selectedItem.itemType;
            selectedItem.itemType = (ItemTypes)EditorGUILayout.EnumPopup(selectedItem.itemType, new GUILayoutOption[] {
                GUILayout.Width (115)

            });
            if (save_value2 != selectedItem.itemType)
                SaveItem();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("prefab", new GUILayoutOption[] {
                GUILayout.Width (50)
            });

            var save_value3 = selectedItem.prefab;
            selectedItem.prefab = (GameObject)EditorGUILayout.ObjectField(selectedItem.prefab, typeof(GameObject), new GUILayoutOption[] {
                GUILayout.Width (115),

            });
            if (save_value3 != selectedItem.prefab)
                SaveItem();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {
                GUILayout.Width (10)
            });


            if (selectedItem.itemType == ItemTypes.Simple || selectedItem.itemType == ItemTypes.Cub)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("color", new GUILayoutOption[] {
                    GUILayout.Width (50)
                });

                var save_value1 = selectedItem.color;
                selectedItem.color = (ItemColor)EditorGUILayout.EnumPopup(selectedItem.color, new GUILayoutOption[] {
                    GUILayout.Width (115),

                });
                if (save_value1 != selectedItem.color)
                    SaveItem();
                EditorGUILayout.EndVertical();
            }
            else if (selectedItem.itemType == ItemTypes.Extra)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("power type", new GUILayoutOption[] {
                    GUILayout.Width (70)
                });

                var save_value1 = selectedItem.powerUp;
                selectedItem.powerUp = (Powerups)EditorGUILayout.EnumPopup(selectedItem.powerUp, new GUILayoutOption[] {
                    GUILayout.Width (115),
                });
                if (save_value1 != selectedItem.powerUp)
                    SaveItem();
                EditorGUILayout.EndVertical();
            }
            else if (selectedItem.itemType == ItemTypes.Breakable)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("appear Ball \nAfter Destroy", new GUILayoutOption[] {
                    GUILayout.Width (100)
                });

                var save_value1 = selectedItem.appearBallAfterDestroyNum;
                selectedItem.appearBallAfterDestroyNum = EditorGUILayout.Popup(selectedItem.appearBallAfterDestroyNum, lm.GetItemsName(), new GUILayoutOption[] {
                    GUILayout.Width (115),
                });
                if (save_value1 != selectedItem.appearBallAfterDestroyNum)
                {
                    SaveItem();
                }
                EditorGUILayout.EndVertical();
            }
            if (selectedItem.prefab != null)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("applying prefab", new GUILayoutOption[] {
                    GUILayout.Width (50)
                });

                var save_value4 = selectedItem.applyingPrefab;
                selectedItem.applyingPrefab = (ApplyingPrefabTypes)EditorGUILayout.EnumPopup(selectedItem.applyingPrefab, new GUILayoutOption[] {
                    GUILayout.Width (115),
                });
                if (save_value4 != selectedItem.applyingPrefab)
                    SaveItem();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();


            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("On Destroy Effect", new GUILayoutOption[] {
                GUILayout.Width (100)
            });

            var save_value5 = selectedItem.onDestroyEffect;
            selectedItem.onDestroyEffect = (GameObject)EditorGUILayout.ObjectField(selectedItem.onDestroyEffect, typeof(GameObject), new GUILayoutOption[] {
                GUILayout.Width (115),

            });
            if (save_value5 != selectedItem.onDestroyEffect)
                SaveItem();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("score", new GUILayoutOption[] {
                    GUILayout.Width (50)
                });

            var save_score = selectedItem.score;
            selectedItem.score = EditorGUILayout.IntField(selectedItem.score, new GUILayoutOption[] { GUILayout.Width(115), });
            if (save_score != selectedItem.score)
                SaveItem();
            EditorGUILayout.EndVertical();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add before", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(25) }))
            {
                lm.items.Insert(lm.items.IndexOf(selectedItem), new ItemKind());
                SaveItem();
            }
            if (GUILayout.Button("Add after", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(25) }))
            {
                lm.items.Insert(lm.items.IndexOf(selectedItem) + 1, new ItemKind());
                SaveItem();
            }
            if (GUILayout.Button("Delete", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(25) }))
            {
                lm.items.Remove(selectedItem);
                SaveItem();

            }

            GUILayout.EndHorizontal();
        }
        GUILayout.Space(20);
        ShowItemsPanel();
    }

    void ShowItemsPanel()
    {
        GUILayout.BeginVertical();
        for (int j = 0; j < 20; j++)
        {
            GUILayout.BeginHorizontal();
            for (int i = 1; i <= 6; i++)
            {
                int index = j * 6 + i;
                if (index == 0)
                    continue;
                if (index >= lm.items.Count)
                    continue;
                ItemKind item = lm.items[index];
                //if (item.powerUp == Powerups.TRIPLE && selectedTab == 0) continue;
                if (item.sprite != null)
                {
                    Texture2D tex = item.sprite.texture;
                    if (GUILayout.Button(tex, new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(50) }))
                    {
                        selectedItem = item;
                        if (item.powerUp != Powerups.TRIPLE)
                        {
                            brush = index;
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button("", new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(50) }))
                    {
                        selectedItem = item;
                    }
                }
                if (index + 1 >= lm.items.Count)
                {
                    GUILayout.BeginVertical();
                    if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(25) }))
                    {
                        selectedTab = 1;
                        selectedItem = new ItemKind();
                        lm.items.Add(selectedItem);
                        SaveItem();
                    }
                    if (GUILayout.Button("-", new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(25) }))
                    {
                        selectedItem = new ItemKind();
                        lm.items.Remove(lm.items[lm.items.Count - 1]);
                        SaveItem();
                    }
                    GUILayout.EndVertical();
                }

            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    void SaveItem()
    {
        EditorUtility.SetDirty(lm.gameObject);
        AssetDatabase.SaveAssets();

    }

    #endregion

    #region settings
    private bool score_settings;
    private bool life_settings_show;
    private bool failed_settings_show;

    void GUISettings()
    {
        GUILayout.Label("Game settings:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });
        GUILayout.BeginHorizontal();
        //if (GUILayout.Button("Reset to default", new GUILayoutOption[] { GUILayout.Width(150) })) {
        //	//			ResetSettings ();
        //}
        if (GUILayout.Button("Clear player prefs", new GUILayoutOption[] { GUILayout.Width(150) }))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("Player prefs cleared");
        }
        if (GUILayout.Button("Open all levels", new GUILayoutOption[] { GUILayout.Width(150) }))
        {
            for (int i = 1; i < 1000; i++)
            {
                SaveLevelStarsCount(i, 3);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        //		bool oldFacebookEnable = lm.FacebookEnable;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Facebook", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });
        string facebookButton = "Install";
#if FACEBOOK
        facebookButton = "Installed";
#endif
        if (GUILayout.Button(facebookButton, new GUILayoutOption[] { GUILayout.Width(70) }))
        {
            Application.OpenURL("https://developers.facebook.com/docs/unity/downloads");
        }
        if (GUILayout.Button("Account", new GUILayoutOption[] { GUILayout.Width(70) }))
        {
            Application.OpenURL("https://developers.facebook.com");
        }
        if (GUILayout.Button("Help", new GUILayoutOption[] { GUILayout.Width(60) }))
        {
            Application.OpenURL("https://docs.google.com/document/d/1bTNdM3VSg8qu9nWwO7o7WeywMPhVLVl8E_O0gMIVIw0/edit?usp=sharing");
        }
        GUILayout.EndHorizontal();

#if FACEBOOK
        share_settings = EditorGUILayout.Foldout(share_settings, "Share settings:");
        if (share_settings)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.BeginVertical();
            {
                lm.androidSharingPath = EditorGUILayout.TextField("Android path", lm.androidSharingPath, new GUILayoutOption[] { GUILayout.MaxWidth(500) });
                lm.iosSharingPath = EditorGUILayout.TextField("iOS path", lm.iosSharingPath, new GUILayoutOption[] { GUILayout.MaxWidth(500) });
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }
#endif

        GUILayout.BeginHorizontal();
        GUILayout.Label("Leadboard Gamesparks", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });
        string gamesparksButton = "Install";
#if GAMESPARKS
        gamesparksButton = "Installed";
#endif

        if (GUILayout.Button(gamesparksButton, new GUILayoutOption[] { GUILayout.Width(70) }))
        {
            Application.OpenURL("https://docs.gamesparks.com/sdk-center/unity.html");
        }
        if (GUILayout.Button("Account", new GUILayoutOption[] { GUILayout.Width(70) }))
        {
            Application.OpenURL("https://portal.gamesparks.net");
        }
        if (GUILayout.Button("Help", new GUILayoutOption[] { GUILayout.Width(60) }))
        {
            Application.OpenURL("https://docs.google.com/document/d/1JcQfiiD2ALz6v_i9UIcG93INWZKC7z6FHXH_u6w9A8E");
        }
        GUILayout.EndHorizontal();


        GUILayout.Space(10);

        score_settings = EditorGUILayout.Foldout(score_settings, "Score settings:");
        if (score_settings)
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            lm.showPopupScores = EditorGUILayout.Toggle("Show popup scores", lm.showPopupScores, new GUILayoutOption[] {
                GUILayout.Width (50),
                GUILayout.MaxWidth (200)
            });
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }
        GUILayout.Space(20);

        life_settings_show = EditorGUILayout.Foldout(life_settings_show, "Lifes settings:");
        if (life_settings_show)
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.BeginVertical();


            lm.CapOfLife = EditorGUILayout.IntField("Max of lifes", lm.CapOfLife, new GUILayoutOption[] {
                GUILayout.Width (200),
                GUILayout.MaxWidth (200)
            });
            GUILayout.Space(10);

            GUILayout.Label("Total time for refill lifes:", EditorStyles.label);
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.Label("Hour", EditorStyles.label, GUILayout.Width(50));
            GUILayout.Label("Min", EditorStyles.label, GUILayout.Width(50));
            GUILayout.Label("Sec", EditorStyles.label, GUILayout.Width(50));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            lm.TotalTimeForRestLifeHours = EditorGUILayout.FloatField("", lm.TotalTimeForRestLifeHours, new GUILayoutOption[] { GUILayout.Width(50) });
            lm.TotalTimeForRestLifeMin = EditorGUILayout.FloatField("", lm.TotalTimeForRestLifeMin, new GUILayoutOption[] { GUILayout.Width(50) });
            lm.TotalTimeForRestLifeSec = EditorGUILayout.FloatField("", lm.TotalTimeForRestLifeSec, new GUILayoutOption[] { GUILayout.Width(50) });
            GUILayout.EndHorizontal();
            GUILayout.Space(10);


            lm.CostIfRefill = EditorGUILayout.IntField("Cost of refilling lifes", lm.CostIfRefill, new GUILayoutOption[] {
                GUILayout.Width (200),
                GUILayout.MaxWidth (200)
            });
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        GUILayout.Space(20);

        lm.FirstGems = EditorGUILayout.IntField("Start gems", lm.FirstGems, new GUILayoutOption[] {
            GUILayout.Width (200),
            GUILayout.MaxWidth (200)
        });
        GUILayout.Space(20);


        GUILayout.Space(20);


        failed_settings_show = EditorGUILayout.Foldout(failed_settings_show, "Failed settings:");
        if (failed_settings_show)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.BeginVertical();

            lm.FailedCost = EditorGUILayout.IntField(new GUIContent("Cost of continue", "Cost of continue after failed"), lm.FailedCost, new GUILayoutOption[] {
                GUILayout.Width (200),
                GUILayout.MaxWidth (200)
            });
            lm.ExtraFailedMoves = EditorGUILayout.IntField(new GUIContent("Extra moves", "Extra moves after continue"), lm.ExtraFailedMoves, new GUILayoutOption[] {
                GUILayout.Width (200),
                GUILayout.MaxWidth (200)
            });

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

        }

        GUILayout.Space(10);

        if (GUILayout.Button("Save", new GUILayoutOption[] { GUILayout.Width(150) }))
        {
            SaveSettings();
        }

    }

    public void SaveLevelStarsCount(int level, int starsCount)
    {
        Debug.Log(string.Format("Stars count {0} of level {1} saved.", starsCount, level));
        PlayerPrefs.SetInt(GetLevelKey(level), starsCount);
    }

    private string GetLevelKey(int number)
    {
        return string.Format("Level.{0:000}.StarsCount", number);
    }

    #endregion

    void GUIAds()
    {
        bool oldenableAds = lm.enableUnityAds;

        GUILayout.Label("Ads settings:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });
        GUILayout.BeginHorizontal();

        //UNITY ADS

        //        lm.enableUnityAds = EditorGUILayout.Toggle("Enable Unity ads", lm.enableUnityAds, new GUILayoutOption[] {
        //            GUILayout.Width (200)
        //        });
        string labelText = "Install: Windows->\n Services->Ads - ON";
#if UNITY_ADS
        labelText = "Installed";
#endif

        GUILayout.Label("Unity ads", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });
        GUILayout.Label(labelText, new GUILayoutOption[] { GUILayout.Width(130) });
        if (GUILayout.Button("Help", new GUILayoutOption[] { GUILayout.Width(80) }))
        {
            Application.OpenURL("https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0");
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        //		if (oldenableAds != lm.enableUnityAds) {
        //			//			SetScriptingDefineSymbols ();
        //			SaveSettings ();
        //		}
        //		if (lm.enableUnityAds) {
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        lm.rewardedGems = EditorGUILayout.IntField("Rewarded gems", lm.rewardedGems, new GUILayoutOption[] {
            GUILayout.Width (200),
            GUILayout.MaxWidth (200)
        });
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        //		}

        //GOOGLE MOBILE ADS

        bool oldenableGoogleMobileAds = lm.enableGoogleMobileAds;
        GUILayout.BeginHorizontal();
        //        lm.enableGoogleMobileAds = EditorGUILayout.Toggle("Enable Google Mobile Ads", lm.enableGoogleMobileAds, new GUILayoutOption[] {
        //            GUILayout.Width (50),
        //            GUILayout.MaxWidth (200)
        //        });
        labelText = "Install";
#if GOOGLE_MOBILE_ADS
        labelText = "Installed";
#endif
        GUILayout.Label("Google mobile ads", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });
        if (GUILayout.Button(labelText, new GUILayoutOption[] { GUILayout.Width(100) }))
        {
            Application.OpenURL("https://github.com/googleads/googleads-mobile-unity/releases");
        }
        if (GUILayout.Button("Help", new GUILayoutOption[] { GUILayout.Width(80) }))
        {
            Application.OpenURL("https://docs.google.com/document/d/1I69mo9yLzkg35wtbHpsQd3Ke1knC5pf7G1Wag8MdO-M/edit?usp=sharing");
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        if (oldenableGoogleMobileAds != lm.enableGoogleMobileAds)
        {

            //			SetScriptingDefineSymbols ();
            if (lm.enableGoogleMobileAds)
            {
                enableGoogleAdsProcessing = true;
            }
            SaveSettings();

        }
        //		if (lm.enableGoogleMobileAds) {
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        lm.admobUIDAndroid = EditorGUILayout.TextField("Admob Interstitial ID Android ", lm.admobUIDAndroid, new GUILayoutOption[] {
            GUILayout.Width (220),
            GUILayout.MaxWidth (220)
        });
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        lm.admobUIDIOS = EditorGUILayout.TextField("Admob Interstitial ID iOS", lm.admobUIDIOS, new GUILayoutOption[] {
            GUILayout.Width (220),
            GUILayout.MaxWidth (220)
        });
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        //			GUILayout.BeginHorizontal ();
        //			GUILayout.Space (20);
        //			lm.admobRewardedUIDAndroid = EditorGUILayout.TextField ("Rewarded Video ID Android ", lm.admobRewardedUIDAndroid, new GUILayoutOption[] {
        //				GUILayout.Width (220),
        //				GUILayout.MaxWidth (220)
        //			});
        //			GUILayout.EndHorizontal ();
        //			GUILayout.BeginHorizontal ();
        //			GUILayout.Space (20);
        //			lm.admobRewardedUIDIOS = EditorGUILayout.TextField ("Rewarded Video ID iOS", lm.admobRewardedUIDIOS, new GUILayoutOption[] {
        //				GUILayout.Width (220),
        //				GUILayout.MaxWidth (220)
        //			});
        //			GUILayout.EndHorizontal ();
        GUILayout.Space(10);

        //		}

        //CHARTBOOST ADS

        GUILayout.BeginHorizontal();
        bool oldenableChartboostAds = lm.enableChartboostAds;
        //		lm.enableChartboostAds = EditorGUILayout.Toggle ("Enable Chartboost Ads", lm.enableChartboostAds, new GUILayoutOption[] {
        //			GUILayout.Width (50),
        //			GUILayout.MaxWidth (200)
        //		});
        labelText = "Install";
#if CHARTBOOST_ADS
        labelText = "Installed";
#endif
        GUILayout.Label("Chartboost ads", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });
        if (GUILayout.Button(labelText, new GUILayoutOption[] { GUILayout.Width(100) }))
        {
            Application.OpenURL("http://www.chartboo.st/sdk/unity");
        }
        if (GUILayout.Button("Help", new GUILayoutOption[] { GUILayout.Width(80) }))
        {
            Application.OpenURL("https://docs.google.com/document/d/1ibnQbuxFgI4izzyUtT45WH5m1ab3R5d1E3ke3Wrb10Y");
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        if (oldenableChartboostAds != lm.enableChartboostAds)
        {
            //			SetScriptingDefineSymbols ();
            SaveSettings();

        }
        //		if (lm.enableChartboostAds) {
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("menu Chartboost->Edit settings", new GUILayoutOption[] {
            GUILayout.Width (50),
            GUILayout.MaxWidth (200)
        });
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Put ad ID to appropriate platform to prevent crashing!", EditorStyles.boldLabel, new GUILayoutOption[] {
            GUILayout.Width (100),
            GUILayout.MaxWidth (400)
        });
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        //		}
        if (GUILayout.Button("Save", new GUILayoutOption[] { GUILayout.Width(150) }))
        {
            SaveSettings();
        }

        GUILayout.Space(10);

        GUILayout.Label("Ads controller:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });

        GUILayout.Label("Event:               Status:                            Show every:", new GUILayoutOption[] { GUILayout.Width(400) });

        foreach (AdItem item in lm.adsEvents)
        {
            AdItem oldItem = item;
            EditorGUILayout.BeginHorizontal();
            item.gameEvent = (GameState)EditorGUILayout.EnumPopup(item.gameEvent, new GUILayoutOption[] { GUILayout.Width(100) });
            item.adType = (AdType)EditorGUILayout.EnumPopup(item.adType, new GUILayoutOption[] { GUILayout.Width(150) });
            item.callsTreshold = EditorGUILayout.IntPopup(item.callsTreshold, new string[] {
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "10"
            }, new int[] {
                1,
                2,
                3,
                4,
                5,
                6,
                7,
                8,
                9,
                10
            }, new GUILayoutOption[] { GUILayout.Width(100) });
            EditorGUILayout.EndHorizontal();
        }
        for (int i = 0; i < oldList.Count; i++)
        {
            if (oldList[i].adType != lm.adsEvents[i].adType || oldList[i].callsTreshold != lm.adsEvents[i].callsTreshold || oldList[i].gameEvent != lm.adsEvents[i].gameEvent)
            {
                SaveSettings();
                break;
            }
        }
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            lm.adsEvents.Add(new AdItem());
            SaveSettings();

        }
        if (GUILayout.Button("Delete"))
        {
            if (lm.adsEvents.Count > 0)
                lm.adsEvents.Remove(lm.adsEvents[lm.adsEvents.Count - 1]);
            SaveSettings();

        }


        GUILayout.Space(10);



    }

    void SaveSettings()
    {
        oldList.Clear();
        for (int i = 0; i < lm.adsEvents.Count; i++)
        {
            oldList.Add(new AdItem());
            oldList[i].adType = lm.adsEvents[i].adType;
            oldList[i].callsTreshold = lm.adsEvents[i].callsTreshold;
            oldList[i].gameEvent = lm.adsEvents[i].gameEvent;
        }
        EditorUtility.SetDirty(lm.gameObject);
        AssetDatabase.SaveAssets();
    }

    void GUIHelp()
    {
        GUILayout.Label("Please read our documentation:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(200) });
        if (GUILayout.Button("DOCUMENTATION", new GUILayoutOption[] { GUILayout.Width(150) }))
        {
            Application.OpenURL("https://docs.google.com/document/d/1RRw7XNXGslH8pto0BYwjk_ZcDUKxS2QHzs-7LbSYPRI");
        }
        GUILayout.Space(10);
        GUILayout.Label("To get support you should provide \n ORDER NUMBER (asset store) \n or NICKNAME and DATE of purchase (other stores):", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(350) });
        GUILayout.Space(10);
        GUILayout.TextArea("info@candy-smith.com", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(350) });

    }



    void GUIInappSettings()
    {

        GUILayout.Label("In-apps settings:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });

        GUILayout.Space(10);
        bool oldenableInApps = lm.enableInApps;

        GUILayout.BeginHorizontal();
        //		lm.enableInApps = EditorGUILayout.Toggle ("Enable In-apps", lm.enableInApps, new GUILayoutOption[] {
        //			GUILayout.Width (180)
        //		});
        if (GUILayout.Button("Help", new GUILayoutOption[] { GUILayout.Width(80) }))
        {
            Application.OpenURL("https://docs.google.com/document/d/1HeN8JtQczTVetkMnd8rpSZp_TZZkEA7_kan7vvvsMw0#bookmark=id.b1efplsspes5");
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.Label("Install: Windows->Services->\n In-app Purchasing - ON->Import", new GUILayoutOption[] { GUILayout.Width(400) });
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        //		if (oldenableInApps != lm.enableInApps) {
        //			//			SetScriptingDefineSymbols ();
        //			SaveSettings ();
        //
        //		}


        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();
        for (int i = 0; i < 4; i++)
        {
            lm.InAppIDs[i] = EditorGUILayout.TextField("Product id " + (i + 1), lm.InAppIDs[i], new GUILayoutOption[] {
                GUILayout.Width (300),
                GUILayout.MaxWidth (300)
            });

        }
        GUILayout.Space(10);

        GUILayout.Label("Android:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        GUILayout.BeginVertical();
        GUILayout.Space(10);
        //GUILayout.Label(" Put Google license key into the field \n from the google play account ", EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(300) });
        //GUILayout.Space(10);

        //lm.GoogleLicenseKey = EditorGUILayout.TextField("Google license key", lm.GoogleLicenseKey, new GUILayoutOption[] {
        //    GUILayout.Width (300),
        //    GUILayout.MaxWidth (300)
        //});

        GUILayout.Space(10);
        if (GUILayout.Button("Android account help", new GUILayoutOption[] { GUILayout.Width(400) }))
        {
            Application.OpenURL("http://developer.android.com/google/play/billing/billing_admin.html");
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();

        GUILayout.Space(10);
        GUILayout.Label("iOS:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(150) });
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        GUILayout.BeginVertical();

        //GUILayout.Label(" StoreKit library must be added \n to the XCode project, generated by Unity ", EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(300) });
        GUILayout.Space(10);
        if (GUILayout.Button("iOS account help", new GUILayoutOption[] { GUILayout.Width(400) }))
        {
            Application.OpenURL("https://developer.apple.com/library/ios/qa/qa1329/_index.html");
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

    }

    #region leveleditor

    void GUILevelSelector()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Level editor", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(70) });
        //if (GUILayout.Button("Save level", new GUILayoutOption[] { GUILayout.Width(100) }))
        //{
        //	SaveLevel();
        //}

        if (GUILayout.Button("Test level", new GUILayoutOption[] { GUILayout.Width(150) }))
        {
            SaveLevel();
            PlayerPrefs.SetInt("OpenLevelTest", levelNumber);
            PlayerPrefs.SetInt("OpenLevel", levelNumber);
            PlayerPrefs.Save();
            EditorSceneManager.OpenScene("Assets/RaccoonRescue/Scenes/game.unity");

            EditorApplication.isPlaying = true;


        }

        GUILayout.EndHorizontal();

        //     myString = EditorGUILayout.TextField("Text Field", myString);
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Level:", EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(50) });

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        if (GUILayout.Button("<<", new GUILayoutOption[] { GUILayout.Width(50) }))
        {
            PreviousLevel();
        }
        string changeLvl = GUILayout.TextField(" " + levelNumber, new GUILayoutOption[] { GUILayout.Width(50) });
        try
        {
            if (int.Parse(changeLvl) != levelNumber)
            {
                if (LoadDataFromLocal(int.Parse(changeLvl)))
                    levelNumber = int.Parse(changeLvl);

            }
        }
        catch (Exception)
        {

            throw;
        }

        if (GUILayout.Button(">>", new GUILayoutOption[] { GUILayout.Width(50) }))
        {
            NextLevel();
        }

        if (GUILayout.Button("New level", new GUILayoutOption[] { GUILayout.Width(100) }))
        {
            AddLevel();
        }


        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Space(60);

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

    }


    void AddLevel()
    {
        SaveLevel();
        levelNumber = GetLastLevel() + 1;
        Initialize();
        SaveLevel();
    }

    void NextLevel()
    {
        levelNumber++;
        if (!LoadDataFromLocal(levelNumber))
            levelNumber--;
    }

    void PreviousLevel()
    {
        levelNumber--;
        if (levelNumber < 1)
            levelNumber = 1;
        if (!LoadDataFromLocal(levelNumber))
            levelNumber++;


    }


    #endregion


    void GUILimit()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(60);

        GUILayout.Label("Limit:", EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(50) });
        LIMIT limitTypeSave = limitType;
        int oldLimit = limit;
        limitType = (LIMIT)EditorGUILayout.EnumPopup(limitType, GUILayout.Width(93));
        if (limitType == LIMIT.MOVES)
            limit = EditorGUILayout.IntField(limit, new GUILayoutOption[] { GUILayout.Width(50) });
        else
        {
            GUILayout.BeginHorizontal();
            int limitMin = EditorGUILayout.IntField(limit / 60, new GUILayoutOption[] { GUILayout.Width(30) });
            GUILayout.Label(":", new GUILayoutOption[] { GUILayout.Width(10) });
            int limitSec = EditorGUILayout.IntField(limit - (limit / 60) * 60, new GUILayoutOption[] { GUILayout.Width(30) });
            limit = limitMin * 60 + limitSec;
            GUILayout.EndHorizontal();
        }
        if (limit <= 0)
            limit = 1;
        GUILayout.EndHorizontal();

        if (limitTypeSave != limitType || oldLimit != limit)
            SaveLevel();

    }

    void GUIColorLimit()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(60);

        int saveInt = colorLimit;
        GUILayout.Label("Color limit:", EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(100) });
        colorLimit = (int)GUILayout.HorizontalSlider(colorLimit, 3, 6, new GUILayoutOption[] { GUILayout.Width(100) });
        colorLimit = EditorGUILayout.IntField("", colorLimit, new GUILayoutOption[] { GUILayout.Width(50) });
        if (colorLimit < 3)
            colorLimit = 3;
        if (colorLimit > 6)
            colorLimit = 6;

        GUILayout.EndHorizontal();

        if (saveInt != colorLimit)
        {
            SaveLevel();
        }

    }

    void GUIPowerups()
    {
        GUILayout.BeginHorizontal();
        for (int i = 0; i < powerups.Length; i++)
        {
            Texture2D tex = lm.powerupTextures[i];
            if (powerups[i] == 1)
                tex = lm.powerupTexturesOn[i];
            if (GUILayout.Button(tex, new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(50) }))
            {
                powerups[i] = (int)Mathf.Repeat(powerups[i] + 1, 1.1f);
                SaveLevel();
            }
        }
        GUILayout.EndHorizontal();
    }


    void GUIStars()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();

        GUILayout.Label("Stars:", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.Label("Star1", new GUILayoutOption[] { GUILayout.Width(100) });
        GUILayout.Label("Star2", new GUILayoutOption[] { GUILayout.Width(100) });
        GUILayout.Label("Star3", new GUILayoutOption[] { GUILayout.Width(100) });
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        int s = 0;
        s = EditorGUILayout.IntField("", star1, new GUILayoutOption[] { GUILayout.Width(100) });
        if (s != star1)
        {
            star1 = s;
            SaveLevel();
        }
        if (star1 < 0)
            star1 = 10;
        s = EditorGUILayout.IntField("", star2, new GUILayoutOption[] { GUILayout.Width(100) });
        if (s != star2)
        {
            star2 = s;
            SaveLevel();
        }
        if (star2 < star1)
            star2 = star1 + 10;
        s = EditorGUILayout.IntField("", star3, new GUILayoutOption[] { GUILayout.Width(100) });
        if (s != star3)
        {
            star3 = s;
            SaveLevel();
        }
        if (star3 < star2)
            star3 = star2 + 10;
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

    }

    void GUITarget()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();
        GUILayout.Label("Target:", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();
        TargetType saveTarget = target;
        target = (TargetType)EditorGUILayout.EnumPopup(target, GUILayout.Width(100));
        if (target == TargetType.Top)
        {
        }
        GUILayout.EndVertical();
        if (saveTarget != target)
        {
            SaveLevel();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();


        GUILayout.EndHorizontal();
    }


    void GUIBlocks()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("Powerups ON/OFF:", EditorStyles.boldLabel, new GUILayoutOption[] {
            GUILayout.Width (150)
        });

        GUILayout.BeginHorizontal();

        GUILayout.Space(30);

        GUIPowerups();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();


        GUILayout.Label("Items:", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();

        ShowItemsPanel();

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(" X ", new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(50) }))
        {
            brush = 0;
        }
        if (GUILayout.Button("Clear \nlevel", new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(50) }))
        {
            for (int i = 0; i < levelSquares.Length; i++)
            {
                levelSquares[i] = 0;
            }
            SaveLevel();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

    }

    List<Vector2> mousePosList = new List<Vector2>();
    Vector2 mPos;
    bool mDown;
    private bool share_settings;

    void MouseControl()
    {

        if (Event.current.type == EventType.MouseDown)
        {
            mDown = true;
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            mDown = false;
            SaveLevel();
        }
    }


    void GUIGameField()
    {
        MouseControl();
        GUILayout.BeginVertical();
        bool offset = false;

        for (int row = 0; row < maxRows; row++)
        {
            GUILayout.BeginHorizontal();
            if (offset)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);

            }

            for (int col = 0; col < maxCols; col++)
            {

                var imageButton = new object();
                if (levelSquares[row * maxCols + col] == 0)
                {
                    imageButton = "X";
                }
                else if (levelSquares[row * maxCols + col] != 0)
                {
                    int index = levelSquares[row * maxCols + col];
                    if (lm.items.Count > index)
                    {
                        if (lm.items[index].sprite != null)
                            imageButton = lm.items[index].sprite.texture;
                    }
                }

                if (GUILayout.Button(imageButton as Texture, new GUILayoutOption[] {
                    GUILayout.Width (50),
                    GUILayout.Height (50)
                }))
                {
                    //					SetType (col, row);
                }

                if (mDown && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && levelSquares[row * maxCols + col] != brush)
                {
                    SetType(col, row);
                }

            }
            GUILayout.EndHorizontal();
            if (offset)
            {
                GUILayout.EndHorizontal();

            }


            offset = !offset;
        }
        GUILayout.EndVertical();
    }

    void SetType(int col, int row)
    {

        levelSquares[row * maxCols + col] = brush;

        //			SaveLevel ();

        // GetSquare(col, row).type = (int) squareType;
    }


    int GetLastLevel()
    {
        TextAsset mapText = null;
        for (int i = levelNumber; i < 50000; i++)
        {
            mapText = Resources.Load("Levels/" + i) as TextAsset;
            if (mapText == null)
            {
                return i - 1;
            }
        }
        return 0;
    }

    void SaveLevel()
    {
        bool rotatingItemExist = false;
        bool petExist = false;
        foreach (int itemNum in levelSquares)
        {
            if (itemNum > 0 && itemNum < lm.items.Count)
            {
                ItemKind itemKind = lm.items[itemNum];
                if (itemKind.itemType == ItemTypes.Rotation)
                    rotatingItemExist = true;
                else if (itemKind.itemType == ItemTypes.Cub)
                    petExist = true;
            }
        }
        if (rotatingItemExist)
            target = TargetType.Round;
        else if (petExist)
            target = TargetType.RescuePets;
        else
            target = TargetType.Top;


        if (!fileName.Contains(".txt"))
            fileName += ".txt";
        SaveMap(fileName);
    }

    public void SaveMap(string fileName)
    {
        string saveString = "";
        //Create save string
        saveString += "MODE " + (int)target;
        saveString += "\r\n";
        saveString += "SIZE " + maxCols + "/" + maxRows;
        saveString += "\r\n";
        saveString += "LIMIT " + (int)limitType + "/" + limit;
        saveString += "\r\n";
        saveString += "COLOR LIMIT " + colorLimit;
        saveString += "\r\n";
        saveString += "STARS " + star1 + "/" + star2 + "/" + star3;
        saveString += "\r\n";
        saveString += "POWERUPS " + powerups[0] + "/" + powerups[1] + "/" + powerups[2] + "/" + powerups[3];
        saveString += "\r\n";

        //set map data
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                saveString += (int)levelSquares[row * maxCols + col];
                //if this column not yet end of row, add space between them
                if (col < (maxCols - 1))
                    saveString += " ";
            }
            //if this row is not yet end of row, add new line symbol between rows
            if (row < (maxRows - 1))
                saveString += "\r\n";
        }
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
        {
            //Write to file
            string activeDir = Application.dataPath + @"/RaccoonRescue/Resources/Levels/";
            string newPath = System.IO.Path.Combine(activeDir, levelNumber + ".txt");
            StreamWriter sw = new StreamWriter(newPath);
            sw.Write(saveString);
            sw.Close();
        }
        AssetDatabase.Refresh();
    }

    public bool LoadDataFromLocal(int currentLevel)
    {
        //Read data from text file
        TextAsset mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        if (mapText == null)
        {
            return false;
            SaveLevel();
            mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        }
        ProcessGameDataFromString(mapText.text);
        return true;
    }

    void ProcessGameDataFromString(string mapText)
    {
        string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < 4; i++)
        {
            powerups[i] = 0;
        }

        int mapLine = 0;
        foreach (string line in lines)
        {
            if (line.StartsWith("MODE "))
            {
                string modeString = line.Replace("MODE", string.Empty).Trim();
                target = (TargetType)int.Parse(modeString);
            }
            else if (line.StartsWith("SIZE "))
            {
                string blocksString = line.Replace("SIZE", string.Empty).Trim();
                string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                maxCols = int.Parse(sizes[0]);
                maxRows = int.Parse(sizes[1]);
                Initialize();
            }
            else if (line.StartsWith("LIMIT "))
            {
                string blocksString = line.Replace("LIMIT", string.Empty).Trim();
                string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                limitType = (LIMIT)int.Parse(sizes[0]);
                limit = int.Parse(sizes[1]);

            }
            else if (line.StartsWith("COLOR LIMIT "))
            {
                string blocksString = line.Replace("COLOR LIMIT", string.Empty).Trim();
                colorLimit = int.Parse(blocksString);
            }
            else if (line.StartsWith("STARS "))
            {
                string blocksString = line.Replace("STARS", string.Empty).Trim();
                string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                star1 = int.Parse(blocksNumbers[0]);
                star2 = int.Parse(blocksNumbers[1]);
                star3 = int.Parse(blocksNumbers[2]);
            }
            else if (line.StartsWith("POWERUPS "))
            {
                string blocksString = line.Replace("POWERUPS", string.Empty).Trim();
                string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < 4; i++)
                {
                    powerups[i] = int.Parse(blocksNumbers[i]);
                }

            }
            else
            { //Maps
              //Split lines again to get map numbers
                string[] st = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < st.Length; i++)
                {
                    levelSquares[mapLine * maxCols + i] = int.Parse(st[i].ToString());
                }
                mapLine++;
            }
        }
    }



}


public class MySprite : ScriptableObject
{
    public Sprite background;
}