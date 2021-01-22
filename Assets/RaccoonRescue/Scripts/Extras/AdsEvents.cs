using UnityEngine;
using System.Collections;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif
using System;
using UnityEngine.SceneManagement;

#if CHARTBOOST_ADS
using ChartboostSDK;
#endif
using InitScriptName;
#if APPODEAL
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
#endif
public enum AdType
{
    AdmobInterstitial,
    ChartboostInterstitial,
    UnityAdsVideo
    // Appodeal
}


[System.Serializable]
public class AdItem
{
    public GameState gameEvent;
    public AdType adType;
    public int callsTreshold;
    public int calls;
}


public class AdsEvents : MonoBehaviour//, INonSkippableVideoAdListener
{
    public static AdsEvents THIS;
    private string admobUIDAndroid;
    private string admobUIDIOS;

    public string nonRewardedVideoZone;
    public RewardedAdsType currentReward;
#if GOOGLE_MOBILE_ADS
	private InterstitialAd interstitial;
	private AdRequest requestAdmob;

#endif

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (THIS == null)
            THIS = this;
        else if (THIS != this)
            Destroy(gameObject);

        admobUIDAndroid = LevelEditorBase.THIS.admobUIDAndroid;
        admobUIDIOS = LevelEditorBase.THIS.admobUIDIOS;

#if GOOGLE_MOBILE_ADS
#if UNITY_ANDROID
        MobileAds.Initialize(admobUIDAndroid);//2.1.6
        interstitial = new InterstitialAd(admobUIDAndroid);
#elif UNITY_IOS
        MobileAds.Initialize(admobUIDIOS);//2.1.6
        interstitial = new InterstitialAd(admobUIDIOS);
#else
        MobileAds.Initialize(admobUIDAndroid);//2.1.6
		interstitial = new InterstitialAd (admobUIDAndroid);
#endif
		Debug.Log("admob init");
		// Create an empty ad request.
		requestAdmob = new AdRequest.Builder().Build();
		// Load the interstitial with the request.
		interstitial.LoadAd(requestAdmob);
		interstitial.OnAdLoaded += HandleInterstitialLoaded;
		interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;

#endif
#if APPODEAL

		string appKey = "bde15e00c921518626556ef7a2c72599fc56e337a1ef9bc2";
		Appodeal.disableLocationPermissionCheck();
		Appodeal.initialize(appKey, Appodeal.NON_SKIPPABLE_VIDEO | Appodeal.BANNER | Appodeal.REWARDED_VIDEO);
		Appodeal.setNonSkippableVideoCallbacks(this);
#endif

#if CHARTBOOST_ADS
        Chartboost.cacheInterstitial(CBLocation.Default);
#endif
#if GOOGLE_MOBILE_ADS
		//RequestRewardBasedVideo();
#endif
    }

#if GOOGLE_MOBILE_ADS

	public void HandleInterstitialLoaded(object sender, EventArgs args)
	{
		print("HandleInterstitialLoaded event received.");
	}

	public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		print("HandleInterstitialFailedToLoad event received with message: " + args.Message);
	}
