using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hybriona
{
	public class UIVisibility : MonoBehaviour 
	{
		public bool active {get; private set;}
        public float timeLength { get; set; }
        #region Visibility Alpha
        private CanvasGroup _canvasGroup;
		public CanvasGroup canvasGroup
		{
			get
			{
				if(_canvasGroup == null)
				{
					_canvasGroup = GetComponent<CanvasGroup>();
					if(_canvasGroup == null)
					{
						_canvasGroup = gameObject.AddComponent<CanvasGroup>();
					}
				}
				return _canvasGroup;
			}
		}

		#endregion

		#region Visibility Scale
		private List<Transform> _childrens = null;
		private List<UIVisibility> visiChilds =  new List<UIVisibility>();
		public List<Transform> childrens
		{
			get
			{
				if(_childrens == null)
				{
					_childrens = new List<Transform>();
					for(int i=0;i<transform.childCount;i++)
					{
						Transform child = transform.GetChild(i);
						UIVisibility o = child.GetComponent<UIVisibility>();
						if(o)
						{
							visiChilds.Add(o);
						}
						else
						{
							_childrens.Add(child);
						}
					}
				}
				return _childrens;
			}
		}








		#endregion

		#region Core
		public enum Mode {Scale, CanvasGroup};
		public Mode mode = Mode.CanvasGroup;
		public void Hide(bool instant = false,float delay = 0)
		{
			active = false;
			if(instant)
			{
				if(mode == Mode.Scale)
				{
					for(int i=0;i<childrens.Count;i++)
					{
						childrens[i].localScale = Vector3.zero;
					}
				}
				else if(mode == Mode.CanvasGroup)
				{
					canvasGroup.alpha = 0;
					canvasGroup.interactable = false;
					canvasGroup.blocksRaycasts = false;
				}
			}
			else
			{
				StartCoroutine(Lerp(0,delay));

				/*if(mode == Mode.Scale)
				{
					
				}
				else if(mode == Mode.CanvasGroup)
				{

				}
				*/
			}

			if(mode == Mode.Scale)
			{
				for(int i=0;i<visiChilds.Count;i++)
				{
					visiChilds[i].Hide(instant);
				}
			}
		}

		public void Show(float delay = 0)
		{
			active = true;
			StartCoroutine(Lerp(1,delay));
			if(mode == Mode.Scale)
			{
				for(int i=0;i<visiChilds.Count;i++)
				{
					visiChilds[i].Show();
				}
			}
		}


		IEnumerator Lerp(float target = 0,float delay = 0)
		{
			if(delay > 0)
			{
				yield return new WaitForSeconds(delay);
			}
			float timeStarted = Time.time;
			float elapsed = 0;

			Vector3 value = Vector3.one * target;

            float from = canvasGroup.alpha;

			while( (elapsed = Time.time - timeStarted) <= timeLength)
			{
				float time = elapsed/timeLength;
				if(mode == Mode.Scale)
				{
					for(int i=0;i<childrens.Count;i++)
					{
						childrens[i].localScale = Vector3.Lerp( childrens[i].localScale,value, time);
					}
				}
				else if(mode == Mode.CanvasGroup)
				{
					canvasGroup.alpha = Mathf.Lerp(from, target,time);
				}
				yield return null;
			}

			if(mode == Mode.Scale)
			{
				for(int i=0;i<childrens.Count;i++)
				{
					childrens[i].localScale = value;
				}
			}
			else if(mode == Mode.CanvasGroup)
			{
				canvasGroup.alpha = target;
				canvasGroup.interactable = (target == 1);
				canvasGroup.blocksRaycasts = (target == 1);
			}

		}
		#endregion

	}
}