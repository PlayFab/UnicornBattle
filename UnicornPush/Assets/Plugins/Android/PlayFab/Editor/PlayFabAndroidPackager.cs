
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
            var packagePath = "C:/depot/sdks/UnitySDK/Packages/PushNotification_Unity5_0/AndroidPushPlugin.unitypackage";
            AssetDatabase.ExportPackage(SdkAssets, packagePath, ExportPackageOptions.Recurse);
            Debug.Log("Package built: " + packagePath);
        }
    }
}
