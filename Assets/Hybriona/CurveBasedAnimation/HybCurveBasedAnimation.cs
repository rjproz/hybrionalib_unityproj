using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HybCBAnimation : MonoBehaviour   
{

	private static HybCBAnimation instance;
	public static HybCBAnimation Instance
	{
		get
		{
			if(instance == null)
			{
				instance = (new GameObject("HybCBAnimation")).AddComponent<HybCBAnimation>();
				instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
			}
			return instance;
		}
	}



	private Dictionary<int,HybAnimationID> pool = new Dictionary<int,HybAnimationID>();

	public enum Mode {LocalPosition,LocalScale,LocalEulers,Position,Eulers};

	private int m_animationCounter = 100;

	public void StopAnimation(HybAnimationID id)
	{
		if(pool.ContainsKey(id.index))
		{
			StopCoroutine(id.routine);
			pool.Remove(id.index);
		}

	}

	public HybAnimationID Animate(Transform m_transform,Mode mode, Vector3 target,HybAnimationCurveXYZ curve,float animationLength,float delay,bool loop,System.Action callback)
	{
		if(loop)
		{
			curve.PrepareForLoop();
		}
		curve.CorrectTheCurveEnds();
		m_animationCounter++;
		


		Coroutine routine = StartCoroutine(_Animate( m_transform, mode,  target, curve, animationLength,delay, loop,m_animationCounter, callback));
		HybAnimationID id = new HybAnimationID(m_animationCounter,routine);
		pool.Add(m_animationCounter,id);
		return id;
	}


	IEnumerator _Animate(Transform m_transform,Mode mode,Vector3 target,HybAnimationCurveXYZ curve,float animationLength,float delay,bool loop,int id,System.Action callback)
	{
		if(delay > 0)
		{
			yield return new WaitForSeconds(delay);
		}

		Vector3 from = Vector3.zero;
		if(mode == Mode.Eulers)
		{
			from = m_transform.eulerAngles;
		}
		else if(mode == Mode.LocalEulers)
		{
			from = m_transform.eulerAngles;
		}
		else if(mode == Mode.LocalPosition)
		{
			from = m_transform.localPosition;
		}
		else if(mode == Mode.LocalScale)
		{
			from = m_transform.localScale;
		}
		else if(mode == Mode.Position)
		{
			from = m_transform.position;
		}

		float timeStarted = Time.time;
		float elapsed = 0;
		while((elapsed = Time.time - timeStarted) < animationLength || loop)
		{
			float x = elapsed / animationLength;

			float tX = curve.x.Evaluate(x);
			float tY = curve.y.Evaluate(x);
			float tZ = curve.z.Evaluate(x);
		
			Vector3 t = Vector3.zero;
			if(mode != Mode.Eulers && mode != Mode.LocalEulers)
			{
				t.x = from.x + (target.x - from.x) * tX;
				t.y = from.y + (target.y - from.y) * tY;
				t.z = from.z + (target.z - from.z) * tZ;
			}
			else
			{
				
				t.x = Mathf.LerpAngle(from.x,target.x * Mathf.Sign(tX),Mathf.Abs(tX));
				t.y = Mathf.LerpAngle(from.y,target.y * Mathf.Sign(tY),Mathf.Abs(tY));
				t.z = Mathf.LerpAngle(from.z,target.z * Mathf.Sign(tZ),Mathf.Abs(tZ));


			}

			Apply(mode,m_transform,t);
			yield return new WaitForEndOfFrame();
		}
		Apply(mode,m_transform,target);
		pool.Remove(id);

		if(callback != null)
		{
			callback();
		}
	}

	private void Apply(Mode mode,Transform m_transform,Vector3 target)
	{
		if(mode == Mode.Eulers)
		{
			m_transform.eulerAngles = target;
		}
		else if(mode == Mode.LocalEulers)
		{
			m_transform.eulerAngles = target;
		}
		else if(mode == Mode.LocalPosition)
		{
			m_transform.localPosition = target;
		}
		else if(mode == Mode.LocalScale)
		{
			m_transform.localScale = target;
		}
		else if(mode == Mode.Position)
		{
			m_transform.position = target;
		}
	}
}


