using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CreateAssetBundles
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

    [MenuItem("PlayFab/Build Unicorn Battle/Game: Android")]
    public static void BuildForAndroid()
    {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, UbBundleIdentifier);
        var androidPackage = Path.Combine(GetBuildPath(), "UnicornBattle.apk");
        MkDir(GetBuildPath());
        BuildPipeline.BuildPlayer(TestScenes, androidPackage, BuildTarget.Android, BuildOptions.None);
        if (Directory.GetFiles(androidPackage).Length == 0)
            throw new Exception("Target file did not build: " + androidPackage);
    }

    [MenuItem("PlayFab/Build Unicorn Battle/Bundles: All AssetBundles")]
    public static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/", BuildAssetBundleOptions.None, BuildTarget.StandaloneOSXUniversal);
        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/Android", BuildAssetBundleOptions.None, BuildTarget.Android);
        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/iOS", BuildAssetBundleOptions.None, BuildTarget.iOS);
    }

    [MenuItem("PlayFab/Build Unicorn Battle/Cache: Clear")]
    public static void ClearBundleCache()
    {
#if UNITY_2017_1_OR_NEWER
        Caching.ClearCache();
#else
        Caching.CleanCache();
#endif
    }
}
