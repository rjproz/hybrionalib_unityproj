/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  GoogleMobileAdsImplementation.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  17-09-2023 22:16:55

*************************************************************************/

using GoogleMobileAds.Api;
using UnityEngine;

namespace Hybriona
{
	public class GoogleMobileAdsImplementation  
	{
		
		public static void InvokeSDKInitialization(System.Action completion)
        {
			MobileAds.RaiseAdEventsOnUnityMainThread = true;
			MobileAds.Initialize((initStatus) =>
			{
				if (completion != null)
				{
					completion();
				}
			});
		}

		

		public class RewardedAdWrapper
        {
			private RewardedAd rewardedAd;
             
			public RewardedAdWrapper()
            {

            }

			public void LoadAdBinding(string unitId)
            {
                if (rewardedAd != null)
                {
                    rewardedAd.Destroy();
                    rewardedAd = null;
                }

                Debug.Log("Loading the rewarded ad.");




                
                var adRequest = new AdRequest();

                /*
                RewardedAd.Load(unitId, adRequest, (RewardedAd ad, LoadAdError error) => {
                    m_IsAdTryingToLoad = false;
                    if (error != null || ad == null)
                    {
                        Debug.LogError("Rewarded ad failed to load an ad " +
                                        "with error : " + error);

                        InvokeLoadErrorEvent();
                        return;
                    }

                    Debug.Log("Rewarded ad loaded with response : "
                                + ad.GetResponseInfo());

                    m_IsAdAvailable = true;
                    rewardedAd = ad;
                    InvokeAdLoadedEvent();
                });
                */
            }
		}


	}
	
	

	public class GoogleAdTest
    {
		public static void Test()
        {
			AdUnit rewardAdUnit = new AdUnit(AdType.Interstitial, "ss");
			rewardAdUnit.LoadAd();

		}
    }
}
