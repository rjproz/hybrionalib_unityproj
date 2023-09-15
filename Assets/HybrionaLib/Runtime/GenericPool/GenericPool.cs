/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  GenericPool.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  15-09-2023 12:05:42

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class GenericPool<T> 
	{
		//public string poolId { get; private set; }

		
		private System.Action<T> onReturnedToPoolCallback;
		private System.Func<T> createCopyFunction;


		
		private Queue<T> pool = new Queue<T>();
		

		public int poolCount
        {
			get
            {
				return pool.Count;
            }
        }

		public GenericPool(System.Func<T> createCopyFunction, System.Action<T> onReturnedToPoolCallback = null)
		{
			Initialized( createCopyFunction: createCopyFunction, onReturnedToPoolCallback);
		}

		
		private void Initialized( System.Func<T> createCopyFunction, System.Action<T> onReturnedToPoolCallback = null)
        {
			//this.poolId = poolId;
			
			this.createCopyFunction = createCopyFunction;
			this.onReturnedToPoolCallback = onReturnedToPoolCallback;

			if (createCopyFunction == null)
			{
				throw new System.Exception("createCopyFunction shouldn't be null. This function should specify how a new copy will be generated");
			}
		}


		public void PreCache(uint preCacheCount = 5)
        {
			for (int i = 0; i < preCacheCount; i++)
			{
				var copy = createCopyFunction();
				pool.Enqueue(copy);

			}
		}

		public T FetchFromPool()
        {
			
			if(pool.Count > 0)
            {
				var obj =  pool.Dequeue();
				
				return obj;

            }
			else
            {
				var obj = createCopyFunction();
				
				return obj;
            }

			
        }

		public void ReturnToPool(T obj)
        {
			pool.Enqueue(obj);
			if (onReturnedToPoolCallback != null)
			{
				onReturnedToPoolCallback(obj);
			}
			
        }


		public void Clean()
        {
			pool.Clear();

		}
		
		
	}
}
