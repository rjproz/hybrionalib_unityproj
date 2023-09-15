/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  ProjectMetaData.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  06/21/2018 19:49:16

*************************************************************************/
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Hybriona
{
	public class ProjectMetaDataWindow : EditorWindow
	{
		
		[MenuItem("Hybriona/Project/MetaData")]
		public static void OpenProjectMetaUI()
		{
			ProjectMetaDataWindow window = EditorWindow.GetWindow<ProjectMetaDataWindow>(false,"Project Settings");
			window.maxSize = window.minSize = new Vector2(400,300);
			window.Show();
		}

		private void OnGUI()
		{
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(GUI.skin.box);
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			ProjectMetaData.Instance().company = EditorGUILayout.TextField("Company",ProjectMetaData.Instance().company);
			ProjectMetaData.Instance().productname = EditorGUILayout.TextField("ProductName",ProjectMetaData.Instance().productname);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Description");
			ProjectMetaData.Instance().description = EditorGUILayout.TextArea(ProjectMetaData.Instance().description);
			ProjectMetaData.Instance().scriptnamespace = EditorGUILayout.TextField("Namespace",ProjectMetaData.Instance().scriptnamespace);
			ProjectMetaData.Instance().Save();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();
		}
	}
}
#endif
