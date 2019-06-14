using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;

namespace AppCenterEditor
{
#if UNITY_5_3_OR_NEWER
    [CreateAssetMenu(fileName = "AppCenterEditorPrefsSO", menuName = "App Center/Make Prefs SO", order = 1)]
#endif
    public class AppCenterEditorPrefsSO : ScriptableObject
    {
        public string SdkPath;
        public string EdExPath;
        public bool PanelIsShown;
        public int curMainMenuIdx;
        private string _latestSdkVersion;
        private string _latestEdExVersion;
        private DateTime _lastSdkVersionCheck;
        private DateTime _lastEdExVersionCheck;
        private static AppCenterEditorPrefsSO _instance;

        public string EdSet_latestSdkVersion { get { return _latestSdkVersion; } set { _latestSdkVersion = value; _lastSdkVersionCheck = DateTime.UtcNow; } }
        public string EdSet_latestEdExVersion { get { return _latestEdExVersion; } set { _latestEdExVersion = value; _lastEdExVersionCheck = DateTime.UtcNow; } }
        public DateTime EdSet_lastSdkVersionCheck { get { return _lastSdkVersionCheck; } }
        public DateTime EdSet_lastEdExVersionCheck { get { return _lastEdExVersionCheck; } }

        public static AppCenterEditorPrefsSO Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                var settingsList = Resources.LoadAll<AppCenterEditorPrefsSO>("AppCenterEditorPrefsSO");
                if (settingsList.Length == 1)
                    _instance = settingsList[0];
                if (_instance != null)
                    return _instance;

                _instance = CreateInstance<AppCenterEditorPrefsSO>();
                if (!Directory.Exists(Path.Combine(Application.dataPath, "AppCenterEditorExtensions/Editor/Resources")))
                    Directory.CreateDirectory(Path.Combine(Application.dataPath, "AppCenterEditorExtensions/Editor/Resources"));
                AssetDatabase.CreateAsset(_instance, "Assets/AppCenterEditorExtensions/Editor/Resources/AppCenterEditorPrefsSO.asset");
                AssetDatabase.SaveAssets();
                EdExLogger.LoggerInstance.LogWithTimeStamp("Created missing AppCenterEditorPrefsSO file");
                return _instance;
            }
        }

        public static void Save()
        {
            EditorUtility.SetDirty(_instance);
            AssetDatabase.SaveAssets();
        }
    }
}
