#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Hybriona
{
	public class EditorCommon : EditorWindow 
	{
		[MenuItem("Hybriona/Common/Open Persistent Path")]
		public static void OpenPersistentDataPath()
		{
			EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
		}
	}
}
#endif
