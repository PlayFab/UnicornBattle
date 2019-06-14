using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace AppCenterEditor
{
    public abstract class AppCenterSDKPackage
    {
        private static int angle = 0;

        public static IEnumerable<AppCenterSDKPackage> SupportedPackages = new AppCenterSDKPackage[]
        {
            AppCenterAnalyticsPackage.Instance,
            AppCenterCrashesPackage.Instance,
            AppCenterDistributePackage.Instance,
            AppCenterPushPackage.Instance
        };

        public string InstalledVersion { get; private set; }
        public bool IsInstalled { get; set; }
        public bool IsPackageInstalling { get; set; }
        public bool IsObjectFieldActive { get; set; }
        protected abstract bool IsSupportedForWSA { get; }
        public abstract string Name { get; }
        public abstract string DownloadLatestUrl { get; }
        public abstract string DownloadUrlFormat { get; }
        public abstract string TypeName { get; }
        public abstract string VersionFieldName { get; }
        protected abstract bool IsSdkPackageSupported();

        public static IEnumerable<AppCenterSDKPackage> GetInstalledPackages()
        {
            var installedPackages = new List<AppCenterSDKPackage>();
            foreach (var package in SupportedPackages)
            {
                if (package.IsInstalled)
                {
                    installedPackages.Add(package);
                }
            }
            return installedPackages;
        }

        private void RemovePackage(bool prompt = true)
        {
            if (prompt && !EditorUtility.DisplayDialog("Confirm SDK Removal", string.Format("This action will remove the current {0} SDK.", Name), "Confirm", "Cancel"))
            {
                return;
            }
            EdExLogger.LoggerInstance.LogWithTimeStamp(string.Format("Removing {0} package...", Name));

            var toDelete = new List<string>();
            string pluginsPath = Path.Combine(AppCenterEditorPrefsSO.Instance.SdkPath, "Plugins");
            string androidPath = Path.Combine(pluginsPath, "Android");
            string sdkPath = Path.Combine(pluginsPath, "AppCenterSDK");
            string iosPath = Path.Combine(pluginsPath, "iOS");
            string wsaPath = Path.Combine(pluginsPath, "WSA");
            toDelete.Add(Path.Combine(androidPath, string.Format("appcenter-{0}-release.aar", Name.ToLower())));
            toDelete.AddRange(Directory.GetFiles(Path.Combine(sdkPath, Name)));
            toDelete.AddRange(Directory.GetDirectories(Path.Combine(sdkPath, Name)));
            toDelete.Add(Path.Combine(sdkPath, Name));
            toDelete.AddRange(Directory.GetFiles(Path.Combine(iosPath, Name)));
            toDelete.AddRange(Directory.GetDirectories(Path.Combine(iosPath, Name)));
            toDelete.Add(Path.Combine(iosPath, Name));
            if (IsSupportedForWSA)
            {
                toDelete.AddRange(Directory.GetFiles(Path.Combine(wsaPath, Name)));
                toDelete.AddRange(Directory.GetDirectories(Path.Combine(wsaPath, Name)));
                toDelete.Add(Path.Combine(wsaPath, Name));
            }

            bool deleted = true;

            foreach (var path in toDelete)
            {
                if (!FileUtil.DeleteFileOrDirectory(path))
                {
                    if (!path.EndsWith("meta"))
                    {
                        deleted = false;
                    }
                }
                FileUtil.DeleteFileOrDirectory(path + ".meta");
            }

            // Remove Core if no packages left.
            List<AppCenterSDKPackage> installedPackages = new List<AppCenterSDKPackage>();
            installedPackages.AddRange(GetInstalledPackages());
            if (installedPackages.Count <= 1)
            {
                AppCenterEditorSDKTools.RemoveSdk(false);
            }

            if (deleted)
            {
                EdExLogger.LoggerInstance.LogWithTimeStamp(string.Format("{0} package removed.", Name));
                AppCenterEditor.RaiseStateUpdate(AppCenterEditor.EdExStates.OnSuccess, string.Format("App Center {0} SDK removed.", Name));

                // HACK for 5.4, AssetDatabase.Refresh(); seems to cause the install to fail.
                if (prompt)
                {
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                AppCenterEditor.RaiseStateUpdate(AppCenterEditor.EdExStates.OnError, string.Format("An unknown error occured and the {0} SDK could not be removed.", Name));
            }
        }

        public void ShowPackageInstalledMenu()
        {
            var isPackageSupported = IsSdkPackageSupported();

            using (new AppCenterGuiFieldHelper.UnityVertical(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleEmpty")))
            {
                var sdkPackageVersion = InstalledVersion;
                using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleEmpty")))
                {
                    GUILayout.FlexibleSpace();
                    var labelStyle = new GUIStyle(AppCenterEditorHelper.uiStyle.GetStyle("versionText"));
                    EditorGUILayout.LabelField(string.Format("{0} SDK {1} is installed", Name, sdkPackageVersion), labelStyle);
                    GUILayout.FlexibleSpace();
                }

                bool packageVersionIsValid = sdkPackageVersion != null && sdkPackageVersion != Constants.UnknownVersion;
                if (packageVersionIsValid && sdkPackageVersion.CompareTo(AppCenterEditorSDKTools.InstalledSdkVersion) != 0)
                {
                    using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleEmpty")))
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField("Warning! Package version is not equal to the AppCenter Core SDK version. ", AppCenterEditorHelper.uiStyle.GetStyle("orTxt"));
                        GUILayout.FlexibleSpace();
                    }
                }

                if (isPackageSupported && AppCenterEditorSDKTools.SdkFolder != null)
                {
                    using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleEmpty")))
                    {
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Remove SDK", AppCenterEditorHelper.uiStyle.GetStyle("textButton")))
                        {
                            RemovePackage();
                        }

                        GUILayout.FlexibleSpace();
                    }
                }
            }
        }

        public void ShowPackageNotInstalledMenu()
        {
            using (new AppCenterGuiFieldHelper.UnityVertical(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleEmpty")))
            {
                using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleEmpty")))
                {
                    GUILayout.FlexibleSpace();
                    var labelStyle = new GUIStyle(AppCenterEditorHelper.uiStyle.GetStyle("versionText"));
                    EditorGUILayout.LabelField(string.Format("{0} SDK is not installed.", Name), labelStyle);
                    GUILayout.FlexibleSpace();
                }

                using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleEmpty")))
                {
                    GUILayout.FlexibleSpace();
                    if (IsPackageInstalling)
                    {
                        var image = DrawUtils.RotateImage(AssetDatabase.LoadAssetAtPath("Assets/AppCenterEditorExtensions/Editor/UI/Images/wheel.png", typeof(Texture2D)) as Texture2D, angle++);
                        GUILayout.Button(new GUIContent(string.Format("  {0} SDK is installing", Name), image), AppCenterEditorHelper.uiStyle.GetStyle("customButton"), GUILayout.MaxWidth(200), GUILayout.MinHeight(32));
                    }
                    else
                    {
                        if (GUILayout.Button("Install SDK", AppCenterEditorHelper.uiStyle.GetStyle("textButton")))
                        {
                            AppCenterEditorSDKTools.IsInstalling = true;
                            IsPackageInstalling = true;
                            ImportLatestPackageSDK();
                        }
                    }
                    GUILayout.FlexibleSpace();
                }
            }
        }

        public string GetDownloadUrl(string version)
        {
            if (string.IsNullOrEmpty(version) || version == Constants.UnknownVersion)
            {
                return DownloadLatestUrl;
            }
            else
            {
                return string.Format(DownloadUrlFormat, version);
            }
        }

        public void GetInstalledVersion(Type type, string coreVersion)
        {
            foreach (var field in type.GetFields())
            {
                if (field.Name == VersionFieldName)
                {
                    InstalledVersion = field.GetValue(field).ToString();
                    break;
                }
            }
            if (string.IsNullOrEmpty(InstalledVersion))
            {
                InstalledVersion = Constants.UnknownVersion;
            }
        }

        private void ImportLatestPackageSDK()
        {
            PackagesInstaller.ImportLatestSDK(new[] { this }, AppCenterEditorSDKTools.LatestSdkVersion);
        }
    }
}
