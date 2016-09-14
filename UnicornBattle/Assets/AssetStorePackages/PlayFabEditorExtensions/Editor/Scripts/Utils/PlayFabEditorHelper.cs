using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using PlayFab.Editor.Json;
using PlayFab.Editor.EditorModels;

namespace PlayFab.Editor
{
    [InitializeOnLoad]
    public class PlayFabEditorHelper : UnityEditor.Editor 
    {
        #region EDITOR_STRINGS
        public static string EDEX_NAME = "PlayFab_EditorExtensions";
        public static string EDEX_VERSION = "0.0.99";
        public static string DEV_API_ENDPOINT = "https://editor.playfabapi.com";
        public static string TITLE_ENDPOINT = ".playfabapi.com";
        public static string GAMEMANAGER_URL = "https://developer.playfab.com";
        public static string PLAYFAB_ASSEMBLY = "PlayFabSettings";
        public static string PLAYFAB_EDEX_MAINFILE = "PlayFabEditor.cs";
        public static string SDK_DOWNLOAD_PATH = "/Resources/PlayFabUnitySdk.unitypackage";
        public static string EDEX_DOWNLOAD_PATH = "/Resources/PlayFabUnityEditorExtensions.unitypackage";
        public static string VAR_REQUEST_TIMING = "PLAYFAB_REQUEST_TIMING";

        public static string ADMIN_API = "ENABLE_PLAYFABADMIN_API";
        public static string SERVER_API = "ENABLE_PLAYFABSERVER_API";
        public static string CLIENT_API = "DISABLE_PLAYFABCLIENT_API";
        public static string DEBUG_REQUEST_TIMING = "PLAYFAB_REQUEST_TIMING";
        public static string EDITOR_ROOT =  Application.dataPath + "/PlayFabEditorExtensions/Editor";
        public static string DEFAULT_SDK_LOCATION = "Assets/PlayFabSdk";
        public static string STUDIO_OVERRIDE = "_OVERRIDE_";

        public static string MSG_SPIN_BLOCK = "{\"useSpinner\":true, \"blockUi\":true }";

        public static string EDPREF_INSTALLED = "NewlyInstalled";

        //public static string MSG_SPIN = "{\"useSpinner\":true}";
        //public static string MSG_BLOCK = "{\"blockUi\":true }";
        #endregion
            
        public static GUISkin uiStyle = GetUiStyle();



        static PlayFabEditorHelper()
        {
            // scan for changes to the editor folder / structure.
            if(uiStyle == null)
            {
                string[] rootFiles = new string[0];
                bool relocatedEdEx = false;

                try
                {
                    EDITOR_ROOT = PlayFabEditorDataService.envDetails.edexPath ?? EDITOR_ROOT;
                    rootFiles = Directory.GetDirectories(EDITOR_ROOT);

                    uiStyle = GetUiStyle();
                }
                catch
                {

                    if(rootFiles.Length == 0)
                    {
                        // this probably means the editor folder was moved.
                        //see if we can locate the moved root
                        // and reload the assets

                        var movedRootFiles = Directory.GetFiles(Application.dataPath, PLAYFAB_EDEX_MAINFILE, SearchOption.AllDirectories);
                        if(movedRootFiles.Length > 0)
                        {
                            relocatedEdEx = true;
                            EDITOR_ROOT = movedRootFiles[0].Substring(0, movedRootFiles[0].IndexOf(PLAYFAB_EDEX_MAINFILE)-1);
                            PlayFabEditorDataService.envDetails.edexPath = EDITOR_ROOT;
                            PlayFabEditorDataService.SaveEnvDetails();

                            uiStyle = GetUiStyle();
                        }

                    }
                }
                finally
                {
                    if(relocatedEdEx && rootFiles.Length == 0)
                    {
                        Debug.Log(string.Format("Found new EdEx root: {0}", EDITOR_ROOT));
                    }
                    else if(rootFiles.Length == 0)
                    {
                        Debug.Log("Could not relocate the PlayFab Editor Extension");
                        EDITOR_ROOT = string.Empty;
                    }
                }
            }
        }


        public static GUISkin GetUiStyle()
        {
            if(uiStyle == null)
            {
                var relRoot = EDITOR_ROOT.Substring(EDITOR_ROOT.IndexOf("Assets/"));
                return (GUISkin)AssetDatabase.LoadAssetAtPath(relRoot+ "/UI/PlayFabStyles.guiskin", typeof(GUISkin));
            }
            else
            {
                return uiStyle;
            }
        }


        public static void SharedErrorCallback(EditorModels.PlayFabError error)
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, error.ErrorMessage);
        }

        public static void SharedErrorCallback(string error)
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, error);
        }


        protected internal static PlayFab.Editor.EditorModels.PlayFabError GeneratePlayFabError(string json, object customData = null)
        {
            JsonObject errorDict = null;
            Dictionary<string, List<string>> errorDetails = null;
            try
            {
                //deserialize the error
                errorDict = JsonWrapper.DeserializeObject<JsonObject>(json, PlayFabEditorUtil.ApiSerializerStrategy);

                
                if (errorDict.ContainsKey("errorDetails"))
                {
                    var ed = JsonWrapper.DeserializeObject<Dictionary<string, List<string>>>( errorDict["errorDetails"].ToString());
                    errorDetails = ed;
                }
            }
            catch (Exception e)
            {
                return new PlayFab.Editor.EditorModels.PlayFabError()
                {
                    ErrorMessage = e.Message
                };
            }

            //create new error object
            return new PlayFab.Editor.EditorModels.PlayFabError
            {
                HttpCode = errorDict.ContainsKey("code") ? Convert.ToInt32(errorDict["code"]) : 400,
                HttpStatus = errorDict.ContainsKey("status")
                    ? (string)errorDict["status"]
                    : "BadRequest",
                Error = errorDict.ContainsKey("errorCode")
                    ? (PlayFab.Editor.EditorModels.PlayFabErrorCode)Convert.ToInt32(errorDict["errorCode"])
                    : PlayFab.Editor.EditorModels.PlayFabErrorCode.ServiceUnavailable,
                ErrorMessage = errorDict.ContainsKey("errorMessage")
                    ? (string)errorDict["errorMessage"]
                    : string.Empty,
                ErrorDetails = errorDetails,
                CustomData = customData ?? new object()
            };
        }

        #region unused, but could be useful

        /// <summary>
        /// Tool to create a color background texture
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="col"></param>
        /// <returns>Texture2D</returns>
        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width*height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        public static Vector3 GetColorVector(int colorValue)
        {
            return new Vector3((colorValue/255f), (colorValue/255f), (colorValue/255f));
        }
        #endregion
    }
}