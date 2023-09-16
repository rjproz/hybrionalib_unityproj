/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  TweenAnimation.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  17-09-2023 02:27:10

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class TweenAnimation : HybSingleton<TweenAnimation>
	{
        
        private List<TweenAnimData> activeAnimations = new List<TweenAnimData>();
		
		

		public static void Animate(float from, float to, float timeLength, System.Action<float> onValueUpdated,bool loop = false,AnimationCurve curve = null )
		{
            if(curve == null)
            {
                curve = AnimationCurve.Linear(0, 0, 1, 1);
            }

            TweenAnimFloatData animData = (TweenAnimFloatData) Instance.floatAnimPool.FetchFromPool();
            animData.fromValue = from;
            animData.targetValue = to;
            animData.timeLength = timeLength;
            animData.loop = loop;
            animData.curve = curve;
            animData.onValueUpdated = onValueUpdated;

            animData.Reset();
            Instance.activeAnimations.Add((TweenAnimData)animData);
        }

        public static void Animate(Vector3 from, Vector3 to, float timeLength, System.Action<Vector3> onValueUpdated, bool loop = false, AnimationCurve curve = null)
        {
            if (curve == null)
            {
                curve = AnimationCurve.Linear(0, 0, 1, 1);
            }

            TweenAnimVector3Data animData = (TweenAnimVector3Data)Instance.vec3AnimPool.FetchFromPool();
            
            animData.fromValue = from;
            animData.targetValue = to;
            animData.timeLength = timeLength;
            animData.loop = loop;
            animData.curveX = curve;
            animData.curveY = curve;
            animData.curveZ = curve;
            animData.onValueUpdated = onValueUpdated;

            animData.Reset();
            Instance.activeAnimations.Add((TweenAnimData)animData);

#if UNITY_EDITOR
            Debug.Log("vec3AnimPool.totalCopyGenerated: " + Instance.vec3AnimPool.totalCopyGenerated);
#endif
        }







        private GenericPool<TweenAnimFloatData> floatAnimPool;
        private GenericPool<TweenAnimVector3Data> vec3AnimPool;
        private IEnumerator Loop()
        {
            {
                floatAnimPool = new GenericPool<TweenAnimFloatData>(createCopyFunction: () =>
                {

                    var animData = new TweenAnimFloatData();
                    animData.returnToPoolCallback = () => { floatAnimPool.ReturnToPool(animData); };
                    return animData;
                }, onReturnedToPoolCallback: null);

                floatAnimPool.PreCache(10);
            }

            {
                vec3AnimPool = new GenericPool<TweenAnimVector3Data>(createCopyFunction: () =>
                {

                    var animData = new TweenAnimVector3Data();
                    animData.returnToPoolCallback = () => { vec3AnimPool.ReturnToPool(animData); };
                    return animData;
                }, onReturnedToPoolCallback: null);

                vec3AnimPool.PreCache(10);
            }


            while (true)
            {
                for(int i= activeAnimations.Count-1;i >= 0;i--)
                {
                    var activeAnim = activeAnimations[i];
                    if(activeAnim.Update())
                    {
                        activeAnimations.RemoveAt(i);
                        activeAnim.ReturnToPool();
                    }
                }
                yield return null;
            }
        }

        private void OnEnable()
        {
            StartCoroutine(Loop());
        }

    }
}
