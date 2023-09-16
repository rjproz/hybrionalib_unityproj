/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  ImportPreprocessor.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  16-09-2023 20:23:17

*************************************************************************/
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Hybriona
{
	public class ImportPreprocessor : AssetPostprocessor
	{
		void OnPreprocessAsset()
		{
			Debug.Log("OnPreprocessAsset " + assetPath);
			if(assetPath.Contains("hybriona.") && assetPath.Contains("asmdef"))
            {
				Debug.Log("hybriona lib patching " + assetPath);
				HybrionaUnityLibActivator.ApplyChanges();
			}
			

		}

		//static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
		//{
		//	Debug.Log("OnPostprocessAllAssets");
		//	HybrionaUnityLibActivator.ApplyChanges();
		//}
	}
}
#endif