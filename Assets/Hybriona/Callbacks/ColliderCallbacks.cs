using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hybriona
{
	public delegate void OnMouseUpAsButton();
	public delegate void OnMouseUp();
	public delegate void OnMouseDown();

	public class ColliderCallbacks : MonoBehaviour 
	{
		public OnMouseUpAsButton onMouseUpAsButton;
		public OnMouseUp onMouseUp;
		public OnMouseDown onMouseDown;

		void OnMouseUpAsButton()
		{
			if(onMouseUpAsButton != null)
			{
				onMouseUpAsButton();
			}
		}

		void OnMouseUp()
		{
			if(onMouseUp != null)
			{
				onMouseUp();
			}
		}

		void OnMouseDown()
		{
			if(onMouseDown != null)
			{
				onMouseDown();
			}
		}


	}


}
