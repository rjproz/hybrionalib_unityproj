using System.Collections.Generic;

namespace Hybriona
{
    [System.Serializable]
    public class ModulesData
    {
        public string modulesConfigPath;
        public string rootPath;
        public List<Module> modules = new List<Module>();

#if UNITY_EDITOR
        public bool IsModuleUsedInActiveModules(Module module)
        {
            var oneDependentModule = modules.FindLast(o => o.dependencies.Contains(module.id) && ModulesUserPrefs.Instance().dic.GetValue(o.id));
            if (oneDependentModule != null)
            {
                UnityEngine.Debug.Log(oneDependentModule.id + " is dependent on " + module.id);
            }
            return oneDependentModule != null;
        }
#endif
    }

    [System.Serializable]
    public class Module
    {
        public string id;
       
        public bool alwaysEnabled;
        //public string define_symbol;
        public List<string> dependencies;
        public string root;
        public string sample;
        public string description;


        public string displayname;


       

    }

}