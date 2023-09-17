/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  TweenAnimationTest.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  17-09-2023 02:55:22

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class TweenAnimationTest : MonoBehaviour 
	{
		public float progress;
		void Start () 
		{
			TweenAnimation.Animate(0, 5, 5, (val) =>
			{
				progress = val;
			},true);


		
		}

		

	}
}
