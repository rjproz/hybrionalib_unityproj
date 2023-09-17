/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  TweenAnimData.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  17-09-2023 02:32:50

*************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class TweenAnimData  
	{
		public float timeLength;
		public bool loop;
		public System.Func<float,float> easingCurveFunc;
		public float speed = 1;
		public bool timeScaleIndependent;
		public bool animationStopped { get; private set; }
		internal System.Action returnToPoolCallback;
		private float timeTracker = 0;
		private bool paused;
        internal ulong id;
		internal TweenAnimHandler assignedHandler;

		public void Reset()
        {
			paused = false;
			animationStopped = false;
			timeTracker = 0;
		}


		public bool IsPlaying()
        {
			return !paused;
        }
		public void Pause()
        {
			paused = true;

		}

        public void Stop()
        {
			animationStopped = true;
        }

        public void Resume()
        {
			paused = false;
        }

		public bool Update()
		{
			if(animationStopped)
            {
				return true;
            }

			if(paused)
            {
				return false;
            }

			bool animCompleted = false;
			timeTracker += Time.unscaledDeltaTime * speed * (timeScaleIndependent ? 1 : Time.timeScale);
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
			UpdateValue(easingCurveFunc(tn));

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
