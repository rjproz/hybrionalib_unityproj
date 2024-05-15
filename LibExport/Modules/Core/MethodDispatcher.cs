/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  MethodDispatcher.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  20-10-2023 11:14:10

*************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hybriona
{
	public class MethodDispatcher : MonoBehaviour 
	{

		private Queue<UnityAction> methodsQueue = new Queue<UnityAction>();


		public static void Enqueue(UnityAction action)
        {
			GetInstance().methodsQueue.Enqueue(action);

		}

        private void Update()
        {
            while(methodsQueue.Count > 0)
            {
				methodsQueue.Dequeue()();

			}
        }

		private static MethodDispatcher m_instance;
		private static MethodDispatcher GetInstance()
		{
			if (m_instance == null)
			{
				m_instance = GameObject.FindObjectOfType<MethodDispatcher>();
				if (m_instance == null)
				{
					m_instance = new GameObject("Hybriona.MethodDispatcher").AddComponent<MethodDispatcher>();
					DontDestroyOnLoad(m_instance.gameObject);
				}

			}
			return m_instance;

		}

		private void Awake()
		{
			if (m_instance != null && m_instance != this)
			{
				Destroy(gameObject);
				return;
			}
		}

	}
}
