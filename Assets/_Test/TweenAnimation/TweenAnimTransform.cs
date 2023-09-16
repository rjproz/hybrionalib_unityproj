/*************************************************************************
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

namespace Hybriona
{
	public class TweenAnimTransform : MonoBehaviour 
	{
		public AnimationCurve curve;

		[ContextMenu("StartAnim")]
		void StartAnim() 
		{
			TweenAnimation.Animate(transform.position, transform.position + Vector3.up * 2, 2, (pos) =>
			{
				transform.position = pos;
			}, false, curve);
		}


	}
}
