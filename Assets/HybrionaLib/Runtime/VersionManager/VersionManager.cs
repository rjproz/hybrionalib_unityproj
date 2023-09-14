using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class VersionManager  
	{
		private static bool isVersionFetched = false;
		private static AppVersion version;
		public static AppVersion Version()
		{
			if(!isVersionFetched || !Application.isPlaying)
			{
				TextAsset textAsset = Resources.Load<TextAsset>("app_version");
				if(textAsset == null)
				{
					version = new AppVersion();
				}
				else
				{
					version = JsonUtility.FromJson<AppVersion>(textAsset.text);
				}
				isVersionFetched = true;
			}
			return version;

		}
	}
}
