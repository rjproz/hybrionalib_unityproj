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
	[ExecuteInEditMode()]
	public class LibActivatorAdminExtension : MonoBehaviour
	{
        private void Update()
        {
			HybrionaUnityLibActivator.adminUIExtension = ExtendedUI;

		}

        static string[] deleteFileFolders = new string[]
		{
			//"LibExport/Modules/HybrionaLibAdmin",
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

				CopyFilesRecursively("Assets/HybrionaLib", "LibExport");

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


				//Patch assembly with no active modules
				{
					//create main lib assembly
					string readPathOfMainLibAssembly = Path.Combine(modulesData.rootPath, "Modules/Hybriona.Lib.asmdef");
					string writePathOfMainLibAssembly =  "LibExport/Modules/Hybriona.Lib.asmdef";
					AssemblyDefinitionAsset mainLibAssembly = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(readPathOfMainLibAssembly);
					var jsonNode = JSON.Parse(mainLibAssembly.text);

					jsonNode["references"] = new JSONArray();
					jsonNode["references"].Add("hybriona.core");
					jsonNode["versionDefines"] = new JSONArray();


					File.WriteAllText(writePathOfMainLibAssembly, jsonNode.ToString());
				}
			}
		}


		private static void CopyFilesRecursively(string sourcePath, string targetPath)
		{
			//Now Create all of the directories
			foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
			{
				Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
			}

			//Copy all the files & Replaces any files with the same name
			foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
			{
				File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
			}
		}

	}
}
