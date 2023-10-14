/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  HybrionaAnalytics.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  13-10-2023 18:34:26

*************************************************************************/
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Hybriona
{
	public class HybrionaAnalytics : MonoBehaviour
	{


        public const string REPORT_URL = "https://vps.hybriona.com/api/analytics/report";

        public bool isInitialized { get; private set; }
        public string userId { get; private set; }
        public string sessionId { get; private set; }

        private string projectId;
        private string storeName;

        private HybAnalyticsUser hybAnalyticsUser;
        private HybAnalyticsEventData eventData = null;


  

        private HybAnalyticsAllPendingEvents pendingEvents = new HybAnalyticsAllPendingEvents();
        private HybAnalyticsAllPendingEvents tempPendingEvents = new HybAnalyticsAllPendingEvents();


        private bool forcedFlush;
        static object accessLock = new object();

        public void Initialize(string projectId,string storeName = null)
        {
            if(isInitialized)
            {
                return;
            }
            this.projectId = projectId;
            this.storeName = storeName;

            hybAnalyticsUser = new HybAnalyticsUser();
            hybAnalyticsUser.Load();

            eventData = new HybAnalyticsEventData();
           


            eventData.environment = (Application.isEditor || Debug.isDebugBuild) ? 0 : 1;
            eventData.project_id = this.projectId;
            eventData.bundle_id = Application.identifier;

            eventData.client_version = Application.version;

            eventData.event_name = "";
            eventData.event_data = "{ }";

            eventData.platform = PlatformToInt();
            eventData.country = System.Globalization.RegionInfo.CurrentRegion.ThreeLetterISORegionName;

            eventData.user_id = userId = hybAnalyticsUser.userId;
            eventData.session_id = hybAnalyticsUser.GetNextSessionId() ;

            if (string.IsNullOrEmpty(storeName))
            {
#if UNITY_IOS
                storeName = "Appstore";
#elif UNITY_ANDROID
                storeName = "GooglePlay";
#elif UNITY_WEBGL
                storeName = Application.absoluteURL.Replace("https://", "").Replace("http://", "").Split('/')[0];
#else
                storeName = "None";
#endif
            }

            eventData.store_name = this.storeName;
            isInitialized = true;

            ReportCustomEvent("newPlayer");

            StartCoroutine(AutoFlushEvents());
        }

        public void Reset()
        {
            PlayerPrefs.DeleteKey(HybAnalyticsUser.HybrionaAnalyticsUserDataKey);
            PlayerPrefs.Save();
        }

        IEnumerator AutoFlushEvents()
        {
            float timer = 0;
            while (true)
            {
                forcedFlush = false;
                timer = 0;
                if (pendingEvents.eventsRaw.Count > 0)
                {
                    string alleventDataString = null;
                    lock (accessLock)
                    {
                        alleventDataString = pendingEvents.ToJSON();
                        pendingEvents.eventsRaw.Clear();
                    }
                    using (var request = new UnityWebRequest(REPORT_URL, UnityWebRequest.kHttpVerbPOST))
                    {
                        //int byteCount = Zip.Compress(alleventDataString).Length;
                        //Debug.LogFormat("Compressed from {0} to {1}", alleventDataString.Length, byteCount);
                        var bytes = System.Text.Encoding.UTF8.GetBytes(alleventDataString);
                        request.uploadHandler = new UploadHandlerRaw(bytes);
                        request.downloadHandler = new DownloadHandlerBuffer();

                        request.SetRequestHeader("Content-Type", "application/json");

                        yield return request.SendWebRequest();


                        if (request.responseCode == 201)
                        {
                            //success
#if UNITY_EDITOR
                            Debug.LogFormat("Analytics Reported {0} ",alleventDataString);
#endif
                        }
                        else
                        {
                            //retry again
                            Debug.LogErrorFormat("Analytics Failed {0} ", alleventDataString);
                            lock (accessLock)
                            {
                                JsonUtility.FromJsonOverwrite(alleventDataString, tempPendingEvents);
                                pendingEvents.eventsRaw.AddRange(tempPendingEvents.eventsRaw);
                                tempPendingEvents.eventsRaw.Clear();

                            }
                        }
                    }
                }

                while (timer < 30 && !forcedFlush)
                {
                    timer += Time.fixedUnscaledDeltaTime;
                    yield return null;
                }
            }
        }




        public void ReportCustomEvent(string eventName)
        {
            ReportCustomEvent(eventName,"{ }");
        }

        public void ReportCustomEvent(string eventName,string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
            {
                jsonData = "{ }";
            }

          
            lock (accessLock)
            {
                eventData.event_id = "Evt-"+System.Guid.NewGuid();
                eventData.timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss");
                eventData.event_name = eventName;
                eventData.event_data = jsonData;
                string eventDataString = eventData.ToJSON();
                pendingEvents.eventsRaw.Add(eventDataString);
            }
        }


        public void Flush()
        {
            forcedFlush = true;
        }



        private int PlatformToInt()
        {
#if UNITY_IOS
            return 1;
#elif UNITY_ANDROID
            return 2;
#elif UNITY_WEBGL
            return 3;
#elif UNITY_STANDALONE_WIN
            return 4;
#elif UNITY_STANDALONE_OSX
            return 5;
#elif UNITY_STANDALONE_LINUX
            return 6;
#endif
            return 0;
        }


      


       
        /// <summary>
        /// 
        /// </summary>
        private static HybrionaAnalytics m_Instance;
		public static HybrionaAnalytics Instance
        {
			get
            {
				if(m_Instance == null)
                {
					m_Instance = GameObject.FindObjectOfType<HybrionaAnalytics>();
					if(m_Instance == null)
                    {
						m_Instance = new GameObject("HybrionaAnalytics").AddComponent<HybrionaAnalytics>();
						DontDestroyOnLoad(m_Instance.gameObject);
                    }
                }
				return m_Instance;
            }
        }

        private void Awake()
        {
            if(m_Instance != null && m_Instance != this)
            {
                Destroy(gameObject);
            }
        }
        
		
	}
}
