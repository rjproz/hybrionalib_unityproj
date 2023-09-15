#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using XORHEAD.CodeTemplates;

namespace Hybriona
{
	public class AssetCreateExtension : EditorWindow
	{


		[MenuItem("Assets/Create/C# Script (Namespace)",false,70)]
		public static void CreateCSharpFileWithNameSpace(MenuCommand cmd)
		{
			if(!ProjectMetaData.Instance().HasValidNameSpace())
			{
				EditorUtility.DisplayDialog("Error!","Please set valid namespace first","Ok");
				return;
			}
			string path = GetSelectedPathOrFallback();
			string fullpathTemplate = path+"/"+"NewBehaviourScript_{0}.cs";
			string fullpath = "";
			for(int i=0;;i++)
			{
				if(i == 0)
				{
					fullpath = fullpathTemplate.Replace("_{0}","");
				}
				else
				{
					fullpath = string.Format(fullpathTemplate,i);
				}
				if(!File.Exists(fullpath))
				{
					break;
				}
			}
			string templatepath = "Assets/Hybriona/EditorTools/AssetCreateExtension/Templates/csharp_namespace.txt";
			CodeTemplates.CreateFromTemplate(
				fullpath, 
				templatepath);
		
		}

		[MenuItem("Assets/Create/C# Script (Namespace) No MetaData",false,70)]
		public static void CreateCSharpFileWithNameSpaceNoMeta(MenuCommand cmd)
		{
			if(!ProjectMetaData.Instance().HasValidNameSpace())
			{
				EditorUtility.DisplayDialog("Error!","Please set valid namespace first","Ok");
				return;
			}
			string path = GetSelectedPathOrFallback();
			string fullpathTemplate = path+"/"+"NewBehaviourScript_{0}.cs";
			string fullpath = "";
			for(int i=0;;i++)
			{
				if(i == 0)
				{
					fullpath = fullpathTemplate.Replace("_{0}","");
				}
				else
				{
					fullpath = string.Format(fullpathTemplate,i);
				}
				if(!File.Exists(fullpath))
				{
					break;
				}
			}
			string templatepath = "Assets/Hybriona/EditorTools/AssetCreateExtension/Templates/csharp_namespace_nometa.txt";
			CodeTemplates.CreateFromTemplate(
				fullpath, 
				templatepath);


		}



		[MenuItem("Assets/Create/Text File",false,70)]
		public static void CreateTextFile()
		{
			string path = GetSelectedPathOrFallback();
			string fullpathTemplate = path+"/"+"New Text {0}.txt";
			string fullpath = "";
			for(int i=0;;i++)
			{
				fullpath = string.Format(fullpathTemplate,i);
				if(!File.Exists(fullpath))
				{
					break;
				}
			}
			System.IO.File.WriteAllText(fullpath,"");
			AssetDatabase.Refresh();
			Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(fullpath);
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
