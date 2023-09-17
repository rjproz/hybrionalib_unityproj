/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  TweenAnimData.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  17-09-2023 02:32:50

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class TweenAnimData  
	{
		public float timeLength;
		public bool loop;
		public bool timeScaleIndependent;
		internal System.Action returnToPoolCallback;
		private float timeTracker = 0;
		private bool paused;
		public void Reset()
        {
			timeTracker = 0;
		}

		public void Pause()
        {
			paused = true;

		}

		public void Resume()
        {
			paused = false;
        }

		public bool Update()
		{
			if(paused)
            {
				return false;
            }

			bool animCompleted = false;
			timeTracker += Time.unscaledDeltaTime * (timeScaleIndependent ? 1 : Time.timeScale);
			float tn = timeTracker / timeLength;
			if(tn >= 1)
            {
				if (!loop)
				{
					//completed
					animCompleted = true;
					tn = 1;
				}
				else
                {
					//patch timeTracker
					timeTracker = timeTracker % timeLength;
				}
			}
			UpdateValue(tn);

			return animCompleted;

		}

		


		public virtual void UpdateValue(float timeNormalized)
		{
			
        }
		

        internal void ReturnToPool()
        {
			returnToPoolCallback();

		}
    }
}
