/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  ParticleSystemPoolObject.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  18-09-2023 23:19:27

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class ParticleSystemPoolObject : GOPoolObject 
	{
		public ParticleSystem particleSystem;

        public override void Activate()
        {
            autoDestroy = true;
            lifeIfAutoDestroy = particleSystem.main.duration;
            particleSystem.Clear(true);
            base.Activate();
        }


    }
}