#endif

    void OnEnable()
    {
        GameEvent.OnStatus += CheckAdsEvents;
    }

    public void CheckAdsEvents(GameState state)
    {
        foreach (AdItem item in LevelEditorBase.THIS.adsEvents)
        {
            if (item.gameEvent == state)
            {
                item.calls++;
                // Debug.Log(item.calls);
                if (item.calls % item.callsTreshold == 0)
                    ShowAdByType(item.adType);
            }

        }
    }

    void ShowAdByType(AdType adType)
    {
        if (adType == AdType.AdmobInterstitial)
            ShowAds(false);
        else if (adType == AdType.UnityAdsVideo)
            ShowVideo();
        else if (adType == AdType.ChartboostInterstitial)
            ShowAds(true);
        // else if (adType == AdType.Appodeal) ShowAppodeal();
    }

    public void ShowVideo()
    {
#if UNITY_ADS
		Debug.Log("show Unity ads video in " + GameEvent.Instance.GameStatus);

		if (Advertisement.IsReady("video"))
		{
			Advertisement.Show("video");
		}
		else
		{
			if (Advertisement.IsReady("defaultZone"))
			{
				Advertisement.Show("defaultZone");
			}
		}
#endif
    }

    public void ShowAppodeal()//1.1
    {
#if APPODEAL
		Debug.Log("show Appodeal in " + GameEvent.Instance.GameStatus);

		if (Appodeal.isLoaded(Appodeal.NON_SKIPPABLE_VIDEO))
			Appodeal.show(Appodeal.NON_SKIPPABLE_VIDEO);
#endif

    }


    public void ShowAds(bool chartboost = true)
    {
        if (chartboost)
        {
#if CHARTBOOST_ADS
            Debug.Log("show Chartboost Interstitial in " + GameEvent.Instance.GameStatus);

            Chartboost.showInterstitial(CBLocation.Default);
            Chartboost.cacheInterstitial(CBLocation.Default);
#endif
        }
        else
        {
#if GOOGLE_MOBILE_ADS
			Debug.Log("show Google mobile ads Interstitial in " + GameEvent.Instance.GameStatus);
			if (interstitial.IsLoaded())
			{
				interstitial.Show();
#if UNITY_ANDROID
				interstitial = new InterstitialAd(admobUIDAndroid);
#elif UNITY_IOS
				interstitial = new InterstitialAd(admobUIDIOS);
#else
				interstitial = new InterstitialAd (admobUIDAndroid);
#endif

				// Create an empty ad request.
				requestAdmob = new AdRequest.Builder().Build();
				// Load the interstitial with the request.
				interstitial.LoadAd(requestAdmob);
			}
#endif
        }
    }

    private bool gameQuit;
#if GOOGLE_MOBILE_ADS
	public RewardBasedVideoAd admobRewardedVideo;

	private void RequestRewardBasedVideo()
	{
#if UNITY_EDITOR
		string adUnitId = "unused";
#elif UNITY_ANDROID
			string adUnitId = LevelEditorBase.THIS.admobRewardedUIDAndroid;
#elif UNITY_IOS
			string adUnitId = LevelEditorBase.THIS.admobRewardedUIDIOS;
#else
			string adUnitId = "unexpected_platform";
#endif
		admobRewardedVideo = RewardBasedVideoAd.Instance;
		print("admob reward request " + adUnitId);

		AdRequest request = new AdRequest.Builder().Build();
		admobRewardedVideo.LoadAd(request, adUnitId);
		admobRewardedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
		admobRewardedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
	}

	void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
	{
		print("On admob reward loaded " + args);
	}

	void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		print("On admob reward load failed " + args.Message);
	}
#endif
    void OnDisable()
    {
        GameEvent.OnStatus -= CheckAdsEvents;
    }

    private void OnApplicationQuit()//1.2
    {
        gameQuit = true;
    }

    private void OnDestroy()//1.2
    {
#if GOOGLE_MOBILE_ADS
		if (!gameQuit)
		{
			interstitial.OnAdLoaded -= HandleInterstitialLoaded;
			interstitial.OnAdFailedToLoad -= HandleInterstitialFailedToLoad;
		}
#endif
    }

    #region Interstitial callback handlers

    public void onInterstitialLoaded()
    {
        print("Interstitial loaded");
    }

    public void onInterstitialFailedToLoad()
    {
        print("Interstitial failed");
    }

    public void onInterstitialShown()
    {
        print("Interstitial opened");
    }

    public void onInterstitialClosed()
    {
        print("Interstitial closed");
    }

    public void onInterstitialClicked()
    {
        print("Interstitial clicked");
    }

    public void onNonSkippableVideoLoaded()
    {
        print(" video loaded");
    }

    public void onNonSkippableVideoFailedToLoad()
    {
        print(" video failed");

    }

    public void onNonSkippableVideoShown()
    {
        print(" video shown");

    }

    public void onNonSkippableVideoFinished()
    {
        print(" video finished");

    }

    public void onNonSkippableVideoClosed()
    {
        print(" video closed");

    }

    #endregion



}
