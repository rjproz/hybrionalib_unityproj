/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  GameSaveTest.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  07/19/2018 12:40:28

*************************************************************************/
using UnityEngine;

namespace Hybriona
{
	[System.Serializable]
	public class GameDataSaveDemo : BasePersistentDataManager<GameDataSaveDemo>
	{
		//Params
		public int id = 0;
		public string data = "";
		public int recentLevelCompleted;
		public AnimationCurve curve;
		public override void LoadProfile (string profileName)
		{
			base.LoadProfile (profileName);
		}
	}
}
