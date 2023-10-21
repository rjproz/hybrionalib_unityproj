#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Hybriona
{
    [System.Serializable]
    public class ModulesUserPrefs 
    {
        //public const string SaveKey = "Hybriona_Lib_ModulesUserPrefs";
        public const string SAVE_PATH = "ProjectSettings/hybrionalib.prefs";
        private static ModulesUserPrefs instance;
        public static ModulesUserPrefs Instance()
        {
            if (instance == null)
            {
               
                if (File.Exists(SAVE_PATH))
                {
                    instance = JsonUtility.FromJson<ModulesUserPrefs>(File.ReadAllText(SAVE_PATH));
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
            if (File.Exists(SAVE_PATH))
            {
                File.Delete(SAVE_PATH);
            }
            dic = new PersistentDictionary<bool>();
            Save();
        }
       
        public PersistentDictionary<bool> dic = new PersistentDictionary<bool>();
        public string selectedModuleId;
        public void Save()
        {
            File.WriteAllText(SAVE_PATH, JsonUtility.ToJson(this));
        }
    }
}
#endif