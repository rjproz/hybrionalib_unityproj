#if HYBRIONA_LIB_ENABLE_HTTP_CLIENT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Hybriona
{
	public class RestClient : HybSingleton<RestClient> {

		void Start()
		{
			GameObject.DontDestroyOnLoad(gameObject);
		}

#region Using WWW
		/*
		public delegate void OnComplete(WWW www);
		public WWW Post(string url,string data,Dictionary<string,string> headers,OnComplete onComplete = null)
		{
			
			return Post(url,System.Text.Encoding.ASCII.GetBytes(data),headers,onComplete);
		}
		public WWW Post(string url,byte [] data,Dictionary<string,string> headers,OnComplete onComplete= null)
		{
			
			WWW www = null;
			if(headers == null)
			{
				www = new WWW(url,data);
			}
			else
			{
				www = new WWW(url,data,headers);
			}

			if(!Application.isEditor || Application.isPlaying)
			{
				StartCoroutine(ProcessWWW(www,onComplete));

			}
			return www;
		}

		public WWW Get(string url,Dictionary<string,string> headers,OnComplete onComplete = null)
		{
			WWW www = null;
			if(headers == null)
			{
				www = new WWW(url);
			}
			else
			{
				www = new WWW(url,null,headers);
			}

			if(!Application.isEditor || Application.isPlaying)
			{
				StartCoroutine(ProcessWWW(www,onComplete));

			}
			return www;
		}



		IEnumerator ProcessWWW(WWW www, OnComplete onComplete)
		{
			yield return www;
			onComplete(www);
		}
		*/
#endregion

#region using UnityWebrequest

		public delegate void OnCompleteRequest(HttpResponse response);

		public void Process(string url,string method,byte [] data,Dictionary<string,string> headers = null,OnCompleteRequest onComplete = null)
		{
			UnityWebRequest request = new UnityWebRequest();

			request.disposeDownloadHandlerOnDispose = true;
			request.disposeUploadHandlerOnDispose = true;

			request.url = url;
			request.method = method;


			if(data != null && data.Length > 0)
			{
				request.uploadHandler = new UploadHandlerRaw(data);
				
			}
			if(headers != null)
			{
				foreach(KeyValuePair<string,string> header in headers)
				{
					request.SetRequestHeader(header.Key,header.Value);
				}
			}
			request.downloadHandler = new DownloadHandlerBuffer();

			UnityWebRequestAsyncOperation operation = request.SendWebRequest();
			if(!Application.isEditor || Application.isPlaying)
			{
				StartCoroutine(ProcessWebRequest(operation,onComplete));
			}
			else
			{
				
				while(!operation.isDone)
				{
					
				}
				if(onComplete != null)
				{
					onComplete(new HttpResponse( operation.webRequest , -1) );
				}
				operation.webRequest.Dispose();

			}
				


		}
		IEnumerator ProcessWebRequest(UnityWebRequestAsyncOperation operation, OnCompleteRequest onComplete)
		{
			float timeStarted = Time.time;
			yield return operation;

			if(onComplete != null)
			{
				onComplete(new HttpResponse( operation.webRequest , Time.time - timeStarted ) );
			}
			operation.webRequest.Dispose();
		}


		public void Get(string url,Dictionary<string,string> headers = null,OnCompleteRequest onComplete = null)
		{
			Process(url,UnityWebRequest.kHttpVerbGET,null,headers,onComplete);
		}
		public void Delete(string url,Dictionary<string,string> headers = null,OnCompleteRequest onComplete = null)
		{
			Process(url,UnityWebRequest.kHttpVerbDELETE,null,headers,onComplete);
		}

		public void Post(string url,byte [] data,Dictionary<string,string> headers = null,OnCompleteRequest onComplete = null)
		{
			Process(url,UnityWebRequest.kHttpVerbPOST,data,headers,onComplete);
		}

		public void Put(string url,byte [] data,Dictionary<string,string> headers = null,OnCompleteRequest onComplete = null)
		{
			Process(url,UnityWebRequest.kHttpVerbPUT,data,headers,onComplete);
		}

		public void Head(string url,Dictionary<string,string> headers = null,OnCompleteRequest onComplete = null)
		{
			Process(url,UnityWebRequest.kHttpVerbHEAD,null,headers,onComplete);
		}


#endregion

	}
	public static class RestClientExtensions
	{
		public static byte [] ToBytes(this string data)
		{
			return System.Text.Encoding.UTF8.GetBytes(data);
		}
	}
}
#endif
