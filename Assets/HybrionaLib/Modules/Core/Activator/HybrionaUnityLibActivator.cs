using UnityEngine;
using UnityEditor;
using SimpleJSON;
using System.IO;
using UnityEditorInternal;
using System.Collections.Generic;

namespace Hybriona
{

    [InitializeOnLoad]
    static class HybrionaUnityLibInit
    {
        static bool scriptCompiledCalled;
        static HybrionaUnityLibInit()
        {
            scriptCompiledCalled = false;
            UnityEditor.Compilation.CompilationPipeline.compilationStarted += CompilationPipeline_compilationStarted;
            
        }

        private static System.DateTime lastTimeCompiled;
        private static void CompilationPipeline_compilationStarted(object obj)
        {
            
            if(!scriptCompiledCalled)
            {
                //Debug.Log("CompilationPipeline_compilationStarted");
                lastTimeCompiled = System.DateTime.Now;
                HybrionaUnityLibActivator.ApplyChanges(doAssetDatabaseRefresh: false);
                scriptCompiledCalled = true;
            }
        }

     
    }


    public class HybrionaUnityLibActivatorUIStyle
    {
        public GUIStyle detailedViewTitleStyle { get; private set; }
        public GUIStyle detailedViewIdStyle { get; private set; }
        public GUIStyle moduleInstalledIconStyle { get; private set; }

        public HybrionaUnityLibActivatorUIStyle()
        {
            detailedViewTitleStyle = new GUIStyle(GUI.skin.label);
            detailedViewIdStyle = new GUIStyle(GUI.skin.label);
            moduleInstalledIconStyle = new GUIStyle(GUI.skin.label);


            detailedViewTitleStyle.fontSize = 20;
            detailedViewTitleStyle.alignment = TextAnchor.MiddleLeft;
            
            detailedViewTitleStyle.fontStyle = FontStyle.Bold;
           

            detailedViewIdStyle.fontStyle = FontStyle.Italic;
            detailedViewIdStyle.alignment = TextAnchor.MiddleLeft;

            moduleInstalledIconStyle.alignment = TextAnchor.MiddleRight;
        }
    }


    public class HybrionaUnityLibActivator : EditorWindow
    {

        public static System.Action<ModulesData> adminUIExtension;
        [MenuItem("Hybriona/Hybriona Lib Manager")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(HybrionaUnityLibActivator));
        }

       

        private void OnEnable()
        {
            
            ApplyChanges();
            //Debug.Log("HybrionaUnityLibActivator patched ScriptingDefineSymbols.");
        }

        

       

        private static ModulesData modulesData;
        

        static HybrionaUnityLibActivatorUIStyle uiStyle;
        private static void InitializeModulesData(bool forceReload = false)
        {
            if (modulesData == null || forceReload)
            {
                //string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
                string[] resultGuids = AssetDatabase.FindAssets("t:Script HybrionaUnityLibActivator");
                string path = AssetDatabase.GUIDToAssetPath(resultGuids[0]);

                if (path.Contains("\\"))
                {
                    path = path.Replace("\\Modules\\Core\\Activator\\HybrionaUnityLibActivator.cs", "\\modules.json");
                    modulesData = JsonUtility.FromJson<ModulesData>(System.IO.File.ReadAllText(path));
                    modulesData.modulesConfigPath = path;
                    modulesData.rootPath = path.Replace("\\modules.json", "\\");
                }
                else
                {
                    path = path.Replace("/Modules/Core/Activator/HybrionaUnityLibActivator.cs", "/modules.json");
                    modulesData = JsonUtility.FromJson<ModulesData>(System.IO.File.ReadAllText(path));
                    modulesData.modulesConfigPath = path;
                    modulesData.rootPath = path.Replace("/modules.json", "/");
                }
                //Debug.Log("Path: "+path);

                if(string.IsNullOrEmpty( ModulesUserPrefs.Instance().selectedModuleId ))
                {
                    ModulesUserPrefs.Instance().selectedModuleId = modulesData.modules[0].id;
                    ModulesUserPrefs.Instance().Save();
                }
               
            }
        }

        private void OnFocus()
        {
            InitializeModulesData(forceReload: true);
        }


        private Vector2 moduleListScrollPos;
        
