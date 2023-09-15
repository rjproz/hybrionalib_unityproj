using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hybriona
{
	public class HybScreen : HybSingleton<HybScreen> 
	{
		public float aspectRatio { get; private set; }

		private int lastScreenWidth = -1;
		private int lastScreenHeight = -1;
	
		

		private void Start()
		{
			SetDontDestroyOnLoad();
			StartCoroutine(CheckOrientation());
		}

		private void CalculcateAspectRatio()
		{
			aspectRatio = (float) Screen.width / (float)Screen.height;
		}

		IEnumerator CheckOrientation()
		{
			while(true)
			{
				if(Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
				{
					lastScreenWidth = Screen.width;
					lastScreenHeight = Screen.height;
					CalculcateAspectRatio();
					if (onScreenResolutionChanged != null)
					{
						if(lastScreenWidth < lastScreenHeight)
						{
							onScreenResolutionChanged(ScreenOrientationH.Portrait);
						}
						else
						{
							onScreenResolutionChanged(ScreenOrientationH.Landscape);
						}
					}
				}
				yield return null;
			}
		}


		public delegate void OnScreenResolutionChanged(ScreenOrientationH orientation);
		public OnScreenResolutionChanged onScreenResolutionChanged;
	}

	public enum ScreenOrientationH {Portrait,Landscape};
}