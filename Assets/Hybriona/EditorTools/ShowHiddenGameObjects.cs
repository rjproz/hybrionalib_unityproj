/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  ShowHiddenGameObjects.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  08/27/2018 14:03:04

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class ShowHiddenGameObjects : MonoBehaviour 
	{
		void Start () 
		{
			
		}

		[ContextMenu("Execute")]
		public void Show()
		{
			GameObject [] all = GameObject.FindObjectsOfType<GameObject>();
			Debug.Log(all.Length);
			for(int i=0;i<all.Length;i++)
			{
				all[i].hideFlags = HideFlags.None;
			}
		}

	}
}
