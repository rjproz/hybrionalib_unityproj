using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona.Private
{
	[System.Serializable]
	public struct FeedbackForm 
	{

		public string user_deviceID;
		public string user_email;
		public string game_Name;
		public string game_Version;
		public string systeminfo;
		public string user_message;

		public void FillDefaults()
		{
			user_deviceID = SystemInfo.deviceUniqueIdentifier;


			#if TTI
			game_Name = "TTI";
			game_Version = Resources.Load<TextAsset>("GameBuildNumber").text;
			#else
			game_Version = "3.2."+Random.Range(100,999);
			#endif
			systeminfo = GetSystemInfo();
		}


		public string ToJson()
		{
			return JsonUtility.ToJson(this);
		}


		private static string _systemInfo = null;
		public static string GetSystemInfo()
		{
			if(_systemInfo == null)
			{
				_systemInfo = "";
				_systemInfo += "DeviceMode : "+SystemInfo.deviceModel +"\n";
				_systemInfo += "DeviceType : "+SystemInfo.deviceType +"\n";
				_systemInfo += "GraphicsDeviceType : "+SystemInfo.graphicsDeviceName +"\n";
				_systemInfo += "OS : "+SystemInfo.operatingSystem +"\n";
				_systemInfo += "System Memory : "+SystemInfo.systemMemorySize +"\n";
			}
			return _systemInfo;

		}
	}


}
