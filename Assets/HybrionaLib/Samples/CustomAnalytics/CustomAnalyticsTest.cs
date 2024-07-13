/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  CustomAnalyticsTest.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  13-10-2023 19:27:10

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class CustomAnalyticsTest : MonoBehaviour 
	{
		void Start () 
		{
			HybrionaAnalytics.Instance.Init("hybriona-lib-project");
			HybrionaAnalytics.Instance.StartDataCollection();
			HybrionaAnalytics.Instance.EnableSessionTimeReporting();
			Debug.Log(HybrionaAnalytics.Instance.userId);
		}


	}
}
