using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public static class RendererExtension 
	{
		public static Bounds CalculateBounds(this GameObject o)
		{
			Bounds bounds = new Bounds();

			if(o.GetComponent<Renderer>())
			{
				bounds = o.GetComponent<Renderer>().bounds;
			}
			else
			{
				bounds.center = o.transform.position;
				bounds.size = Vector3.zero;
			}

			Renderer [] renderers = o.GetComponentsInChildren<Renderer>(true);
			for(int i=0;i<renderers.Length;i++)
			{
				//Debug.Log("renderers : "+renderers[i].name);
				if(renderers[i].name != "Canvas")
				{
					bounds.Encapsulate(renderers[i].bounds);
				}
			}

			Vector3 localScale = o.transform.localScale;

			bounds.size = new Vector3(bounds.size.x/localScale.x, bounds.size.y/localScale.y ,bounds.size.z/localScale.z);
			//bounds.center = new Vector3(bounds.center.x/localScale.x, bounds.center.y/localScale.y ,bounds.center.z/localScale.z);
			return bounds;
		}
	}
}
