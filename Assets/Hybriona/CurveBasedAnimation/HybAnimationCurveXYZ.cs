using UnityEngine;
using System.Collections;

[System.Serializable]
public class HybAnimationCurveXYZ
{
	public AnimationCurve x;
	public AnimationCurve y;
	public AnimationCurve z;

	public HybAnimationCurveXYZ()
	{
		x = get();
		y = get();
		z = get();
	}

	private static HybAnimationCurveXYZ _defaultCurve;
	public static HybAnimationCurveXYZ defaultcurve
	{
		get
		{
			if(_defaultCurve == null)
			{
				_defaultCurve = new HybAnimationCurveXYZ();
			}
			return _defaultCurve;
		}
	}

	private static AnimationCurve get()
	{
		AnimationCurve curve = new AnimationCurve();
		Keyframe [] frames = new Keyframe[2];
		frames[0].time = 0;
		frames[0].value = 0;
		
		frames[1].time = 1;
		frames[1].value = 1;
	
		curve.keys = frames;
		return curve;
	}



	public void CorrectTheCurveEnds()
	{
		correct(x);
		correct(y);
		correct(z);
	}

	private void correct(AnimationCurve curve)
	{
		if(curve.keys.Length >= 2)
		{
			Keyframe [] frames = curve.keys;
			frames[0].time = 0;
			frames[0].value = 0;

			frames[curve.keys.Length - 1].time = 1;
			frames[curve.keys.Length - 1].value = 1;
		}
	}

	public void PrepareForLoop()
	{
		if( x.postWrapMode != WrapMode.PingPong)
		{
			x.postWrapMode = WrapMode.PingPong;
		}

		if( y.postWrapMode != WrapMode.PingPong)
		{
			y.postWrapMode = WrapMode.PingPong;
		}

		if( z.postWrapMode != WrapMode.PingPong)
		{
			z.postWrapMode = WrapMode.PingPong;
		}
	}
	
}