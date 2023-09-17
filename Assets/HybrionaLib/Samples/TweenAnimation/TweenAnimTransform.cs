﻿/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  TweenAnimTransform.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  17-09-2023 03:52:47

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Hybriona
{
	public class TweenAnimTransform : MonoBehaviour 
	{
		
		public bool loop;

		[Range(0, 4)]
		public float animSpeed = 1;


		

		public TweenAnimHandler handler;
		
		public void StartAnimation() 
		{
            handler = TweenAnimation.Animate(new Vector3(0,5,0), new Vector3(0, .5f, 0), 2, (pos) =>
            {
                transform.position = pos;
            }, loop, TweenCurve.EaseOutBounce);


   //         TweenAnimation.Animate(Vector3.one, Vector3.one * 1.2f, 1, (scale) =>
			//{
			//	transform.localScale = scale;
			//}, false, TweenCurve.EaseOutBounce);

            TweenAnimation.Animate(transform.rotation,Quaternion.Euler(0,45,0), 2, (rot) =>
            {
                transform.rotation = rot;
            }, false, TweenCurve.EaseOutElastic);
        }


		private void Update()
        {
			if (!handler.IsNull())
			{
				handler.speed = animSpeed;
			}

		}
    }


	[CustomEditor(typeof(TweenAnimTransform))]
	public class TweenAnimTransformEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
			var script = (TweenAnimTransform)target;
			if (!Application.isPlaying)
			{
				return;
			}
			if(!script.handler.IsNull())
            {
				if(script.handler.IsPlaying())
                {
					if(GUILayout.Button("Pause Anim"))
                    {
						script.handler.Pause();

					}
					if (GUILayout.Button("Stop Anim"))
					{
						script.handler.Stop();
					}
				}
				else
                {
					if (GUILayout.Button("Resume Anim"))
					{
						script.handler.Resume();
					}
				}
            }
			else
            {
				if (GUILayout.Button("Start Anim without Loop"))
				{
					script.loop = false;
					script.StartAnimation();

				}
				if (GUILayout.Button("Start Anim with Loop"))
				{
					script.loop = true;
					script.StartAnimation();

				}
			}

        }
    }
}
