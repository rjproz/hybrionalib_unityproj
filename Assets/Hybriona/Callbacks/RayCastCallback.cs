/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  RayCastCallback.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  06/30/2018 11:42:24

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public static class RayCastCallback 
	{
		private static Queue<RayCastChecker> pool = new Queue<RayCastChecker>();
		private static Dictionary<int,RayCastChecker> activeSessions = new Dictionary<int, RayCastChecker>();
		private static int nextID = 1;
		public static int AddRayCastSession(this Transform source,YieldInstruction waitInstruction,float checkDistance,LayerMask mask,OnRayCastHitEvent callback)
		{
			
			RayCastChecker o = null;
			if(pool.Count > 0 )
			{
				o = pool.Dequeue();
			}
			else
			{
				GameObject go = new GameObject("RayCastChecker");
				GameObject.DontDestroyOnLoad(go);
				o = go.AddComponent<RayCastChecker>();

			}
			o.StartCheck(source,waitInstruction,checkDistance,mask,callback);
			int id = nextID;
			activeSessions.Add(id,o);
			nextID++;
			return id;
		}

		public static void RemoveSession(int id)
		{
			if(activeSessions.ContainsKey(id))
			{
				RayCastChecker o = activeSessions[id];
				o.StopAllCoroutines();
				pool.Enqueue(o);
				activeSessions.Remove(id);
			}
		}

	}


	public class RayCastChecker : MonoBehaviour
	{
		private Transform _source;
		private LayerMask _mask;
		private YieldInstruction _waitInstruction;
		private float _checkDistance;
		private OnRayCastHitEvent _callback;
		public void StartCheck(Transform source,YieldInstruction waitInstruction,float checkDistance,LayerMask mask,OnRayCastHitEvent callback)
		{
			_source = source;
			_mask = mask;
			_waitInstruction = waitInstruction;
			_checkDistance = checkDistance;
			_callback = callback;
			StartCoroutine(Process());
		}
			

		private IEnumerator Process()
		{
			while(true)
			{
				Ray ray = new Ray(_source.position,_source.forward);
				RaycastHit hit;
				if(Physics.Raycast(ray,out hit,_checkDistance,_mask))
				{
					if(_callback != null)
					{
						_callback(hit);
					}
				}
				yield return _waitInstruction;
			}
		}


	}
	public delegate void OnRayCastHitEvent(RaycastHit hit);

}
