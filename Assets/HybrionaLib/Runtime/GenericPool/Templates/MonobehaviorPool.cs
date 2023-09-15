/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  MonobehaviorPool.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  15-09-2023 13:05:26

*************************************************************************/
using UnityEngine;

namespace Hybriona
{
	public class MonobehaviorPool : MonoBehaviour 
	{

		
		public GenericPool<MonobehaviorPool> pool {  get; set; }
		public GenericPool<MonobehaviorPool> RegisterPool()
        {
			gameObject.SetActive(false);

			pool = new GenericPool<MonobehaviorPool>(createCopyFunction: () =>
			{
				GameObject o = Instantiate(this.gameObject);
				var script = o.GetComponent<MonobehaviorPool>();
				script.pool = this.pool;

				return script;
			},onReturnedToPoolCallback: (obj) =>
			{

				obj.OnReturnToPool();

			});

			pool.PreCache(10);
			
			return pool;
			

		}


		public void ReturnToPool()
        {
			
			pool.ReturnToPool(this);
		}


		public virtual void Activate()
        {
			gameObject.SetActive(true);
        }

		public virtual void OnReturnToPool()
        {
			gameObject.SetActive(false);
        }
	}
}
