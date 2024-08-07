using GoogleMobileAds.Api;
using UnityEngine;

namespace Hybriona
{
    public class AdUnit
    {
        public string identifier { get; private set; }
        public AdType type { get; private set; }
        

        public string unitId { get; private set; }

        public System.Action onAdLoaded { get;  set; }
        public System.Action onAdFailedToLoad { get; set; }
        public System.Action onAdClosed { get; set; }
        public System.Action onRewardGranted { get; set; }

        private InterstitialAd interstitialAd;
        private RewardedAd rewardedAd;


        private bool m_IsAdAvailable;
        private bool m_IsAdTryingToLoad;

       

        public AdUnit(AdType type, string unitId)
        {
            this.identifier = Random.Range(100000, 999999) + "_" + Random.Range(100000, 999999);
            this.type = type;
            this.unitId = unitId;
           

        }

        public string GetUnitId()
        {
            return unitId;
        }

        public void LoadAd()
        {
            if(m_IsAdAvailable || m_IsAdTryingToLoad)
            {
                return;
            }

           
           
            
            if(type == AdType.Interstitial)
            {
                if (interstitialAd != null)
                {
                    interstitialAd.Destroy();
                    interstitialAd = null;
                }

                m_IsAdTryingToLoad = true;
                var adRequest = new AdRequest();
                InterstitialAd.Load(GetUnitId(), adRequest,
                    (InterstitialAd ad, LoadAdError error) =>
                    {
                        m_IsAdTryingToLoad = false;
                        // if error is not null, the load request failed.
                        if (error != null || ad == null)
                        {


                            //error event

                            InvokeLoadErrorEvent();
                            return;
                        }

                        Debug.Log("Interstitial ad loaded with response : "
                                + ad.GetResponseInfo());
                        m_IsAdAvailable = true;
                        interstitialAd = ad;

                        InvokeAdLoadedEvent();
                    });

            }
            else if(type == AdType.Rewarded)
            {
                m_IsAdTryingToLoad = true;

                if (rewardedAd != null)
                {
                    rewardedAd.Destroy();
                    rewardedAd = null;
                }

                Debug.Log("Loading the rewarded ad.");


                

                // create our request used to load the ad.
                var adRequest = new AdRequest();
                  
                // send the request to load the ad.
                RewardedAd.Load(GetUnitId(), adRequest,(RewardedAd ad, LoadAdError error) =>{
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

            }
            
        }

        public bool IsAdAvailable()
        {
            return m_IsAdAvailable;
        }



        public void ShowAd()
        {
            m_IsAdAvailable = false;
          
            if (type == AdType.Interstitial)
            {
                var ad = interstitialAd;
                // Raised when the ad is estimated to have earned money.
                ad.OnAdPaid += (AdValue adValue) =>
                {
                        
                    //Debug.Log(String.Format("Interstitial ad paid {0} {1}.",adValue.Value,adValue.CurrencyCode));
                };
                // Raised when an impression is recorded for an ad.
                ad.OnAdImpressionRecorded += () =>
                {
                    Debug.Log("Interstitial ad recorded an impression.");
                };
                // Raised when a click is recorded for an ad.
                ad.OnAdClicked += () =>
                {
                    Debug.Log("Interstitial ad was clicked.");
                };
                // Raised when an ad opened full screen content.
                ad.OnAdFullScreenContentOpened += () =>
                {
                    Debug.Log("Interstitial ad full screen content opened.");
                };
                // Raised when the ad closed full screen content.
                ad.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("Interstitial ad full screen content closed.");
                    InvokeAdClosedEvent();

                };
                // Raised when the ad failed to open full screen content.
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Debug.LogError("Interstitial ad failed to open full screen content " +
                                    "with error : " + error);
                    InvokeAdClosedEvent();
                };

                interstitialAd.Show();

            }
            else if(type == AdType.Rewarded)
            {
                var ad = rewardedAd;
                // Raised when the ad is estimated to have earned money.
                ad.OnAdPaid += (AdValue adValue) =>
                {

                    //Debug.Log(String.Format("Interstitial ad paid {0} {1}.",adValue.Value,adValue.CurrencyCode));
                };
                // Raised when an impression is recorded for an ad.
                ad.OnAdImpressionRecorded += () =>
                {
                    Debug.Log("Rewarded ad recorded an impression.");
                };
                // Raised when a click is recorded for an ad.
                ad.OnAdClicked += () =>
                {
                    Debug.Log("Rewarded ad was clicked.");
                };
                // Raised when an ad opened full screen content.
                ad.OnAdFullScreenContentOpened += () =>
                {
                    Debug.Log("Rewarded ad full screen content opened.");
                };
                // Raised when the ad closed full screen content.
                ad.OnAdFullScreenContentClosed += () =>
                {
                       
                    Debug.Log("Rewarded ad full screen content closed.");
                    InvokeAdClosedEvent();
                };
                // Raised when the ad failed to open full screen content.
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Debug.LogError("Rewarded ad failed to open full screen content " +
                                    "with error : " + error);
                    InvokeAdClosedEvent();
                };

                rewardedAd.Show((reward) =>
                {
                    InvokeRewardGrantedEvent();
                });
            }
            
        }

       

        public void Dispose()
        {
            this.identifier = null;
            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
                rewardedAd = null;
            }


            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
                interstitialAd = null;
            }
        }


        private void InvokeAdClosedEvent()
        {
            onAdClosed?.Invoke();
        }

        private void InvokeRewardGrantedEvent()
        {
            onRewardGranted?.Invoke();
        }

        private void InvokeLoadErrorEvent()
        {
            onAdFailedToLoad?.Invoke();
        }

        private void InvokeAdLoadedEvent()
        {
            onAdLoaded?.Invoke();
        }

       
    }
}

