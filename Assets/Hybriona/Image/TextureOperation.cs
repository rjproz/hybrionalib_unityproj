using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class TextureOperation 
	{
		public static Texture2D ConvertToTexture2D(RenderTexture renderTex)
		{
			Texture2D tex2D = new Texture2D(renderTex.width,renderTex.height,TextureFormat.ARGB32,false);
			RenderTexture.active = renderTex;
			tex2D.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
			tex2D.Apply();
			RenderTexture.active = null;
			return tex2D;
		}
	}
}
