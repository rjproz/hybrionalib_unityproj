/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  PhysicalBall.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  15-09-2023 15:43:11

*************************************************************************/
using System.Collections;
using UnityEngine;

namespace Hybriona
{
    public class PhysicsBall : MonobehaviorPoolObject 
	{

        public Rigidbody rigidbody;
        public float life = 10;
        public override void Activate()
        {
            gameObject.SetActive(true);
            StartCoroutine(Lifecycle());
        }

        public void ThrowRandomUp()
        {
            var direction = new Vector3(Random.Range(-.1f, .1f), 1,Random.Range(-.1f,.1f));
            rigidbody.velocity = direction * 30;
        }
        public override void OnReturnToPool()
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            gameObject.SetActive(false);
           
        }

        IEnumerator Lifecycle()
        {
            yield return new WaitForSeconds(life);
            ReturnToPool();
        }

    }
}
