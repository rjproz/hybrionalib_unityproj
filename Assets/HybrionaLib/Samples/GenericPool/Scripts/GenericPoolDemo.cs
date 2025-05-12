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
			StartCoroutine(DirectReferenceDemo());
		}

#region Direct Reference Demo
		public Rigidbody cubeRigidBodyPrefab;
		private GenericPool<Rigidbody> cubeRigidBodyPool;
		IEnumerator DirectReferenceDemo()
		{
			cubeRigidBodyPrefab.gameObject.SetActive(false);

			cubeRigidBodyPool = new GenericPool<Rigidbody>(createCopyFunction: () =>
			{
				return Instantiate(cubeRigidBodyPrefab.gameObject).GetComponent<Rigidbody>();
			},onReturnedToPoolCallback: (rb) =>{

				//Reset Values
				rb.linearVelocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				rb.gameObject.SetActive(false);
			});

			while (true)
			{

				yield return new WaitForSeconds(Random.Range(.2f, 1f));
				
				var rb = cubeRigidBodyPool.FetchFromPool();
				rb.transform.position = cubeRigidBodyPrefab.transform.position; // set position
				rb.gameObject.SetActive(true);
				rb.linearVelocity = new Vector3(Random.Range(-.1f, .1f), 1, Random.Range(-.1f, .1f)) * 30;

				StartCoroutine(CustomLife(rb, 10));
			}
		}

		IEnumerator CustomLife(Rigidbody rb,float life)
        {
			yield return new WaitForSeconds(life);
			cubeRigidBodyPool.ReturnToPool(rb);

		}
#endregion

#region Template Demo
		public PhysicsBall ballPrefab;
		IEnumerator MonobehaviorDemo()
        {
			ballPrefab.gameObject.SetActive(false);
			var pool = ballPrefab.RegisterPool();
			
			while (true)
			{

				yield return new WaitForSeconds(Random.Range(.2f, 1f));
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
			processorPool = new GenericPool<Processor>(createCopyFunction:() =>
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

