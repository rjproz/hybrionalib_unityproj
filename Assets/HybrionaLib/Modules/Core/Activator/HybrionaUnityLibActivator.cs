#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using SimpleJSON;
using System.IO;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEngine.Networking;

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
        public GUIStyle detailedViewHeaderBGStyle { get; private set; }
        public GUIStyle detailedViewTitleStyle { get; private set; }
        public GUIStyle detailedViewIdStyle { get; private set; }

        public GUIStyle detailedViewInfoLblStyle { get; private set; }

        public GUIStyle moduleInstalledIconStyle { get; private set; }



        public GUIStyle moduleSelectBtnStyle { get; private set; }
        public GUIStyle moduleSelectedBtnStyle { get; private set; }

        public HybrionaUnityLibActivatorUIStyle()
        {
            detailedViewHeaderBGStyle = new GUIStyle(GUI.skin.textArea);
            detailedViewTitleStyle = new GUIStyle(GUI.skin.label);
            detailedViewIdStyle = new GUIStyle(GUI.skin.label);
            detailedViewInfoLblStyle = new GUIStyle(GUI.skin.label);

            moduleInstalledIconStyle = new GUIStyle(GUI.skin.label);
            moduleSelectBtnStyle = new GUIStyle(EditorStyles.helpBox);
            moduleSelectedBtnStyle = new GUIStyle(GUI.skin.button);


            detailedViewHeaderBGStyle.padding = new RectOffset(5, 5, 10, 10);
            detailedViewHeaderBGStyle.margin = new RectOffset(0, 0, 5, 5); 

            detailedViewTitleStyle.fontSize = 20;
            detailedViewTitleStyle.alignment = TextAnchor.MiddleLeft;
            //detailedViewTitleStyle.margin = new RectOffset(0, 0, 5, 5);
            detailedViewTitleStyle.fontStyle = FontStyle.Bold;


            


            detailedViewIdStyle.fontStyle = FontStyle.Italic;
            detailedViewIdStyle.alignment = TextAnchor.MiddleLeft;

            detailedViewInfoLblStyle.fontStyle = FontStyle.Italic;
            detailedViewInfoLblStyle.alignment = TextAnchor.MiddleLeft;
            //detailedViewInfoLblStyle.fontSize = 12;
            detailedViewInfoLblStyle.normal.textColor = Color.gray;

            moduleInstalledIconStyle.alignment = TextAnchor.MiddleRight;

            moduleSelectBtnStyle.alignment = TextAnchor.MiddleLeft;

            moduleSelectedBtnStyle.alignment = TextAnchor.MiddleLeft;
            
            //moduleSelectedBtnStyle.normal.background = (Texture2D) EditorGUIUtility.IconContent("AvatarController.LayerSelected").image;
            moduleSelectedBtnStyle.normal = moduleSelectedBtnStyle.focused = moduleSelectedBtnStyle.hover = moduleSelectedBtnStyle.active;
        }
    }


    public class HybrionaUnityLibActivator : EditorWindow
    {
        public const string PackageDownloadPath = "https://media.githubusercontent.com/media/rjproz/hybrionalib_unityproj/master/PackageDownload";


        public static System.Action<ModulesData> adminUIExtension;
        [MenuItem("Hybriona/Hybriona Lib Manager")]
        public static void ShowWindow()
        {
            
            var window = GetWindow(typeof(HybrionaUnityLibActivator));
            var titleContent = window.titleContent;
            titleContent.text = "HybLib Activator";
            window.Show();
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
                moduleListScrollPos = EditorGUILayout.BeginScrollView(moduleListScrollPos, false, false, GUI.skin.verticalScrollbar, GUI.skin.verticalScrollbar,GUI.skin.box, GUILayout.Width(200)) ;
                EditorGUILayout.BeginVertical(uiStyle.detailedViewHeaderBGStyle);
                GUILayout.Label("Modules", uiStyle.detailedViewTitleStyle);
                if (GUILayout.Button("Update Library"))
                {
                    var request = UnityEditor.PackageManager.Client.List();
                    while(!request.IsCompleted)
                    {

                    }
                    var collection = request.Result;
                    foreach(var pkg in collection)
                    {
                        
                        if (pkg.packageId.Contains("hybriona.unitylib"))
                        {
                            //Debug.Log(pkg.packageId);
                            //Debug.Log("Trying to update hybrionalib");
                            //UnityEditor.PackageManager.Client.Add(pkg.packageId);
                            
                            var downloadRequest = UnityWebRequest.Get(PackageDownloadPath+ "/hybriona.unitylib.tgz?"+System.DateTime.Now.ToFileTimeUtc());
                            var downloadOperation = downloadRequest.SendWebRequest();

                            while(!downloadOperation.isDone)
                            {

                            }
                            if(downloadRequest.result == UnityWebRequest.Result.Success)
                            {

                                string downloadPkgPath = "Packages/hybriona.unitylib.archive";
                                downloadPkgPath = Path.GetFullPath(downloadPkgPath);
                                //Debug.Log("saving package to " + downloadPkgPath);
                                File.WriteAllBytes(downloadPkgPath, downloadRequest.downloadHandler.data);

                                //Debug.Log("hybriona.unitylib downloaded @ " + downloadPkgPath);


                                var removePkgProcess = UnityEditor.PackageManager.Client.Remove(pkg.packageId);
                                while (!removePkgProcess.IsCompleted) { }

                                var addpkgProcess = UnityEditor.PackageManager.Client.Add("hybriona.unitylib@file:hybriona.unitylib.archive");
                                while(!addpkgProcess.IsCompleted)
                                {

                                }

                                if(addpkgProcess.Error != null)
                                {
                                    Debug.LogError("Failed to add package "+addpkgProcess.Error.message);
                                }
                                UnityEditor.PackageManager.Client.Resolve();
                                AssetDatabase.Refresh();
                            }
                            else
                            {
                                Debug.LogError("Failed to update hybriona.unitylib from url " + downloadRequest.url+ " due to " + downloadRequest.error);
                            }

                            
                        }
                    }


                }
                EditorGUILayout.EndVertical();
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

                        GUIStyle selectedStyle = null;

                        if(ModulesUserPrefs.Instance().selectedModuleId == moduleData.id)
                        {
                            selectedStyle = uiStyle.moduleSelectedBtnStyle;
                        }
                        else
                        {
                            selectedStyle = uiStyle.moduleSelectBtnStyle;
                        }
                        EditorGUILayout.BeginVertical(selectedStyle);


                        //GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button(moduleData.displayname, GUI.skin.label))
                        {
                            
                            
                            ModulesUserPrefs.Instance().selectedModuleId = moduleData.id;
                            ModulesUserPrefs.Instance().Save();
                        }
                        if (isActivated)
                        {
                            GUILayout.Label(EditorGUIUtility.IconContent("d_GreenCheckmark"), uiStyle.moduleInstalledIconStyle, GUILayout.Width(20),GUILayout.Height(20));
                        }
                        else
                        {
                            GUILayout.Label(EditorGUIUtility.IconContent("Cancel"), uiStyle.moduleInstalledIconStyle, GUILayout.Width(20), GUILayout.Height(20));
                           
                        }
                        EditorGUILayout.EndHorizontal();

                     

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space(.3f);
                    }

                }
            


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

                        EditorGUILayout.BeginVertical(uiStyle.detailedViewHeaderBGStyle);
                        {
                            EditorGUILayout.LabelField(selectedModule.displayname, uiStyle.detailedViewTitleStyle,GUILayout.Height(30));
                            EditorGUILayout.LabelField(selectedModule.id, uiStyle.detailedViewIdStyle);
                        }
                        EditorGUILayout.EndVertical();
                        GUILayout.Space(3);
                        EditorGUILayout.LabelField(selectedModule.description, EditorStyles.wordWrappedLabel, GUILayout.Height(100));
                        GUILayout.Box("", GUI.skin.button, GUILayout.Height(2));
                        EditorGUILayout.BeginHorizontal(GUI.skin.box);
                        if (!isModuleActive)
                        {
                            if (GUILayout.Button("Enable Module", GUILayout.Width(110)))
                            {
                                ModulesUserPrefs.Instance().dic.Add(selectedModule.id, true);
                                ModulesUserPrefs.Instance().Save();
                                ApplyChanges();
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(selectedModule.sample))
                            {
                                string targetPath = Path.Combine("Assets/Samples/HybrionaLib/", selectedModule.sample);
                                bool samplesAlreadyImported = Directory.Exists(targetPath);

                                if (GUILayout.Button(samplesAlreadyImported ? "Reimport Samples":"Import Samples", GUILayout.Width(120)))
                                {
                                    bool import = !samplesAlreadyImported ||
                                        EditorUtility.DisplayDialog("Confirm Overwrite?","It looks like the samples of \""+selectedModule.id+"\" are already imported. Do you still want to overwrite it? Any changes made by you will get lost.","Yes, Overwrite","Cancel");

                                    if (import)
                                    {
                                        try
                                        {
                                            string fromPath = Path.Combine(modulesData.rootPath, selectedModule.sample.Replace("Samples","Samples~"));

                                            
                                            if (!Directory.Exists(targetPath))
                                            {
                                                Directory.CreateDirectory(targetPath);
                                            }
                                            CopyFilesRecursively(fromPath, targetPath);
                                            AssetDatabase.Refresh();
                                        }
                                        catch (System.Exception ex)
                                        {
                                            Debug.LogError("error " + ex.Message + " - " + ex.StackTrace);
                                        }
                                    }
                                    
                                }
                            }
                            if (!selectedModule.alwaysEnabled)
                            {
                                if (GUILayout.Button("Disable Module", GUILayout.Width(110)))
                                {

                                    if(modulesData.IsModuleUsedInActiveModules(selectedModule))
                                    {
                                        EditorUtility.DisplayDialog("Error!", "Module \"" + selectedModule.id + "\" cannot be disabled as one or more active module is using it.", "Ok") ;
                                    }

                                    else if (EditorUtility.DisplayDialog("Confirm Disable?", "Are you sure you want to disable module \"" + selectedModule.id + "\"", "Yes, Disable", "Cancel"))
                                    {
                                        ModulesUserPrefs.Instance().dic.Add(selectedModule.id, false);
                                        ModulesUserPrefs.Instance().Save();
                                        ApplyChanges();
                                    }
                                }
                            }
                            else
                            {
                                GUILayout.Label("This module cannot be disabled.", uiStyle.detailedViewInfoLblStyle);
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                       

                        
                        
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
            


           


        }

        public static void ApplyChanges(bool doAssetDatabaseRefresh = true)
        {
            InitializeModulesData();



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
                assemblyDataNode["defineConstraints"].Add("ENABLE_LIB");

                assemblyDataNode["versionDefines"] = new JSONArray();
                assemblyDataNode["versionDefines"].Add(JSON.Parse("{\n}"));
                assemblyDataNode["versionDefines"][0]["name"] = "Unity";

                if (toActivate)
                {
                    assemblyDataNode["versionDefines"][0]["define"] = "ENABLE_LIB";//moduleData.define_symbol;


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

        public static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
    }
}
#endif