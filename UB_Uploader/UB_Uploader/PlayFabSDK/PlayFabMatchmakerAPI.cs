using PlayFab.MatchmakerModels;
using PlayFab.Internal;
using PlayFab.Json;
using System;
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
        public static async Task<PlayFabResult<AuthUserResponse>> AuthUserAsync(AuthUserRequest request, object customData = null)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            var httpResult = await PlayFabHttp.DoPost("/Matchmaker/AuthUser", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                var error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<AuthUserResponse> { Error = error, CustomData = customData };
            }

            var resultRawJson = (string)httpResult;
            var resultData = JsonWrapper.DeserializeObject<PlayFabJsonSuccess<AuthUserResponse>>(resultRawJson);
            var result = resultData.data;

            return new PlayFabResult<AuthUserResponse> { Result = result, CustomData = customData };
        }

        /// <summary>
        /// Informs the PlayFab game server hosting service that the indicated user has joined the Game Server Instance specified
        /// </summary>
        public static async Task<PlayFabResult<PlayerJoinedResponse>> PlayerJoinedAsync(PlayerJoinedRequest request, object customData = null)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            var httpResult = await PlayFabHttp.DoPost("/Matchmaker/PlayerJoined", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                var error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<PlayerJoinedResponse> { Error = error, CustomData = customData };
            }

            var resultRawJson = (string)httpResult;
            var resultData = JsonWrapper.DeserializeObject<PlayFabJsonSuccess<PlayerJoinedResponse>>(resultRawJson);
            var result = resultData.data;

            return new PlayFabResult<PlayerJoinedResponse> { Result = result, CustomData = customData };
        }

        /// <summary>
        /// Informs the PlayFab game server hosting service that the indicated user has left the Game Server Instance specified
        /// </summary>
        public static async Task<PlayFabResult<PlayerLeftResponse>> PlayerLeftAsync(PlayerLeftRequest request, object customData = null)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            var httpResult = await PlayFabHttp.DoPost("/Matchmaker/PlayerLeft", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                var error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<PlayerLeftResponse> { Error = error, CustomData = customData };
            }

            var resultRawJson = (string)httpResult;
            var resultData = JsonWrapper.DeserializeObject<PlayFabJsonSuccess<PlayerLeftResponse>>(resultRawJson);
            var result = resultData.data;

            return new PlayFabResult<PlayerLeftResponse> { Result = result, CustomData = customData };
        }

        /// <summary>
        /// Instructs the PlayFab game server hosting service to instantiate a new Game Server Instance
        /// </summary>
        public static async Task<PlayFabResult<StartGameResponse>> StartGameAsync(StartGameRequest request, object customData = null)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            var httpResult = await PlayFabHttp.DoPost("/Matchmaker/StartGame", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                var error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<StartGameResponse> { Error = error, CustomData = customData };
            }

            var resultRawJson = (string)httpResult;
            var resultData = JsonWrapper.DeserializeObject<PlayFabJsonSuccess<StartGameResponse>>(resultRawJson);
            var result = resultData.data;

            return new PlayFabResult<StartGameResponse> { Result = result, CustomData = customData };
        }

        /// <summary>
        /// Retrieves the relevant details for a specified user, which the external match-making service can then use to compute effective matches
        /// </summary>
        public static async Task<PlayFabResult<UserInfoResponse>> UserInfoAsync(UserInfoRequest request, object customData = null)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            var httpResult = await PlayFabHttp.DoPost("/Matchmaker/UserInfo", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                var error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UserInfoResponse> { Error = error, CustomData = customData };
            }

            var resultRawJson = (string)httpResult;
            var resultData = JsonWrapper.DeserializeObject<PlayFabJsonSuccess<UserInfoResponse>>(resultRawJson);
            var result = resultData.data;

            return new PlayFabResult<UserInfoResponse> { Result = result, CustomData = customData };
        }

    }
}
