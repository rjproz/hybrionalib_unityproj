using UnityEngine;
using UnityEditor;
using SimpleJSON;
using System.IO;
using UnityEditorInternal;
using System.Collections.Generic;

namespace Hybriona
{
    [InitializeOnLoad]
    public class HybrionaUnityLibActivator : EditorWindow
    {
        [MenuItem("Hybriona/Hybriona Lib Manager")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(HybrionaUnityLibActivator));
        }

        static HybrionaUnityLibActivator()
        {
           
        }

        private void OnEnable()
        {
            ApplyChanges();
            //Debug.Log("HybrionaUnityLibActivator patched ScriptingDefineSymbols.");
        }

        private static ModulesData modulesData;

        private static void InitializeModulesData(bool forceReload = false)
        {
            if (modulesData == null || forceReload)
            {
                //string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
                string[] resultGuids = AssetDatabase.FindAssets("t:Script HybrionaUnityLibActivator");
                string path = AssetDatabase.GUIDToAssetPath(resultGuids[0]);

                if (path.Contains("\\"))
                {
                    path = path.Replace("\\Editor\\Activator\\HybrionaUnityLibActivator.cs", "\\modules.json");
                    modulesData = JsonUtility.FromJson<ModulesData>(System.IO.File.ReadAllText(path));
                    modulesData.modulesConfigPath = path;
                    modulesData.rootPath = path.Replace("\\modules.json", "\\");
                }
                else
                {
                    path = path.Replace("/Editor/Activator/HybrionaUnityLibActivator.cs", "/modules.json");
                    modulesData = JsonUtility.FromJson<ModulesData>(System.IO.File.ReadAllText(path));
                    modulesData.modulesConfigPath = path;
                    modulesData.rootPath = path.Replace("/modules.json", "/");
                }
                //Debug.Log("Path: "+path);
               


            }
        }

        private void OnFocus()
        {
            InitializeModulesData(forceReload: true);
        }
        private void OnGUI()
        {


            InitializeModulesData(forceReload: true);

            float padding = 10;
            Rect area = new Rect(padding, padding,
                 position.width - padding * 2f, position.height - padding * 2f);

            GUILayout.BeginArea(area);
            {
                GUILayout.Label("Modules Settings", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical(GUI.skin.box);
                for (int i = 0; i < modulesData.modules.Count; i++)
                {
                    
                    var moduleData = modulesData.modules[i];

                    if (moduleData.alwaysEnabled)
                    {
                       
                        EditorGUILayout.LabelField(moduleData.id + " - Always Enabled");
                        
                    }
                    else
                    {
                        var oldValue = false;
                        if (ModulesUserPrefs.Instance().dic.ContainsKey(moduleData.id))
                        {
                            oldValue = ModulesUserPrefs.Instance().dic.GetValue(moduleData.id);
                        }
                        else
                        {
                            oldValue = moduleData.enabled;
                        }

                        var newValueIfChanged = EditorGUILayout.Toggle(moduleData.id, oldValue);
                        ModulesUserPrefs.Instance().dic.Add(moduleData.id, newValueIfChanged);
                    }

                }

                if(GUILayout.Button("Apply Changes"))
                {
                    ModulesUserPrefs.Instance().Save();
                    ApplyChanges();
                }

                if (GUILayout.Button("Reset"))
                {
                    ModulesUserPrefs.Instance().Reset();
                    ApplyChanges();
                    
                }


                EditorGUILayout.EndVertical();

            }
            GUILayout.EndArea();
            if (GUI.changed)
            {
                //Debug.Log("Saving at " + System.DateTime.Now.ToShortDateString());

                //ApplyChanges();
            }


           


        }

        static void ApplyChanges()
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

            List<string> scriptingDefineSymbols = new List<string>();

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

            string pathOfAssemblyDef = Path.Combine(modulesData.rootPath, "Runtime/Hybriona.Lib.asmdef");
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
