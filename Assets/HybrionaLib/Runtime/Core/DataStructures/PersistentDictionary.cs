/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  PersistentDictionary.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  07/19/2018 15:43:10

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	[System.Serializable]
	public class PersistentDictionary<T> 
	{
		[SerializeField]
		private List<string> keys = new List<string>();

		[SerializeField]
		private List<T> values  = new List<T>();


		public int IndexOfKey(string key)
		{
			return keys.IndexOf(key);
		}

		public int Count
		{
			get
			{
				return keys.Count;
			}
		}
		public bool ContainsKey(string key)
		{
			return keys.Contains(key);
		}

		public void Add(string key,T value)
		{
			if(keys == null)
			{
				keys = new List<string>();
				values = new List<T>();
			}
			if(ContainsKey(key))
			{
				int index = IndexOfKey(key);
				values[index] = value;
			}
			else
			{
				keys.Add(key);
				values.Add(value);
			}
		}


		public T GetValue(string key)
		{
			int index = IndexOfKey(key);
			if(index >= 0)
			{
				return values[index];
			}

			throw new System.Exception("Specifed Key doesn't exist!");
		}

		public void Remove(string key)
		{
			int index = IndexOfKey(key);
			if(index >= 0)
			{
				keys.RemoveAt(index);
				values.RemoveAt(index);
			}
		}


	}
}
