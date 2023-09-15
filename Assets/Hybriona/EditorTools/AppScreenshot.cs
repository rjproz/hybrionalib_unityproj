#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace Hybriona
{
	public class AppScreenshot : MonoBehaviour
	{
		public bool enable = false;
		public string directory = "../Screenshots";
		private string path = "{0}/{1}_({2})_({3}x{4}).png";
		private int count = 1;

		// Update is called once per frame
		void Update () 
		{
			if (enable)
			{
				//Debug.Log("directory enable");
				if (Input.GetKeyDown(KeyCode.P))
				{
					//Debug.Log("directory");
					if (!Directory.Exists(directory))
					{
						Directory.CreateDirectory(directory);
					}
					string writePath = string.Format(path, directory, count, Camera.main.aspect, Screen.width, Screen.height);
					ScreenCapture.CaptureScreenshot(writePath,1);
					Debug.Log("Screen Captured : " + writePath);
					count++;
				}
			}
		}
	}
}
#endif
