/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  TweenAnimationTransform.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  18-09-2023 17:38:29

*************************************************************************/
using System.Collections;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using UnityEngine;

namespace Hybriona
{
	public class TweenAnimationTransform : MonoBehaviour 
	{
		public float timeLength;
		public TweenAnimVector3Data position = new TweenAnimVector3Data();
		public TweenAnimVector3Data scale = new TweenAnimVector3Data();

		public bool isPlayingInEditor;
		[ContextMenu("Play in Editor")]
		public void PlaInEditor()
		{
			if (isPlayingInEditor)
				return;

			
			position.timeLength = timeLength;
			position.easingCurveFunc = TweenCurve.EaseInOutBack;
			position.onValueUpdated = (Vector3 pos) => {
				transform.position = pos;
			};

			position.Reset();
			isPlayingInEditor = true;

#if UNITY_EDITOR
			EditorCoroutineUtility.StartCoroutine(RunAnimationInEditor(),this);

#endif
		}

      


		private IEnumerator RunAnimationInEditor()
        {
			while(!position.Update())
            {
				yield return null;
            }
			isPlayingInEditor = false;

		}
    }

	
}
