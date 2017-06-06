
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PlayFab.Internal
{
    public static class PlayFabAndroidPackager
    {
        private static readonly Dictionary<string, string> packagePaths = new Dictionary<string, string> {
            {"10.0.1", "C:/depot/sdks/UnitySDK/Packages/Push_Unity5_GPS10.0.1.unitypackage"},
            {"8.4.0", "C:/depot/sdks/UnitySDK/Packages/Push_Unity5_GPS8.4.0.unitypackage"},
        };

        private static readonly string[] SdkAssets = {
            "Assets/Plugins/Android"
        };

        [MenuItem("PlayFab/Testing/Build PlayFab Android-Push UnityPackage")]
        public static void BuildAndroidPushPluginPackage()
        {
            var files = Directory.GetFiles(Application.dataPath + "/Plugins/Android");
            foreach (var versionPair in packagePaths)
            {
                foreach (var file in files)
                {
                    if (file.Contains("play-services-gcm") && file.Contains(versionPair.Key))
                    {
                        var packagePath = packagePaths[versionPair.Key];
                        Debug.Log("Building package: " + packagePath);
                        AssetDatabase.ExportPackage(SdkAssets, packagePath, ExportPackageOptions.Recurse);
                        Debug.Log("Package built: " + packagePath);
                        return;
                    }
                }
            }
        }
    }
}
