/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  ScreenUtility.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  20-10-2023 10:57:42

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hybriona
{
	public sealed class ScreenUtility : MonoBehaviour 
	{
		public enum Orientation { Portrait = 0,Landscape = 1};
		
		private Coroutine processRoutine;
		private UnityEvent<Orientation> orientationChangeCallback;


		private int m_LastScreenWidth = -1;
		private int m_LastScreenHeight = -1;

		private static ScreenUtility m_instance;
		private static ScreenUtility GetInstance()
        {
			if(m_instance == null)
            {
				m_instance = GameObject.FindObjectOfType<ScreenUtility>();
				if(m_instance == null)
                {
					m_instance = new GameObject("Hybriona.ScreenUtility").AddComponent<ScreenUtility>();
					DontDestroyOnLoad(m_instance.gameObject);
                }

			}
			return m_instance;

		}
		

		public static int screenWidth
        {
			get
            {
				return Screen.width;
            }
        }

		public static int screenHeight
		{
			get
			{
				return Screen.height;
			}
		}

		public static float aspectRatio
        {
			get
            {
				return (float)Screen.width / (float)Screen.height;
            }
        }

		


		public static void AddToOrientationChangeEvent(UnityAction<Orientation> action)
        {
			
			GetInstance().orientationChangeCallback.AddListener(action);
			if(GetInstance().processRoutine == null)
            {
				GetInstance().processRoutine = GetInstance().StartCoroutine(GetInstance().Loop());

			}

		}
		public static void RemoveFromOrientationChangeEvent(UnityAction<Orientation> action)
		{
			GetInstance().orientationChangeCallback.RemoveListener(action);
			if(GetInstance().orientationChangeCallback.GetPersistentEventCount() <= 0)
            {
				if (GetInstance().processRoutine != null)
				{
					GetInstance().StopCoroutine(GetInstance().processRoutine);
					GetInstance().processRoutine = null;
				}
            }

		}

		private void Awake()
        {
            if(m_instance != null && m_instance != this)
            {
				Destroy(gameObject);
				return;
            }
        }



        private IEnumerator Loop()
        {
            while(true)
            {
				if(Screen.width != m_LastScreenWidth || Screen.height != m_LastScreenHeight)
                {
					m_LastScreenWidth = Screen.width;
					m_LastScreenHeight = Screen.height;

					MethodDispatcher.Enqueue(() =>
					{
						orientationChangeCallback?.Invoke(aspectRatio < 1 ? Orientation.Portrait : Orientation.Landscape);
					});
				}

				yield return null;
            }
        }

    }
}
