#if HYBRIONA_LIB_ENABLE_CACHE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HowtoUseHybCache : MonoBehaviour {


	public Transform mVisualQuad;
	public List<Texture2D> textures = new List<Texture2D>();
	void Start()
	{
		StartCoroutine(AnimateTextureQuad());

		// Set config
		HybCache.SetMaxCacheSize(20000000); // Setting the max cache size. 20 MB 


		// Set Expiry (Optional)
		HybCache.EnableExpiry();

		// or HybCache.DisableExpiry() to disable it

		HybCache.SetExpiryAgeHours(10);
		// or HybCache.SetExpiryAgeDays(1);
		// or HybCache.SetExpiryAgeMinutes(1000);
		

		// To Clear cache
		// HybCache.Clear();

		

		//or you can use the below event based function
		// LoadURLEventBased();

		LoadURLEventBased("http://projectvoid.hybriona.com/website/images/WebsiteBanner.jpg");
		LoadURLEventBased("https://img.itch.zone/aW1nLzU4MzQzMDguanBn/original/aQw9%2BA.jpg");

		

		//Load URL with standard HTTP Caching (via Headers). Currently only available as event based method
		float timeStarted = 0;
		string url_http_cache = "http://apis.hybriona.com/hybriona/unity/hugetext";
		HybWWW.RequestWithHeaderCache(url_http_cache,new DownloadHandlerBuffer(),(UnityWebRequest request)=> {
			if(!request.isHttpError && !request.isNetworkError)
			{
				Debug.Log("WWW ("+url_http_cache+") loaded in "+(Time.fixedTime - timeStarted)+" seconds.");
				// Process the resource
				Debug.Log(request.downloadHandler.text);
			}
			else
			{
				Debug.LogError(request.error);
			}
		});


		//Direct Cache for non-web implementation
		Texture2D generatedTex = null;
		if (HybWWW.HasValidCache("gen123"))
        {
			var imageData = HybWWW.FetchFromCache("gen123");
			generatedTex = new Texture2D(1,1);
			generatedTex.LoadImage(imageData, true);
			Debug.Log("Texture loaded from cache");
		}
		else
        {
			generatedTex = GenerateTextureRandom();
			HybWWW.SaveToCache("gen123", generatedTex.EncodeToPNG());
			Debug.Log("Texture generated");
		}
		textures.Add(generatedTex);

		//Delete Cache
		Invoke("DeleteSpecficCache",10);
	}


	
	void LoadURLEventBased(string url)
	{
		

		float timeStarted = Time.fixedTime;
		HybWWW.RequestURL(url,"7216723", new DownloadHandlerTexture(readable:false), (UnityWebRequest request) =>{

			if(request.result == UnityWebRequest.Result.Success)
			{
				Debug.Log("WWW ("+url+") loaded in "+(Time.fixedTime - timeStarted)+" seconds.");
				// Process the resource
				Texture2D textureLoaded = DownloadHandlerTexture.GetContent(request);
				textures.Add(textureLoaded);
				
			}
			else
			{
				Debug.LogError(request.error);
			}
		});

	}



	private Texture2D GenerateTextureRandom()
    {
		Texture2D tex = new Texture2D(1000, 1000);
		for(int x=0;x<tex.width;x++)
        {
			for (int y = 0; y < tex.height; y++)
			{
				tex.SetPixel(x, y, new Color(Random.value,Random.value,Random.value,1));
			}
		}
		tex.Apply();
		return tex;
    }


	IEnumerator AnimateTextureQuad()
	{
		Transform mVisualTransform = mVisualQuad.transform;
		Material mVisualMaterial = mVisualQuad.GetComponent<Renderer>().material;

		int indexofTexture = 0;
		while(true)
		{
			if(textures.Count > 0)
			{
				float height = 10;
				float width = 10 * (float)textures[indexofTexture].width / (float)textures[indexofTexture].height;

				float screenClipping = (Camera.main.aspect * 10) / width;
				if(screenClipping < 1)
				{
					width = width * screenClipping; // To fit the quad under view
					height = height * screenClipping;
				}

				mVisualMaterial.mainTexture = textures[indexofTexture];
				mVisualTransform.localScale = new Vector3(width,height,1);
			}

			indexofTexture++;
			if(indexofTexture >= textures.Count)
			{
				indexofTexture = 0;
			}
			yield return new WaitForSeconds(2);
		}
	}

	public void DeleteSpecficCache()
	{
		HybCache.DeleteCache("http://hybriona.com/services/api/extraservices/images/banner/alienracer.png");
	}
}
#endif