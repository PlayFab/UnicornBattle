using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AppCenterEditor
{
    public class AppCenterEditorSDKTools : Editor
    {
        public enum SDKState
        {
            SDKNotInstalled,
            SDKNotInstalledAndInstalling,
            SDKNotFull,
            SDKNotFullAndInstalling,
            SDKIsFull
        }
        public static bool IsInstalled { get { return AreSomePackagesInstalled(); } }
        public static bool IsFullSDK { get { return CheckIfAllPackagesInstalled(); } }
        public static bool IsInstalling { get; set; }
        public static bool IsUpgrading { get; set; }
        public static string LatestSdkVersion { get; private set; }
        public static UnityEngine.Object SdkFolder { get; private set; }
        public static string InstalledSdkVersion { get; private set; }
        public static GUIStyle TitleStyle { get { return new GUIStyle(AppCenterEditorHelper.uiStyle.GetStyle("titleLabel")); } }

        private static Type appCenterSettingsType = null;
        private static bool isInitialized; // used to check once, gets reset after each compile
        private static UnityEngine.Object _previousSdkFolderPath;
        private static bool sdkFolderNotFound;
        private static int angle = 0;

        public static SDKState GetSDKState()
        {
            if (!IsInstalled)
            {
                if (IsInstalling)
                {
                    return SDKState.SDKNotInstalledAndInstalling;
                }
                else
                {
                    return SDKState.SDKNotInstalled;
                }
            }

            //SDK installed.
            if (IsFullSDK)
            {
                return SDKState.SDKIsFull;
            }

            //SDK is not full.
            if (IsInstalling)
            {
                return SDKState.SDKNotFullAndInstalling;
            }
            else
            {
                return SDKState.SDKNotFull;
            }
        }

        public static void DrawSdkPanel()
        {
            if (!isInitialized)
            {
                //SDK is installed.
                CheckSdkVersion();
                isInitialized = true;
                GetLatestSdkVersion();
                SdkFolder = FindSdkAsset();

                if (SdkFolder != null)
                {
                    AppCenterEditorPrefsSO.Instance.SdkPath = AssetDatabase.GetAssetPath(SdkFolder);
                    // AppCenterEditorDataService.SaveEnvDetails();
                }
            }
            ShowSdkInstallationPanel();
        }

        public static void DisplayPackagePanel(AppCenterSDKPackage sdkPackage)
        {
            using (new AppCenterGuiFieldHelper.UnityVertical(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleGray1")))
            {
                using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                {
                    GUILayout.FlexibleSpace();
                    if (sdkPackage.IsInstalled)
                    {
                        sdkPackage.ShowPackageInstalledMenu();
                    }
                    else
                    {
                        sdkPackage.ShowPackageNotInstalledMenu();
                    }
                    GUILayout.FlexibleSpace();
                }
            }
        }

        private static void ShowSdkInstallationPanel()
        {
            sdkFolderNotFound = SdkFolder == null;

            if (_previousSdkFolderPath != SdkFolder)
            {
                // something changed, better save the result.
                _previousSdkFolderPath = SdkFolder;

                AppCenterEditorPrefsSO.Instance.SdkPath = (AssetDatabase.GetAssetPath(SdkFolder));
                //TODO: check if we need this?
                // AppCenterEditorDataService.SaveEnvDetails();

                sdkFolderNotFound = false;
            }
            SDKState SDKstate = GetSDKState();
            using (new AppCenterGuiFieldHelper.UnityVertical(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleGray1")))
            {
                switch (SDKstate)
                {
                    case SDKState.SDKNotInstalled:
                        ShowNOSDKLabel();
                        ShowInstallButton();
                        break;

                    case SDKState.SDKNotInstalledAndInstalling:
                        ShowNOSDKLabel();
                        ShowInstallingButton();
                        break;

                    case SDKState.SDKNotFull:
                        ShowSdkInstalledLabel();
                        ShowFolderObject();
                        ShowInstallButton();
                        ShowRemoveButton();
                        break;

                    case SDKState.SDKNotFullAndInstalling:
                        ShowSdkInstalledLabel();
                        ShowFolderObject();
                        ShowInstallingButton();
                        ShowRemoveButton();
                        break;

                    case SDKState.SDKIsFull:
                        ShowSdkInstalledLabel();
                        ShowFolderObject();
                        ShowRemoveButton();
                        break;
                }
            }
        }

        public static void ShowUpgradePanel()
        {
            if (!sdkFolderNotFound)
            {
                using (new AppCenterGuiFieldHelper.UnityVertical(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleGray1")))
                {
                    string[] versionNumber = !string.IsNullOrEmpty(InstalledSdkVersion) ? InstalledSdkVersion.Split('.') : new string[0];

                    var numerical = 0;
                    bool isEmptyVersion = string.IsNullOrEmpty(InstalledSdkVersion) || versionNumber == null || versionNumber.Length == 0;
                    if (isEmptyVersion || (versionNumber.Length > 0 && int.TryParse(versionNumber[0], out numerical) && numerical < 0))
                    {
                        //older version of the SDK
                        using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                        {
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.LabelField("SDK is outdated. Consider upgrading to the get most features.", AppCenterEditorHelper.uiStyle.GetStyle("orTxt"));
                            GUILayout.FlexibleSpace();
                        }
                    }

                    var buttonWidth = 200;

                    GUILayout.Space(5);
                    if (ShowSDKUpgrade())
                    {
                        if (IsUpgrading)
                        {
                            using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                            {
                                GUILayout.FlexibleSpace();
                                var image = DrawUtils.RotateImage(AssetDatabase.LoadAssetAtPath("Assets/AppCenterEditorExtensions/Editor/UI/Images/wheel.png", typeof(Texture2D)) as Texture2D, angle++);
                                GUILayout.Button(new GUIContent("  Upgrading to " + LatestSdkVersion, image), AppCenterEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxWidth(buttonWidth), GUILayout.MinHeight(32));
                                GUILayout.FlexibleSpace();
                            }
                        }
                        else
                        {
                            using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                            {
                                GUILayout.FlexibleSpace();
                                if (GUILayout.Button("Upgrade to " + LatestSdkVersion, AppCenterEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32)))
                                {
                                    IsUpgrading = true;
                                    UpgradeSdk();
                                }
                                GUILayout.FlexibleSpace();
                            }
                        }
                    }
                    else
                    {
                        using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                        {
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.LabelField("You have the latest SDK!", TitleStyle, GUILayout.MinHeight(32));
                            GUILayout.FlexibleSpace();
                        }
                    }
                    GUILayout.Space(5);

                    using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("VIEW RELEASE NOTES", AppCenterEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinHeight(32), GUILayout.MinWidth(200)))
                        {
                            Application.OpenURL("https://github.com/Microsoft/AppCenter-SDK-Unity/releases");
                        }
                        GUILayout.FlexibleSpace();
                    }
                }
            }
        }

        private static void ShowRemoveButton()
        {
            if (!sdkFolderNotFound)
            {
                using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("REMOVE SDK", AppCenterEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinHeight(32), GUILayout.MinWidth(200)))
                    {
                        RemoveSdk();
                    }

                    GUILayout.FlexibleSpace();
                }
            }
        }

        private static void ShowFolderObject()
        {
            if (sdkFolderNotFound)
            {
                EditorGUILayout.LabelField("An SDK was detected, but we were unable to find the directory. Drag-and-drop the top-level App Center SDK folder below.",
                    AppCenterEditorHelper.uiStyle.GetStyle("orTxt"));
            }
            else
            {
                // This hack is needed to disable folder object and remove the blue border around it.
                // Other UI is getting enabled later in the method.
                GUI.enabled = false;
            }

            GUILayout.Space(5);
            using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleClearWithleftPad")))
            {
                GUILayout.FlexibleSpace();
                SdkFolder = EditorGUILayout.ObjectField(SdkFolder, typeof(UnityEngine.Object), false, GUILayout.MaxWidth(200));
                GUILayout.FlexibleSpace();
            }
            GUILayout.Space(5);
            GUI.enabled = AppCenterEditor.IsGUIEnabled();
        }

        private static void ShowSdkInstalledLabel()
        {
            GUILayout.Space(5);
            EditorGUILayout.LabelField(string.Format("SDK {0} is installed", string.IsNullOrEmpty(InstalledSdkVersion) ? Constants.UnknownVersion : InstalledSdkVersion),
                TitleStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(5);
        }

        private static void ShowInstallingButton()
        {
            var buttonWidth = 250;
            GUILayout.Space(5);
            using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleEmpty")))
            {
                GUILayout.FlexibleSpace();
                var image = DrawUtils.RotateImage(AssetDatabase.LoadAssetAtPath("Assets/AppCenterEditorExtensions/Editor/UI/Images/wheel.png", typeof(Texture2D)) as Texture2D, angle++);
                GUILayout.Button(new GUIContent("  SDK is installing", image), AppCenterEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxWidth(buttonWidth), GUILayout.MinHeight(32));
                GUILayout.FlexibleSpace();
            }
            GUILayout.Space(5);
        }

        private static void ShowInstallButton()
        {
            var buttonWidth = 250;
            GUILayout.Space(5);
            using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleEmpty")))
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Install all App Center SDK packages", AppCenterEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxWidth(buttonWidth), GUILayout.MinHeight(32)))
                {
                    IsInstalling = true;
                    PackagesInstaller.ImportLatestSDK(GetNotInstalledPackages(), LatestSdkVersion);
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.Space(5);
        }

        private static void ShowNOSDKLabel()
        {
            EditorGUILayout.LabelField("No SDK is installed.", TitleStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(10);
        }

        public static bool AreSomePackagesInstalled()
        {
            return GetAppCenterSettings() != null;
        }

        public static List<AppCenterSDKPackage> GetNotInstalledPackages()
        {
            List<AppCenterSDKPackage> notInstalledPackages = new List<AppCenterSDKPackage>();
            if (!IsInstalled)
            {
                notInstalledPackages.AddRange(AppCenterSDKPackage.SupportedPackages);
                return notInstalledPackages;
            }
            foreach (var package in AppCenterSDKPackage.SupportedPackages)
            {
                if (!package.IsInstalled)
                {
                    notInstalledPackages.Add(package);
                }
            }
            return notInstalledPackages;
        }

        public static bool CheckIfAllPackagesInstalled()
        {
            foreach (var package in AppCenterSDKPackage.SupportedPackages)
            {
                if (!package.IsInstalled)
                {
                    return false;
                }
            }
            return GetAppCenterSettings() != null;
        }

        public static Type GetAppCenterSettings()
        {
            if (appCenterSettingsType == typeof(object))
                return null; // Sentinel value to indicate that AppCenterSettings doesn't exist
            if (appCenterSettingsType != null)
                return appCenterSettingsType;

            appCenterSettingsType = typeof(object); // Sentinel value to indicate that AppCenterSettings doesn't exist
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in allAssemblies)
                foreach (var eachType in assembly.GetTypes())
                    if (eachType.Name == AppCenterEditorHelper.APPCENTER_SETTINGS_TYPENAME)
                        appCenterSettingsType = eachType;
            //if (appCenterSettingsType == typeof(object))
            //    Debug.LogWarning("Should not have gotten here: "  + allAssemblies.Length);
            //else
            //    Debug.Log("Found Settings: " + allAssemblies.Length + ", " + appCenterSettingsType.Assembly.FullName);
            return appCenterSettingsType == typeof(object) ? null : appCenterSettingsType;
        }

        private static bool ShowSDKUpgrade()
        {
            if (string.IsNullOrEmpty(LatestSdkVersion) || LatestSdkVersion == Constants.UnknownVersion)
            {
                return false;
            }

            if (string.IsNullOrEmpty(InstalledSdkVersion) || InstalledSdkVersion == Constants.UnknownVersion)
            {
                return true;
            }
            bool isOutdated = false;

            foreach (var package in AppCenterSDKPackage.SupportedPackages)
            {
                if (package.IsInstalled)
                {
                    string packageVersion = package.InstalledVersion;
                    bool isPackageOutdated = false;
                    if (string.IsNullOrEmpty(packageVersion) || packageVersion == Constants.UnknownVersion)
                    {
                        isPackageOutdated = true;
                    }
                    else
                    {
                        string[] current = packageVersion.Split('.');
                        string[] latest = LatestSdkVersion.Split('.');
                        isPackageOutdated = int.Parse(latest[0]) > int.Parse(current[0])
                        || int.Parse(latest[1]) > int.Parse(current[1])
                        || int.Parse(latest[2]) > int.Parse(current[2]);
                    }
                    if (isPackageOutdated)
                    {
                        isOutdated = true;
                    }
                }
            }

            return isOutdated;            
        }

        private static void UpgradeSdk()
        {
            if (EditorUtility.DisplayDialog("Confirm SDK Upgrade", "This action will remove the current App Center SDK and install the lastet version.", "Confirm", "Cancel"))
            {
                IEnumerable<AppCenterSDKPackage> installedPackages = AppCenterSDKPackage.GetInstalledPackages();
                RemoveSdkBeforeUpdate();
                PackagesInstaller.ImportLatestSDK(installedPackages, LatestSdkVersion, AppCenterEditorPrefsSO.Instance.SdkPath);
            }
        }

        private static void RemoveSdkBeforeUpdate()
        {
            var skippedFiles = new[]
            {
                "AppCenterSettings.asset",
                "AppCenterSettings.asset.meta",
                "AppCenterSettingsAdvanced.asset",
                "AppCenterSettingsAdvanced.asset.meta"
            };

            RemoveAndroidSettings();

            var toDelete = new List<string>();
            toDelete.AddRange(Directory.GetFiles(AppCenterEditorPrefsSO.Instance.SdkPath));
            toDelete.AddRange(Directory.GetDirectories(AppCenterEditorPrefsSO.Instance.SdkPath));

            foreach (var path in toDelete)
            {
                if (!skippedFiles.Contains(Path.GetFileName(path)))
                {
                    FileUtil.DeleteFileOrDirectory(path);
                }
            }
        }

        public static void RemoveSdk(bool prompt = true)
        {
            if (prompt && !EditorUtility.DisplayDialog("Confirm SDK Removal", "This action will remove the current App Center SDK.", "Confirm", "Cancel"))
            {
                return;
            }
            EdExLogger.LoggerInstance.LogWithTimeStamp("Removing SDK...");

            RemoveAndroidSettings();

            if (FileUtil.DeleteFileOrDirectory(AppCenterEditorPrefsSO.Instance.SdkPath))
            {
                FileUtil.DeleteFileOrDirectory(AppCenterEditorPrefsSO.Instance.SdkPath + ".meta");
                AppCenterEditor.RaiseStateUpdate(AppCenterEditor.EdExStates.OnSuccess, "App Center SDK removed.");

                EdExLogger.LoggerInstance.LogWithTimeStamp("App Center SDK removed.");
                // HACK for 5.4, AssetDatabase.Refresh(); seems to cause the install to fail.
                if (prompt)
                {
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                AppCenterEditor.RaiseStateUpdate(AppCenterEditor.EdExStates.OnError, "An unknown error occured and the App Center SDK could not be removed.");
            }
        }

        private static void RemoveAndroidSettings()
        {
            if (Directory.Exists(Application.dataPath + "/Plugins/Android/res/values"))
            {
                var files = Directory.GetFiles(Application.dataPath + "/Plugins/Android/res/values", "appcenter-settings.xml*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    FileUtil.DeleteFileOrDirectory(file);
                }
            }
        }

        private static void CheckSdkVersion()
        {
            if (!string.IsNullOrEmpty(InstalledSdkVersion))
                return;

            var packageTypes = new Dictionary<AppCenterSDKPackage, Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.FullName == Constants.WrapperSdkClassName)
                        {
                            foreach (var field in type.GetFields())
                            {
                                if (field.Name == Constants.WrapperSdkVersionFieldName)
                                {
                                    InstalledSdkVersion = field.GetValue(field).ToString();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (var package in AppCenterSDKPackage.SupportedPackages)
                            {
                                if (type.FullName == package.TypeName)
                                {
                                    package.IsInstalled = true;
                                    packageTypes[package] = type;
                                }
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    // For this failure, silently skip this assembly unless we have some expectation that it contains App Center
                    if (assembly.FullName.StartsWith("Assembly-CSharp")) // The standard "source-code in unity proj" assembly name
                    {
                        EdExLogger.LoggerInstance.LogWarning("App Center Editor Extension error, failed to access the main CSharp assembly that probably contains App Center SDK");
                    }
                    continue;
                }
            }
            foreach (var packageType in packageTypes)
            {
                packageType.Key.GetInstalledVersion(packageType.Value, InstalledSdkVersion);
            }
        }

        private static void GetLatestSdkVersion()
        {
            var threshold = AppCenterEditorPrefsSO.Instance.EdSet_lastSdkVersionCheck != DateTime.MinValue ? AppCenterEditorPrefsSO.Instance.EdSet_lastSdkVersionCheck.AddHours(1) : DateTime.MinValue;

            if (DateTime.Today > threshold)
            {
                AppCenterEditorHttp.MakeGitHubApiCall("https://api.github.com/repos/Microsoft/AppCenter-SDK-Unity/git/refs/tags", (version) =>
                {
                    LatestSdkVersion = version ?? Constants.UnknownVersion;
                    AppCenterEditorPrefsSO.Instance.EdSet_latestSdkVersion = LatestSdkVersion;
                });
            }
            else
            {
                LatestSdkVersion = AppCenterEditorPrefsSO.Instance.EdSet_latestSdkVersion;
            }
        }

        private static UnityEngine.Object FindSdkAsset()
        {
            UnityEngine.Object sdkAsset = null;

            // look in editor prefs
            if (AppCenterEditorPrefsSO.Instance.SdkPath != null)
            {
                sdkAsset = AssetDatabase.LoadAssetAtPath(AppCenterEditorPrefsSO.Instance.SdkPath, typeof(UnityEngine.Object));
            }
            if (sdkAsset != null)
                return sdkAsset;

            sdkAsset = AssetDatabase.LoadAssetAtPath(AppCenterEditorHelper.DEFAULT_SDK_LOCATION, typeof(UnityEngine.Object));
            if (sdkAsset != null)
                return sdkAsset;

            var fileList = Directory.GetDirectories(Application.dataPath, "*AppCenter", SearchOption.AllDirectories);
            if (fileList.Length == 0)
                return null;

            var relPath = fileList[0].Substring(fileList[0].LastIndexOf("Assets" + Path.DirectorySeparatorChar));
            return AssetDatabase.LoadAssetAtPath(relPath, typeof(UnityEngine.Object));
        }
    }
}
