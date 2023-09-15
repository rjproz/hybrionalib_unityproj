/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  TransformHierarchy.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  08/24/2018 11:59:33

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	[System.Serializable]
	public class TransformHierarchy
	{
		public bool isUI;
		public List<TransformElement> elements = new List<TransformElement>();
		public List<RectTransformElement> uielements = new List<RectTransformElement>();
		public TransformHierarchy(Transform parent,bool _isUI,bool includeInactive = true)
		{
			isUI = _isUI;
			Transform [] children = parent.GetComponentsInChildren<Transform>(includeInactive);

			for(int i=0;i<children.Length;i++)
			{
				if(children[i] != parent)
				{
					if(!isUI)
					{
						TransformElement element = new TransformElement(children[i],parent);
						elements.Add(element);
					}
					else
					{
						RectTransformElement uielement = new RectTransformElement(children[i],parent);
						uielements.Add(uielement);
					}
				}

			}
			children = null;
		}

		public static TransformHierarchy FromJson(string json)
		{
			return JsonUtility.FromJson<TransformHierarchy>(json);
		}


		public string ToJson()
		{
			return JsonUtility.ToJson(this);
		}

		public void Apply(Transform transform)
		{
			if(!isUI)
			{
				for(int i=0;i<elements.Count;i++)
				{
					Debug.Log(elements[i].path);
					Transform child = transform.Find(elements[i].path);
					child.localPosition = elements[i].localPosition;
					child.localEulerAngles = elements[i].localEulers;
					child.localScale = elements[i].localScale;
				}
			}
			else
			{
				for(int i=0;i<uielements.Count;i++)
				{
					
					//Transform child = transform.Find(uielements[i].path).GetComponent<RectTransform>();


				}
			}
		}


		public static string GetTransformPath(Transform transform,Transform parent)
		{
			string path = transform.name;
			while (transform.parent != parent)
			{
				transform = transform.parent;
				path = transform.name + "/" + path;
			}
			return path;
		}


	}

	[System.Serializable]
	public struct TransformElement
	{
		public string path;
		public Vector3 localPosition;
		public Vector3 localEulers;
		public Vector3 localScale;

		public TransformElement(Transform transform,Transform parent)
		{
			path = TransformHierarchy.GetTransformPath(transform,parent);
			localPosition = transform.localPosition;
			localEulers = transform.localEulerAngles;
			localScale = transform.localScale;
		}

	}
	[System.Serializable]
	public struct RectTransformElement
	{
//		public string path;
//		public Vector2 anchoredPosition;
//		public Vector3 anchoredPosition3D;
//		public Vector2 anchorMax;
//		public Vector2 anchorMin;

		public RectTransformElement(Transform transform,Transform parent)
		{
//			RectTransform t;
//			t.anchoredPosition;
//			t.anchoredPosition3D;
//			t.anchorMax;
//			t.anchorMin;
//
//			t.localEulerAngles;
//			t.localPosition;
//			t.localScale;
//			t.offsetMax;
//			t.offsetMin;
//			t.pivot;
//			t.right;
//			t.sizeDelta;
//
//			path = TransformHierarchy.GetTransformPath(transform,parent);
//			localPosition = transform.localPosition;
//			localEulers = transform.localEulerAngles;
//			localScale = transform.localScale;
		}

	}
}
