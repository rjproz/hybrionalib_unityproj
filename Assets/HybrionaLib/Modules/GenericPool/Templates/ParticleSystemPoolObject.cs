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
        [HideInInspector] public bool autoDestroy;

        [field: SerializeField]
        public new ParticleSystem particleSystem { get; private set; }

        [field: SerializeField]
        public ParticleSystemRenderer particleSystemRenderer { get; private set; }

        public override void Activate()
        {
            if (particleSystem)
            {
                autoDestroy = true;
                lifeIfAutoDestroy = particleSystem.main.duration + .5f;

                particleSystem.Clear(true);
            }
            base.Activate();

          
        }

        public void SetParticle(ParticleSystem particleSystem)
        {
            this.particleSystem = particleSystem;
        }
    }
}
