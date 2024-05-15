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
    public enum TweenAnimationLoopMode { Clamped = 0, Loop = 1 , PingpongOnce = 2, PingpongForever = 3 };
	public class TweenAnimation : HybSingleton<TweenAnimation>
	{
        private ulong animationIdCounter = 1;
        private List<TweenAnimData> activeAnimations = new List<TweenAnimData>();
        private List<TweenAnimData> activeFixedUpdateAnimations = new List<TweenAnimData>();

        public static TweenAnimHandler Animate(float from, float to, float timeLength, System.Action<float> onValueUpdated,System.Func<float,float> easingCurveFunc = null, TweenAnimationLoopMode loopMode = TweenAnimationLoopMode.Clamped, bool timeScaleIndependent = false , bool useFixedUpdate = false)
		{
            TweenAnimFloatData animData = Instance.floatAnimPool.FetchFromPool();
            animData.fromValue = from;
            animData.targetValue = to;
            animData.onValueUpdated = onValueUpdated;
            
            return AssignCommonValues(animData, timeLength, loopMode, easingCurveFunc, timeScaleIndependent,useFixedUpdate);
        }

        public static TweenAnimHandler Animate(Vector3 from, Vector3 to, float timeLength, System.Action<Vector3> onValueUpdated, System.Func<float, float> easingCurveFunc = null, TweenAnimationLoopMode loopMode = TweenAnimationLoopMode.Clamped, bool timeScaleIndependent = false, bool useFixedUpdate = false)
        {
           
            TweenAnimVector3Data animData = Instance.vec3AnimPool.FetchFromPool();
            
            animData.fromValue = from;
            animData.targetValue = to;
            animData.onValueUpdated = onValueUpdated;

            return AssignCommonValues(animData, timeLength, loopMode, easingCurveFunc, timeScaleIndependent, useFixedUpdate);

        }

        public static TweenAnimHandler Animate(Vector4 from, Vector4 to, float timeLength, System.Action<Vector4> onValueUpdated, System.Func<float, float> easingCurveFunc = null, TweenAnimationLoopMode loopMode = TweenAnimationLoopMode.Clamped,bool timeScaleIndependent = false, bool useFixedUpdate = false)
        {
           

            TweenAnimVector4Data animData = Instance.vec4AnimPool.FetchFromPool();

            animData.fromValue = from;
            animData.targetValue = to;
            animData.onValueUpdated = onValueUpdated;

            return AssignCommonValues(animData, timeLength, loopMode, easingCurveFunc, timeScaleIndependent, useFixedUpdate);

        }

        public static TweenAnimHandler Animate(Color from, Color to, float timeLength, System.Action<Color> onValueUpdated, System.Func<float, float> easingCurveFunc = null,TweenAnimationLoopMode loopMode = TweenAnimationLoopMode.Clamped, bool timeScaleIndependent = false, bool useFixedUpdate = false)
        {
           

            var animData = Instance.colorAnimPool.FetchFromPool();

            animData.fromValue = from;
            animData.targetValue = to;
            animData.onValueUpdated = onValueUpdated;

            return AssignCommonValues(animData, timeLength, loopMode, easingCurveFunc, timeScaleIndependent, useFixedUpdate);

        }

        public static TweenAnimHandler Animate(Quaternion from, Quaternion to, float timeLength, System.Action<Quaternion> onValueUpdated, System.Func<float, float> easingCurveFunc = null, TweenAnimationLoopMode loopMode = TweenAnimationLoopMode.Clamped,bool timeScaleIndependent = false, bool useFixedUpdate = false)
        {


            var animData = Instance.quaternionAnimPool.FetchFromPool();

            animData.fromValue = from;
            animData.targetValue = to;
            animData.onValueUpdated = onValueUpdated;

            return AssignCommonValues(animData, timeLength, loopMode, easingCurveFunc, timeScaleIndependent, useFixedUpdate);

        }



        static TweenAnimHandler AssignCommonValues(TweenAnimData animData,float timeLength, TweenAnimationLoopMode loopMode, System.Func<float, float> easingCurveFunc, bool timeScaleIndependent, bool useFixedUpdate )
        {
            if(easingCurveFunc == null)
            {
                easingCurveFunc = TweenCurve.Linear;
            }
            animData.id = ++Instance.animationIdCounter;
            animData.timeLength = timeLength;
            animData.loopMode = loopMode;
            animData.easingCurveFunc = easingCurveFunc;
            animData.timeScaleIndependent = timeScaleIndependent;
            animData.useFixedUpdate = useFixedUpdate;
            animData.Reset();
            var handler = new TweenAnimHandler(animData.id);
            animData.assignedHandler = handler;
            if (useFixedUpdate)
            {
                Instance.activeFixedUpdateAnimations.Add(animData);
            }
            else
            {

                Instance.activeAnimations.Add(animData);
            }
            return handler;
        }
        




        internal static TweenAnimData FindAnimDataById(ulong animId)
        {
            var result = Instance.activeAnimations.FindLast(o => o.id == animId);
            if(result == null)
            {
                result = Instance.activeFixedUpdateAnimations.FindLast(o => o.id == animId);
            }
            return result;
        }

        private GenericPool<TweenAnimFloatData> floatAnimPool;
        private GenericPool<TweenAnimVector3Data> vec3AnimPool;
        private GenericPool<TweenAnimVector4Data> vec4AnimPool;
        private GenericPool<TweenAnimQuaternionData> quaternionAnimPool;
        private GenericPool<TweenAnimColorData> colorAnimPool;


        

        private IEnumerator Loop()
        {

            yield return null;
            while (true)
            {
                for (int i = activeAnimations.Count - 1; i >= 0; i--)
                {
                    var activeAnim = activeAnimations[i];
                    if (activeAnim.Update())
                    {
                        activeAnimations.RemoveAt(i);
                        activeAnim.assignedHandler.Release();
                        activeAnim.ReturnToPool();
                    }
                }
                yield return null;
            }
        }

        private void FixedUpdate()
        {
            for (int i = activeFixedUpdateAnimations.Count - 1; i >= 0; i--)
            {
                var activeAnim = activeFixedUpdateAnimations[i];
                if (activeAnim.FixedUpdate())
                {
                    activeFixedUpdateAnimations.RemoveAt(i);
                    activeAnim.assignedHandler.Release();
                    activeAnim.ReturnToPool();
                }
            }
        }

        public override void OnInstantiated()
        {

            {
                floatAnimPool = new GenericPool<TweenAnimFloatData>(createCopyFunction: () =>
                {

                    var animData = new TweenAnimFloatData();
                    animData.returnToPoolCallback = () => { floatAnimPool.ReturnToPool(animData); };
                    return animData;
                }, onReturnedToPoolCallback: null);


            }

            {
                vec3AnimPool = new GenericPool<TweenAnimVector3Data>(createCopyFunction: () =>
                {

                    var animData = new TweenAnimVector3Data();
                    animData.returnToPoolCallback = () => { vec3AnimPool.ReturnToPool(animData); };
                    return animData;
                }, onReturnedToPoolCallback: null);


            }

            {
                vec4AnimPool = new GenericPool<TweenAnimVector4Data>(createCopyFunction: () =>
                {

                    var animData = new TweenAnimVector4Data();
                    animData.returnToPoolCallback = () => { vec4AnimPool.ReturnToPool(animData); };
                    return animData;
                }, onReturnedToPoolCallback: null);


            }

            {
                quaternionAnimPool = new GenericPool<TweenAnimQuaternionData>(createCopyFunction: () =>
                {

                    var animData = new TweenAnimQuaternionData();
                    animData.returnToPoolCallback = () => { quaternionAnimPool.ReturnToPool(animData); };
                    return animData;
                }, onReturnedToPoolCallback: null);


            }

            {
                colorAnimPool = new GenericPool<TweenAnimColorData>(createCopyFunction: () =>
                {

                    var animData = new TweenAnimColorData();
                    animData.returnToPoolCallback = () => { colorAnimPool.ReturnToPool(animData); };
                    return animData;
                }, onReturnedToPoolCallback: null);


            }


            
        }

        private void OnEnable()
        {
            StartCoroutine(Loop());
        }

    }
}
