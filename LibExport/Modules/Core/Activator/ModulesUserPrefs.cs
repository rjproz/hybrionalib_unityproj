#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Hybriona
{
    [System.Serializable]
    public class ModulesUserPrefs 
    {
        public const string SaveKey = "Hybriona_Lib_ModulesUserPrefs";

        private static ModulesUserPrefs instance;
        public static ModulesUserPrefs Instance()
        {
            if (instance == null)
            {
               
                if (EditorPrefs.HasKey(SaveKey))
                {
                    instance = JsonUtility.FromJson<ModulesUserPrefs>(EditorPrefs.GetString(SaveKey));
                }
                else
                {
                    instance = new ModulesUserPrefs();
                }
            }
            return instance;
        }

        public void Reset()
        {
            instance = new ModulesUserPrefs();
            Save();
        }
       
        public PersistentDictionary<bool> dic = new PersistentDictionary<bool>();
        public void Save()
        {
            EditorPrefs.SetString(SaveKey,JsonUtility.ToJson(this));
            
        }
    }
}
#endif