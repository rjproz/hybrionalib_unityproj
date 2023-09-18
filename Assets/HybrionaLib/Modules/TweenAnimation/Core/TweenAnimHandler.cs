/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  TweenAnimHandler.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  17-09-2023 18:20:51

*************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public struct TweenAnimHandler
	{

		internal ulong animId;


        private TweenAnimData PrepareData()
        {
            var animData = TweenAnimation.FindAnimDataById(animId);
            if(animData == null)
            {
                throw new Exception("TweenAnimHandler is null because the animation was stopped or completed");
            }
            else
            {
                return animData;
            }
        }

        public bool IsNull()
        {
            return TweenAnimation.FindAnimDataById(animId) == null;
        }

		public float speed
        {
			get
            {
				return PrepareData().speed;
            }
			set
            {
                PrepareData().speed = value;
            }
        }


		public bool IsPlaying()
        {
			return PrepareData().IsPlaying();
        }

		public void Pause()
        {
            PrepareData().Pause();
        }

		public void Resume()
        {
            PrepareData().Resume();
        }

		public void Stop()
		{
            PrepareData().Stop();
		}


		public TweenAnimHandler(ulong animId)
        {
			this.animId = animId;
        }

        internal void Release()
        {
            animId = 0;
            
        }
    }
}
