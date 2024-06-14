using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public static class HybCache  
{

	public static HybCacheMetaData metaData {get; private set; }

	static HybCache()
	{
		if(!System.IO.Directory.Exists(HybCacheMetaData.directory))
		{
			System.IO.Directory.CreateDirectory(HybCacheMetaData.directory);
		}
		metaData = HybCacheMetaData.Load();
		metaData.Process();

	}

	/// <summary>
	/// Set maximum space alloted for Cache
	/// </summary>
	/// <param name="newmaxCacheSize">Newmax cache size.</param>
	public static void SetMaxCacheSize(int newmaxCacheSize)
	{
		//maxCacheSize = newmaxCacheSize;
		metaData.maxCacheSize = newmaxCacheSize;
		metaData.Save();
	}


	public static void SetMaxCacheSizeKiloByte(int newmaxCacheSizeKB)
	{
		//maxCacheSize = newmaxCacheSize;
		metaData.maxCacheSize = newmaxCacheSizeKB * 1000 ;
		metaData.Save();
	}

	public static void SetMaxCacheSizeMegaByte(int newmaxCacheSizeMB)
	{
		//maxCacheSize = newmaxCacheSize;
		metaData.maxCacheSize = newmaxCacheSizeMB * 1000 * 1000;
		metaData.Save();
	}

	/// <summary>
	/// Delete Cache
	/// </summary>
	public static void Clear()
	{
		try
		{
			System.IO.Directory.Delete(HybCacheMetaData.directory,true);

			if(!System.IO.Directory.Exists(HybCacheMetaData.directory))
			{
				System.IO.Directory.CreateDirectory(HybCacheMetaData.directory);
			}
		}
		catch
		{
			
		}
	}

	/// <summary>
	/// To Enable Expiry Mechanism. This will delete file if it older than some specified time.
	/// </summary>
	public static void EnableExpiry()
	{
		metaData.isExpiryEnabled = true;
		metaData.Save();
	}

	public static void SetExpiryAgeHours(float hours)
	{
		metaData.hours = hours;
		metaData.Save();
	}

	public static void SetExpiryAgeMinutes(float minutes)
	{
		metaData.hours = minutes / 60f;
		metaData.Save();
	}

	public static void SetExpiryAgeDays(float days)
	{
		metaData.hours = days * 24;
		metaData.Save();
	}

	/// <summary>
	/// Disables the expiry mechanism
	/// </summary>
	public static void DisableExpiry()
	{
		metaData.isExpiryEnabled = false;
		metaData.Save();
	}

	/// <summary>
	/// Deletes cache of a specific id or url. 
	/// </summary>
	/// <returns><c>true</c>, if cache was deleted, <c>false</c> otherwise, like if the cache doesnt exist.</returns>
	/// <param name="url">URL or identifier</param>
	public static bool DeleteCache(string identifier)
	{
		HybCacheElement element = HybCache.metaData.GetCache(identifier);
		if(element != null)
		{
			element.Delete();
			metaData.EvaluateEntries();
			return true;
		}
		return false;
	}



	#region Public methods to get/set cache directly

	/// <summary>
	/// Check if specified resource id has cache independent of version mismatch
	/// </summary>
	/// <param name="identifier">Id or url</param>
	/// <param name="version">latest version of the resource/url. It can be null for default</param>
	public static bool HasCache(string identifier)
	{

		HybCacheElement element = HybCache.metaData.GetCache(identifier);
		if (element == null || (element != null && !System.IO.File.Exists(element.GetPath())))
		{
			return false;
		}
		return true;
	}


	


	/// <summary>
	/// Check if specified resource id has cache with version matching
	/// </summary>
	/// <param name="identifier">Id or url</param>
	/// <param name="version">latest version of the resource/url. It can be null for default</param>
	public static bool HasCacheWithMatchingVersion(string identifier, string version = "default")
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
	/// Tries to get the version of a cache if exist
	/// </summary>
	/// <param name="identifier"></param>
	/// <param name="versionOfIdentifier"></param>
	/// <returns></returns>
	public static bool TryGetCacheVersion(string identifier, out string versionOfIdentifier)
	{
		versionOfIdentifier = null;
		if (HasCache(identifier))
		{
			versionOfIdentifier = HybCache.metaData.GetCache(identifier).version;
			return true;

		}
		return false;
	}

	/// <summary>
	/// Save specific resource to disk.
	/// </summary>
	/// <param name="identifier">Identifier</param>
	/// <param name="data">Raw data</param>
	/// <param name="version">latest version of the resource. It can be null for default</param>
	public static void SaveToCache(string identifier, byte[] data, string version = null)
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
	public static byte[] FetchFromCache(string identifier, string version = null)
	{
		if (string.IsNullOrEmpty(version))
		{
			version = "default";
		}

		if (HasCacheWithMatchingVersion(identifier, version))
		{
			HybCacheElement element = HybCache.metaData.GetCache(identifier);
			return System.IO.File.ReadAllBytes(element.GetPath());
		}
		return null;
	}

	/// <summary>
	/// Try to Get Cache file path.
	/// </summary>
	/// <param name="identifier"></param>
	/// <param name="cacheFilePath"></param>
	/// <returns></returns>
	public static bool TryGetCachePath(string identifier,out string cacheFilePath)
    {
		cacheFilePath = null;
		if (HasCache(identifier))
        {
			cacheFilePath = HybCache.metaData.GetCache(identifier).GetPath();
			return true;
		}
		else
        {
			return false;
        }
    }

	public static bool TryGetCachePathWithMatchingVersion(string identifier, string version,out string cacheFilePath)
	{
		if (string.IsNullOrEmpty(version))
		{
			version = "default";
		}

		cacheFilePath = null;
		if (HasCacheWithMatchingVersion(identifier, version))
		{
			cacheFilePath = HybCache.metaData.GetCache(identifier).GetPath();
			return true;
		}
		else
		{
			return false;
		}
	}

	#endregion
}

