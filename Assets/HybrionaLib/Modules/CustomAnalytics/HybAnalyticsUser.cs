/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  HybAnalyticsUser.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  13-10-2023 22:03:42

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
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
			if(string.IsNullOrEmpty(userId))
            {
				userId = System.Guid.NewGuid().ToString();
				sessionId = 0;

			}
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

#if LOG_HYBRIONA_ANALYTICS
			Debug.Log("Saving " + PlayerPrefs.GetString(HybrionaAnalyticsUserDataKey));
#endif
			PlayerPrefs.Save();
		}

	}
}
