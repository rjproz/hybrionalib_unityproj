using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hybriona
{
	
	[System.Serializable]
	public class AppVersion 
	{
		public int major;
		public int minor;
		public int patch;
		public int build_number;
		public string description;
		private string version_string = null;


		public override string ToString ()
		{
			if(version_string == null || !Application.isPlaying)
			{
				version_string = major+"."+minor+"."+patch;
			}
			return version_string;
		}
	}
}

