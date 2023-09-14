using UnityEngine;
using UnityEditor;
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
            Debug.Log("HybrionaUnityLibActivator patched ScriptingDefineSymbols.");
        }

        private static ModulesData modulesData;

        private static void InitializeModulesData()
        {
            if (modulesData == null)
            {
                //string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
                string[] resultGuids = AssetDatabase.FindAssets("t:Script HybrionaUnityLibActivator");
                string path = AssetDatabase.GUIDToAssetPath(resultGuids[0]);
                path = path.Replace("/Editor/Activator/HybrionaUnityLibActivator.cs", "/module.json");
                //Debug.Log("Path: "+path);
                modulesData = JsonUtility.FromJson<ModulesData>(System.IO.File.ReadAllText(path));

               
            }
        }

        void OnGUI()
        {


            InitializeModulesData();

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

                        var newValueIfChanged = EditorGUILayout.Toggle(moduleData.id, oldValue);
                        ModulesUserPrefs.Instance().dic.Add(moduleData.id, newValueIfChanged);
                    }

                }

                if(GUILayout.Button("Apply Changes"))
                {
                    ModulesUserPrefs.Instance().Save();
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

        }
    }
}
