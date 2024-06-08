﻿/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  CacheRequest.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  07-06-2024 20:40:13

*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class CacheRequest 
{
	/// <summary>
	/// LoadMode specifies how the loading should behave
	/// LoadMode.Default = Tries to load the live version first.  
	/// </summary>
	public enum LoadMode { Default = 0, UseCacheFirstEvenIfVersionMismatch = 1,  ForceRefresh = 2};
	public enum ResultMode {LoadedLive,LoadedFromCache,LiveFailedLoadedFromCache,Failed };
	
	public static async Task<(UnityWebRequest request, ResultMode resultMode)> Get(string url,string version, LoadMode loadMode = LoadMode.Default)
    {
		ResultMode resultMode = ResultMode.LoadedLive;
		bool loadingFromCache = false;
		bool hasCache = false;
		bool isCacheVersionMisMatch = false;
		UnityWebRequest request = null;
		HybCacheElement cache = null;
		if (string.IsNullOrEmpty(version))
        {
			version = "default";
        }
		if (loadMode == LoadMode.ForceRefresh)
		{
			request = UnityWebRequest.Get(url);
		}
		else
		{
			cache = HybCache.metaData.GetCache(url);
			hasCache = !(cache == null || (cache != null && !System.IO.File.Exists(cache.GetPath())));

			if(hasCache && cache.version != version)
            {
				isCacheVersionMisMatch = true;
			}

			if (cache != null)
			{
				if(hasCache && (!isCacheVersionMisMatch || loadMode == LoadMode.UseCacheFirstEvenIfVersionMismatch))
				{
					loadingFromCache = true;
					resultMode = ResultMode.LoadedFromCache;
					request = UnityWebRequest.Get("file://" + cache.GetPath());
				}
				else
				{
					loadingFromCache = false;
					request = UnityWebRequest.Get(url);
				}
			}
			else
            {
				request = UnityWebRequest.Get(url);
			}
		}

		
		var operation = request.SendWebRequest();
		while (!operation.isDone)
		{
			await Task.Yield();
		}

		if (!loadingFromCache && request.result == UnityWebRequest.Result.Success)
		{
			resultMode = ResultMode.LoadedLive;
			HybCache.metaData.AddEntry(request, version);
		}

		if(!loadingFromCache && request.result != UnityWebRequest.Result.Success)
        {
			if (hasCache)
			{
				resultMode = ResultMode.LiveFailedLoadedFromCache;
				loadingFromCache = true;
				request.Dispose();
				request = UnityWebRequest.Get("file://" + cache.GetPath());
				
				operation = request.SendWebRequest();
				while (!operation.isDone)
				{
					
					await Task.Yield();
				}
				
			}
		}
		
		return (request, resultMode);

	}


    
}
public static class HybCacheExtensions
{
	public static async void Callback(this Task<(UnityWebRequest request, CacheRequest.ResultMode resultMode)> task, System.Action<(UnityWebRequest request, CacheRequest.ResultMode resultMode)> callback)
    {
		while(!task.IsCompleted)
        {
			await Task.Yield();
        }
		callback(task.Result);
    }
}