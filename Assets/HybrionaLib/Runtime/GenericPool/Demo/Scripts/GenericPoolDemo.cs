/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  GenericPoolDemo.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  15-09-2023 12:49:22

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class GenericPoolDemo : MonoBehaviour 
	{
		
		void Start()
		{
			StartCoroutine(CustomClassPoolingTest());
			StartCoroutine(MonobehaviorDemo());
			
		}
        #region Template Demo
        public PhysicsBall ballPrefab;
		IEnumerator MonobehaviorDemo()
        {
			ballPrefab.gameObject.SetActive(false);
			var pool = ballPrefab.RegisterPool();
			while (true)
			{

				yield return new WaitForSeconds(Random.Range(.2f, 1f));
				//var script = (PhysicsBall)GenericPoolManager<MonobehaviorPool>.GetPool(ballPrefab.poolId).FetchFromPool();
				var script = (PhysicsBall)pool.FetchFromPool();
				script.transform.position = ballPrefab.transform.position; // set position
				script.Activate();
				script.ThrowRandomUp();

			}
		}

        #endregion

        #region Custom Class Caching

        public List<Processor> processors = new List<Processor>();
		private GenericPool<Processor> processorPool;


		public int processorPoolCount;
		public int totalCreatedCount;

		IEnumerator CustomClassPoolingTest()
		{
			processorPool = new GenericPool<Processor>( () =>
			{
				totalCreatedCount = totalCreatedCount + 1;
				return new Processor();
			});


			while (true)
			{
				yield return new WaitForSeconds(Random.Range(.2f, 1f));
				processorPoolCount = processorPool.poolCount;
				var progessor = processorPool.FetchFromPool();
				progessor.Start(Random.Range(1, 10));
				processors.Add(progessor);
			}
		}

		private void Update()
        {
            for(int i = processors.Count -1; i >= 0;i--)
            {
				var processor = processors[i];
				processor.Update();
				if(processor.progress >= 1)
                {
					processorPool.ReturnToPool(processor);
					processors.RemoveAt(i);
                }
            }
        }

		[System.Serializable]
        public class Processor
        {
			public string id;
			public float progress;
			
			private float timeOut;
			private float timeStarted;
			public void Start(float timeOut)
            {
				timeStarted = Time.time;
				this.timeOut = timeOut;
				id = System.DateTime.Now.ToFileTime().ToString();
            }

			public void Update()
            {
				progress = (Time.time - timeStarted) / timeOut;
            }
        }

        #endregion

    }
}
