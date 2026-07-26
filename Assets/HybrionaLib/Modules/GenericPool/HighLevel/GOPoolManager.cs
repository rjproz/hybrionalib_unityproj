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
using UnityEngine.SceneManagement;
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
		public static void RegisterPool(string poolId, GOPoolObject prefabObj, uint preCache = 0)
		{
			var pool = prefabObj.RegisterPool();


			pools[poolId] = pool;
			if (preCache > 0)
			{
				pool.PreCache(preCache);
			}
		}


		public static void RemovePool(string poolId)
		{
			if (pools.TryGetValue(poolId, out var pool))
			{
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
			if (pools.TryGetValue(poolID, out var pool))
			{
				return pool;

			}
			throw new System.Exception("Pool doesn't exist with poolId : " + poolID);
		}

		public static MonobehaviorPoolObject GetFromPool(string poolID)
		{
			if (pools.TryGetValue(poolID, out var pool))
			{
				var obj = (GOPoolObject)pool.FetchFromPool();
				return obj;
			}
			throw new System.Exception("Pool doesn't exist with poolId : " + poolID);
		}

		public static void ReturnToPool(GOPoolObject obj)
		{
			obj.ReturnToPool();
		}


		private static void CleanAll()
		{
			foreach (var poolPair in pools)
			{
				poolPair.Value.Clean();
			}

			pools.Clear();

			System.GC.Collect();
		}

        private void Awake()
        {
			
			SceneManager.sceneUnloaded += (Scene scene) =>
			{
				CleanAll();
			};
        }

    }
}

