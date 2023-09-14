#if HYBRIONA_LIB_ENABLE_HTTP_CLIENT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
namespace Hybriona
{
	public struct HttpResponse
	{
		public int status { get; private set;}

		public UnityWebRequest webRequest {get;private set;}

		public Dictionary<string,string> responseHeaders { get; private set;}
		public ulong downloadedBytes { get; private set;}
		public byte [] bytes { get; private set;}
		public string text { get; private set;}

		public string error { get; private set;}
		public bool isHttpError { get; private set;}
		public bool isNetworkError { get; private set;}

		public float time {get;private set;}

		public enum ContentType {Unknown,Json};

		public HttpResponse(UnityWebRequest request,float timeTaken)
		{
			if(!request.isDone)
			{
				throw new System.Exception("HttpResponse shouldn't call before UnityWebRequest finishes!");
			}
			webRequest = request;
			time = timeTaken;

			status = (int) request.responseCode;

			downloadedBytes = request.downloadedBytes;
			bytes = request.downloadHandler.data;
			text = request.downloadHandler.text;
			error = request.error;

			isHttpError = request.isHttpError;
			isNetworkError = request.isNetworkError;

			responseHeaders = request.GetResponseHeaders();

		}

		public ContentType GetContentType()
		{
			if(responseHeaders!= null && responseHeaders.ContainsKey("Content-type"))
			{
				string type = responseHeaders["Content-type"];
				if(type == "application/json")
				{
					return ContentType.Json;
				}

			}
			return ContentType.Unknown;
		}
	}
}
#endif