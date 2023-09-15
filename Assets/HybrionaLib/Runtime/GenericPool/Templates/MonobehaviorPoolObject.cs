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
	public class MonobehaviorPoolObject : MonoBehaviour 
	{

		
		public GenericPool<MonobehaviorPoolObject> pool {  get; set; }
		public GenericPool<MonobehaviorPoolObject> RegisterPool()
        {
			gameObject.SetActive(false);

			pool = new GenericPool<MonobehaviorPoolObject>(createCopyFunction: () =>
			{
				GameObject o = Instantiate(this.gameObject);
				var script = o.GetComponent<MonobehaviorPoolObject>();
				script.pool = this.pool;

				return script;
			},onReturnedToPoolCallback: (obj) =>
			{

				obj.OnReturnToPool();

			});

			
			
			return pool;
			

		}


		public bool ObjectInsidePool(MonobehaviorPoolObject obj)
		{
			return pool.ObjectInsidePool(obj);

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
