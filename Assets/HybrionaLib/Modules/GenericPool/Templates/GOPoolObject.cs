/*************************************************************************
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
		public uint lifeIfAutoDestroy;
		//public string poolId { get; set; }


        public override void Activate()
        {
            base.Activate();
            if(autoDestroy)
            {
                EventTriggerManager.AddTriggerEvent(lifeIfAutoDestroy, ReturnToPool);
            }
        }

    }
}