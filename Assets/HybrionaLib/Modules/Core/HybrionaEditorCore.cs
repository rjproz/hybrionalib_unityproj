#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Hybriona
{
	public class HybrionaEditorCore : EditorWindow 
	{
		[MenuItem("Hybriona/Common/Open Persistent Path")]
		public static void OpenPersistentDataPath()
		{
			EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
		}

		[MenuItem("Assets/Create/Text File", false, 70)]
		public static void CreateTextFile()
		{
			string path = GetSelectedPathOrFallback();
			string fullpathTemplate = path + "/" + "New Text {0}.txt";
			string fullpath = "";
			for (int i = 0; ; i++)
			{
				fullpath = string.Format(fullpathTemplate, i);
				if (!File.Exists(fullpath))
				{
					break;
				}
			}
			System.IO.File.WriteAllText(fullpath, "");
			AssetDatabase.Refresh();
			Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(fullpath);
		}

		[MenuItem("Hybriona/Common/Clean C# Meta Files (Intellisense Fix)")]
		public static void CleanCSharpMetaFiles()
		{
			string fullpath = System.IO.Path.GetFullPath(".");
			List<string> files = new List<string>();
			string[] l = System.IO.Directory.GetFiles(fullpath, "*.sln", System.IO.SearchOption.TopDirectoryOnly);
			files.AddRange(l);
			l = System.IO.Directory.GetFiles(fullpath, "*.csproj", System.IO.SearchOption.TopDirectoryOnly);
			files.AddRange(l);
			for (int i = 0; i < files.Count; i++)
			{
				System.IO.File.Delete(files[i]);
			}

		}

		public static string GetSelectedPathOrFallback()
		{
			string path = "Assets";

			foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
			{
				path = AssetDatabase.GetAssetPath(obj);
				if (!string.IsNullOrEmpty(path) && File.Exists(path))
				{
					path = Path.GetDirectoryName(path);
					break;
				}
			}
			return path;
		}
	}
}
#endif
