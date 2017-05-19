
using UnityEditor;
using UnityEngine;

namespace PlayFab.Internal
{
    public static class PlayFabAndroidPackager
    {
        private static readonly string[] SdkAssets = {
            "Assets/Plugins/Android"
        };

        [MenuItem("PlayFab/Testing/Build PlayFab Android-Push UnityPackage")]
        public static void BuildAndroidPushPluginPackage()
        {
            var packagePath = "C:/depot/sdks/UnitySDK/Packages/Push_Unity5_GPS10.0.1/AndroidPushPlugin.unitypackage";
            AssetDatabase.ExportPackage(SdkAssets, packagePath, ExportPackageOptions.Recurse);
            Debug.Log("Package built: " + packagePath);
        }
    }
}
