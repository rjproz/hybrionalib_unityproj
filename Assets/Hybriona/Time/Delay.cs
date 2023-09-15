using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hybriona
{
	public class Delay : HybSingleton<Delay> 
	{


		void Start () 
		{
			DontDestroyOnLoad(gameObject);
		}
		


		private Dictionary<int,WaitForSeconds> waitForSecondsData = new Dictionary<int, WaitForSeconds>();

		public WaitForSeconds GetWait(int seconds)
		{
			if(waitForSecondsData.ContainsKey(seconds))
			{
				return waitForSecondsData[seconds];
			}
			WaitForSeconds wait = new WaitForSeconds(seconds);
			waitForSecondsData.Add(seconds,wait);
			return wait;
		}
	}
}
