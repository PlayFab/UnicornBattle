using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using PlayFab.Editor.EditorModels;

namespace PlayFab.Editor
{
    [InitializeOnLoad]
    public class PlayFabEditorDataService : UnityEditor.Editor {
        public static PlayFab_DeveloperAccountDetails accountDetails;
        public static PlayFab_DeveloperEnvironmentDetails envDetails;
        public static PlayFab_EditorSettings editorSettings;

        public static bool isNewlyInstalled { get {

            if(EditorPrefs.HasKey(keyPrefix + PlayFabEditorHelper.EDPREF_INSTALLED))
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

        }}

        public static string keyPrefix { get { 
       
            string dataPath = Application.dataPath;
            int lastIndex = dataPath.LastIndexOf('/');
            int secondToLastIndex = dataPath.LastIndexOf('/', lastIndex-1);

            return dataPath.Substring(secondToLastIndex, (lastIndex-secondToLastIndex));

            }}

        public static bool isDataLoaded = false;

        public static EditorModels.Title activeTitle 
       { 
            get {
                if(accountDetails != null && accountDetails.studios.Count > 0 && envDetails != null)
                {
                    if(string.IsNullOrEmpty(envDetails.selectedStudio) || envDetails.selectedStudio == PlayFabEditorHelper.STUDIO_OVERRIDE)
                    {
                        return new Title() { Id = envDetails.selectedTitleId, SecretKey = envDetails.developerSecretKey, GameManagerUrl = PlayFabEditorHelper.GAMEMANAGER_URL };
                    }
                    else if(!string.IsNullOrEmpty(envDetails.selectedStudio) && !string.IsNullOrEmpty(envDetails.selectedTitleId))
                    {
                        int studioIndex = 1; // 0 is used for override
                        int titleIndex = 0;

                        try
                        {
                            if(DoesTitleExistInStudios(envDetails.selectedTitleId, out studioIndex, out titleIndex))
                            {
                                return accountDetails.studios[studioIndex].Titles[titleIndex];
                            }
                        }
                        catch(Exception ex)
                        {
                            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
                        }
                    }

                }
                return null;
            }
       }

        public static void SaveAccountDetails()
        {
            try
            {
                var serialized = Json.JsonWrapper.SerializeObject(accountDetails);
                EditorPrefs.SetString(keyPrefix+"PlayFab_DeveloperAccountDetails", serialized);
            }
            catch(Exception ex)
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
            }
        }

