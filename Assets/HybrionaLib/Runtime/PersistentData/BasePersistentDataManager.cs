/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  BaseGameSave.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  07/19/2018 12:22:07

*************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Assets.SimpleZip;

namespace Hybriona
{
	[System.Serializable]
	public class BasePersistentDataManager<T> : PersistentDataManagerTemplate where T : PersistentDataManagerTemplate
	{
		#region Default Parameters & Methods
		public string lastWriteTime;
		public string version;
		public PersistentDictionary<string> dictionary = new PersistentDictionary<string>();


		// Tag Check Starts here
		[SerializeField]
		private List<string> tags = new List<string>();
		public bool HasTag(string tag)
		{
			return tags.Contains(tag);
		}
		public void AddTag(string tag)
		{
			if(!HasTag(tag))
			{
				tags.Add(tag);
			}
		}
		public void RemoveTag(string tag)
		{
			if(HasTag(tag))
			{
				tags.Remove(tag);	
			}
		}

		#endregion
		//

		public string profilename {get;private set;}
		public string saveFilePath {get; private set;}

		//Static Methods & Params
		public static T current {get;set;}


		//Public Methods
		public override void LoadProfile(string profileName)
		{
			profilename = profileName;
			CreateGameSaveDirectory();
			PrepareWritePath();

			if(File.Exists(saveFilePath))
			{
				try
				{
					JsonUtility.FromJsonOverwrite( Zip.Decompress(Decyrpt(File.ReadAllBytes(saveFilePath))) ,this);
				}
				catch
				{
					version = VersionManager.Version().ToString();
				}
			}
			else
			{
				version = VersionManager.Version().ToString();
			}
		}


		public virtual byte [] ToBytesEncrypted()
		{
			return Encyrpt(Zip.Compress( JsonUtility.ToJson(this)) );
		}

		public virtual void Save()
		{
			CreateGameSaveDirectory();
			lastWriteTime = System.DateTime.UtcNow.ToString();
			File.WriteAllBytes( saveFilePath , ToBytesEncrypted());

#if UNITY_EDITOR
            File.WriteAllText(saveFilePath+".txt", JsonUtility.ToJson(this));
#endif

        }


		private void PrepareWritePath()
		{
			saveFilePath = PersistentDataConstants.saveFileDirectory + string.Format(PersistentDataConstants.saveFileFormat,profilename);
		}
		private void CreateGameSaveDirectory()
		{
			PersistentDataConstants.saveFileDirectory = Application.persistentDataPath + "/GameSave02142010/";
			if(!Directory.Exists(PersistentDataConstants.saveFileDirectory))
			{
				Directory.CreateDirectory(PersistentDataConstants.saveFileDirectory);
			}
		}

		public virtual byte [] Encyrpt(byte [] data)
		{
			return Reverse(data);
		}
		public virtual byte [] Decyrpt(byte [] data)
		{
			return Reverse(data);
		}

		private byte [] Reverse(byte [] data)
		{
			byte [] temp = new byte[data.Length];
			int count = 0;
			for(int i=temp.Length-1;i>=0;i--)
			{
				temp[count] = data[i];
				count++;
			}
			data = null;
			return temp;
		}


	}



}
