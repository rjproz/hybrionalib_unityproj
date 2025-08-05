/*************************************************************************
 *------------------------------------------------------------------------
 *  File         :  GoPoolDataManager.cs
 *  Description  :  Null.
 *------------------------------------------------------------------------
 *  Author       :  rjproz
 *  Date         :  15-09-2023 20:08:11

*************************************************************************/
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hybriona
{
	public class GOPoolDataManager : MonoBehaviour
	{
		[SerializeField] private bool registerOnAwake = true;
		[SerializeField]
		private List<GOPoolDataSource> registerOnStartList = new List<GOPoolDataSource>();
		void Awake()
		{
			if (registerOnAwake)
			{
				RegisterPools();
			}
		}

		public void RegisterPools()
		{
			for (int i = 0; i < registerOnStartList.Count; i++)
			{
				var poolSource = registerOnStartList[i];
				if (poolSource != null)
				{
					if (poolSource.source == null)
					{

					}
					else if (string.IsNullOrEmpty(poolSource.poolId))
					{
						Debug.Log("poolSource's poolId is null or empty");
					}
					else
					{
						if (!GOPoolManager.ContainsPool(poolSource.poolId))
						{
							GOPoolManager.RegisterPool(poolSource.poolId, poolSource.source, poolSource.preCache);
						}
						else
						{
							Debug.LogError($"GOPoolManager poolId='{poolSource.poolId}' already exist");
						}
					}
				}
			}
		}

	}

	[System.Serializable]
	public class GOPoolDataSource
    {
		public string poolId;
		public GOPoolObject source;
		public uint preCache = 0;
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(GOPoolDataManager))]
	public class GOPoolDataManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
			if(GUILayout.Button("Create Pool Constants"))
            {

            }
        }
    }
	#endif


}