[System.Serializable]
public class HybCacheMetaData
{
	public static string directory = Application.persistentDataPath+"/cache_26287";
	public static string metaDataPath = directory + "/metadata_262819.data";

	public int maxCacheSize = 50000000; // 50 MB
	public List<HybCacheElement> entries = new List<HybCacheElement>();

	public float currentCacheSize {get; private set;}

	public bool isExpiryEnabled = false;
	public float hours = 0;

    public readonly object thread_lock = new object();
	public static HybCacheMetaData Load()
	{
		
		HybCacheMetaData o = null;

		if(System.IO.File.Exists(HybCacheMetaData.metaDataPath))
		{
			o = JsonUtility.FromJson<HybCacheMetaData>(System.IO.File.ReadAllText(HybCacheMetaData.metaDataPath));
		}
		else
		{
			o = new HybCacheMetaData();
		}
		return o;
	}
	public void Process()
	{
        new Thread(() =>
        {
            try
            {
              
                ProcessExpiredFiles();
                Sort();
                CheckIfMaxedSizeProcess();
                EvaluateEntries();
                
                Save();
            }
            catch { }
        }).Start();
	}


	private void Sort()
	{
        lock (thread_lock)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                System.DateTime iDate = System.DateTime.Parse(entries[i].dateCreated);
                for (int j = i + 1; j < entries.Count; j++)
                {
                    System.DateTime jDate = System.DateTime.Parse(entries[j].dateCreated);
                    if ((jDate - iDate).TotalHours < 0)
                    {
                        HybCacheElement temp = entries[i];
                        entries[i] = entries[j];
                        entries[j] = temp;
                    }

                }
            }
        }
	}


	public void EvaluateEntries()
	{
        lock (thread_lock)
        {
            currentCacheSize = 0;
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                if (!entries[i].IsValid())
                {
                    entries.RemoveAt(i);
                }
                else
                {
                    currentCacheSize += entries[i].size;
                }
            }
        }
	}



	public HybCacheElement GetCache(string url)
	{
		string id = HybIDGenerator.Md5Sum(url);
		for(int i=0;i<entries.Count;i++)
		{
			if(entries[i].id == id)
			{
				return entries[i];
			}
		}
		return null;
	}
	public void AddEntry(WWW www,string newVersion)
	{
        lock (thread_lock)
        {
            HybCacheElement entry = GetCache(www.url);
            if (entry == null)
            {
                entry = new HybCacheElement();
                entries.Add(entry);
                entry.url = www.url;
            }
            entry.dateCreated = System.DateTime.Now.ToString();
            entry.size = www.bytes.Length;
            entry.version = newVersion;
            currentCacheSize += www.bytes.Length;


            System.IO.File.WriteAllBytes(entry.GetPath(), www.bytes);

        }
		//System.GC.Collect();
		Process();
	}

	public void AddEntry(UnityWebRequest request, string newVersion)
	{
		lock (thread_lock)
		{
			HybCacheElement entry = GetCache(request.url);
			if (entry == null)
			{
				entry = new HybCacheElement();
				entries.Add(entry);
				entry.url = request.url;
			}

			byte[] data = request.downloadHandler.data;
			entry.dateCreated = System.DateTime.Now.ToString();
			entry.size = data.Length;
			entry.version = newVersion;
			currentCacheSize += data.Length;


			System.IO.File.WriteAllBytes(entry.GetPath(), data);

		}
		//System.GC.Collect();
		Process();
	}

	public void AddEntry(string identifier, byte [] data, string newVersion)
	{
		lock (thread_lock)
		{
			HybCacheElement entry = GetCache(identifier);
			if (entry == null)
			{
				entry = new HybCacheElement();
				entries.Add(entry);
				entry.url = identifier;
			}

			
			entry.dateCreated = System.DateTime.Now.ToString();
			entry.size = data.Length;
			entry.version = newVersion;
			currentCacheSize += data.Length;


			System.IO.File.WriteAllBytes(entry.GetPath(), data);

		}
		//System.GC.Collect();
		Process();
	}

	private void CheckIfMaxedSizeProcess()
	{
		if(currentCacheSize > maxCacheSize)
		{
            lock (thread_lock)
            {
                for (int i = entries.Count - 1; i >= 0 && currentCacheSize > maxCacheSize; i--)
                {
                    HybCacheElement curr = entries[i];
                    curr.Delete();
                    entries.RemoveAt(i);
                }
            }
		}
	}


	public void ProcessExpiredFiles()
	{
		if(isExpiryEnabled)
		{
            lock (thread_lock)
            {
                for (int i = entries.Count - 1; i >= 0; i--)
                {
                    HybCacheElement curr = entries[i];
                    if (curr.IsExpired())
                    {
                        curr.Delete();
                        entries.RemoveAt(i);
                    }
                }
            }
			Save();
		}
	}

	public void Save()
	{
        lock (thread_lock)
        {
            System.IO.File.WriteAllText(HybCacheMetaData.metaDataPath, JsonUtility.ToJson(this));
        }
	}



}
[System.Serializable]
public class HybCacheElement
{
	public string url;
	public string dateCreated;
	public string version = "default";

