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
			"LibExport/AdminData","LibExport/AdminDat.meta",
			//"LibExport/Modules/HybrionaLibAdmin.meta"
		};

		public static void ExtendedUI(ModulesData modulesData)
		{
			GUILayout.Label("Admin Tools", EditorStyles.boldLabel);
			if (GUILayout.Button("Export Library"))
			{
				string exportVersionVal = VersionManager.Version().ToString();

				if (Directory.Exists("LibExport"))
				{
					Directory.Delete("LibExport", true);
				}

				HybrionaUnityLibActivator.CopyFilesRecursively("Assets/HybrionaLib", "LibExport");

				{
					//move
					File.Move("LibExport/AdminData/DonotCompileSamples.asmdef", "LibExport/Samples/DonotCompileSamples.asmdef");
					File.Move("LibExport/AdminData/DonotCompileSamples.asmdef.meta", "LibExport/Samples/DonotCompileSamples.asmdef.meta");
				}


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
                {
					string packageJsonPath = "LibExport/package.json";
					var packageJsonNode = JSON.Parse(File.ReadAllText( packageJsonPath) );

                    {
						packageJsonNode["version"] = VersionManager.Version().ToString();
					}


					
					File.WriteAllText(packageJsonPath, packageJsonNode.ToString());
					
					EditorUtility.DisplayDialog("Alert!", "Package exported with version " + packageJsonNode["version"].Value, "Ok");
				}

				
			}
		}


		

	}
}
