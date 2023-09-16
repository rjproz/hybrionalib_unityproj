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
		internal System.Action returnToPoolCallback;
		private float timeTracker = 0;
		
		public virtual void Reset()
        {
			timeTracker = 0;
		}

		public bool Update()
		{
			bool animCompleted = false;
			timeTracker += Time.deltaTime;
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
