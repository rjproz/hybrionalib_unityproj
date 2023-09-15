using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Hybriona.Private
{
	public class SendFeedBack : MonoBehaviour {


		private static SendFeedBack _instance;
		public static SendFeedBack Instance
		{
			get
			{
				if(_instance == null)
				{
					GameObject o = GameObject.Instantiate( Resources.Load<GameObject>("HybFeedBackForm"));
					o.SetActive(false);
					_instance = o.GetComponent<SendFeedBack>();
					_instance.Init();
				}
				return _instance;
			}
		}

		public CanvasGroup inputGroup;
		public InputField useremail;
		public InputField usermessage;



		private HttpHeaders headers = null;
		public void Init()
		{
			if(headers == null)
			{
				headers = new HttpHeaders();
				headers.ContentTypeJson();
			}

		}


		public void Send()
		{

			string _useremail = useremail.text;
			string _usermessage = usermessage.text;
			
			FeedbackForm form = new FeedbackForm();
			form.user_email = _useremail;
			form.user_message = _usermessage;
			form.FillDefaults();

			inputGroup.interactable = false;
			RestClient.Instance.Post("http://apis.hybriona.com/hybriona/app/feedback", form.ToJson().ToBytes() ,headers.headers(),delegate(HttpResponse www){
			
				if(!www.isHttpError && !www.isNetworkError && string.IsNullOrEmpty(www.error))
				{
					useremail.text = "";
					usermessage.text = "";
					Debug.Log(www.text);
				}
				else
				{
					if(!string.IsNullOrEmpty(www.text))
					{
						Debug.LogError(www.text);
					}
					else
					{
						Debug.LogError(www.error);
					}
				}
				inputGroup.interactable = true;
			});


		}


		public void Show()
		{
			gameObject.SetActive(true);
		}
		public void Close()
		{
			gameObject.SetActive(false);
		}
	}
}