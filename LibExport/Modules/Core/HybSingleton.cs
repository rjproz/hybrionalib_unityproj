using UnityEngine;
using System.Collections;

public class HybSingleton<T> : HybMonoBehaviourForSingleton where T : HybMonoBehaviourForSingleton
{


	protected static T m_instance;
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
					m_instance.OnInstantiated();
				}
			}
			return m_instance;
		}
	}

	public static bool HasInstance()
    {
		return m_instance != null;
    }

	public void SetDontDestroyOnLoad()
	{
		DontDestroyOnLoad (Instance.gameObject);
	}

}
public class HybMonoBehaviourForSingleton : MonoBehaviour
{
	public virtual void OnInstantiated()
	{

	}
}
