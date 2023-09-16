using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HybWWW : MonoBehaviour {


	private static HybWWW m_instance;
	public static HybWWW Instance
	{
		get
		{
			if(m_instance == null)
			{
				GameObject o = new GameObject("Caching_SingleTon");
				GameObject.DontDestroyOnLoad(o);
				m_instance = o.AddComponent<HybWWW>();


				string userAgent = "UNITY_PC_WEB";
				#if UNITY_ANDROID
				userAgent = "UNITY_ANDROID";
				#elif UNITY_IOS
				userAgent = "UNITY_IOS";
				#endif
				m_headers.Add("User-Agent",userAgent);

			}
			return m_instance;
		}
	}
	private static Dictionary<string,string> m_headers = new Dictionary<string, string>();

	public delegate void OnHybWWWCompleted(WWW www);


	#region Public methods to get/set cache directly

	/// <summary>
	/// Check if specified resource id has valid cache
	/// </summary>
	/// <param name="identifier">Id or url</param>
	/// <param name="version">latest version of the resource/url. It can be null for default</param>
	public static bool HasValidCache(string identifier,string version = null)
	{
		if (string.IsNullOrEmpty(version))
		{
			version = "default";
		}

		HybCacheElement element = HybCache.metaData.GetCache(identifier);
		if (element == null || (element != null && version != null && element.version != version) || (element != null && !System.IO.File.Exists(element.GetPath())))
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// Save specific resource to disk.
	/// </summary>
	/// <param name="identifier">Identifier</param>
	/// <param name="data">Raw data</param>
	/// <param name="version">latest version of the resource. It can be null for default</param>
	public static void SaveToCache(string identifier,byte [] data, string version = null)
    {
		if (string.IsNullOrEmpty(version))
		{
			version = "default";
		}
		HybCache.metaData.AddEntry(identifier, data, version);

	}

	/// <summary>
	/// Fetch Raw data from cache
	/// </summary>
	/// <param name="identifier">Identifier</param>
	/// <param name="version">latest version of the resource. It can be null for default</param>
	/// <returns></returns>
	public static byte [] FetchFromCache(string identifier, string version = null)
    {
		if (string.IsNullOrEmpty(version))
		{
			version = "default";
		}

		if (HasValidCache(identifier,version))
        {
			HybCacheElement element = HybCache.metaData.GetCache(identifier);
			return System.IO.File.ReadAllBytes(element.GetPath());
		}
		return null;
    }

    #endregion

    #region Public methods for Web Caching

    /// <summary>
    /// Loads from Cache or downloads
    /// </summary>
    /// <param name="url">Request URL.</param>
    /// <param name="version">latest version of the resource/url. It can be null for default</param>
    /// <param name="downloadHandler">Downloadhandler for optimised handing of data. By default it uses DownloadHandlerBuffer</param>
    /// <param name="callback">Callback</param>
    public static void RequestURL(string url, string version,DownloadHandler downloadHandler, System.Action<UnityWebRequest> callback)
	{
		Instance.StartCoroutine(Instance.HandleUnityWebRequest(url,version, downloadHandler, callback));
	}


	/// <summary>
	/// Loads from Cache or downloads
	/// url => Url to load
	/// version => latest version of the resource/url. It can be null for defaults
	/// </summary>
	[System.Obsolete("LoadUrl is deprecated. use RequestUrl Instead", true)]
	public static WWW LoadUrl(string url,string version = null)
	{
		WWW www = HybWWW.GetActiveDownloadIfExist(url);
		if(www == null)
		{
			www = HybWWW.Load(url,version); 
		}
		return www;
	}


	/// <summary>
	/// Loads from Cache or downloads
	/// url => Url to load
	/// version => latest version of the resource/url. It can be null for defaults
	/// </summary>
	[System.Obsolete("LoadUrl is deprecated. use RequestUrl Instead", true)]
	public static void LoadUrl(string url,string version,OnHybWWWCompleted onHybWWWCompleted)
	{
		WWW www = LoadUrl(url,version);
		Instance.WaitForWWW(www,onHybWWWCompleted);
	}

	/// <summary>
	/// Loads from Cache or downloads
	/// url => Url to load
	/// version => null
	/// </summary>
	[System.Obsolete("LoadUrl is deprecated. use RequestUrl Instead", true)]
	public static void LoadUrl(string url,OnHybWWWCompleted onHybWWWCompleted)
	{
		LoadUrl(url,"default",onHybWWWCompleted);
	}


	public void WaitForWWW(WWW www,OnHybWWWCompleted onHybWWWCompleted)
	{
		StartCoroutine(WaitForWWW_I(www,onHybWWWCompleted));
	}

#endregion

    #region Methods - Custom Caching

    IEnumerator HandleUnityWebRequest(string url, string version,DownloadHandler downloadHandler, System.Action<UnityWebRequest> callback)
	{
		if (string.IsNullOrEmpty(version))
        {
			version = "default";
        }
		if(downloadHandler == null)
        {
			downloadHandler = new DownloadHandlerBuffer();

		}

		HybCacheElement element = HybCache.metaData.GetCache(url);

		UnityWebRequest request = null;
		bool loadingFromCache= false;
		if (element == null || (element != null && version != null && element.version != version) || (element != null && !System.IO.File.Exists(element.GetPath())))
		{
			loadingFromCache = false;
			request = UnityWebRequest.Get(url);
		}
		else
		{
			loadingFromCache = true;
			request = UnityWebRequest.Get("file://" + element.GetPath());
		}
		request.downloadHandler = downloadHandler;
		yield return request.SendWebRequest();
		if(!loadingFromCache && !request.isHttpError && !request.isNetworkError)
        {
			HybCache.metaData.AddEntry(request, version);
		}

		if(callback != null)
        {
			callback(request);
		}
		
	}

	IEnumerator WaitForWWW_I(WWW www,OnHybWWWCompleted onHybWWWCompleted)
	{
		yield return www;
		if(onHybWWWCompleted != null)
		{
			onHybWWWCompleted(www);
		}
	}
		

	protected static WWW Load(string url,string version = "default")
	{
		HybCacheElement element = HybCache.metaData.GetCache(url);
		
		if(element == null || (element != null && version != null && element.version != version) || (element != null && !System.IO.File.Exists( element.GetPath() )) ) 
		{
			
			WWW newDownload = new WWW(url);
			Instance.AddCacheElement(newDownload,version);
			activeDownloads.Add(url,newDownload);
			return newDownload;
		}
		else
		{
			return new WWW("file:///"+element.GetPath());
		}
	}
	private static Dictionary<string,WWW> activeDownloads = new Dictionary<string, WWW>();

	public void AddCacheElement(WWW www,string newVersion)
	{
		StartCoroutine(AddCacheElementIenu(www,newVersion));
	}
	IEnumerator AddCacheElementIenu(WWW www,string newVersion)
	{
		while( !www.isDone )
		{
			yield return null;
		}
		if(string.IsNullOrEmpty(www.error))
		{
			if(string.IsNullOrEmpty(newVersion))
			{
				newVersion = "default";
			}
			HybCache.metaData.AddEntry(www,newVersion);
			if(activeDownloads.ContainsKey(www.url))
			{
				activeDownloads.Remove(www.url);
			}
		}

	}

	public static WWW GetActiveDownloadIfExist(string url)
	{
		if(activeDownloads.ContainsKey(url))
		{
			WWW www = activeDownloads[url];
			if(www == null)
			{
				activeDownloads.Remove(www.url);
				return null;
			}
			else
			{
				return www;
			}
		}
		else
		{
			return null;
		}
	}
    #endregion


    #region Methods HTTP Caching


	/// <summary>
	/// Loads URL with standard HTTP caching (via headers)
	/// </summary>
	/// <param name="url">Request URL.</param>
	/// <param name="downloadHandler">Downloadhandler for optimised handing of data. By default it uses DownloadHandlerBuffer</param>
	/// <param name="callback">Callback</param>
	public static void RequestWithHeaderCache(string url, DownloadHandler downloadHandler,System.Action<UnityWebRequest> callback)
	{
		HybCacheElement element = HybCache.metaData.GetCache(url);
		//Dictionary<string,string> headers = new Dictionary<string, string>();
		var request = UnityWebRequest.Get(url);
		if(downloadHandler == null)
        {
			downloadHandler = new DownloadHandlerBuffer();

		}
		request.downloadHandler = downloadHandler;
		
		if (element != null && !string.IsNullOrEmpty(element.version) && element.version.Contains("\n") && System.IO.File.Exists( element.GetPath() ))
		{
			string [] fields = element.version.Split('\n');
			if(!string.IsNullOrEmpty(fields[0]))
			{
				//headers.Add("If-Modified-Since",fields[0]);
				request.SetRequestHeader("If-Modified-Since", fields[0]);
			}
			if(!string.IsNullOrEmpty(fields[1]))
			{
				//headers.Add("If-None-Match",fields[1]);
				request.SetRequestHeader("If-None-Match", fields[1]);
			}
		}
		//WWW downloadWWW = new WWW(url,null,headers);

		//Instance.HandleHTTPWWW(downloadWWW,onHybWWWCompleted);
		Instance.StartCoroutine(Instance.HandleHTTPWWWIenu(request, callback));
	}

	
	IEnumerator HandleHTTPWWWIenu(UnityWebRequest request, System.Action<UnityWebRequest> callback)
	{
		yield return request.SendWebRequest();


		if (request.result == UnityWebRequest.Result.Success)
		{
			string lastMod = "";
			string ETag = "";
			var responseHeaders = request.GetResponseHeaders();

			if (responseHeaders.ContainsKey("Last-Modified"))
			{
				lastMod = responseHeaders["Last-Modified"];
			}

			if (responseHeaders.ContainsKey("Etag"))
			{
				ETag = responseHeaders["Etag"];
			}
			string newVersion = lastMod + "\n" + ETag;


			if (request.responseCode == 304)
			{
				HybCacheElement element = HybCache.metaData.GetCache(request.url);
				var cacheRequest = UnityWebRequest.Get("file://" + element.GetPath());
				cacheRequest.downloadHandler = request.downloadHandler;
				yield return cacheRequest.SendWebRequest();
				if (callback != null)
				{
					callback(request);
				}
			}
			else
			{
				HybCache.metaData.AddEntry(request, newVersion);


				if (callback != null)
				{
					callback(request);
				}
			}


		}
		else
		{
			if(callback != null)
            {
				callback(request);
			}
		}


	}

	#endregion

}
