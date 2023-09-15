/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  ComponentExtensions'.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  07/27/2018 21:07:57

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public  static class ComponentExtensions 
	{

		#region Material
		public static void SetColorAlpha(this Material mat,float alpha)
		{
			Color color = mat.color;
			color.a = alpha;
			mat.color = color;
		}
		#endregion
	}
}
