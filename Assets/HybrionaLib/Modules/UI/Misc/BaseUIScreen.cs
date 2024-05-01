/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  BaseUIScreen.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  01-05-2024 09:30:14

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class BaseUIScreen : MonoBehaviour 
	{
       
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide(bool instant = false)
        {
            gameObject.SetActive(false);

        }

        public virtual void Awake()
        {
            transform.localPosition = Vector3.zero;
        }


    }
}
