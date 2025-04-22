/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  LibActivatorAdminExtension.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  16-09-2023 15:41:41

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditorInternal;
using SimpleJSON;

namespace Hybriona
{
	[InitializeOnLoad]
	public static class LibActivatorAdminExtension 
	{
		static LibActivatorAdminExtension()
        {
			EditorApplication.update += Update;
		}

        static void Update()
        {
			HybrionaUnityLibActivator.adminUIExtension = ExtendedUI;
		}

        static string[] deleteFileFolders = new string[]
		{
			"LibExport/Samples.meta"
		};

		public static void ExtendedUI(ModulesData modulesData)
		{
			Debug.Log("ExtendedUI");
			GUILayout.Label("Admin Tools", EditorStyles.boldLabel);
			if (GUILayout.Button("Export Library"))
			{
				VersionManagerEditor.IncrementPatchVersion();
				string exportVersionVal = VersionManager.Version().ToString();

				if (Directory.Exists("LibExport"))
				{
					Directory.Delete("LibExport", true);
				}

				HybrionaUnityLibActivator.CopyFilesRecursively("Assets/HybrionaLib", "LibExport");

				


				for (int i = 0; i < deleteFileFolders.Length; i++)
				{
					if (File.Exists(deleteFileFolders[i]))
					{
						File.Delete(deleteFileFolders[i]);
					}
					if (Directory.Exists(deleteFileFolders[i]))
					{
						Directory.Delete(deleteFileFolders[i], true);
					}
				}



				Directory.Move("LibExport/Samples", "LibExport/Samples~");


				{
					//patch assemblies
					for (int i=0;i<modulesData.modules.Count;i++)
                    {
						var moduleData = modulesData.modules[i];
						string assemblyPath = Path.Combine("LibExport", moduleData.root);
						assemblyPath = Path.Combine(assemblyPath, moduleData.id + ".asmdef");
						if (File.Exists(assemblyPath))
						{
							var assemblyJsonData = JSON.Parse(File.ReadAllText(assemblyPath));
							assemblyJsonData["versionDefines"] = new JSONArray();
							if (moduleData.alwaysEnabled)
							{
								assemblyJsonData["versionDefines"].Add(JSON.Parse("{\n}"));
								assemblyJsonData["versionDefines"][0]["name"] = "Unity";

								assemblyJsonData["versionDefines"][0]["define"] = "ENABLE_LIB";

							}
							

							File.WriteAllText(assemblyPath, assemblyJsonData.ToString());
						}
                    }
                }

				//Patch Package.json
				string packageJsonPath = "LibExport/package.json";
				var packageJsonNode = JSON.Parse(File.ReadAllText(packageJsonPath));
				packageJsonNode["version"] = VersionManager.Version().ToString();
				File.WriteAllText(packageJsonPath, packageJsonNode.ToString());


				//Export meta data for download

				string packageTarballFolder = "PackageDownload";
				if (Directory.Exists(packageTarballFolder))
				{
					Directory.Delete(packageTarballFolder,true);
				}
				Directory.CreateDirectory(packageTarballFolder);

				string packageMetaPath = Path.Combine(packageTarballFolder, "package.json");
				System.IO.File.WriteAllText(packageMetaPath, packageJsonNode.ToString());


				
				var packRequest = UnityEditor.PackageManager.Client.Pack("LibExport", packageTarballFolder);
				while(!packRequest.IsCompleted)
				{

				}

				if(packRequest.Status != UnityEditor.PackageManager.StatusCode.Success)
                {
					EditorUtility.DisplayDialog("Export Error", "Package failed to export "+packRequest.Error.message, "Ok");
					return;

				}
				Directory.Delete("LibExport", true);

				File.Move(packRequest.Result.tarballPath,Path.Combine(packageTarballFolder, packageJsonNode["name"].Value+ ".tgz"));


				EditorUtility.DisplayDialog("Alert!", "Package exported with version " + exportVersionVal, "Ok");


			}
		}


		

	}
}
