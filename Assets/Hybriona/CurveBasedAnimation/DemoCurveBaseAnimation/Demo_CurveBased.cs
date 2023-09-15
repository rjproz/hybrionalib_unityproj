using UnityEngine;
using System.Collections;

public class Demo_CurveBased : MonoBehaviour {

	// Use this for initialization
	public Transform subject;
	public HybAnimationCurveXYZ curve;
	public HybAnimationCurveXYZ curveSin;
	IEnumerator Start () {
	
		yield return new WaitForSeconds(4);
		HybAnimationID idLocalPos = subject.Animate(HybCBAnimation.Mode.LocalPosition,subject.position + Vector3.up *6f,curveSin,3,0,false,()=>{
		
			Debug.Log("Animation Ended");
		});

		HybAnimationID idLocalEuler = subject.Animate(HybCBAnimation.Mode.LocalEulers,new Vector3(60,190,-30),curveSin,1,0,true);

		HybAnimationID idLocalScale = subject.Animate(HybCBAnimation.Mode.LocalScale,new Vector3(2,2,2),curveSin,2,0,true);

		yield return new WaitForSeconds(15);
		idLocalPos.StopAnimation();

	}

}
