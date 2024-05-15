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
    [System.Serializable]
    public class TweenAnimFloatData  : TweenAnimData
	{
        public float fromValue;
        public float targetValue;
       

        public System.Action<float> onValueUpdated;
       

        public override void UpdateValue(float timeNormalized)
        {
            float y = Mathf.LerpUnclamped(fromValue, targetValue, timeNormalized);
            onValueUpdated(y);

        }
    }

    [System.Serializable]
    public class TweenAnimVector3Data : TweenAnimData
    {
        
        public Vector3 fromValue;
        public Vector3 targetValue;
        
        public System.Action<Vector3> onValueUpdated;

        public override void UpdateValue(float timeNormalized)
        {
            Vector3 value = Vector3.LerpUnclamped(fromValue, targetValue, timeNormalized);
            onValueUpdated(value);

        }
    }

    [System.Serializable]
    public class TweenAnimVector4Data : TweenAnimData
    {
        public Vector4 fromValue;
        public Vector4 targetValue;


        public System.Action<Vector4> onValueUpdated;

        public override void UpdateValue(float timeNormalized)
        {
            Vector4 value = Vector4.LerpUnclamped(fromValue,targetValue, timeNormalized);
            onValueUpdated(value);

        }
    }

    [System.Serializable]
    public class TweenAnimQuaternionData : TweenAnimData
    {
        public Quaternion fromValue;
        public Quaternion targetValue;


        public System.Action<Quaternion> onValueUpdated;

        public override void UpdateValue(float timeNormalized)
        {
            Quaternion value = Quaternion.LerpUnclamped(fromValue, targetValue, timeNormalized);
            onValueUpdated(value);

        }
    }

    [System.Serializable]
    public class TweenAnimColorData : TweenAnimData
    {
        public Color fromValue;
        public Color targetValue;


        public System.Action<Color> onValueUpdated;

        public override void UpdateValue(float timeNormalized)
        {
            var value = Color.LerpUnclamped(fromValue, targetValue, timeNormalized);
            onValueUpdated(value);

        }
    }
}
