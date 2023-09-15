using UnityEngine;
using System.Collections;

public struct HybAnimationID  {


	public HybAnimationID(int _index,Coroutine _routine)
	{
		index = _index;
		routine = _routine;
	}
	public int index
	{
		get;
		private set;
	}
	public Coroutine routine
	{
		get;
		private set;
	}

	public void StopAnimation()
	{
		HybCBAnimation.Instance.StopAnimation(this);
	}
}