        public static void SaveEnvDetails()
        {
            try
            {
                var serialized = Json.JsonWrapper.SerializeObject(envDetails);
                EditorPrefs.SetString(keyPrefix+"PlayFab_DeveloperEnvironmentDetails", serialized);

                //update scriptable object
                UpdateScriptableObject();
            }
            catch(Exception ex)
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
            }
        }

        public static void SaveEditorSettings()
        {
            try
            {
                var serialized = Json.JsonWrapper.SerializeObject(editorSettings);
                EditorPrefs.SetString(keyPrefix+"PlayFab_EditorSettings", serialized);
            }
            catch(Exception ex)
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
            }
        }

        public static void LoadAccountDetails()
        {
            if(EditorPrefs.HasKey(keyPrefix+"PlayFab_DeveloperAccountDetails"))
            {
                var serialized = EditorPrefs.GetString(keyPrefix+"PlayFab_DeveloperAccountDetails");
                try
                {
                    accountDetails = Json.JsonWrapper.DeserializeObject<PlayFab_DeveloperAccountDetails>(serialized);
                    return;

                }
                catch(Exception ex)
                {
                    PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
                }
            }
            accountDetails = new PlayFab_DeveloperAccountDetails();
        }

        public static void LoadEnvDetails()
        {
            if(EditorPrefs.HasKey(keyPrefix+"PlayFab_DeveloperEnvironmentDetails"))
            {
                var serialized = EditorPrefs.GetString(keyPrefix+"PlayFab_DeveloperEnvironmentDetails");
                try
                {
                    envDetails = Json.JsonWrapper.DeserializeObject<PlayFab_DeveloperEnvironmentDetails>(serialized);

                    return;

                }
                catch(Exception ex)
                {
                    PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
                }
            }
            envDetails = new PlayFab_DeveloperEnvironmentDetails();


        }


        public static void LoadEditorSettings()
        {
            if(EditorPrefs.HasKey(keyPrefix+"PlayFab_EditorSettings"))
            {
                var serialized = EditorPrefs.GetString(keyPrefix+"PlayFab_EditorSettings");
                try
                {
                    editorSettings = Json.JsonWrapper.DeserializeObject<PlayFab_EditorSettings>(serialized);
                    LoadFromScriptableObject();
                    return;
                }
                catch(Exception ex)
                {
                    PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
                }
            }
            editorSettings = new PlayFab_EditorSettings();
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
            if(envDetails == null)
                return;
               
                var playfabSettingsType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where type.Name == "PlayFabSettings"
                    select type);

                if (playfabSettingsType.ToList().Count > 0)
                {
                    var type = playfabSettingsType.ToList().FirstOrDefault();
                    var props = type.GetProperties();

                    envDetails.selectedTitleId = string.IsNullOrEmpty((string) props.ToList().Find(p => p.Name == "TitleId").GetValue(null, null)) ? envDetails.selectedTitleId : (string) props.ToList().Find(p => p.Name == "TitleId").GetValue(null, null);
                    envDetails.webRequestType = (PlayFabEditorSettings.WebRequestType)props.ToList().Find(p => p.Name == "RequestType").GetValue(null, null);
                    envDetails.timeOut = (int) props.ToList().Find(p => p.Name == "RequestTimeout").GetValue(null, null);
                    envDetails.keepAlive = (bool) props.ToList().Find(p => p.Name == "RequestKeepAlive").GetValue(null, null);
                    envDetails.compressApiData = (bool)props.ToList().Find(p => p.Name == "CompressApiData").GetValue(null, null);



#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                    envDetails.developerSecretKey = (string) props.ToList().Find(p => p.Name == "DeveloperSecretKey").GetValue(null, null) ?? envDetails.developerSecretKey;
#else
                    envDetails.developerSecretKey = string.Empty;
#endif
                }
        }
      
        public static void UpdateScriptableObject()
        {
                //TODO move this logic to the data service
                var playfabSettingsType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where type.Name == "PlayFabSettings"
                    select type);
                if (playfabSettingsType.ToList().Count > 0)
                {
                    var type = playfabSettingsType.ToList().FirstOrDefault();
                   // var fields = type.GetFields();
                    var props = type.GetProperties();

                    foreach (var property in props)
                    {
                        //Debug.Log(property.Name);
                        if (property.Name.ToLower() == "titleid")
                        {
                            property.SetValue(null, envDetails.selectedTitleId, null);
                        }
                        if (property.Name.ToLower() == "requesttype")
                        {
                            property.SetValue(null, (int)envDetails.webRequestType, null);
                        }
                        if (property.Name.ToLower() == "timeout")
                        {
                            property.SetValue(null, envDetails.timeOut, null);
                        }
                        if (property.Name.ToLower() == "requestkeepalive")
                        {
                            property.SetValue(null, envDetails.keepAlive, null);
                        }
                        if (property.Name.ToLower() == "compressapidata")
                        {
                            property.SetValue(null, envDetails.compressApiData, null);
                        }

                        //this is a fix for the S.O. getting blanked repeatedly.
                        if (property.Name.ToLower() == "productionenvironmenturl")
                        {
                            property.SetValue(null, PlayFabEditorHelper.TITLE_ENDPOINT, null);
                        }


#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
                        if (property.Name.ToLower() == "developersecretkey")
                        {
                            property.SetValue(null, envDetails.developerSecretKey, null);
                        }
#endif
                    }

                    AssetDatabase.SaveAssets();
                 }
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
            for(int z = 0; z < PlayFabEditorDataService.accountDetails.studios.Count; z++)
            {
                for(int y = 0; y < PlayFabEditorDataService.accountDetails.studios[z].Titles.Length; y++)
                {
                    if( PlayFabEditorDataService.accountDetails.studios[z].Titles[y].Id.ToLower() == searchFor.ToLower())
                        return true;     
                }
            }

            return false;
        }


        public static bool DoesTitleExistInStudios(string searchFor, out int studioIndex, out int titleIndex) //out Studio studio
        {
            for(int z = 0; z < PlayFabEditorDataService.accountDetails.studios.Count; z++)
            {
                for(int y = 0; y < PlayFabEditorDataService.accountDetails.studios[z].Titles.Length; y++)
                {
                    if( PlayFabEditorDataService.accountDetails.studios[z].Titles[y].Id.ToLower() == searchFor.ToLower())
                    {
                        studioIndex = z;
                        titleIndex = y;
                        return true;     
                    }
                }
            }

            studioIndex = 0; // corresponds to our _OVERRIDE_
            titleIndex = -1;
            return false;

        }



        //CTOR
        static PlayFabEditorDataService()
        {
            LoadAllData();
        }

    }
}