using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HybHttpResponse  
{
	public int status {get;private set;}
	public bool isSuccess {get;private set;}
	public string error {get;private set;}
	public string text {get;private set;}
	public byte [] bytes {get;private set;}
	public Texture2D texture {get;private set;}
	public Texture2D textureNonReadable {get;private set;}
	public AssetBundle assetBundle {get;private set;}

	private WWW _www;
	public HybHttpResponse(WWW www)
	{
		_www = www;


		if(string.IsNullOrEmpty(www.error))
		{
			isSuccess = true;
			if(status == 304)
			{
				
				HybCacheElement element = HybCache.metaData.GetCache(www.url);
				www = new WWW("file:///"+element.GetPath());
				while(!www.isDone)
				{
					
				}
			}
			text = www.text;
			bytes = www.bytes;
			texture = www.texture;
			textureNonReadable = www.textureNonReadable;
			assetBundle = www.assetBundle;


		}
		else
		{
			isSuccess = false;
			text = www.text;
			error = www.error;
		}
	}


	public void LoadImageIntoTexture(Texture2D tex)
	{
		_www.LoadImageIntoTexture(tex);
	}

	public void Dispose()
	{
		_www.Dispose();
	}
}
