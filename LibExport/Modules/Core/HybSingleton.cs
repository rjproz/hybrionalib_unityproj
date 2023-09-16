using UnityEngine;
using System.Collections;

public class HybSingleton<T> : MonoBehaviour where T : MonoBehaviour
{


	private static T m_instance;
	public static T Instance
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = GameObject.FindObjectOfType<T>();
				if(m_instance == null)
				{
					Debug.Log("Creating new instance of "+typeof(T).ToString());
					GameObject o = new GameObject(typeof(T).ToString());
					m_instance = o.AddComponent<T>();
				}
			}
			return m_instance;
		}
	}

	public void SetDontDestroyOnLoad()
	{
		DontDestroyOnLoad (Instance.gameObject);
	}

}
