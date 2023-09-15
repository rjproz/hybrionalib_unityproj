/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  TransformPersistence.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  08/24/2018 11:54:42

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Hybriona
{
	public class TransformPersistence : MonoBehaviour 
	{
		public string directory;
		public string id;
		public float timeplayed;
		void Start () 
		{
			
		}


	}
	#if UNITY_EDITOR
	[CustomEditor(typeof(TransformPersistence))]
	public class TransformPersistenceEditor : Editor 
	{
		public override void OnInspectorGUI ()
		{
			TransformPersistence script = (TransformPersistence) target;
			script.directory = ".HybrionaCache/";
			if(string.IsNullOrEmpty(script.id))
			{
				script.id = script.gameObject.name +"_"+Random.Range(100000,999999)+Random.Range(100000,999999)+Random.Range(100000,999999)+Random.Range(100000,999999);
			}

			if(GUILayout.Button("Save Transfrom Hierarchy"))
			{
				
				if(!Directory.Exists(script.directory))
				{
					Directory.CreateDirectory(script.directory);
				}

				string data = (new TransformHierarchy(script.transform,false)).ToJson();
				System.IO.File.WriteAllText(script.directory+script.id,data);

			}


			if(GUILayout.Button("Apply Transfrom Hierarchy"))
			{
				string path = script.directory+script.id;
				if(Directory.Exists(script.directory) && File.Exists(path))
				{
					TransformHierarchy.FromJson(File.ReadAllText(path)).Apply(script.transform);
					File.Delete(path);
				}
			}



		}



	}
	#endif


}
