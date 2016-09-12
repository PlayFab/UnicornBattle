using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace PlayFab.Editor
{
    public class PlayFabEditorSDKTools : UnityEditor.Editor
    {
        internal static bool IsInstalled = IsSDKInstalled();
        private static bool isInstalled = false;

        private static string installedSdkVersion = string.Empty;
        private static string latestSdkVersion = string.Empty;



        public static UnityEngine.Object sdkFolder;
        private static UnityEngine.Object _previoussSdkFolderPath;
        private static bool isObjectFieldActive;
        private static bool isInitialized; //used to check once, gets reset after each compile;
        public static bool isSdkSupported = true;

        public static void DrawSdkPanel()
        {
            if(!isInitialized )
            {
                //SDK is installed.
                isInstalled = IsInstalled;
                CheckSdkVersion();
                isInitialized = true;
                GetLatestSdkVersion();
                sdkFolder = FindSdkAsset();

                if(sdkFolder != null)
                {
                    PlayFabEditorDataService.envDetails.sdkPath = AssetDatabase.GetAssetPath(sdkFolder);
                    PlayFabEditorDataService.SaveEnvDetails();
                }
            }


            if (isInstalled)
            {
    
                // cant find the sdk, but we suspect its in the project (either in-full or in-part)
                isObjectFieldActive = sdkFolder == null ? true : false;

                if(_previoussSdkFolderPath != sdkFolder)
                {
                    // something changed, better save the result.
                    _previoussSdkFolderPath = sdkFolder;

                    PlayFabEditorDataService.envDetails.sdkPath = (AssetDatabase.GetAssetPath(sdkFolder));
                    PlayFabEditorDataService.SaveEnvDetails();

                    isObjectFieldActive = false;
                }

                GUIStyle labelStyle = new GUIStyle(PlayFabEditorHelper.uiStyle.GetStyle("titleLabel"));


                GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                    GUILayout.Label(string.Format("SDK v{0} is installed", string.IsNullOrEmpty(installedSdkVersion) ? "Unknown" : installedSdkVersion), labelStyle, GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));

                    if(!isObjectFieldActive)
                    {
                        GUI.enabled = false;
                       
                    }
                    else
                    {
                    GUILayout.Label("An SDK was detected, but we were unable to find the directory. Drag-and-drop the top-level PlayFab SDK folder below.", PlayFabEditorHelper.uiStyle.GetStyle("orTxt"));
                    }
           
                    GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                        GUILayout.FlexibleSpace();
                        sdkFolder = EditorGUILayout.ObjectField(sdkFolder, typeof(UnityEngine.Object), false, GUILayout.MaxWidth(200));
                        GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    if(!isObjectFieldActive)
                    {
                        // this is a hack to prevent our "block while loading technique" from breaking up at this point.
                        GUI.enabled = EditorApplication.isCompiling || PlayFabEditor.blockingRequests.Count > 0 ? false : true;
                    }

                    if(isSdkSupported == true && sdkFolder != null)
                        {
                            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));

                                GUILayout.FlexibleSpace();

                                if (GUILayout.Button("REMOVE SDK", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinHeight(32), GUILayout.MinWidth(200)))
                                {
                                    RemoveSDK();
                                }

                                GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal(); 
                        }
                
                GUILayout.EndVertical();

                if(sdkFolder != null)
                {

                  
                    //TODO START BACK HERE...

                    GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                        if(installedSdkVersion == "Unknown")
                        {
                            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                            GUILayout.Label("We were unable to determine what version of SDK is installed.", PlayFabEditorHelper.uiStyle.GetStyle("cGenTxt"), GUILayout.MinHeight(32));
                            GUILayout.EndHorizontal();    
                            isSdkSupported = false;
                        }
                        else
                        {
                            isSdkSupported = false;
                            string[] versionNumber = !string.IsNullOrEmpty(installedSdkVersion) ? installedSdkVersion.Split('.') : null;

                            int numerical = 0;
                            if(versionNumber.Length > 0 && int.TryParse(versionNumber[0], out numerical) && numerical < 2)
                            {
                                  //older version of the SDK
                                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));    
                                GUILayout.Label("Most of the Editor Extensions depend on SDK versions >2.0. Consider upgrading to the get most features.", PlayFabEditorHelper.uiStyle.GetStyle("orTxt"));
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                                GUILayout.FlexibleSpace();
                                if (GUILayout.Button("READ THE UPGRADE GUIDE", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinHeight(32)))
                                {
                                    Application.OpenURL("https://github.com/PlayFab/UnitySDK/blob/master/UPGRADE.md");
                                }
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                            }
                            else if(numerical >= 2)
                            {
                                isSdkSupported = true;
                            }
                        }

                        GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));

                        if(ShowSDKUpgrade())
                        {
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("Upgrade to " + latestSdkVersion, PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32)))
                            {
                                UpgradeSdk();
                            }
                            GUILayout.FlexibleSpace();
                        
                        }
                        else if (isSdkSupported)
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.Label("You have the latest SDK!", labelStyle, GUILayout.MinHeight(32));
                            GUILayout.FlexibleSpace();
                        }
                   
                        GUILayout.EndHorizontal();

 
                    GUILayout.EndVertical();
                  }
               
                if(string.IsNullOrEmpty(PlayFabEditorDataService.envDetails.selectedTitleId))
                {
                    GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                        GUILayout.Label("Before making PlayFab API calls, the SDK must be configured to your PlayFab Title.", PlayFabEditorHelper.uiStyle.GetStyle("orTxt"));
                        GUILayout.BeginHorizontal();
                                GUILayout.FlexibleSpace();
                                if(GUILayout.Button("SET MY TITLE", PlayFabEditorHelper.uiStyle.GetStyle("textButton") ))
                                {
                                    PlayFabEditorMenu.OnSettingsClicked();
                                }
                                GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }

                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("VIEW RELEASE NOTES", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinHeight(32), GUILayout.MinWidth(200)))
                    {
                        Application.OpenURL("https://api.playfab.com/releaseNotes/");
                    }

                    GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


               

            }
            else
            {
                //NO SDK Is Installed..

                GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));

                GUIStyle labelStyle = new GUIStyle(PlayFabEditorHelper.uiStyle.GetStyle("titleLabel"));

                GUILayout.Label("No SDK is installed.", labelStyle, GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));
                GUILayout.Space(20);

                GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                var buttonWidth = 150;

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Install PlayFab SDK", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxWidth(buttonWidth), GUILayout.MinHeight(32)))
                {
                    ImportLatestSDK();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();


            }

        }

        public static void ImportLatestSDK()
        {
            PlayFabEditorHttp.MakeDownloadCall("https://api.playfab.com/sdks/download/unity-via-edex", (fileName) => 
            {
                Debug.Log("PlayFab SDK Install: Complete");
                AssetDatabase.ImportPackage(fileName, false); 

                PlayFabEditorDataService.envDetails.sdkPath = PlayFabEditorHelper.DEFAULT_SDK_LOCATION;
                PlayFabEditorDataService.SaveEnvDetails();
                  
            });
        }

        private static bool IsSDKInstalled()
        {
            var playfabVersionType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetTypes()
                        where type.Name == "PlayFabSettings"
                        select type);
            

            return playfabVersionType.ToList().Count > 0;
        }
 
        private static string CheckSdkVersion()
        {
            // check for less old SDK versions next
            if(string.IsNullOrEmpty(installedSdkVersion))
            {
                
                List<Type> types = new List<Type>();

                // check for older SDK versions first
                var oldPlayfabVersionTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetTypes()
                        where type.Name == "PlayFabVersion"
                        select type);

                types = oldPlayfabVersionTypes.ToList();

                if(types.Count > 0)
                {
                    foreach(var type in types)
                    {
                        var properties = (from property in type.GetProperties()
                            where property.Name == "SdkVersion" || property.Name == "SdkRevision" 
                            select property);

                        foreach(var prop in properties)
                        {
                            installedSdkVersion += prop.GetValue(prop,null).ToString();
                        }
                    }
                    if(!String.IsNullOrEmpty(installedSdkVersion))
                    {
                        return installedSdkVersion;
                    }
                }
            }

            // check for newer SDK versions last  
            if(string.IsNullOrEmpty(installedSdkVersion))
            {
                
                List<Type> types = new List<Type>();

                // check for older SDK versions first
                var oldPlayfabVersionTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetTypes()
                        where type.Name == "PlayFabSettings"
                        select type);

                types = oldPlayfabVersionTypes.ToList();

                if(types.Count > 0)
                {
                    foreach(var type in types)
                    {
                        var fields = (from field in type.GetFields()
                            where field.Name == "SdkVersion" || field.Name == "SdkRevision" 
                            select field);

                        foreach(var f in fields)
                        {
                            installedSdkVersion += f.GetValue(f).ToString();
                        }
                    }
                    if(!String.IsNullOrEmpty(installedSdkVersion))
                    {
                        return installedSdkVersion;
                    }
                }
            }
            return null;
           
        }

        private static UnityEngine.Object FindSdkAsset()
        {
            UnityEngine.Object sdkAsset = null;

            // look in editor prefs
            if(PlayFabEditorDataService.envDetails.sdkPath != null)
            {
                sdkAsset = AssetDatabase.LoadAssetAtPath(PlayFabEditorDataService.envDetails.sdkPath, typeof(UnityEngine.Object));
            }

            if(sdkAsset == null)
            {
                sdkAsset = AssetDatabase.LoadAssetAtPath(PlayFabEditorHelper.DEFAULT_SDK_LOCATION, typeof(UnityEngine.Object));
            }
            else
            {
                return sdkAsset;
            }

            if(sdkAsset == null)
            {
                var fileList = Directory.GetDirectories(Application.dataPath, "*PlayFabSdk", SearchOption.AllDirectories);

                if(fileList.Length == 0)
                {
                    fileList = Directory.GetDirectories(Application.dataPath, "*PlayFabSDK", SearchOption.AllDirectories);
                    if(fileList.Length > 0)
                    {
                        return AssetDatabase.LoadAssetAtPath(fileList[0], typeof(UnityEngine.Object));
                    }  
                }
                else
                {
                    var relPath = fileList[0].Substring(fileList[0].IndexOf("Assets/"));
                    return AssetDatabase.LoadAssetAtPath(relPath, typeof(UnityEngine.Object));
                }
            }
            else
            {
                return sdkAsset;
            }

            return null;
        }

        private static bool ShowSDKUpgrade()
        {
            if(string.IsNullOrEmpty(latestSdkVersion) || latestSdkVersion == "Unknown")
            {
                return false;
            }


            if(string.IsNullOrEmpty(installedSdkVersion) || installedSdkVersion == "Unknown")
            {
                return true;
            }

           string[] currrent = installedSdkVersion.Split('.');
           string[] latest = latestSdkVersion.Split('.');

            if(int.Parse(currrent[0]) < 2)
           {
               return false;
           }

           if(int.Parse(latest[0]) > int.Parse(currrent[0]))
           {
                return true;
           }
            else if(int.Parse(latest[1]) > int.Parse(currrent[1]))
           {
                return true;
           }
            else if(int.Parse(latest[2]) > int.Parse(currrent[2]))
           {
                return true;
           }

           return false;
        }

        private static void UpgradeSdk()
        {
            if( EditorUtility.DisplayDialog("Confirm SDK Upgrade", "This action will remove the current PlayFab SDK and install the lastet version. Related plug-ins will need to be manually upgraded.", "Confirm", "Cancel"))
            {
                RemoveSDK(false);
                ImportLatestSDK();
            }
        }

        private static void RemoveSDK(bool prompt = true)
        {
            if(prompt)
            {
                if(!EditorUtility.DisplayDialog("Confirm SDK Removal", "This action will remove the current PlayFab SDK. Related plug-ins will need to be manually removed.", "Confirm", "Cancel"))
                {
                    return;
                }
            }

            //try to clean-up the plugin dirs
            var folders = Directory.GetDirectories(Application.dataPath+"/Plugins", "PlayFabShared", SearchOption.AllDirectories);
            foreach(var folder in folders)
            {
                FileUtil.DeleteFileOrDirectory(folder);
            }


            //try to clean-up the plugin files (if anything is left)
            var files = Directory.GetFiles(Application.dataPath+"/Plugins", "PlayFabErrors.cs", SearchOption.AllDirectories);
            foreach(var file in files)
            {
                FileUtil.DeleteFileOrDirectory(file);
            }


            if(FileUtil.DeleteFileOrDirectory(PlayFabEditorDataService.envDetails.sdkPath))
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnSuccess, "PlayFab SDK Removed!");

                // HACK for 5.4, AssetDatabase.Refresh(); seems to cause the install to fail.
                if(prompt)
                {
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, "An unknown error occured and the PlayFab SDK could not be removed.");
            }
           
        }

        private static void GetLatestSdkVersion()
        {
            //TODO start back here
            DateTime threshold = PlayFabEditorDataService.editorSettings.lastSdkVersionCheck != DateTime.MinValue ? PlayFabEditorDataService.editorSettings.lastSdkVersionCheck.AddHours(1) : DateTime.MinValue;

            if(DateTime.Today > threshold)
            {
                PlayFabEditorHttp.MakeGitHubApiCall("https://api.github.com/repos/PlayFab/UnitySDK/git/refs/tags", (version) => 
                {
                    latestSdkVersion = version ?? "Unknown";
                    PlayFabEditorDataService.editorSettings.lastSdkVersionCheck = DateTime.UtcNow;
                    PlayFabEditorDataService.editorSettings.latestSdkVersion = latestSdkVersion;
                    PlayFabEditorDataService.SaveEditorSettings();
                });
            }
            else
            {
                latestSdkVersion = PlayFabEditorDataService.editorSettings.latestSdkVersion;
            }
        }






    }
}