        private void OnGUI()
        {

            if(uiStyle == null)
            {
                uiStyle = new HybrionaUnityLibActivatorUIStyle();
            }
            InitializeModulesData(forceReload: true);

            float padding = 10;
            Rect area = new Rect(padding, padding,
                 position.width - padding * 2f, position.height - padding * 2f);




            GUILayout.BeginArea(area);
            {


#if HYBRIONA_LIB_ADMIN

               

                if(adminUIExtension != null)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    adminUIExtension(modulesData);
                    EditorGUILayout.EndVertical();
                }
                
#endif

                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                moduleListScrollPos = EditorGUILayout.BeginScrollView(moduleListScrollPos,GUI.skin.box, GUILayout.Width(200));
                GUILayout.Label("Modules Settings", uiStyle.detailedViewTitleStyle);
                GUILayout.Space(5);

                
                for (int i = 0; i < modulesData.modules.Count; i++)
                {
                    
                    var moduleData = modulesData.modules[i];

                    
                    {
                        var isActivated = moduleData.alwaysEnabled;
                        if (!isActivated)
                        {
                            if (ModulesUserPrefs.Instance().dic.ContainsKey(moduleData.id))
                            {
                                isActivated = ModulesUserPrefs.Instance().dic.GetValue(moduleData.id);
                            }
                        }
                       
                        
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);


                        //GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button(moduleData.displayname,GUI.skin.label))
                        {
                            
                            
                            ModulesUserPrefs.Instance().selectedModuleId = moduleData.id;
                            ModulesUserPrefs.Instance().Save();
                        }
                        if (isActivated)
                        {
                            GUILayout.Label(EditorGUIUtility.IconContent("Installed"), uiStyle.moduleInstalledIconStyle, GUILayout.Width(25),GUILayout.Height(25));
                        }
                        else
                        {
                            GUILayout.Label(EditorGUIUtility.IconContent("Add-Available"), uiStyle.moduleInstalledIconStyle, GUILayout.Width(25), GUILayout.Height(25));
                           
                        }
                        EditorGUILayout.EndHorizontal();

                        //var newValueIfChanged = EditorGUILayout.BeginToggleGroup(moduleData.displayname, oldValue);
                      

                      
                        //EditorGUILayout.EndToggleGroup();
                       
                        //ModulesUserPrefs.Instance().dic.Add(moduleData.id, newValueIfChanged);

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space(1);
                    }

                }
               

                //if (GUILayout.Button("Apply Changes"))
                //{
                //    ModulesUserPrefs.Instance().Save();
                //    ApplyChanges();
                //}

                //if (GUILayout.Button("Reset"))
                //{
                //    ModulesUserPrefs.Instance().Reset();
                //    ApplyChanges();
                    
                //}


