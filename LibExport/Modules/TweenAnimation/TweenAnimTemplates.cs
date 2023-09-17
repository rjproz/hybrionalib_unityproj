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
        

        public System.Action<Vector3> onValueUpdated;


        

        public override void UpdateValue(float timeNormalized)
        {
            float time = curve.Evaluate(timeNormalized);

            Vector3 value = Vector3.Lerp(fromValue, targetValue, time);
            onValueUpdated(value);

        }
    }

    public class TweenAnimVector4Data : TweenAnimData
    {
        public Vector4 fromValue;
        public Vector4 targetValue;


        public System.Action<Vector4> onValueUpdated;

        public override void UpdateValue(float timeNormalized)
        {
            float time = curve.Evaluate(timeNormalized);
            Vector4 value = Vector4.Lerp(fromValue,targetValue,time);
            onValueUpdated(value);

        }
    }

    public class TweenAnimColorData : TweenAnimData
    {
        public Color fromValue;
        public Color targetValue;


        public System.Action<Color> onValueUpdated;

        public override void UpdateValue(float timeNormalized)
        {
            float time = curve.Evaluate(timeNormalized);
            var value = Color.Lerp(fromValue, targetValue, time);
            onValueUpdated(value);

        }
    }
}
