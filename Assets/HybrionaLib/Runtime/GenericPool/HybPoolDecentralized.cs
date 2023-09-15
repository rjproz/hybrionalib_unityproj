using UnityEngine;
using System.Collections;

public class HybPoolDecentralized : MonoBehaviour {

	// Use this for initialization

	[HideInInspector]public string poolID;
	public bool autoDestroy = false;
	public float lifeIfAutoDestroy = 5;

	public virtual void Activate()
	{
		gameObject.SetActive(true);
		if(autoDestroy)
		{
			StartCoroutine(AutoDestroy());
		}
	}

	public virtual void Deactivate()
	{
		gameObject.SetActive(false);
	}
	public virtual void DeactivateToPool ()
	{
		Deactivate();
		HybPoolManager.Instance.PutInPool(poolID,this);
	}


	IEnumerator AutoDestroy()
	{
		yield return new WaitForSeconds(lifeIfAutoDestroy);
		DeactivateToPool ();
	}
}
