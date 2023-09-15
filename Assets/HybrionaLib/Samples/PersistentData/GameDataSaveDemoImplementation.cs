/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  GameDataSaveDemoImplementation.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  07/19/2018 20:33:07

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class GameDataSaveDemoImplementation : MonoBehaviour 
	{
		public AnimationCurve curve;
		public AnimationCurve savedcurve;
		void Start () 
		{
			GameDataSaveDemo.current = new GameDataSaveDemo();
			GameDataSaveDemo.current.LoadProfile("rjproz");

			if(!GameDataSaveDemo.current.HasTag("CHECK2"))
			{
				GameDataSaveDemo.current.curve = curve;
				GameDataSaveDemo.current.id = Random.Range(1000,9999);
				GameDataSaveDemo.current.recentLevelCompleted = 25;
				GameDataSaveDemo.current.data = "who are you?";
				GameDataSaveDemo.current.dictionary.Add("MODE","1");
				GameDataSaveDemo.current.dictionary.Add("DIFFICULTY","100");
				GameDataSaveDemo.current.dictionary.Add("MESSAGE","YOU ARE AWESOME");
				GameDataSaveDemo.current.dictionary.Add("TAG","FLOAT");
				GameDataSaveDemo.current.RemoveTag("CHECK");
				GameDataSaveDemo.current.AddTag("CHECK2");
				GameDataSaveDemo.current.Save();
			}
			else
			{
				savedcurve = GameDataSaveDemo.current.curve;
			}
			UnityEngine.Debug.Log(GameDataSaveDemo.current.lastWriteTime+","+GameDataSaveDemo.current.data +" , "+GameDataSaveDemo.current.dictionary.GetValue("MESSAGE"));
		}


	}
}
