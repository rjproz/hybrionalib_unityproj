/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  GenericPool.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  15-09-2023 12:05:42

*************************************************************************/

using System.Collections.Generic;

namespace Hybriona
{
	public class GenericPool<T> 
	{
		//public string poolId { get; private set; }

		public System.Func<bool> notNullRule { get; private set; }
		public System.Action destroyProcedure { get;  private set;}
		private System.Action<T> onReturnedToPoolCallback;
		private System.Func<T> createCopyFunction;


		
		private Queue<T> pool = new Queue<T>();

		public int totalCopyGenerated { get; private set; }

		public int totalActiveObjects
        {
			get
            {
				return totalCopyGenerated - poolCount;
            }
        }

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
				totalCopyGenerated++;
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
				totalCopyGenerated++;
				return obj;
            }

			
        }

		public void ReturnToPool(T obj)
        {
			if (!pool.Contains(obj))
			{
				pool.Enqueue(obj);
				if (onReturnedToPoolCallback != null)
				{
					onReturnedToPoolCallback(obj);
				}
			}
			
        }

		public bool ObjectInsidePool(T obj)
        {
			return pool.Contains(obj);
		}


		public void Clean()
        {
			pool.Clear();

		}
		
		
	}
}

