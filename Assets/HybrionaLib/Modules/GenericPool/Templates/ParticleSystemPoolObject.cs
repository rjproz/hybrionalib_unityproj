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
		public new ParticleSystem particleSystem;

        public override void Activate()
        {
            if (!autoDestroy)
            {
                autoDestroy = true;
                lifeIfAutoDestroy = particleSystem.main.duration + 1;
            }
            particleSystem.Clear(true);
            base.Activate();
        }


    }
}