                EditorGUILayout.EndScrollView();
                
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    var selectedModule = modulesData.modules.FindLast( o => o.id == ModulesUserPrefs.Instance().selectedModuleId);
                    if (selectedModule != null)
                    {

                        var isModuleActive = selectedModule.alwaysEnabled;
                        if (!isModuleActive)
                        {
                            if (ModulesUserPrefs.Instance().dic.ContainsKey(selectedModule.id))
                            {
                                isModuleActive = ModulesUserPrefs.Instance().dic.GetValue(selectedModule.id);
                            }
                        }

                        EditorGUILayout.BeginVertical(GUI.skin.textArea);
                        {
                            EditorGUILayout.LabelField(selectedModule.displayname, uiStyle.detailedViewTitleStyle);
                            EditorGUILayout.LabelField(selectedModule.id, uiStyle.detailedViewIdStyle);
                        }
                        EditorGUILayout.EndVertical();
                        GUILayout.Space(3);
                    
                        EditorGUILayout.BeginHorizontal(GUI.skin.box);
                        if (!isModuleActive)
                        {
                            if (GUILayout.Button("Install Module", GUILayout.Width(150)))
                            {
                                ModulesUserPrefs.Instance().dic.Add(selectedModule.id, true);
                                ApplyChanges();
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(selectedModule.sample))
                            {
                                if (GUILayout.Button("Import Samples", GUILayout.Width(150)))
                                {
                                    
                                }
                            }
                            if (GUILayout.Button("Remove Module", GUILayout.Width(150)))
                            {
                                ModulesUserPrefs.Instance().dic.Add(selectedModule.id, false);
                                ApplyChanges();
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        GUILayout.Space(3);

                        EditorGUILayout.LabelField(selectedModule.description, EditorStyles.wordWrappedLabel, GUILayout.Height(100));
                        
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
            if (GUI.changed)
            {
                //Debug.Log("Saving at " + System.DateTime.Now.ToShortDateString());

                //ApplyChanges();
            }


           


        }

        public static void ApplyChanges(bool doAssetDatabaseRefresh = true)
        {
            InitializeModulesData();


            /*
            for (int i = 0; i < modulesData.modules.Count; i++)
            {
                var moduleData = modulesData.modules[i];

                var isActivated = false;
                if (ModulesUserPrefs.Instance().dic.ContainsKey(moduleData.id))
                {
                    isActivated = ModulesUserPrefs.Instance().dic.GetValue(moduleData.id);

                }
                else
                {
                    isActivated = moduleData.enabled;
                }


                for (int folderCounter = 0; folderCounter < moduleData.folders.Count; folderCounter++)
                {
                    string fullFolderPath = System.IO.Path.Combine(modulesData.rootPath, moduleData.folders[folderCounter]);

                    if (isActivated)
                    {

                        string fromPath = fullFolderPath + "~";
                        string toPath = fullFolderPath;

                        if(Directory.Exists(fromPath))
                        {
                            Directory.Move(fromPath, toPath);
                        }

                        fromPath = fullFolderPath + ".meta~";
                        toPath = fullFolderPath + ".meta";

                        if(File.Exists(fromPath))
                        {
                            File.Move(fromPath, toPath);
                        }
                    }
                    else
                    {

                        string fromPath = fullFolderPath;
                        string toPath = fullFolderPath + "~";

                        if (Directory.Exists(fromPath))
                        {
                            Directory.Move(fromPath, toPath);
                        }

                        fromPath = fullFolderPath + ".meta";
                        toPath = fullFolderPath + ".meta~";

                        if (File.Exists(fromPath))
                        {
                            File.Move(fromPath, toPath);
                        }
                    }
                }

                

            }
            AssetDatabase.Refresh();

            return;
            */


            List<Module> activateModuleList = new List<Module>();
           

            for (int i = 0; i < modulesData.modules.Count; i++)
            {
                var moduleData = modulesData.modules[i];

              
                //Other stuff

                var isActivated = moduleData.alwaysEnabled;
                if (!isActivated)
                {
                    if (ModulesUserPrefs.Instance().dic.ContainsKey(moduleData.id))
                    {
                        isActivated = ModulesUserPrefs.Instance().dic.GetValue(moduleData.id);

                    }
                   
                }

                if(isActivated)
                {
                    activateModuleList.Add(moduleData);

                    for(int dependencyCounter = 0; dependencyCounter < moduleData.dependencies.Count; dependencyCounter++)
                    {
                        string dependencyId = moduleData.dependencies[dependencyCounter];
                        var matchedModule = modulesData.modules.FindLast(o => o.id == dependencyId);
                        if (matchedModule == null)
                        {
                            Debug.LogError(dependencyId + " : Selected dependency not found");
                        }
                        else
                        {
                            activateModuleList.Add(matchedModule);
                        }
                    }
                }


               



            }


            for (int i = 0; i < modulesData.modules.Count; i++)
            {
                var moduleData = modulesData.modules[i];

                bool toActivate = activateModuleList.Contains(moduleData);

                //Generate Assemblies

                string createPath = Path.Combine(modulesData.rootPath, moduleData.root);
                createPath = Path.Combine(createPath, moduleData.id + ".asmdef");

                var assemblyDataNode = JSON.Parse("{\n}");
                assemblyDataNode["name"] = moduleData.id;
                assemblyDataNode["references"] = new JSONArray();
                foreach (var dependency in moduleData.dependencies)
                {
                    assemblyDataNode["references"].Add(dependency);
                }

                assemblyDataNode["allowUnsafeCode"] = false;
                assemblyDataNode["autoReferenced"] = true;
                assemblyDataNode["defineConstraints"] = new JSONArray();
                assemblyDataNode["defineConstraints"].Add(moduleData.define_symbol);

                assemblyDataNode["versionDefines"] = new JSONArray();
                assemblyDataNode["versionDefines"].Add(JSON.Parse("{\n}"));
                assemblyDataNode["versionDefines"][0]["name"] = "Unity";

                if (toActivate)
                {
                    assemblyDataNode["versionDefines"][0]["define"] = moduleData.define_symbol;
                    
                }

                File.WriteAllText(createPath, assemblyDataNode.ToString());
            }

            if (doAssetDatabaseRefresh)
            {
                AssetDatabase.Refresh();
            }

            /*
            {
                //create main lib assembly
                string pathOfMainLibAssembly = Path.Combine(modulesData.rootPath, "Modules/Hybriona.Lib.asmdef");
                AssemblyDefinitionAsset mainLibAssembly = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(pathOfMainLibAssembly);
                var jsonNode = JSON.Parse(mainLibAssembly.text);

                jsonNode["references"] = new JSONArray();

                {
                    for (int i = 0; i < modulesData.modules.Count; i++)
                    {
                        var moduleData = modulesData.modules[i];

                        bool toActivate = activateModuleList.Contains(moduleData);

                        //Generate Assemblies

                        string createPath = Path.Combine(modulesData.rootPath, moduleData.root);
                        createPath = Path.Combine(createPath, moduleData.id + ".asmdef");

                        var assemblyDataNode = JSON.Parse("{\n}");
                        assemblyDataNode["name"] = moduleData.id;
                        assemblyDataNode["references"] = new JSONArray();
                        foreach (var dependency in moduleData.dependencies)
                        {
                            assemblyDataNode["references"].Add(dependency);
                        }

                        assemblyDataNode["allowUnsafeCode"] = false;
                        assemblyDataNode["autoReferenced"] = true;
                        assemblyDataNode["defineConstraints"] = new JSONArray();
                        assemblyDataNode["defineConstraints"].Add(moduleData.define_symbol);

                        assemblyDataNode["versionDefines"] = new JSONArray();
                        assemblyDataNode["versionDefines"].Add(JSON.Parse("{\n}"));
                        assemblyDataNode["versionDefines"][0]["name"] = "Unity";

                        if (toActivate)
                        {
                            assemblyDataNode["versionDefines"][0]["define"] = moduleData.define_symbol;
                            jsonNode["references"].Add(moduleData.id);
                        }

                        File.WriteAllText(createPath, assemblyDataNode.ToString());
                    }
                }

                jsonNode["versionDefines"] = new JSONArray();
                
                

               

                File.WriteAllText(pathOfMainLibAssembly, jsonNode.ToString());
                AssetDatabase.Refresh();
            }
            */


            /*
            string newSymbolsCombined = "";

            for (int i = 0; i < scriptingDefineSymbols.Count; i++)
            {
                if (i == scriptingDefineSymbols.Count - 1)
                {
                    newSymbolsCombined += scriptingDefineSymbols[i];
                }
                else
                {
                    newSymbolsCombined += scriptingDefineSymbols[i] + ";";
                }
            }

            string pathOfAssemblyDef = Path.Combine(modulesData.rootPath, "Modules/Hybriona.Lib.asmdef");
            AssemblyDefinitionAsset assemblyDef = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(pathOfAssemblyDef);
            var jsonNode = JSON.Parse(assemblyDef.text);

            jsonNode["versionDefines"] = new JSONArray();
            jsonNode["versionDefines"].Add(JSON.Parse("{\n}"));
            var enabledDefinesNode = jsonNode["versionDefines"][0];
            enabledDefinesNode["name"] = "Unity";
            enabledDefinesNode["expression"] = "";
            enabledDefinesNode["define"] = newSymbolsCombined;

            jsonNode["versionDefines"][0] = enabledDefinesNode;

            File.WriteAllText(pathOfAssemblyDef, jsonNode.ToString());
            AssetDatabase.Refresh();

            */
            //EditMode json

            /*
            List<string> scriptingDefineSymbols = new List<string>();

            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

            string activeSymbolsString = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            //Debug.Log(activeSymbolsString);
            string[] existingSymbols = activeSymbolsString.Split(';');

            scriptingDefineSymbols.AddRange(existingSymbols);

            for (int i = 0; i < modulesData.modules.Count; i++)
            {
                var moduleData = modulesData.modules[i];

                var isActivated = false;
                if (ModulesUserPrefs.Instance().dic.ContainsKey(moduleData.id))
                {
                    isActivated = ModulesUserPrefs.Instance().dic.GetValue(moduleData.id);

                }
                else
                {
                    isActivated = moduleData.enabled;
                }

                if (isActivated)
                {
                    if (!scriptingDefineSymbols.Contains(moduleData.define_symbol))
                    {
                        scriptingDefineSymbols.Add(moduleData.define_symbol);
                    }

                }
                else
                {
                    if (scriptingDefineSymbols.Contains(moduleData.define_symbol))
                    {
                        scriptingDefineSymbols.Remove(moduleData.define_symbol);
                    }

                }

            }


            string newSymbolsCombined = "";

            for (int i = 0; i < scriptingDefineSymbols.Count; i++)
            {
                if (i == scriptingDefineSymbols.Count - 1)
                {
                    newSymbolsCombined += scriptingDefineSymbols[i];
                }
                else
                {
                    newSymbolsCombined += scriptingDefineSymbols[i] + ";";
                }
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, newSymbolsCombined);
            */
        }
        }
}
