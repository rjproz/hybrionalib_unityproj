using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

namespace Hybriona.Private
{
	public class HybSplashManager : MonoBehaviour 
	{

        public GameObject quad;

        private void Awake()
        {
#if UNITY_STANDALONE
			int size = Mathf.RoundToInt(Screen.currentResolution.height * .6f);
			Screen.SetResolution(size, size,false);
#endif
        }

        private void Start () 
		{
            quad.SetActive(false);

            StartCoroutine(PlayVideo());
		}


		IEnumerator PlayVideo()
		{
			VideoPlayer player = gameObject.GetComponent<VideoPlayer>();
            /*
			player.renderMode = VideoRenderMode.CameraFarPlane;

			if(IsLandscape())
			{
				player.aspectRatio = VideoAspectRatio.FitVertically;
			}
			else
			{
				player.aspectRatio = VideoAspectRatio.FitHorizontally;
			}
            */
			player.Play();
            while (!player.isPlaying)
            {
                yield return null;
            }
            quad.SetActive(true);

            yield return new WaitForSeconds(3f);
			while(player.isPlaying)
			{
				yield return null;
			}
			yield return new WaitForSeconds(.5f);

#if !UNITY_EDITOR
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
#endif
		}

		public bool IsLandscape()
		{
			ScreenOrientation orientation = Screen.orientation;

			if(orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.Unknown
				|| orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeRight )
			{
				return true;
			}
			return false;
		}

	}




}