/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  HybAnalyticsUser.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  13-10-2023 22:03:42

*************************************************************************/
using UnityEngine;

namespace Hybriona
{
	[System.Serializable]
	public class HybAnalyticsUser  
	{


		public string userId;
		public int sessionId;
		public HybAnalyticsAllPendingEvents pendingEvents = new HybAnalyticsAllPendingEvents();
		public const string HybrionaAnalyticsUserDataKey = "HybrionaAnalyticsUserData";

		public void Load()
        {
			string json = PlayerPrefs.GetString(HybrionaAnalyticsUserDataKey, "{ }");
			JsonUtility.FromJsonOverwrite(json, this);
			if(string.IsNullOrEmpty(this.userId))
            {
			
				userId = System.Guid.NewGuid().ToString();
				sessionId = 0;

			}
        }

		public void SetUserId(string userId)
        {
			this.userId = userId;
			Save();
        }

		public int GetNextSessionId()
        {
			sessionId = sessionId + 1;
			Save();
			return sessionId;
        }

		public void Save()
        {
			
			if(pendingEvents.eventsRaw.Count > 1000)
            {
				pendingEvents.eventsRaw.Clear();	
			}
			PlayerPrefs.SetString(HybrionaAnalyticsUserDataKey, JsonUtility.ToJson(this));
			PlayerPrefs.Save();

//#if LOG_HYBRIONA_ANALYTICS
//			Debug.LogFormat("Saving {0} with count {2} but got {1} ", JsonUtility.ToJson(this), PlayerPrefs.GetString(HybrionaAnalyticsUserDataKey), pendingEvents.eventsRaw.Count);
//#endif

		}

	}
}
