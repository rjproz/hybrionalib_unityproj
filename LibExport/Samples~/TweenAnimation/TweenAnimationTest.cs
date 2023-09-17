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



			//check speed of direct function invoke
			var timeStarted = System.DateTime.Now;
			for(int i =0;i< 1000000; i++)
            {
				var cond = 3;
				float val = -1;
				switch(cond)
                {
					case 1:
						val = easeInSine(.5f);
						break;
					case 2:
						val = easeInSine(.5f);
						break;
					case 3:
						val = easeInSine(.5f);
						break;
					case 4:
						val = easeInSine(.5f);
						break;
					case 5:
						val = easeInSine(.5f);
						break;

				}
				
            }

			Debug.LogFormat("Direct function tooks {0} seconds",(System.DateTime.Now - timeStarted).TotalSeconds);

			System.GC.Collect();

			//check speed of direct function invoke
			System.Func<float,float> runFuction = null;

			runFuction = easeInSine;
			timeStarted = System.DateTime.Now;
			for (int i = 0; i < 1000000; i++)
			{
				float val = runFuction(.5f);
			}

			Debug.LogFormat("Function reference function tooks {0} seconds", (System.DateTime.Now - timeStarted).TotalSeconds);
		}





		public float easeInSine(float x)
		{
			return 1 - Mathf.Cos((x * Mathf.PI) / 2);

		}



	}
}
