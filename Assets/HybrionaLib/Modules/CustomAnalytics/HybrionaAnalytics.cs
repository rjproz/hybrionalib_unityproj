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
using System.Threading;

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

        private Queue<string> pendingEvents = new Queue<string>();
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
                while (pendingEvents.Count > 0)
                {
                    string eventDataString = null;
                    lock (accessLock)
                    {
                        eventDataString = pendingEvents.Dequeue();
                    }
                    using (var request = new UnityWebRequest(REPORT_URL, UnityWebRequest.kHttpVerbPOST))
                    {
                        var bytes = System.Text.Encoding.UTF8.GetBytes(eventData.ToJSON());
                        request.uploadHandler = new UploadHandlerRaw(bytes);
                        request.downloadHandler = new DownloadHandlerBuffer();

                        request.SetRequestHeader("Content-Type", "application/json");

                        yield return request.SendWebRequest();


                        if (request.responseCode < 400)
                        {
                            //success
#if UNITY_EDITOR
                            Debug.LogFormat("Analytics Reported {0} ",eventDataString);
#endif
                        }
                        else
                        {
                            //retry again
                            Debug.LogErrorFormat("Analytics Failed {0} ", eventDataString);
                            lock (accessLock)
                            {
                                pendingEvents.Enqueue(eventDataString);

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
                eventData.event_name = eventName;
                eventData.event_data = jsonData;
                string eventDataString = eventData.ToJSON();
                pendingEvents.Enqueue(eventDataString);
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
