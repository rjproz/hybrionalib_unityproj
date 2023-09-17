/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  TweenAnimTemplates.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  17-09-2023 02:34:37

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class TweenAnimFloatData  : TweenAnimData
	{
        public float fromValue;
        public float targetValue;
        public AnimationCurve curve;

        public System.Action<float> onValueUpdated;
        
       

        public override void UpdateValue(float timeNormalized)
        {


            float x = curve.Evaluate(timeNormalized);
            float y = Mathf.Lerp(fromValue, targetValue, x);
            onValueUpdated(y);

        }
    }

    public class TweenAnimVector3Data : TweenAnimData
    {
        public Vector3 fromValue;
        public Vector3 targetValue;
        public AnimationCurve curveX;
        public AnimationCurve curveY;
        public AnimationCurve curveZ;

        public System.Action<Vector3> onValueUpdated;


        

        public override void UpdateValue(float timeNormalized)
        {
            float timeX = curveX.Evaluate(timeNormalized);
            float timeY = curveY.Evaluate(timeNormalized);
            float timeZ = curveZ.Evaluate(timeNormalized);

            Vector3 value = new Vector3(Mathf.Lerp(fromValue.x, targetValue.x, timeX),
                Mathf.Lerp(fromValue.y, targetValue.y, timeY),
                Mathf.Lerp(fromValue.z, targetValue.z, timeZ));
            onValueUpdated(value);

        }
    }
}
