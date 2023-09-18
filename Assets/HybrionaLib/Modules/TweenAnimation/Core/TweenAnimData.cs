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
	[System.Serializable]
	public class TweenAnimData  
	{
		[HideInInspector]
		public float timeLength;
		public TweenAnimationLoopMode loopMode;
		public System.Func<float,float> easingCurveFunc;
		public float speed { get; set; } = 1;
		public bool timeScaleIndependent;
		public bool animationStopped { get; private set; }

		internal System.Action returnToPoolCallback;
		[SerializeField]private float timeTracker = 0;
		private float timeLengthDoubled;
		private bool paused;
        internal ulong id;
		internal TweenAnimHandler assignedHandler;

		public void Reset()
        {
			timeLengthDoubled = timeLength * 2;
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

			float timeTrackerModified = timeTracker;
			if(timeTracker > timeLength)
            {
				if (loopMode == TweenAnimationLoopMode.Clamped)
				{
					
					timeTrackerModified = timeLength;
					animCompleted = true;

				}
				else if(loopMode == TweenAnimationLoopMode.Loop)
                {
					//patch timeTracker
					timeTracker = timeTracker % timeLength;
					
				}
				else
				{

					timeTrackerModified = Mathf.PingPong(timeTracker, timeLength);
					if (timeTracker > timeLengthDoubled)
                    {
						if(loopMode == TweenAnimationLoopMode.PingpongOnce)
                        {
							timeTrackerModified = 0;
							animCompleted =  true;
						}
						else
                        {
							timeTracker = timeTracker % timeLength;
							
						}
						
						
					}
					
				}
			}

			float tn = timeTrackerModified / timeLength;

			

			UpdateValue(easingCurveFunc(tn));

#if UNITY_EDITOR
			if(!Application.isPlaying)
            {
				timeTracker += 0.02f;
            }
			else
            {
				timeTracker += Time.unscaledDeltaTime * speed * (timeScaleIndependent ? 1 : Time.timeScale);
			}
#else
			timeTracker += Time.unscaledDeltaTime * speed * (timeScaleIndependent ? 1 : Time.timeScale);
#endif
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
