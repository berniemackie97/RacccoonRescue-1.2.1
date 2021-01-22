using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if APPODEAL
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
#endif

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
using UnityEngine.SceneManagement;
using InitScriptName;

#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif
public class RewardedVideo : MonoBehaviour//, IRewardedVideoAdListener
{
    public VideoWatchedEvent videoWatchedEvent;
    string rewardedVideoZone;
    // Use this for initialization
    void OnEnable()
    {
        if (GetRewardedAdsReady())
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
#if APPODEAL
		
        Appodeal.setRewardedVideoCallbacks(this);
#endif
    }

    public void GetCoins(int addCoins)
    {
        RewardIcon reward = MenuManager.Instance.RewardPopup.GetComponent<RewardIcon>();
        reward.SetIconSprite(0);
        reward.gameObject.SetActive(true);
        InitScript.Instance.AddGems(addCoins);
        MenuManager.Instance.MenuCurrencyShop.GetComponent<AnimationManager>().CloseMenu();
    }

    public void GetLifes()
    {
        RewardIcon reward = MenuManager.Instance.RewardPopup.GetComponent<RewardIcon>();
        reward.SetIconSprite(1);
        reward.gameObject.SetActive(true);
        InitScript.Instance.RestoreLifes();
        MenuManager.Instance.MenuLifeShop.GetComponent<AnimationManager>().CloseMenu();

    }

    public void ContinuePlay()
    {
        MenuManager.Instance.PreFailedBanner.GetComponent<AnimationManager>().GoOnFailed();
    }

    public void ShowRewardedAds()
    {
#if APPODEAL
		// if (Appodeal.isLoaded(Appodeal.REWARDED_VIDEO)) {
		// 	Appodeal.show(Appodeal.REWARDED_VIDEO);
		// }

#endif
#if UNITY_ADS
        if (GetRewardedAdsReady())
        {
            Advertisement.Show(rewardedVideoZone, new ShowOptions
            {
                resultCallback = result =>
                {
                    if (result == ShowResult.Finished)
                    {
                        CheckRewardedAds();
                    }
                }
            });
        }
#endif
#if GOOGLE_MOBILE_ADS
        // ShowAdmobRewarded();
#endif
    }

    void CheckRewardedAds()
    {
        videoWatchedEvent.Invoke(1);
        //		RewardIcon reward = null;
        //		if (SceneManager.GetActiveScene () == SceneManager.GetSceneByName ("map"))//TODO: set reward window to Menu manager
        //			reward = GameObject.Find ("Canvas").transform.Find ("Reward").GetComponent<RewardIcon> ();
        //		if (currentReward == RewardedAdsType.GetGems) {
        //			reward.SetIconSprite (0);
        //
        //			reward.gameObject.SetActive (true);
        //			InitScript.Instance.AddGems (LevelEditorBase.THIS.rewardedGems);
        //			GameObject.Find ("CanvasMenu").transform.Find ("GemsShop").GetComponent<AnimationManager> ().CloseMenu ();
        //		} else if (currentReward == RewardedAdsType.GetLifes) {
        //			reward.SetIconSprite (1);
        //			reward.gameObject.SetActive (true);
        //			InitScript.Instance.RestoreLifes ();
        //			GameObject.Find ("Canvas").transform.Find ("LiveShop").GetComponent<AnimationManager> ().CloseMenu ();
        //		} else if (currentReward == RewardedAdsType.GetGoOn) {
        //			GameObject.Find ("CanvasMenu").transform.Find ("PreFailedBanner").GetComponent<AnimationManager> ().GoOnFailed ();
        //		}

    }

    public bool GetRewardedAdsReady()
    {
#if UNITY_ADS
        rewardedVideoZone = "rewardedVideo";
        if (Advertisement.IsReady(rewardedVideoZone))
        {
            return true;
        }
        else
        {
            rewardedVideoZone = "rewardedVideoZone";
            if (Advertisement.IsReady(rewardedVideoZone))
            {
                return true;
            }
        }
#endif

        return false;
    }


    #region Rewarded Video callback handlers

    public void onRewardedVideoLoaded()
    {
        print("Video loaded");
    }

    public void onRewardedVideoFailedToLoad()
    {
        print("Video failed");
    }

    public void onRewardedVideoShown()
    {
        print("Video shown");
    }

    public void onRewardedVideoClosed()
    {
        print("Video closed");
    }

    public void onRewardedVideoFinished(int amount, string name)
    {
        CheckRewardedAds();

        print("Reward: " + amount + " " + name);
    }

    #endregion
}

[System.Serializable]
public class VideoWatchedEvent : UnityEvent<int>
{
}