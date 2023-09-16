/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  ImportPreprocessor.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  16-09-2023 20:23:17

*************************************************************************/
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hybriona
{
	public class ImportPreprocessor : AssetPostprocessor
	{
		void OnPreprocessAsset()
		{
			//HybrionaUnityLibActivator.ApplyChanges();

		}

		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
		{
			Debug.Log("OnPostprocessAllAssets");
			HybrionaUnityLibActivator.ApplyChanges();
		}
	}
}
#endif