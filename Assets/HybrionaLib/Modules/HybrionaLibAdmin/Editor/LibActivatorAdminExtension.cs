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
namespace Hybriona
{
	public class LibActivatorAdminExtension
	{
		static string[] deleteFileFolders = new string[]
		{
			"LibExport/Modules/HybrionaLibAdmin"
		};

		public static void ExtendedUI(ModulesData modulesData)
		{
			GUILayout.Label("Admin Tools", EditorStyles.boldLabel);
			if (GUILayout.Button("Export Library"))
			{
				string exportVersionVal = VersionManager.Version().ToString();

				if(Directory.Exists("LibExport"))
                {
					Directory.Delete("LibExport",true);
                }

				CopyFilesRecursively("Assets/HybrionaLib", "LibExport");

				for(int i=0;i<deleteFileFolders.Length;i++)
                {
					if(File.Exists(deleteFileFolders[i]))
                    {
						File.Delete(deleteFileFolders[i]);
                    }
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
