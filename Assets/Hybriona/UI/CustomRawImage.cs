using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

	namespace Hybriona
	{
	[RequireComponent(typeof(RawImage))]
	[RequireComponent(typeof(AspectRatioFitter))]
	public class CustomRawImage : MonoBehaviour {

		public RawImage rawImage;
		public AspectRatioFitter aspectRatioFitter;

		public float aspectRatio = 4/3f;

		public float previousAspectRatio;

		public TextAsset currentTexReference;
		#if UNITY_EDITOR
		public TextAsset previousTexReference;
		#endif
		public void LoadTexture(byte [] data,float aspectRatio = 4f/3f,TextureFormat format = TextureFormat.ARGB32)
		{
			if(rawImage.texture != null)
			{
				if(Application.isPlaying)
				{
					GameObject.Destroy(rawImage.texture);
				}
				else
				{
					GameObject.DestroyImmediate(rawImage.texture);
				}
				rawImage.texture = null;
			}
			if(data != null)
			{
				Texture2D texture = new Texture2D(1,1,format,false);
				texture.LoadImage(data);
				rawImage.texture = texture;
				data = null;
			}
		}
	}
	#if UNITY_EDITOR
	[CustomEditor(typeof(CustomRawImage))]
	public class CustomRawImageEditor : Editor
	{
		public override void OnInspectorGUI ()
		{
			CustomRawImage script = (CustomRawImage) target;

			if(script.rawImage == null)
			{
				script.rawImage = script.GetComponent<RawImage>();
				script.rawImage.hideFlags = HideFlags.HideInInspector;
			}
			if(script.aspectRatioFitter == null)
			{
				script.aspectRatioFitter = script.GetComponent<AspectRatioFitter>();
				script.aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
				script.aspectRatioFitter.hideFlags = HideFlags.HideInInspector;
			}

			script.aspectRatio = EditorGUILayout.FloatField("Aspect Ratio",script.aspectRatio);
			if(script.aspectRatio != script.previousAspectRatio)
			{
				script.aspectRatioFitter.aspectRatio = script.aspectRatio;
				script.previousAspectRatio = script.aspectRatio;
			}


			script.currentTexReference = (TextAsset)EditorGUILayout.ObjectField("Texture",script.currentTexReference,typeof(TextAsset));
			if(script.currentTexReference != script.previousTexReference )
			{
				if(script.currentTexReference == null)
				{
					script.LoadTexture(null,script.aspectRatio,TextureFormat.ARGB32);
				}
				else
				{
					script.LoadTexture(script.currentTexReference.bytes,script.aspectRatio,TextureFormat.ARGB32);
				}
				script.previousTexReference = script.currentTexReference;
			}

		}


	}
	#endif
}