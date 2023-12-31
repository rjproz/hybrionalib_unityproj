﻿/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  GOPoolObject.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  15-09-2023 18:42:43

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class GOPoolObject : MonobehaviorPoolObject 
	{
		public bool autoDestroy;
		public float lifeIfAutoDestroy;
        //public string poolId { get; set; }

        private bool isInPool;

        protected ulong eventTriggerId;
        public override void Activate()
        {
            base.Activate();
            if(autoDestroy)
            {
                eventTriggerId = EventTriggerManager.AddTriggerEvent(lifeIfAutoDestroy, AutoDestroyCode);
            }
        }

        public override void OnReturnToPool()
        {
            EventTriggerManager.AbortEvent(eventTriggerId);
            gameObject.SetActive(false);
        }

        void AutoDestroyCode()
        {
            if(this != null && gameObject != null)
            {
                ReturnToPool();
            }
        }
    }
}