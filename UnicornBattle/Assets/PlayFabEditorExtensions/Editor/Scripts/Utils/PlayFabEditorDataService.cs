using PlayFab.PfEditor.EditorModels;
using PlayFab.PfEditor.Json;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PlayFab.PfEditor
{
    [InitializeOnLoad]
    public class PlayFabEditorDataService : UnityEditor.Editor
    {
        public static PlayFab_DeveloperAccountDetails accountDetails;
        public static PlayFab_DeveloperEnvironmentDetails envDetails;
        public static PlayFab_EditorSettings editorSettings;

        public static bool isNewlyInstalled
        {
            get
            {

                if (EditorPrefs.HasKey(keyPrefix + PlayFabEditorHelper.EDPREF_INSTALLED))
                {
                    return false;
                }
                else
                {
                    //TODO find a way to unset this after uninstall.
                    EditorPrefs.SetBool(keyPrefix + PlayFabEditorHelper.EDPREF_INSTALLED, false);

                    envDetails.isClientApiEnabled = true;
                    envDetails.isAdminApiEnabled = false;
                    envDetails.isServerApiEnabled = false;
                    return true;
                }

            }
        }

        public static string keyPrefix
        {
            get
            {
                string dataPath = Application.dataPath;
                int lastIndex = dataPath.LastIndexOf('/');
                int secondToLastIndex = dataPath.LastIndexOf('/', lastIndex - 1);

                return dataPath.Substring(secondToLastIndex, (lastIndex - secondToLastIndex));
            }
        }

        public static bool isDataLoaded = false;

        public static Title activeTitle
        {
            get
            {
                if (accountDetails != null && accountDetails.studios != null && accountDetails.studios.Length > 0 && envDetails != null)
                {
                    if (string.IsNullOrEmpty(envDetails.selectedStudio) || envDetails.selectedStudio == PlayFabEditorHelper.STUDIO_OVERRIDE)
                        return new Title { Id = envDetails.selectedTitleId, SecretKey = envDetails.developerSecretKey, GameManagerUrl = PlayFabEditorHelper.GAMEMANAGER_URL };

                    if (string.IsNullOrEmpty(envDetails.selectedStudio) || string.IsNullOrEmpty(envDetails.selectedTitleId))
                        return null;

                    try
                    {
                        int studioIndex; int titleIndex;
                        if (DoesTitleExistInStudios(envDetails.selectedTitleId, out studioIndex, out titleIndex))
                            return accountDetails.studios[studioIndex].Titles[titleIndex];
                    }
                    catch (Exception ex)
                    {
                        PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
                    }
                }
                return null;
            }
        }

        public static void SaveAccountDetails()
        {
            try
            {
                var serialized = JsonWrapper.SerializeObject(accountDetails);
                EditorPrefs.SetString(keyPrefix + "PlayFab_DeveloperAccountDetails", serialized);
            }
            catch (Exception ex)
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
            }
        }

        public static void SaveEnvDetails()
        {
            try
            {
                var serialized = JsonWrapper.SerializeObject(envDetails);
                EditorPrefs.SetString(keyPrefix + "PlayFab_DeveloperEnvironmentDetails", serialized);

                //update scriptable object
                UpdateScriptableObject();
            }
            catch (Exception ex)
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
            }
        }

        public static void SaveEditorSettings()
        {
            try
            {
                var serialized = JsonWrapper.SerializeObject(editorSettings);
                EditorPrefs.SetString(keyPrefix + "PlayFab_EditorSettings", serialized);
            }
            catch (Exception ex)
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
            }
        }

        public static void LoadAccountDetails()
        {
            if (EditorPrefs.HasKey(keyPrefix + "PlayFab_DeveloperAccountDetails"))
            {
                var serialized = EditorPrefs.GetString(keyPrefix + "PlayFab_DeveloperAccountDetails");
                try
                {
                    accountDetails = JsonWrapper.DeserializeObject<PlayFab_DeveloperAccountDetails>(serialized);
                    return;

                }
                catch (Exception ex)
                {
                    PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
                }
            }
            accountDetails = new PlayFab_DeveloperAccountDetails();
        }

        public static void LoadEnvDetails()
        {
            if (EditorPrefs.HasKey(keyPrefix + "PlayFab_DeveloperEnvironmentDetails"))
            {
                var serialized = EditorPrefs.GetString(keyPrefix + "PlayFab_DeveloperEnvironmentDetails");
                try
                {
                    envDetails = JsonWrapper.DeserializeObject<PlayFab_DeveloperEnvironmentDetails>(serialized);

                    return;

                }
                catch (Exception ex)
                {
                    PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
                }
            }
            envDetails = new PlayFab_DeveloperEnvironmentDetails();


        }


        public static void LoadEditorSettings()
        {
            if (EditorPrefs.HasKey(keyPrefix + "PlayFab_EditorSettings"))
            {
                var serialized = EditorPrefs.GetString(keyPrefix + "PlayFab_EditorSettings");
                try
                {
                    editorSettings = JsonWrapper.DeserializeObject<PlayFab_EditorSettings>(serialized);
                    LoadFromScriptableObject();
                    return;
                }
                catch (Exception ex)
                {
                    PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
                }
            }
            else
            {
                editorSettings = new PlayFab_EditorSettings();
            }
        }

        public static void SaveAllData()
        {
            SaveAccountDetails();
            SaveEnvDetails();
            SaveEditorSettings();
        }

        public static void LoadAllData()
        {
            LoadAccountDetails();
            LoadEnvDetails();
            LoadEditorSettings();

            LoadFromScriptableObject();

            isDataLoaded = true;
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnDataLoaded, "Complete");
        }

        public static void LoadFromScriptableObject()
        {
            if (envDetails == null)
                return;

            Type playfabSettingsType = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.GetTypes())
                    if (type.Name == "PlayFabSettings")
                        playfabSettingsType = type;

            if (playfabSettingsType == null || !PlayFabEditorSDKTools.IsInstalled || !PlayFabEditorSDKTools.isSdkSupported)
                return;

            var props = playfabSettingsType.GetProperties();
            try
            {
                foreach (var prop in props)
                {
                    switch (prop.Name)
                    {
                        case "TitleId":
                            var propValue = (string)prop.GetValue(null, null);
                            envDetails.selectedTitleId = string.IsNullOrEmpty(propValue) ? envDetails.selectedTitleId : propValue;
                            break;
                        case "RequestType":
                            envDetails.webRequestType = (PlayFabEditorSettings.WebRequestType)prop.GetValue(null, null);
                            break;
                        case "RequestTimeout":
                            envDetails.timeOut = (int)prop.GetValue(null, null);
                            break;
                        case "RequestKeepAlive":
                            envDetails.keepAlive = (bool)prop.GetValue(null, null);
                            break;
                        case "CompressApiData":
                            envDetails.compressApiData = (bool)prop.GetValue(null, null);
                            break;
                        case "DeveloperSecretKey":
                            envDetails.developerSecretKey = string.Empty;
#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                            envDetails.developerSecretKey = (string)prop.GetValue(null, null) ?? envDetails.developerSecretKey;
#endif
                            break;
                    }
                }
            }
            catch
            {
                // do nothing, this cathes issues in really old sdks; clearly there is something wrong here.
                PlayFabEditorSDKTools.isSdkSupported = false;
            }
        }

        public static void UpdateScriptableObject()
        {
            //TODO move this logic to the data service
            Type playfabSettingsType = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.GetTypes())
                    if (type.Name == "PlayFabSettings")
                        playfabSettingsType = type;

            if (playfabSettingsType == null || !PlayFabEditorSDKTools.IsInstalled || !PlayFabEditorSDKTools.isSdkSupported)
                return;

            var props = playfabSettingsType.GetProperties();
            foreach (var property in props)
            {
                switch (property.Name.ToLower())
                {
                    case "titleid":
                        property.SetValue(null, envDetails.selectedTitleId, null); break;
                    case "requesttype":
                        property.SetValue(null, (int)envDetails.webRequestType, null); break;
                    case "timeout":
                        property.SetValue(null, envDetails.timeOut, null); break;
                    case "requestkeepalive":
                        property.SetValue(null, envDetails.keepAlive, null); break;
                    case "compressapidata":
                        property.SetValue(null, envDetails.compressApiData, null); break;
                    case "productionenvironmenturl":
                        property.SetValue(null, PlayFabEditorHelper.TITLE_ENDPOINT, null); break;
#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                    case "developersecretkey":
                        property.SetValue(null, envDetails.developerSecretKey, null); break;
#endif
                }
            }

            var getSoMethod = playfabSettingsType.GetMethod("GetSharedSettingsObjectPrivate", BindingFlags.NonPublic | BindingFlags.Static);
            if (getSoMethod != null)
            {
                var so = getSoMethod.Invoke(null, new object[0]) as ScriptableObject;
                if (so != null)
                    EditorUtility.SetDirty(so);
            }
            AssetDatabase.SaveAssets();
        }

        public static void RemoveEditorPrefs()
        {
            EditorPrefs.DeleteKey(keyPrefix + PlayFabEditorHelper.EDPREF_INSTALLED);
            EditorPrefs.DeleteKey(keyPrefix + "PlayFab_EditorSettings");
            EditorPrefs.DeleteKey(keyPrefix + "PlayFab_DeveloperEnvironmentDetails");
            EditorPrefs.DeleteKey(keyPrefix + "PlayFab_DeveloperAccountDetails");
        }

        public static bool DoesTitleExistInStudios(string searchFor) //out Studio studio
        {
            if (accountDetails.studios == null)
                return false;

            searchFor = searchFor.ToLower();
            foreach (var studio in accountDetails.studios)
                foreach (var title in studio.Titles)
                    if (title.Id.ToLower() == searchFor)
                        return true;
            return false;
        }

        public static bool DoesTitleExistInStudios(string searchFor, out int studioIndex, out int titleIndex) //out Studio studio
        {
            studioIndex = 0; // corresponds to our _OVERRIDE_
            titleIndex = -1;

            if (accountDetails.studios == null)
                return false;

            for (var studioIdx = 0; studioIdx < accountDetails.studios.Length; studioIdx++)
            {
                for (var titleIdx = 0; titleIdx < accountDetails.studios[studioIdx].Titles.Length; titleIdx++)
                {
                    if (accountDetails.studios[studioIdx].Titles[titleIdx].Id.ToLower() == searchFor.ToLower())
                    {
                        studioIndex = studioIdx;
                        titleIndex = titleIdx;
                        return true;
                    }
                }
            }

            return false;
        }

        public static void GetStudios()
        {
            PlayFabEditorApi.GetStudios(new GetStudiosRequest(), (getStudioResult) =>
            {
                accountDetails.studios = getStudioResult.Studios;
                SaveAccountDetails();
            }, PlayFabEditorHelper.SharedErrorCallback);
        }

        //CTOR
        static PlayFabEditorDataService()
        {
            LoadAllData();
        }
    }
}
