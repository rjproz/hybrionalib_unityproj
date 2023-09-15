using UnityEngine;
using System.Collections;

public static class HybCBanimationExtension  {

	public static HybAnimationID Animate(this Transform m_transform,HybCBAnimation.Mode mode, Vector3 target,HybAnimationCurveXYZ curve,float animationLength,float delay,bool loop,System.Action callback = null)
	{
		return HybCBAnimation.Instance.Animate( m_transform,mode,  target, curve, animationLength,delay, loop,callback);
	}
}
