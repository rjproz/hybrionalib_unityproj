/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  CleanCsharpFiles.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  06/27/2018 01:13:30

*************************************************************************/



#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Hybriona
{
	public class CleanCsharpFiles : EditorWindow 
	{
		
		[MenuItem("Hybriona/Common/Clean C# Meta Files (Intellisense Fix)")]
		public static void CleanCSharpMetaFiles()
		{
			string fullpath = System.IO.Path.GetFullPath(".");
			List<string> files = new List<string>();
			string [] l = System.IO.Directory.GetFiles(fullpath,"*.sln",System.IO.SearchOption.TopDirectoryOnly);
			files.AddRange(l);
			l = System.IO.Directory.GetFiles(fullpath,"*.csproj",System.IO.SearchOption.TopDirectoryOnly);
			files.AddRange(l);
			for(int i=0;i<files.Count;i++)
			{
				System.IO.File.Delete(files[i]);
			}

		}

	}
}
#endif
