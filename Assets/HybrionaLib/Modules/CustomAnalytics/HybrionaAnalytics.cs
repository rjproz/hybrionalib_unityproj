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

        public static bool isInitialized { get; private set; }
        public static bool isDataCollectionEnabled { get; private set; }
        public static string userId { get; private set; }
        public static string sessionId { get; private set; }

        public static string projectId { get; private set; }
        public static string storeName { get; private set; }

        private static HybAnalyticsUser hybAnalyticsUser;
        private static HybAnalyticsEventData eventData = null;
        private static HybAnalyticsEventData sessionLengthEventData = null;



        private static System.DateTime timeSessionStarted;
       
        
        private static HybAnalyticsAllPendingEvents tempPendingEvents = new HybAnalyticsAllPendingEvents();


        private static bool forcedFlush;
        static object accessLock = new object();
        private Coroutine sessionTimeReportingRoutine;

        [System.ObsoleteAttribute("Use Init(,) instead and call StartDataCollection() to start!")]
        public static void Initialize(string projectId, string storeName = null)
        {
            Init(projectId, storeName);
            StartDataCollection();
        }

        
        public static void Init(string projectId,string storeName = null)
        {
            if(isInitialized)
            {
                return;
            }
            var i = Instance;
            HybrionaAnalytics.projectId = projectId;
            HybrionaAnalytics.storeName = storeName;

            hybAnalyticsUser = new HybAnalyticsUser();
            hybAnalyticsUser.Load();

            eventData = new HybAnalyticsEventData();
           


            eventData.environment = (Application.isEditor || Debug.isDebugBuild) ? 0 : 1;
            eventData.project_id = HybrionaAnalytics.projectId;
            eventData.bundle_id = Application.identifier;

            eventData.client_version = Application.version;

            eventData.event_name = "";
            eventData.event_data = "{ }";

            eventData.platform = PlatformToInt();
            eventData.country = System.Globalization.RegionInfo.CurrentRegion.ThreeLetterISORegionName;

            eventData.user_id = userId = hybAnalyticsUser.userId;
            eventData.session_id = hybAnalyticsUser.GetNextSessionId() ;


            
            if (string.IsNullOrEmpty(HybrionaAnalytics.storeName))
            {
#if UNITY_IOS
                HybrionaAnalytics.storeName = "Appstore";
#elif UNITY_ANDROID
                HybrionaAnalytics.storeName = "GooglePlay";
#elif UNITY_WEBGL
                HybrionaAnalytics.storeName = Application.absoluteURL.Replace("https://", "").Replace("http://", "").Split('/')[0];
#else
                HybrionaAnalytics.storeName = "None";
#endif
            }

            eventData.store_name = HybrionaAnalytics.storeName;
            isInitialized = true;
            timeSessionStarted = System.DateTime.UtcNow;

            sessionLengthEventData = JsonUtility.FromJson<HybAnalyticsEventData>(eventData.ToJSON());
        }

        public static void StartDataCollection(bool enableSessionTimeReporting = false)
        {
            if (!isDataCollectionEnabled)
            {
                isDataCollectionEnabled = true;
                ReportCustomEvent("newPlayer");
                Instance.StartCoroutine(Instance.AutoFlushEvents());
                if(enableSessionTimeReporting)
                {
                    if (Instance.sessionTimeReportingRoutine != null)
                    {
                        Instance.StopCoroutine(Instance.sessionTimeReportingRoutine);
                    }
                    Instance.sessionTimeReportingRoutine = Instance.StartCoroutine(Instance.SessionReportingLoop());
                }
            }

        }

       

        public static void SetUserId(string userId)
        {
            hybAnalyticsUser.SetUserId(userId);
            eventData.user_id = userId = hybAnalyticsUser.userId;
        }


       

        public static void ReportCustomEvent(string eventName)
        {
            ReportCustomEvent(eventName, "{ }");
        }

        public static void ReportCustomEvent(string eventName, string jsonData)
        {
            if (!isDataCollectionEnabled)
                return;

            if (string.IsNullOrEmpty(jsonData))
            {
                jsonData = "{ }";
            }


            lock (accessLock)
            {
                eventData.event_id = "Evt-" + System.Guid.NewGuid() +"_"+ userId.GetHashCode();
                eventData.timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
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

        public static void Reset()
        {
            PlayerPrefs.DeleteKey(HybAnalyticsUser.HybrionaAnalyticsUserDataKey);
            PlayerPrefs.Save();
        }

        IEnumerator SessionReportingLoop()
        {
            WaitForSecondsRealtime wait = new WaitForSecondsRealtime(60);
            bool uploadFailed = false;
            while(true)
            {
                if (!uploadFailed)
                {
                    yield return wait;
                }
               
                var playTimeMins = (System.DateTime.UtcNow - timeSessionStarted).TotalMinutes.ToString("0.00");
                sessionLengthEventData.event_id = sessionLengthEventData.user_id + "_" + sessionLengthEventData.session_id;
                sessionLengthEventData.event_name = "playTime";
                sessionLengthEventData.event_data = "{\"t\":"+ playTimeMins + "}";
                sessionLengthEventData.timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                using (var request = new UnityWebRequest(REPORT_URL, UnityWebRequest.kHttpVerbPOST))
                {
                   
                    var bytes = System.Text.Encoding.UTF8.GetBytes(sessionLengthEventData.ToJSON());
                    request.uploadHandler = new UploadHandlerRaw(bytes);
                    //request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");                
                    yield return request.SendWebRequest();
#if UNITY_EDITOR && LOG_HYBRIONA_ANALYTICS
                    Debug.LogFormat("Analytics Reported {0} ", sessionLengthEventData.ToJSON());
#endif
                    if(request.responseCode == 201)
                    {
                        uploadFailed = false;
                    }
                    else
                    {
                        uploadFailed = true;
                    }
                }

            }
        }

        IEnumerator AutoFlushEvents()
        {
            
            while (true)
            {
                forcedFlush = false;
                bool failedToUpload = false;
                
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
                            failedToUpload = true;
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

                float waitTimer = 30;

                if(failedToUpload && Application.internetReachability != NetworkReachability.NotReachable)
                {
                    waitTimer = 1;
                }

                float timer = 0;
                while (timer < waitTimer && !forcedFlush)
                {
                    timer += Time.fixedUnscaledDeltaTime;
                    yield return null;
                }
            }
        }




       


        public static void Flush()
        {
            forcedFlush = true;
        }



        private static int PlatformToInt()
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
		private static HybrionaAnalytics Instance
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
