using UnityEngine;
using System.Collections;
#if UNITYADS
using UnityEngine.Advertisements;
#endif

public class AdsController : MonoBehaviour {
    public static AdsController THIS;
    public void ShowRewardedAds() {
        //#if UNITYADS
        //        if (Advertisement.IsReady())
        //        {
        //            Advertisement.Show(rewardedVideoZone, new ShowOptions
        //            {
        //                resultCallback = result =>
        //                {
        //                    if (result == ShowResult.Finished)
        //                    {
        //                        CheckRewardedAds();
        //                    }
        //                }
        //            });
        //        }
        //#endif
    }

}


