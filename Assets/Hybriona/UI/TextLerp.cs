/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  TextLerp.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  10/07/2018 11:17:33

*************************************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Hybriona
{
   
    public class TextLerp : Text
    {

        public enum DataType { Int, Float, String };
        public DataType currentDataType;




        private WaitForEndOfFrame waitForFrame;

        public override string text
        {
            get
            {
                return base.text;
            }

            set
            {
                if (base.text != value)
                {
                    Debug.LogError("Can't assign text directly. Use IntValue, FloatValue or StringValue");
                }
            }
        }


        /*  ------------------------------ INT Mode Starts ------------------------------ */
        [HideInInspector]
        public int minIntStep = 10;

        protected int _intValue;
        public int intValue
        {
            get
            {
                return _intValue;
            }
            set
            {
                _intValue = value;
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    currentIntValue = _intValue;
                    base.text = currentIntValue.ToString();
                    return;
                }
#endif
                UpdateValue(_intValue);
            }
        }

        [HideInInspector]
        public int currentIntValue;

        private void UpdateValue(int value)
        {
            if (currentDataType != DataType.Int)
            {
                return;
            }
            StopAllCoroutines();
            StartCoroutine(UpdateIntValue(value));
        }

        private IEnumerator UpdateIntValue(int value)
        {
            _intValue = value;
            while (currentIntValue != intValue)
            {
                int stepLength = Mathf.Max(minIntStep, (int)(Mathf.Abs(_intValue - currentIntValue) / minIntStep));
                int finalstep = Mathf.Abs(_intValue - currentIntValue) < stepLength ? (_intValue - currentIntValue) : stepLength * (int)Mathf.Sign(_intValue - currentIntValue);
                currentIntValue += finalstep;
                base.text = currentIntValue.ToString();
                yield return waitForFrame;
            }
        }

        /*  ------------------------------ INT Mode Ended ------------------------------ */


     


        protected override void Awake()
        {
            intValue = currentIntValue;
            waitForFrame = new WaitForEndOfFrame();
        }

       
        
        
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TextLerp))]
    public class TextLerpEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            
            TextLerp script = (TextLerp)target;
            script.currentDataType = (TextLerp.DataType) EditorGUILayout.EnumPopup("DataType:",(System.Enum) script.currentDataType);
            if (script.currentDataType == TextLerp.DataType.Int)
            {
                script.intValue = EditorGUILayout.IntField("Int Value", script.intValue);
                script.minIntStep = EditorGUILayout.IntField("Min Step (delta 100)", script.minIntStep);
            }
            base.OnInspectorGUI();
        }
    }
#endif
}
