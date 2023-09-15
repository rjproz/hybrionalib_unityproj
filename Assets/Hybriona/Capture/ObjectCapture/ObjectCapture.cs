using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class ObjectCapture : MonoBehaviour 
	{
		public int imageSize = 1024;
		public GameObject [] captureObjects;
		public const string path = "[ObjectCaptures]/";
		[ContextMenu("Create Thumbs")]
		public void CreateThumbs()
		{
			if(!System.IO.Directory.Exists(path))
			{
				System.IO.Directory.CreateDirectory(path);
			}

			RenderTexture captureDest = new RenderTexture(imageSize,imageSize,24,RenderTextureFormat.ARGB32);
			GameObject group = new GameObject("ObjectCapture");
			Camera captureCam = group.AddComponent<Camera>();
			captureCam.aspect = 1;
			captureCam.depth = 99;
			captureCam.farClipPlane = 100;
			captureCam.backgroundColor = new Color(0,0,0,0);
			captureCam.clearFlags = CameraClearFlags.SolidColor;
			captureCam.orthographic = true;
			captureCam.targetTexture = captureDest;
			group.transform.position = new Vector3(10000,-10000,10000);

			for(int i=0;i<captureObjects.Length;i++)
			{
				try
				{
					GameObject obj = GameObject.Instantiate(captureObjects[i]);
					obj.transform.localScale = captureObjects[i].transform.localScale;
					obj.transform.parent = group.transform;
					obj.transform.localPosition = new Vector3(0,0,1);
					Bounds objSize = obj.CalculateBounds();
					captureCam.orthographicSize = 0.5f * Mathf.Max(objSize.size.x,objSize.size.y) * 1.1f;
					captureCam.Render();
					Texture2D image = TextureOperation.ConvertToTexture2D(captureDest);
					byte [] imageData = image.EncodeToPNG();

					string filePath = path + captureObjects[i].name+".png";
					System.IO.File.WriteAllBytes(filePath,imageData);

					imageData = null;
					DestroyImmediate(image);
					DestroyImmediate(obj);
				}
				catch
				{
					
				}
			}
			captureCam.targetTexture = null;
			DestroyImmediate(captureDest);
			DestroyImmediate(group);
			Resources.UnloadUnusedAssets();
		}

	}
}
