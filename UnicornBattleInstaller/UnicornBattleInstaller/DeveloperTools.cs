using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayFab.Internal;
using PlayFab;

namespace UnicornBattleInstaller.DeveloperToolsUtility
{
    public class DeveloperTools
    {
        private const string _editorUrl = "editor.playfabapi.com";
        private string _developerClientToken;

        ITransportPlugin transport = PluginManager.GetPlugin<ITransportPlugin>(PluginContract.PlayFab_Transport);
        ISerializerPlugin json = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);

        public async void Login(string email, string password, Action successCallback, Action<PlayFabError> errorCallback)
        {
            var headers = new Dictionary<string, string>()
            {
                //{"Content-Type", "application/json" },
                {"X-ReportErrorAsSuccess","true" },
                {"X-PlayFabSDK","Unicorn Battle Installer"}
            };
            var path = string.Format($"https://{_editorUrl}/DeveloperTools/User/Login");
            var httpResult = await transport.DoPost(path, new DeveloperLoginRequest() {
                Email = email,
                Password = password
            }, headers);

            var errorResult = json.DeserializeObject<DeveloperToolsError>((string)httpResult);
            if (errorResult != null && errorResult.code != 200)
            {
                var error = new PlayFabError()
                {
                    Error = (PlayFabErrorCode)errorResult.errorCode,
                    ErrorMessage = errorResult.errorMessage
                };
                errorCallback?.Invoke(error);
                return;
            }

            var resultRawJson = (string)httpResult;
            var resultData = json.DeserializeObject<PlayFabJsonSuccess<DeveloperLoginResult>>(resultRawJson);
            var result = resultData.data;
            _developerClientToken = result.DeveloperClientToken;
            successCallback?.Invoke();
        }

        public async void GetStudios(Action<List<Studio>> successCallback, Action<PlayFabError> errorCallback)
        {
            var headers = new Dictionary<string, string>()
            {
                //{"Content-Type", "application/json" },
                {"X-ReportErrorAsSuccess","true" },
                {"X-PlayFabSDK","Unicorn Battle Installer"}
            };
            var path = string.Format($"https://{_editorUrl}/DeveloperTools/User/GetStudios");
            var httpResult = await transport.DoPost(path, new GetStudiosRequest()
            {
                DeveloperClientToken = _developerClientToken
            }, headers);

            var errorResult = json.DeserializeObject<DeveloperToolsError>((string)httpResult);
            if (errorResult != null && errorResult.code != 200)
            {
                var error = new PlayFabError()
                {
                    Error = (PlayFabErrorCode)errorResult.errorCode,
                    ErrorMessage = errorResult.errorMessage
                };
                errorCallback?.Invoke(error);
                return;
            }

            var resultRawJson = (string)httpResult;
            var resultData = json.DeserializeObject<PlayFabJsonSuccess<GetStudiosResult>>(resultRawJson);
            var result = resultData.data;
            successCallback?.Invoke(result.Studios.ToList());
        }



    }

    public class DeveloperLoginRequest : PlayFabRequestCommon
    {
        public string Email;
        public string Password;
        public string DeveloperToolProductName = "Unicorn Battle Installer";
        public string DeveloperToolProductVersion = "1.01";
    }

    public class DeveloperLoginResult : PlayFabLoginResultCommon
    {
        public string DeveloperClientToken;
    }

    public class DeveloperToolsError
    {
        public int code { get; set; }
        public string status { get; set; }
        public string error { get; set; }
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
    }

    [Serializable]
    public class GetStudiosRequest 
    {
        public string DeveloperClientToken;
    }

    public class GetStudiosResult : PlayFabResultCommon
    {
        public Studio[] Studios;
    }

    public class Title
    {
        public string Id;
        public string Name;
        public string SecretKey;
        public string GameManagerUrl;
    }

    public class Studio
    {
        public string Id;
        public string Name;

        public Title[] Titles;

        public Title GetTitle(string titleId)
        {
            if (Titles == null)
                return null;
            for (var i = 0; i < Titles.Length; i++)
                if (Titles[i].Id == titleId)
                    return Titles[i];
            return null;
        }

        public string GetTitleSecretKey(string titleId)
        {
            var title = GetTitle(titleId);
            return title == null ? "" : title.SecretKey;
        }
    }

}
