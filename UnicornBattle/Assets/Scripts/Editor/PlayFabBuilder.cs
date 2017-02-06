using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace PlayFab.Internal
{
    public static class PlayFabBuilder
    {
        private static readonly string UbBundleIdentifier = "com.playfab.unicornbattle2";
        private static readonly string[] TestScenes = {
            "assets/scenes/Splash.unity",
            "assets/scenes/Authenticate.unity",
            "assets/scenes/Profile.unity",
            "assets/scenes/CharacterSelect.unity",
            "assets/scenes/Gameplay.unity",
        };

        #region Utility Functions
        private static string GetBuildPath()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "../../"));
        }
        private static void MkDir(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        #endregion Utility Functions

        [MenuItem("PlayFab/Build Unicorn Battle/Android")]
        public static void BuildForAndroid()
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
            PlayerSettings.bundleIdentifier = UbBundleIdentifier;
            var androidPackage = Path.Combine(GetBuildPath(), "UnicornBattle.apk");
            MkDir(GetBuildPath());
            BuildPipeline.BuildPlayer(TestScenes, androidPackage, BuildTarget.Android, BuildOptions.None);
            if (Directory.GetFiles(androidPackage).Length == 0)
                throw new Exception("Target file did not build: " + androidPackage);
        }
    }
}