	public int size {get;set;}
	public string id {
		get
		{
			if(_id == null)
			{
				_id = HybIDGenerator.Md5Sum(url);
			}
			return _id;
		}
	}	

	private string _id;
	public string GetPath()
	{
		if(!System.IO.Directory.Exists(HybCacheMetaData.directory))
		{
			System.IO.Directory.CreateDirectory(HybCacheMetaData.directory);
		}
		return  HybCacheMetaData.directory +"/"+id+".bdata";
	}
	public bool IsValid()
	{
		string assetPath = GetPath();
		if(System.IO.File.Exists(assetPath))
		{
			size = (int) new System.IO.FileInfo(assetPath).Length;
			//System.GC.Collect();
			return true;
		}

		return false;
	}

	public void Delete()
	{
		string assetPath = GetPath();
		if(System.IO.File.Exists(assetPath))
		{
			System.IO.File.Delete(assetPath);
		}	

	}


	public bool Exist()
	{
		return System.IO.File.Exists(GetPath());
	}

	public bool IsExpired()
	{
		
		float hoursElapsed = (float) (System.DateTime.Now - System.DateTime.Parse(dateCreated)).TotalHours;
		if(Mathf.Abs(hoursElapsed) >  HybCache.metaData.hours) 
		{
			return true;
		}
		return false;

	}
}
