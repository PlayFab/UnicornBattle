using UnityEngine;
using System;
using System.Collections;
using UnityEditor;
using PlayFab.Editor.EditorModels;

using System.Collections.Generic;
using System.Linq;

namespace PlayFab.Editor
{
    public class PlayFabEditorApi
    {
#region FROM EDITOR API SETS ----------------------------------------------------------------------------------------------------------------------------------------

        public static void RegisterAccouint(RegisterAccountRequest request, Action<RegisterAccountResult> resultCallback,
            Action<EditorModels.PlayFabError> errorCallback)
        {
            PlayFabEditorHttp.MakeApiCall("/DeveloperTools/User/RegisterAccount", PlayFabEditorHelper.DEV_API_ENDPOINT, request, resultCallback, errorCallback);            
        }

        public static void Login(LoginRequest request, Action<LoginResult> resultCallback,
            Action<EditorModels.PlayFabError> errorCallback)
        {
            PlayFabEditorHttp.MakeApiCall("/DeveloperTools/User/Login", PlayFabEditorHelper.DEV_API_ENDPOINT, request, resultCallback, errorCallback);
        }
       
        public static void Logout(LogoutRequest request, Action<LogoutResult> resultCallback,
            Action<EditorModels.PlayFabError> errorCallback)
        {
            PlayFabEditorHttp.MakeApiCall("/DeveloperTools/User/Logout", PlayFabEditorHelper.DEV_API_ENDPOINT, request, resultCallback, errorCallback);
        }

        public static void GetStudios(GetStudiosRequest request, Action<GetStudiosResult> resultCallback,
            Action<EditorModels.PlayFabError> errorCallback)
        {
            var token = PlayFabEditorDataService.accountDetails.devToken;
            request.DeveloperClientToken = token;
            PlayFabEditorHttp.MakeApiCall("/DeveloperTools/User/GetStudios", PlayFabEditorHelper.DEV_API_ENDPOINT, request, resultCallback, errorCallback);
        }

        public static void CreateTitle(CreateTitleRequest request, Action<RegisterAccountResult> resultCallback,
            Action<EditorModels.PlayFabError> errorCallback)
        {
            var token = PlayFabEditorDataService.accountDetails.devToken;
            request.DeveloperClientToken = token;
            PlayFabEditorHttp.MakeApiCall("/DeveloperTools/User/CreateTitle", PlayFabEditorHelper.DEV_API_ENDPOINT, request, resultCallback, errorCallback);
        }

#endregion




#region FROM ADMIN / SERVER API SETS ----------------------------------------------------------------------------------------------------------------------------------------
        public static void GetTitleData( Action<GetTitleDataResult> resultCb, Action<EditorModels.PlayFabError> errorCallback)
        {
            var titleId = PlayFabEditorDataService.envDetails.selectedTitleId;
            var apiEndpoint = String.Format("https://{0}{1}", titleId, PlayFabEditorHelper.TITLE_ENDPOINT);
            PlayFabEditorHttp.MakeApiCall<GetTitleDataRequest, GetTitleDataResult>("/Server/GetTitleData", apiEndpoint, new GetTitleDataRequest(), resultCb, errorCallback);
        }

        public static void SetTitleData(Dictionary<string, string> keys, Action<SetTitleDataResult> resultCb, Action<EditorModels.PlayFabError> errorCallback)
        {
            foreach(var pair in keys)
            {
                var req = new SetTitleDataRequest() { Key = pair.Key, Value = pair.Value };

                var titleId = PlayFabEditorDataService.envDetails.selectedTitleId;
                var apiEndpoint = String.Format("https://{0}{1}", titleId, PlayFabEditorHelper.TITLE_ENDPOINT);
                PlayFabEditorHttp.MakeApiCall<SetTitleDataRequest, SetTitleDataResult>("/Admin/SetTitleData", apiEndpoint, req, resultCb, errorCallback);
            }
        }
#endregion
    }
}