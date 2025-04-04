/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  GOPoolManager.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  15-09-2023 18:39:07

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class GOPoolManager : HybSingleton<GOPoolManager>
	{
		private static Dictionary<string, GenericPool<MonobehaviorPoolObject>> pools = new Dictionary<string, GenericPool<MonobehaviorPoolObject>>();

		/// <summary>
        /// Register GameObject Pool
        /// </summary>
        /// <param name="poolId">Unique Pool Id</param>
        /// <param name="prefabObj">Source Prefab Object</param>
        /// <param name="preCache">Number of copies preloaded</param>
		public static void RegisterPool(string poolId,GOPoolObject prefabObj,uint preCache = 0)
        {
			var pool = prefabObj.RegisterPool();
			

			if(pools.ContainsKey(poolId))
            {
				pools.Add(poolId, pool);

			}
			else
            {
				pools[poolId] = pool;

			}
			if (preCache > 0)
			{
				pool.PreCache(preCache);
			}
		}


		public static void RemovePool(string poolId)
        {
			if(ContainsPool(poolId))
            {
				var pool = pools[poolId];
				pool.Clean(); 
				pools.Remove(poolId);
				System.GC.Collect();
            }
        }




		
		public static bool ContainsPool(string poolId)
        {
			return pools.ContainsKey(poolId);
        }

		public static GenericPool<MonobehaviorPoolObject> GetPool(string poolID)
        {
			if(ContainsPool(poolID))
            {
				return pools[poolID];

			}
			throw new System.Exception("Pool doesn't exist with poolId : " + poolID);
		}

		public static MonobehaviorPoolObject GetFromPool(string poolID)
        {
			if(ContainsPool(poolID))
            {
				var obj = (GOPoolObject) pools[poolID].FetchFromPool();
				return obj;
            }
			throw new System.Exception("Pool doesn't exist with poolId : "+poolID);
        }

		public static void ReturnToPool(GOPoolObject obj)
		{
			obj.ReturnToPool();
		}


	}
}

