/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  HybAnalyticsEventData.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  13-10-2023 21:51:49

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	[System.Serializable]
	public class HybAnalyticsEventData  
	{

		public int environment;
		public string project_id;
		public string bundle_id;
		public string user_id;
		public string client_version;
		public int session_id;
		public string event_name;
		public string event_data;
		public int platform;
		public string store_name;
		public string country;


		public string ToJSON()
        {
			return JsonUtility.ToJson(this);
        }

	}
}
