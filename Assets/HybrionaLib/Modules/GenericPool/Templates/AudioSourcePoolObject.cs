/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  AudioSourcePoolObject.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  15-04-2025 10:10:50

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class AudioSourcePoolObject : GOPoolObject
	{
        [HideInInspector] public bool autoDestroy;

        [field:SerializeField]
        public AudioSource audioSource { get; private set; }
 

      
        public override void Activate()
        {
            if (audioSource && audioSource.clip)
            {
                autoDestroy = true;
                lifeIfAutoDestroy = audioSource.clip.length + .5f;
            }
            base.Activate();
           
        }

        public void SetClip(AudioClip clip)
        {
            if (audioSource)
            {
                audioSource.clip = clip;
            }
        }

        public void Play()
        {
            if (!audioSource.playOnAwake)
            {
                audioSource.Stop();
                audioSource.Play();
            }
        }

    }
}
