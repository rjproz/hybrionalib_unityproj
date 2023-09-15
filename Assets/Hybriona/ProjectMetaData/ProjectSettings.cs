#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Hybriona
{
	[System.Serializable]
	public class ProjectMetaData
	{
		public string company;
		public string productname;
		public string description;
		public string scriptnamespace;

		private static ProjectMetaData instance;
		private static string savepath = ".ProjectMetaData";
		public static ProjectMetaData Instance()
		{
			if(instance == null)
			{
				if(System.IO.File.Exists(savepath))
				{
					
					instance = JsonUtility.FromJson<ProjectMetaData>( System.IO.File.ReadAllText(savepath) );
				}
				else
				{
					instance = new ProjectMetaData();
				}
			}
			return instance;
		}

		public void Save()
		{
			System.IO.File.WriteAllText(savepath,JsonUtility.ToJson(this));

		}

		public bool HasValidNameSpace()
		{
			if(!string.IsNullOrEmpty(scriptnamespace) && !scriptnamespace.Contains(" "))
			{
				return true;
			}
			return false;
		}
	}
}
#endif
