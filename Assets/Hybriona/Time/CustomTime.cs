/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  CustomTime.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  07/19/2018 16:50:55

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class CustomTime : HybSingleton<CustomTime> 
	{
		public float timeScale {get;set;}

		public float timeStartedSession {get;private set;}
		public float timeStartedActual {get; private set;}

		public float time {get;private set;}

		public void Initialize(int secondsOffset = 0)
		{
			timeStartedSession = Time.fixedTime;
			timeStartedActual= timeStartedSession = secondsOffset;
			timeScale = 1;
		}

		public void UpdateTime(float delta)
		{
			time += delta * timeScale;
		}
	}
}
