using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Hybriona
{
	public class RestClient {



		public static async Task<UnityWebRequest> Get(string url,Dictionary<string,string> headers = null)
        {
			var request = UnityWebRequest.Get(url);
		
			AddHeadersToRequest(headers, request);
			return await ProcessRequest(request);
        }

		public static async Task<UnityWebRequest> Head(string url, Dictionary<string, string> headers = null)
		{
			var request = UnityWebRequest.Head(url);
			AddHeadersToRequest(headers, request);
			return await ProcessRequest(request);
		}

		public static async Task<UnityWebRequest> Delete(string url, Dictionary<string, string> headers = null)
		{
			var request = UnityWebRequest.Delete(url);
			AddHeadersToRequest(headers, request);
			return await ProcessRequest(request);
		}

		public static async Task<UnityWebRequest> Put(string url,byte [] data, Dictionary<string, string> headers = null)
		{
			var request = UnityWebRequest.Put(url,data);
			AddHeadersToRequest(headers, request);
			return await ProcessRequest(request);
		}

		public static async Task<UnityWebRequest> Post(string url, string data, Dictionary<string, string> headers = null)
		{
			return await Post(url,data.ToBytes(),headers);
		}

		public static async Task<UnityWebRequest> Post(string url, byte [] data, Dictionary<string, string> headers = null)
		{
			var request = new UnityWebRequest(url, "POST");
			request.uploadHandler = new UploadHandlerRaw(data);
			request.downloadHandler = new DownloadHandlerBuffer();
			AddHeadersToRequest(headers, request);
			return await ProcessRequest(request);
		}

		public static async Task<UnityWebRequest> Post(string url, WWWForm form, Dictionary<string, string> headers = null)
		{
			var request = UnityWebRequest.Post(url, form);
			AddHeadersToRequest(headers, request);
			return await ProcessRequest(request);
		}

		public static async Task<UnityWebRequest> ProcessRequest(UnityWebRequest webRequest)
		{
			webRequest.disposeDownloadHandlerOnDispose = true;
			webRequest.disposeUploadHandlerOnDispose = true;
			var operation = webRequest.SendWebRequest();
			while (!operation.isDone)
			{
				await Task.Yield();
			}
			return webRequest;
		}

		private static void AddHeadersToRequest(Dictionary<string,string> headers,UnityWebRequest request)
        {
			if (headers != null)
			{
				foreach (var header in headers)
                {
					request.SetRequestHeader(header.Key, header.Value);
                }
			}
		}

		

	}
	public static class RestClientExtensions
	{
		public static byte [] ToBytes(this string data)
		{
			return System.Text.Encoding.UTF8.GetBytes(data);
		}

		public static async void Callback(this Task<UnityWebRequest> task,System.Action<UnityWebRequest> action)
        {
			while(!task.IsCompleted)
            {
				await Task.Yield();
            }

			if(action != null)
            {
				action(task.Result);
            }
        }
	}
}
