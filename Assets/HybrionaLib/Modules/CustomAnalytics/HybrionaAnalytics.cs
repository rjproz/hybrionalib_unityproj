﻿/*************************************************************************
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


namespace Hybriona
{
	public sealed partial class HybrionaAnalytics : MonoBehaviour
	{


        public const string REPORT_URL = "https://vps.hybriona.com/api/hybriona-services/analytics/report";

        public bool isInitialized { get; private set; }
        public bool isDataCollectionEnabled { get; private set; }
        public string userId { get; private set; }
        public string sessionId { get; private set; }

        public string projectId { get; private set; }
        public string storeName { get; private set; }

        private HybAnalyticsUser hybAnalyticsUser;
        private HybAnalyticsEventData eventData = null;


  

       
        private HybAnalyticsAllPendingEvents tempPendingEvents = new HybAnalyticsAllPendingEvents();


        private bool forcedFlush;
        static object accessLock = new object();


        [System.ObsoleteAttribute("Use Init(,) instead and call StartDataCollection() to start!")]
        public void Initialize(string projectId, string storeName = null)
        {
            Init(projectId, storeName);
            StartDataCollection();
        }

        
        public void Init(string projectId,string storeName = null)
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

            if (string.IsNullOrEmpty(this.storeName))
            {
#if UNITY_IOS
                this.storeName = "Appstore";
#elif UNITY_ANDROID
                this.storeName = "GooglePlay";
#elif UNITY_WEBGL
                this.storeName = Application.absoluteURL.Replace("https://", "").Replace("http://", "").Split('/')[0];
#else
                this.storeName = "None";
#endif
            }

            eventData.store_name = this.storeName;
            isInitialized = true;

            
        }

        public void StartDataCollection()
        {
            if (!isDataCollectionEnabled)
            {
                isDataCollectionEnabled = true;
                ReportCustomEvent("newPlayer");
                StartCoroutine(AutoFlushEvents());
            }

        }

        public void SetUserId(string userId)
        {
            hybAnalyticsUser.SetUserId(userId);
            eventData.user_id = userId = hybAnalyticsUser.userId;
        }


       

        public void ReportCustomEvent(string eventName)
        {
            ReportCustomEvent(eventName, "{ }");
        }

        public void ReportCustomEvent(string eventName, string jsonData)
        {
            if (!isDataCollectionEnabled)
                return;

            if (string.IsNullOrEmpty(jsonData))
            {
                jsonData = "{ }";
            }


            lock (accessLock)
            {
                eventData.event_id = "Evt-" + System.Guid.NewGuid();
                eventData.timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                eventData.event_name = eventName;
                eventData.event_data = jsonData;
                string eventDataString = eventData.ToJSON();
                hybAnalyticsUser.pendingEvents.eventsRaw.Add(eventDataString);
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    hybAnalyticsUser.Save();
                }
            }
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
                if (hybAnalyticsUser.pendingEvents.eventsRaw.Count > 0 && Application.internetReachability != NetworkReachability.NotReachable)
                {
                    
                    string alleventDataString = null;
                    lock (accessLock)
                    {
                        alleventDataString = hybAnalyticsUser.pendingEvents.ToJSON();
                        hybAnalyticsUser.pendingEvents.eventsRaw.Clear();
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
#if UNITY_EDITOR && LOG_HYBRIONA_ANALYTICS
                            Debug.LogFormat("Analytics Reported {0} ",alleventDataString);
#endif
                        }
                        else
                        {
                            //retry again
#if UNITY_EDITOR && LOG_HYBRIONA_ANALYTICS
                            Debug.LogErrorFormat("Analytics Failed {0} ", alleventDataString);
#endif
                            lock (accessLock)
                            {
                                JsonUtility.FromJsonOverwrite(alleventDataString, tempPendingEvents);
     
                                hybAnalyticsUser.pendingEvents.eventsRaw.AddRange(tempPendingEvents.eventsRaw);
                                
                                tempPendingEvents.eventsRaw.Clear();

                            }
                        }
                        
                    }
                    hybAnalyticsUser.Save();
                }

                while (timer < 30 && !forcedFlush)
                {
                    timer += Time.fixedUnscaledDeltaTime;
                    yield return null;
                }
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
