/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  ValueAnimation.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  RJproz
 *  Date         :  07/06/2018 17:50:36

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class ValueAnimation : HybSingleton<ValueAnimation> 
	{
		public AnimationCurve defaultCurve;

		private Dictionary<int,Coroutine> activeSessions = new Dictionary<int, Coroutine>();
		private int id = 0;



		public void OnInstantiated ()
		{
			defaultCurve = new AnimationCurve();
			defaultCurve.AddKey(new Keyframe(0,0));
			defaultCurve.AddKey(new Keyframe(1,1));
			//Debug.Log("OnInstantiated");
		}
		void Awake () 
		{
            OnInstantiated();

        }

		//Animate Float
		public int Animate(float from,float to,float timeLength,YieldInstruction waitInstruction,AnimationCurve curve, System.Action<float> onValueEmitted)
		{
			id++;
			Coroutine routine = StartCoroutine(AnimateFloat(id,from,to,timeLength,waitInstruction,curve, onValueEmitted));

			activeSessions.Add(id,routine);
			return id;
		}

		IEnumerator AnimateFloat(int id,float from,float to,float timeLength,YieldInstruction waitInstruction,AnimationCurve curve, System.Action<float> onValueEmitted)
		{
			if(curve == null)
            {
				curve = defaultCurve;
            }
			float elapsed = 0;
			float timeStarted = Time.time;
			while((elapsed = Time.time - timeStarted) <= timeLength)
			{
				onValueEmitted(Mathf.Lerp(from,to,curve.Evaluate(elapsed/timeLength)));
				yield return waitInstruction;
			}
			onValueEmitted(to);
			activeSessions.Remove(id);
		}


		//Animate Vector3
		public int Animate(Vector3 from,Vector3 to,float timeLength,YieldInstruction waitInstruction,AnimationCurve curve,System.Action<Vector3> onValueEmitted)
		{
			
			id++;
			Coroutine routine = StartCoroutine(AnimateVec3(id,from,to,timeLength,waitInstruction,curve, onValueEmitted));

			activeSessions.Add(id,routine);
			return id;
		}

		IEnumerator AnimateVec3(int id,Vector3 from,Vector3 to,float timeLength,YieldInstruction waitInstruction,AnimationCurve curve, System.Action<Vector3> onValueEmitted)
		{
			if (curve == null)
			{
				curve = defaultCurve;
			}

			float elapsed = 0;
			float timeStarted = Time.time;
			while((elapsed = Time.time - timeStarted) <= timeLength)
			{
				onValueEmitted(Vector3.Lerp(from,to,curve.Evaluate(elapsed/timeLength)));
				yield return waitInstruction;
			}
			onValueEmitted(to);
			activeSessions.Remove(id);
		}


		// Session Manipulation
		public void StopSession(int id)
		{
			if(activeSessions.ContainsKey(id))
			{
				StopCoroutine(activeSessions[id]);
				activeSessions.Remove(id);
			}
		}



		
	}
	

}
