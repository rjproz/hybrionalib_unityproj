using System.Collections.Generic;

namespace Hybriona
{
    [System.Serializable]
    public class ModulesData
    {
        public string modulesConfigPath;
        public string rootPath;
        public List<Module> modules = new List<Module>();
    }

    [System.Serializable]
    public class Module
    {
        public string id;
        public bool enabled;
        public bool alwaysEnabled;
        public string define_symbol;
        public List<string> dependencies;
        public List<string> folders;

    }

}