using UnityEngine;
using System.Collections.Generic;

public class HybPoolManager : MonoBehaviour {

	public class HybPool
	{
		public HybPoolDecentralized basePrefab;
		private Queue<HybPoolDecentralized> pool = new Queue<HybPoolDecentralized>();

		private string _id;
		public HybPool(string id,GameObject o)
		{
			_id = id;
			basePrefab = o.GetComponent<HybPoolDecentralized>();
			o.SetActive(false);
		}

		public HybPoolDecentralized Get()
		{
			if(pool.Count > 0)
			{
				return pool.Dequeue();
			}
			else
			{
				GameObject o = (GameObject) GameObject.Instantiate(basePrefab.gameObject, basePrefab.transform.parent);
				HybPoolDecentralized script = o.GetComponent<HybPoolDecentralized>();
				script.poolID = _id;
				return script;
			}

		}

		public void Put(HybPoolDecentralized o)
		{
			pool.Enqueue(o);
		}
	}

	private static HybPoolManager m_Instance;
	public static HybPoolManager Instance
	{
		get
		{
			if(m_Instance == null)
			{
				GameObject o = new GameObject("HybPoolManager");
				m_Instance = o.AddComponent<HybPoolManager>();
				m_Instance.ClearPool();
			}
			return m_Instance;
		}
	}

	private Dictionary<string,HybPool> pool = new Dictionary<string,HybPool>();
	public void ClearPool()
	{
		pool.Clear();
	}

	public void RegisterPool(string ID,GameObject prefab, int preCacheValue = 0)
	{
		pool.Add(ID,new HybPool(ID,prefab));
	}


	public HybPoolDecentralized GetFromPool(string Key)
	{
		return pool[Key].Get();
	}

	public void PutInPool(string Key,HybPoolDecentralized o)
	{
		pool[Key].Put(o);
	}
}
