using System.Collections.Generic;
using UnityEditor;

namespace AppCenterEditor
{
    public static class PackagesInstaller
    {
        public static void ImportLatestSDK(IEnumerable<AppCenterSDKPackage> packagesToImport, string version, string existingSdkPath = null)
        {
            try
            {
                var downloadUrls = new List<string>();
                foreach (var package in packagesToImport)
                {
                    downloadUrls.Add(package.GetDownloadUrl(version));
                }
                AppCenterEditorHttp.MakeDownloadCall(downloadUrls, downloadedFiles =>
                {
                    try
                    {
                        foreach (var file in downloadedFiles)
                        {
                            EdExLogger.LoggerInstance.LogWithTimeStamp("Importing package: " + file);
                            AssetDatabase.ImportPackage(file, false);
                            EdExLogger.LoggerInstance.LogWithTimeStamp("Deleting file: " + file);
                            FileUtil.DeleteFileOrDirectory(file);
                        }
                        AppCenterEditorPrefsSO.Instance.SdkPath = string.IsNullOrEmpty(existingSdkPath) ? AppCenterEditorHelper.DEFAULT_SDK_LOCATION : existingSdkPath;
                        //AppCenterEditorDataService.SaveEnvDetails();
                        EdExLogger.LoggerInstance.LogWithTimeStamp("App Center SDK install complete");
                    }
                    finally
                    {
                        AppCenterEditorSDKTools.IsInstalling = false;
                    }
                });
            }
            catch
            {
                AppCenterEditorSDKTools.IsInstalling = false;
                throw;
            }
        }
    }
}
