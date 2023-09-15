/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  RateApp.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  10/03/2018 01:35:55

*************************************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace Hybriona
{
    public class RateApp : HybSingleton<RateApp>
    {

        [Header(" -- Rate App Data ---")]
        //public string appurl_android = "";
        public string iOSAppID = "";

        [Header(" -- UI Elements ---")]
        public UIVisibility uiGroup;
        public Button rateNowButton;
        public Button rateLaterButton;
        public Button rateNeverButton;

        public Vector2 m_initialDelayRange { get; private set; }
        public Vector2 m_laterDelayRange { get; private set; }

        public bool canShow
        {
            get
            {
                return !ratingData.isAlreadyRated && !ratingData.isNeverAsk;
            }
        }
        private RatingData ratingData;
    

        public void Prepare(Vector2 initialDelayRange, Vector2 laterDelayRange)
        {
            m_initialDelayRange = initialDelayRange;
            m_laterDelayRange = laterDelayRange;

            ratingData = RatingData.Create();

            rateNowButton.onClick.AddListener(OnRateNowClicked);
            rateLaterButton.onClick.AddListener(OnRateLaterClicked);
            rateNeverButton.onClick.AddListener(OnRateNeverClicked);

            if (canShow)
            {
                StartCoroutine(Timer());
            }

           
        }

        public void CheckAndShow()
        {
            if (canShow)
            {
                if (ratingData.time > ratingData.nextInitialDelayRange)
                {
                    if (ratingData.time - ratingData.lastTimeUserClickedLater > ratingData.nextLaterDelayRange)
                    {
                        TimeEvent.Instance.Add(5, Show) ;
                        ratingData.CreateNewDelayRange();
                        ratingData.Save();
                    }
                }
            }
        }

        public void Show()
        {
            uiGroup.Show();
        }

        public void Hide()
        {
            uiGroup.Hide();
        }


        public void OnRateNowClicked()
        {
            ratingData.isAlreadyRated = true;
            ratingData.Save();
            if (uiGroup.active)
            {
                Hide();
            }
 #if !UNITY_EDITOR
    #if UNITY_ANDROID
                Application.OpenURL("market://details?id=" + Application.identifier);
    #endif
    #if UNITY_IOS
                Application.OpenURL("itms-apps://itunes.apple.com/app/" + iOSAppID);
    #endif
#else
    #if UNITY_ANDROID
            Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
#endif
    #if UNITY_IOS
                Application.OpenURL("https://itunes.apple.com/us/app/id" + iOSAppID);
    #endif
#endif

        }
        public void OnRateLaterClicked()
        {
            ratingData.lastTimeUserClickedLater = ratingData.time;
            ratingData.Save();
            Hide();
        }

        public void OnRateNeverClicked()
        {
            ratingData.isNeverAsk = true;
            ratingData.Save();
            Hide();
        }


        [ContextMenu("Clear Data")]
        public void ClearData()
        {
            PlayerPrefs.DeleteKey("HybrionaRateApp");
        }

        private IEnumerator Timer()
        {
            WaitForSeconds wait5Seconds = new WaitForSeconds(5);
            while (true)
            {
                yield return wait5Seconds;
                ratingData.time += 5;
                ratingData.Save();
            }
        }

        private void Awake () 
		{
            if (uiGroup != null)
            {
                uiGroup.Hide(true);
            }

        }

      


    }

    [System.Serializable]
    public class RatingData
    {

        public float time;
        public float lastTimeUserClickedLater = -1;
        public bool isAlreadyRated;
        public bool isNeverAsk;
        public float nextInitialDelayRange;
        public float nextLaterDelayRange;

        public static RatingData Create()
        {

            if (PlayerPrefs.HasKey("HybrionaRateApp"))
            {
                RatingData ratingData = JsonUtility.FromJson<RatingData>(PlayerPrefs.GetString("HybrionaRateApp"));
                ratingData.CreateNewDelayRange();
                return ratingData;
            }
            else
            {
                return new RatingData();
            }
        }

        public void CreateNewDelayRange()
        {
            nextInitialDelayRange = Random.Range(RateApp.Instance.m_initialDelayRange.x, RateApp.Instance.m_initialDelayRange.y);
            nextLaterDelayRange = Random.Range(RateApp.Instance.m_laterDelayRange.x, RateApp.Instance.m_laterDelayRange.y);
        }

        public void Save()
        {
            PlayerPrefs.SetString("HybrionaRateApp", JsonUtility.ToJson(this));
            PlayerPrefs.Save();
        }
    }
}
