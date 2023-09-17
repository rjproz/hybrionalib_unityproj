
#if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;
namespace Hybriona
{
	public class VersionManagerEditor : EditorWindow {

		[MenuItem("Hybriona/Project/Version Manager/Reset")]
		public static void ResetVersion()
		{
			AppVersion version = VersionManager.Version();
			version.major = 0;
			version.minor = 1;
			version.patch = 100;
			version.build_number = 0;
			version.description = "";
			Save(version);
		}

		[MenuItem("Hybriona/Project/Version Manager/Increment Major")]
		public static void IncrementMajorVersion()
		{
			AppVersion version = VersionManager.Version();
			version.major++;
            version.minor = 0;
            version.patch = 0;
			version.build_number++;
			Save(version);
		}


		[MenuItem("Hybriona/Project/Version Manager/Increment Minor")]
		public static void IncrementMinorVersion()
		{
			AppVersion version = VersionManager.Version();
			version.minor++;
            version.patch = 0;
            version.build_number++;
			Save(version);
		}

		[MenuItem("Hybriona/Project/Version Manager/Increment Patch")]
		public static void IncrementPatchVersion()
		{
			AppVersion version = VersionManager.Version();
			version.patch++;
			version.build_number++;
			Save(version);
		}

        [MenuItem("Hybriona/Project/Version Manager/Increment Build Number Only")]
        public static void IncrementBuildNumberOnly()
        {
            AppVersion version = VersionManager.Version();        
            version.build_number++;
            Save(version);
        }

        private static void Save(AppVersion appversion)
		{

			PlayerSettings.bundleVersion = appversion.ToString();
			PlayerSettings.macOS.buildNumber = appversion.build_number.ToString();
			
			PlayerSettings.Android.bundleVersionCode = appversion.build_number;
			PlayerSettings.iOS.buildNumber =  appversion.build_number.ToString();
		
			//string path = "Assets/Hybriona/VersionManager/Resources/app_version.txt";
			string directory = "Assets/HybrionaGenerated/VersionManager/Resources/";
			if(!System.IO.Directory.Exists(directory))
			{
				System.IO.Directory.CreateDirectory(directory);
			}
			System.IO.File.WriteAllText(directory + "app_version.txt", JsonUtility.ToJson(appversion));
			AssetDatabase.Refresh();
		}
	}
}
#endif