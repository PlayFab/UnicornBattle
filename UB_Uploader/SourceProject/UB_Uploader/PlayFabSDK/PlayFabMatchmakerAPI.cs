using Newtonsoft.Json;
using PlayFab.Internal;
using PlayFab.MatchmakerModels;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PlayFab
{

    /// <summary>
    /// Enables the use of an external match-making service in conjunction with PlayFab hosted Game Server instances
    /// </summary>
    public class PlayFabMatchmakerAPI
    {
        /// <summary>
        /// Validates a user with the PlayFab service
        /// </summary>
        public static async Task<PlayFabResult<AuthUserResponse>> AuthUserAsync(AuthUserRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Matchmaker/AuthUser", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<AuthUserResponse> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<AuthUserResponse>>(new JsonTextReader(new StringReader(resultRawJson)));

            AuthUserResponse result = resultData.data;

            return new PlayFabResult<AuthUserResponse> { Result = result };
        }

        /// <summary>
        /// Informs the PlayFab game server hosting service that the indicated user has joined the Game Server Instance specified
        /// </summary>
        public static async Task<PlayFabResult<PlayerJoinedResponse>> PlayerJoinedAsync(PlayerJoinedRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Matchmaker/PlayerJoined", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<PlayerJoinedResponse> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<PlayerJoinedResponse>>(new JsonTextReader(new StringReader(resultRawJson)));

            PlayerJoinedResponse result = resultData.data;

            return new PlayFabResult<PlayerJoinedResponse> { Result = result };
        }

        /// <summary>
        /// Informs the PlayFab game server hosting service that the indicated user has left the Game Server Instance specified
        /// </summary>
        public static async Task<PlayFabResult<PlayerLeftResponse>> PlayerLeftAsync(PlayerLeftRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Matchmaker/PlayerLeft", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<PlayerLeftResponse> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<PlayerLeftResponse>>(new JsonTextReader(new StringReader(resultRawJson)));

            PlayerLeftResponse result = resultData.data;

            return new PlayFabResult<PlayerLeftResponse> { Result = result };
        }

        /// <summary>
        /// Instructs the PlayFab game server hosting service to instantiate a new Game Server Instance
        /// </summary>
        public static async Task<PlayFabResult<StartGameResponse>> StartGameAsync(StartGameRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Matchmaker/StartGame", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<StartGameResponse> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<StartGameResponse>>(new JsonTextReader(new StringReader(resultRawJson)));

            StartGameResponse result = resultData.data;

            return new PlayFabResult<StartGameResponse> { Result = result };
        }

        /// <summary>
        /// Retrieves the relevant details for a specified user, which the external match-making service can then use to compute effective matches
        /// </summary>
        public static async Task<PlayFabResult<UserInfoResponse>> UserInfoAsync(UserInfoRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Matchmaker/UserInfo", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UserInfoResponse> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<UserInfoResponse>>(new JsonTextReader(new StringReader(resultRawJson)));

            UserInfoResponse result = resultData.data;

            return new PlayFabResult<UserInfoResponse> { Result = result };
        }

    }
}
