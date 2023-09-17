using UnityEngine;
using UnityEngine.UI;


namespace Hybriona
{
	public class VersionToText : MonoBehaviour {

		// Use this for initialization
		void Start () {
			GetComponent<Text>().text= VersionManager.Version().ToString();
		}
		

	}
}
