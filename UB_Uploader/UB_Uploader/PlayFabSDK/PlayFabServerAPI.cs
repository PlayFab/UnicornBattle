using Newtonsoft.Json;
using PlayFab.Internal;
using PlayFab.ServerModels;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PlayFab
{

    /// <summary>
    /// Provides functionality to allow external (developer-controlled) servers to interact with user inventories and data in a trusted manner, and to handle matchmaking and client connection orchestration
    /// </summary>
    public class PlayFabServerAPI
    {
        /// <summary>
        /// Validated a client's session ticket, and if successful, returns details for that user
        /// </summary>
        public static async Task<PlayFabResult<AuthenticateSessionTicketResult>> AuthenticateSessionTicketAsync(AuthenticateSessionTicketRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/AuthenticateSessionTicket", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<AuthenticateSessionTicketResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<AuthenticateSessionTicketResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            AuthenticateSessionTicketResult result = resultData.data;

            return new PlayFabResult<AuthenticateSessionTicketResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the unique PlayFab identifiers for the given set of Facebook identifiers.
        /// </summary>
        public static async Task<PlayFabResult<GetPlayFabIDsFromFacebookIDsResult>> GetPlayFabIDsFromFacebookIDsAsync(GetPlayFabIDsFromFacebookIDsRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetPlayFabIDsFromFacebookIDs", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetPlayFabIDsFromFacebookIDsResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetPlayFabIDsFromFacebookIDsResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetPlayFabIDsFromFacebookIDsResult result = resultData.data;

            return new PlayFabResult<GetPlayFabIDsFromFacebookIDsResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the unique PlayFab identifiers for the given set of Steam identifiers. The Steam identifiers  are the profile IDs for the user accounts, available as SteamId in the Steamworks Community API calls.
        /// </summary>
        public static async Task<PlayFabResult<GetPlayFabIDsFromSteamIDsResult>> GetPlayFabIDsFromSteamIDsAsync(GetPlayFabIDsFromSteamIDsRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetPlayFabIDsFromSteamIDs", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetPlayFabIDsFromSteamIDsResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetPlayFabIDsFromSteamIDsResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetPlayFabIDsFromSteamIDsResult result = resultData.data;

            return new PlayFabResult<GetPlayFabIDsFromSteamIDsResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the relevant details for a specified user
        /// </summary>
        public static async Task<PlayFabResult<GetUserAccountInfoResult>> GetUserAccountInfoAsync(GetUserAccountInfoRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetUserAccountInfo", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetUserAccountInfoResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetUserAccountInfoResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetUserAccountInfoResult result = resultData.data;

            return new PlayFabResult<GetUserAccountInfoResult> { Result = result };
        }

        /// <summary>
        /// Sends an iOS/Android Push Notification to a specific user, if that user's device has been configured for Push Notifications in PlayFab. If a user has linked both Android and iOS devices, both will be notified.
        /// </summary>
        public static async Task<PlayFabResult<SendPushNotificationResult>> SendPushNotificationAsync(SendPushNotificationRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/SendPushNotification", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<SendPushNotificationResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<SendPushNotificationResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            SendPushNotificationResult result = resultData.data;

            return new PlayFabResult<SendPushNotificationResult> { Result = result };
        }

        /// <summary>
        /// Deletes the users for the provided game. Deletes custom data, all account linkages, and statistics.
        /// </summary>
        public static async Task<PlayFabResult<DeleteUsersResult>> DeleteUsersAsync(DeleteUsersRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/DeleteUsers", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<DeleteUsersResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<DeleteUsersResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            DeleteUsersResult result = resultData.data;

            return new PlayFabResult<DeleteUsersResult> { Result = result };
        }

        /// <summary>
        /// Retrieves a list of ranked users for the given statistic, starting from the indicated point in the leaderboard
        /// </summary>
        public static async Task<PlayFabResult<GetLeaderboardResult>> GetLeaderboardAsync(GetLeaderboardRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetLeaderboard", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetLeaderboardResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetLeaderboardResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetLeaderboardResult result = resultData.data;

            return new PlayFabResult<GetLeaderboardResult> { Result = result };
        }

        /// <summary>
        /// Retrieves a list of ranked users for the given statistic, centered on the currently signed-in user
        /// </summary>
        public static async Task<PlayFabResult<GetLeaderboardAroundUserResult>> GetLeaderboardAroundUserAsync(GetLeaderboardAroundUserRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetLeaderboardAroundUser", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetLeaderboardAroundUserResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetLeaderboardAroundUserResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetLeaderboardAroundUserResult result = resultData.data;

            return new PlayFabResult<GetLeaderboardAroundUserResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the current version and values for the indicated statistics, for the local player.
        /// </summary>
        public static async Task<PlayFabResult<GetPlayerStatisticsResult>> GetPlayerStatisticsAsync(GetPlayerStatisticsRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetPlayerStatistics", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetPlayerStatisticsResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetPlayerStatisticsResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetPlayerStatisticsResult result = resultData.data;

            return new PlayFabResult<GetPlayerStatisticsResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the information on the available versions of the specified statistic.
        /// </summary>
        public static async Task<PlayFabResult<GetPlayerStatisticVersionsResult>> GetPlayerStatisticVersionsAsync(GetPlayerStatisticVersionsRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetPlayerStatisticVersions", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetPlayerStatisticVersionsResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetPlayerStatisticVersionsResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetPlayerStatisticVersionsResult result = resultData.data;

            return new PlayFabResult<GetPlayerStatisticVersionsResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the title-specific custom data for the user which is readable and writable by the client
        /// </summary>
        public static async Task<PlayFabResult<GetUserDataResult>> GetUserDataAsync(GetUserDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetUserData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetUserDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetUserDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetUserDataResult result = resultData.data;

            return new PlayFabResult<GetUserDataResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the title-specific custom data for the user which cannot be accessed by the client
        /// </summary>
        public static async Task<PlayFabResult<GetUserDataResult>> GetUserInternalDataAsync(GetUserDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetUserInternalData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetUserDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetUserDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetUserDataResult result = resultData.data;

            return new PlayFabResult<GetUserDataResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the publisher-specific custom data for the user which is readable and writable by the client
        /// </summary>
        public static async Task<PlayFabResult<GetUserDataResult>> GetUserPublisherDataAsync(GetUserDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetUserPublisherData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetUserDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetUserDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetUserDataResult result = resultData.data;

            return new PlayFabResult<GetUserDataResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the publisher-specific custom data for the user which cannot be accessed by the client
        /// </summary>
        public static async Task<PlayFabResult<GetUserDataResult>> GetUserPublisherInternalDataAsync(GetUserDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetUserPublisherInternalData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetUserDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetUserDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetUserDataResult result = resultData.data;

            return new PlayFabResult<GetUserDataResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the publisher-specific custom data for the user which can only be read by the client
        /// </summary>
        public static async Task<PlayFabResult<GetUserDataResult>> GetUserPublisherReadOnlyDataAsync(GetUserDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetUserPublisherReadOnlyData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetUserDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetUserDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetUserDataResult result = resultData.data;

            return new PlayFabResult<GetUserDataResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the title-specific custom data for the user which can only be read by the client
        /// </summary>
        public static async Task<PlayFabResult<GetUserDataResult>> GetUserReadOnlyDataAsync(GetUserDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetUserReadOnlyData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetUserDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetUserDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetUserDataResult result = resultData.data;

            return new PlayFabResult<GetUserDataResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the details of all title-specific statistics for the user
        /// </summary>
        public static async Task<PlayFabResult<GetUserStatisticsResult>> GetUserStatisticsAsync(GetUserStatisticsRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetUserStatistics", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetUserStatisticsResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetUserStatisticsResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetUserStatisticsResult result = resultData.data;

            return new PlayFabResult<GetUserStatisticsResult> { Result = result };
        }

        /// <summary>
        /// Updates the values of the specified title-specific statistics for the user
        /// </summary>
        public static async Task<PlayFabResult<UpdatePlayerStatisticsResult>> UpdatePlayerStatisticsAsync(UpdatePlayerStatisticsRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/UpdatePlayerStatistics", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UpdatePlayerStatisticsResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<UpdatePlayerStatisticsResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            UpdatePlayerStatisticsResult result = resultData.data;

            return new PlayFabResult<UpdatePlayerStatisticsResult> { Result = result };
        }

        /// <summary>
        /// Updates the title-specific custom data for the user which is readable and writable by the client
        /// </summary>
        public static async Task<PlayFabResult<UpdateUserDataResult>> UpdateUserDataAsync(UpdateUserDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/UpdateUserData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UpdateUserDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<UpdateUserDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            UpdateUserDataResult result = resultData.data;

            return new PlayFabResult<UpdateUserDataResult> { Result = result };
        }

        /// <summary>
        /// Updates the title-specific custom data for the user which cannot be accessed by the client
        /// </summary>
        public static async Task<PlayFabResult<UpdateUserDataResult>> UpdateUserInternalDataAsync(UpdateUserInternalDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/UpdateUserInternalData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UpdateUserDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<UpdateUserDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            UpdateUserDataResult result = resultData.data;

            return new PlayFabResult<UpdateUserDataResult> { Result = result };
        }

        /// <summary>
        /// Updates the publisher-specific custom data for the user which is readable and writable by the client
        /// </summary>
        public static async Task<PlayFabResult<UpdateUserDataResult>> UpdateUserPublisherDataAsync(UpdateUserDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/UpdateUserPublisherData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UpdateUserDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<UpdateUserDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            UpdateUserDataResult result = resultData.data;

            return new PlayFabResult<UpdateUserDataResult> { Result = result };
        }

        /// <summary>
        /// Updates the publisher-specific custom data for the user which cannot be accessed by the client
        /// </summary>
        public static async Task<PlayFabResult<UpdateUserDataResult>> UpdateUserPublisherInternalDataAsync(UpdateUserInternalDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/UpdateUserPublisherInternalData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UpdateUserDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<UpdateUserDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            UpdateUserDataResult result = resultData.data;

            return new PlayFabResult<UpdateUserDataResult> { Result = result };
        }

        /// <summary>
        /// Updates the publisher-specific custom data for the user which can only be read by the client
        /// </summary>
        public static async Task<PlayFabResult<UpdateUserDataResult>> UpdateUserPublisherReadOnlyDataAsync(UpdateUserDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/UpdateUserPublisherReadOnlyData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UpdateUserDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<UpdateUserDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            UpdateUserDataResult result = resultData.data;

            return new PlayFabResult<UpdateUserDataResult> { Result = result };
        }

        /// <summary>
        /// Updates the title-specific custom data for the user which can only be read by the client
        /// </summary>
        public static async Task<PlayFabResult<UpdateUserDataResult>> UpdateUserReadOnlyDataAsync(UpdateUserDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/UpdateUserReadOnlyData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UpdateUserDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<UpdateUserDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            UpdateUserDataResult result = resultData.data;

            return new PlayFabResult<UpdateUserDataResult> { Result = result };
        }

        /// <summary>
        /// Updates the values of the specified title-specific statistics for the user. By default, clients are not permitted to update statistics. Developers may override this setting in the Game Manager > Settings > API Features.
        /// </summary>
        public static async Task<PlayFabResult<UpdateUserStatisticsResult>> UpdateUserStatisticsAsync(UpdateUserStatisticsRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/UpdateUserStatistics", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UpdateUserStatisticsResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<UpdateUserStatisticsResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            UpdateUserStatisticsResult result = resultData.data;

            return new PlayFabResult<UpdateUserStatisticsResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the specified version of the title's catalog of virtual goods, including all defined properties
        /// </summary>
        public static async Task<PlayFabResult<GetCatalogItemsResult>> GetCatalogItemsAsync(GetCatalogItemsRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetCatalogItems", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetCatalogItemsResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetCatalogItemsResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetCatalogItemsResult result = resultData.data;

            return new PlayFabResult<GetCatalogItemsResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the key-value store of custom publisher settings
        /// </summary>
        public static async Task<PlayFabResult<GetPublisherDataResult>> GetPublisherDataAsync(GetPublisherDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetPublisherData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetPublisherDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetPublisherDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetPublisherDataResult result = resultData.data;

            return new PlayFabResult<GetPublisherDataResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the key-value store of custom title settings
        /// </summary>
        public static async Task<PlayFabResult<GetTitleDataResult>> GetTitleDataAsync(GetTitleDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetTitleData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetTitleDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetTitleDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetTitleDataResult result = resultData.data;

            return new PlayFabResult<GetTitleDataResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the key-value store of custom internal title settings
        /// </summary>
        public static async Task<PlayFabResult<GetTitleDataResult>> GetTitleInternalDataAsync(GetTitleDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetTitleInternalData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetTitleDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetTitleDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetTitleDataResult result = resultData.data;

            return new PlayFabResult<GetTitleDataResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the title news feed, as configured in the developer portal
        /// </summary>
        public static async Task<PlayFabResult<GetTitleNewsResult>> GetTitleNewsAsync(GetTitleNewsRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetTitleNews", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetTitleNewsResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetTitleNewsResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetTitleNewsResult result = resultData.data;

            return new PlayFabResult<GetTitleNewsResult> { Result = result };
        }

        /// <summary>
        /// Updates the key-value store of custom publisher settings
        /// </summary>
        public static async Task<PlayFabResult<SetPublisherDataResult>> SetPublisherDataAsync(SetPublisherDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/SetPublisherData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<SetPublisherDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<SetPublisherDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            SetPublisherDataResult result = resultData.data;

            return new PlayFabResult<SetPublisherDataResult> { Result = result };
        }

        /// <summary>
        /// Updates the key-value store of custom title settings
        /// </summary>
        public static async Task<PlayFabResult<SetTitleDataResult>> SetTitleDataAsync(SetTitleDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/SetTitleData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<SetTitleDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<SetTitleDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            SetTitleDataResult result = resultData.data;

            return new PlayFabResult<SetTitleDataResult> { Result = result };
        }

        /// <summary>
        /// Updates the key-value store of custom title settings
        /// </summary>
        public static async Task<PlayFabResult<SetTitleDataResult>> SetTitleInternalDataAsync(SetTitleDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/SetTitleInternalData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<SetTitleDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<SetTitleDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            SetTitleDataResult result = resultData.data;

            return new PlayFabResult<SetTitleDataResult> { Result = result };
        }

        /// <summary>
        /// Increments  the character's balance of the specified virtual currency by the stated amount
        /// </summary>
        public static async Task<PlayFabResult<ModifyCharacterVirtualCurrencyResult>> AddCharacterVirtualCurrencyAsync(AddCharacterVirtualCurrencyRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/AddCharacterVirtualCurrency", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<ModifyCharacterVirtualCurrencyResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<ModifyCharacterVirtualCurrencyResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            ModifyCharacterVirtualCurrencyResult result = resultData.data;

            return new PlayFabResult<ModifyCharacterVirtualCurrencyResult> { Result = result };
        }

        /// <summary>
        /// Increments  the user's balance of the specified virtual currency by the stated amount
        /// </summary>
        public static async Task<PlayFabResult<ModifyUserVirtualCurrencyResult>> AddUserVirtualCurrencyAsync(AddUserVirtualCurrencyRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/AddUserVirtualCurrency", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<ModifyUserVirtualCurrencyResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<ModifyUserVirtualCurrencyResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            ModifyUserVirtualCurrencyResult result = resultData.data;

            return new PlayFabResult<ModifyUserVirtualCurrencyResult> { Result = result };
        }

        /// <summary>
        /// Consume uses of a consumable item. When all uses are consumed, it will be removed from the player's inventory.
        /// </summary>
        public static async Task<PlayFabResult<ConsumeItemResult>> ConsumeItemAsync(ConsumeItemRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/ConsumeItem", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<ConsumeItemResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<ConsumeItemResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            ConsumeItemResult result = resultData.data;

            return new PlayFabResult<ConsumeItemResult> { Result = result };
        }

        /// <summary>
        /// Returns the result of an evaluation of a Random Result Table - the ItemId from the game Catalog which would have been added to the player inventory, if the Random Result Table were added via a Bundle or a call to UnlockContainer.
        /// </summary>
        public static async Task<PlayFabResult<EvaluateRandomResultTableResult>> EvaluateRandomResultTableAsync(EvaluateRandomResultTableRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/EvaluateRandomResultTable", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<EvaluateRandomResultTableResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<EvaluateRandomResultTableResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            EvaluateRandomResultTableResult result = resultData.data;

            return new PlayFabResult<EvaluateRandomResultTableResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the specified character's current inventory of virtual goods
        /// </summary>
        public static async Task<PlayFabResult<GetCharacterInventoryResult>> GetCharacterInventoryAsync(GetCharacterInventoryRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetCharacterInventory", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetCharacterInventoryResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetCharacterInventoryResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetCharacterInventoryResult result = resultData.data;

            return new PlayFabResult<GetCharacterInventoryResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the specified user's current inventory of virtual goods
        /// </summary>
        public static async Task<PlayFabResult<GetUserInventoryResult>> GetUserInventoryAsync(GetUserInventoryRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetUserInventory", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetUserInventoryResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetUserInventoryResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetUserInventoryResult result = resultData.data;

            return new PlayFabResult<GetUserInventoryResult> { Result = result };
        }

        /// <summary>
        /// Adds the specified items to the specified character's inventory
        /// </summary>
        public static async Task<PlayFabResult<GrantItemsToCharacterResult>> GrantItemsToCharacterAsync(GrantItemsToCharacterRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GrantItemsToCharacter", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GrantItemsToCharacterResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GrantItemsToCharacterResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GrantItemsToCharacterResult result = resultData.data;

            return new PlayFabResult<GrantItemsToCharacterResult> { Result = result };
        }

        /// <summary>
        /// Adds the specified items to the specified user's inventory
        /// </summary>
        public static async Task<PlayFabResult<GrantItemsToUserResult>> GrantItemsToUserAsync(GrantItemsToUserRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GrantItemsToUser", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GrantItemsToUserResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GrantItemsToUserResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GrantItemsToUserResult result = resultData.data;

            return new PlayFabResult<GrantItemsToUserResult> { Result = result };
        }

        /// <summary>
        /// Adds the specified items to the specified user inventories
        /// </summary>
        public static async Task<PlayFabResult<GrantItemsToUsersResult>> GrantItemsToUsersAsync(GrantItemsToUsersRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GrantItemsToUsers", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GrantItemsToUsersResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GrantItemsToUsersResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GrantItemsToUsersResult result = resultData.data;

            return new PlayFabResult<GrantItemsToUsersResult> { Result = result };
        }

        /// <summary>
        /// Modifies the number of remaining uses of a player's inventory item
        /// </summary>
        public static async Task<PlayFabResult<ModifyItemUsesResult>> ModifyItemUsesAsync(ModifyItemUsesRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/ModifyItemUses", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<ModifyItemUsesResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<ModifyItemUsesResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            ModifyItemUsesResult result = resultData.data;

            return new PlayFabResult<ModifyItemUsesResult> { Result = result };
        }

        /// <summary>
        /// Moves an item from a character's inventory into another of the users's character's inventory.
        /// </summary>
        public static async Task<PlayFabResult<MoveItemToCharacterFromCharacterResult>> MoveItemToCharacterFromCharacterAsync(MoveItemToCharacterFromCharacterRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/MoveItemToCharacterFromCharacter", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<MoveItemToCharacterFromCharacterResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<MoveItemToCharacterFromCharacterResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            MoveItemToCharacterFromCharacterResult result = resultData.data;

            return new PlayFabResult<MoveItemToCharacterFromCharacterResult> { Result = result };
        }

        /// <summary>
        /// Moves an item from a user's inventory into their character's inventory.
        /// </summary>
        public static async Task<PlayFabResult<MoveItemToCharacterFromUserResult>> MoveItemToCharacterFromUserAsync(MoveItemToCharacterFromUserRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/MoveItemToCharacterFromUser", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<MoveItemToCharacterFromUserResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<MoveItemToCharacterFromUserResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            MoveItemToCharacterFromUserResult result = resultData.data;

            return new PlayFabResult<MoveItemToCharacterFromUserResult> { Result = result };
        }

        /// <summary>
        /// Moves an item from a character's inventory into the owning user's inventory.
        /// </summary>
        public static async Task<PlayFabResult<MoveItemToUserFromCharacterResult>> MoveItemToUserFromCharacterAsync(MoveItemToUserFromCharacterRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/MoveItemToUserFromCharacter", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<MoveItemToUserFromCharacterResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<MoveItemToUserFromCharacterResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            MoveItemToUserFromCharacterResult result = resultData.data;

            return new PlayFabResult<MoveItemToUserFromCharacterResult> { Result = result };
        }

        /// <summary>
        /// Adds the virtual goods associated with the coupon to the user's inventory. Coupons can be generated  via the Promotions->Coupons tab in the PlayFab Game Manager. See this post for more information on coupons:  https://playfab.com/blog/2015/06/18/using-stores-and-coupons-game-manager
        /// </summary>
        public static async Task<PlayFabResult<RedeemCouponResult>> RedeemCouponAsync(RedeemCouponRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/RedeemCoupon", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<RedeemCouponResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<RedeemCouponResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            RedeemCouponResult result = resultData.data;

            return new PlayFabResult<RedeemCouponResult> { Result = result };
        }

        /// <summary>
        /// Submit a report about a player (due to bad bahavior, etc.) on behalf of another player, so that customer service representatives for the title can take action concerning potentially toxic players.
        /// </summary>
        public static async Task<PlayFabResult<ReportPlayerServerResult>> ReportPlayerAsync(ReportPlayerServerRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/ReportPlayer", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<ReportPlayerServerResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<ReportPlayerServerResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            ReportPlayerServerResult result = resultData.data;

            return new PlayFabResult<ReportPlayerServerResult> { Result = result };
        }

        /// <summary>
        /// Revokes access to an item in a user's inventory
        /// </summary>
        public static async Task<PlayFabResult<RevokeInventoryResult>> RevokeInventoryItemAsync(RevokeInventoryItemRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/RevokeInventoryItem", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<RevokeInventoryResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<RevokeInventoryResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            RevokeInventoryResult result = resultData.data;

            return new PlayFabResult<RevokeInventoryResult> { Result = result };
        }

        /// <summary>
        /// Decrements the character's balance of the specified virtual currency by the stated amount
        /// </summary>
        public static async Task<PlayFabResult<ModifyCharacterVirtualCurrencyResult>> SubtractCharacterVirtualCurrencyAsync(SubtractCharacterVirtualCurrencyRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/SubtractCharacterVirtualCurrency", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<ModifyCharacterVirtualCurrencyResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<ModifyCharacterVirtualCurrencyResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            ModifyCharacterVirtualCurrencyResult result = resultData.data;

            return new PlayFabResult<ModifyCharacterVirtualCurrencyResult> { Result = result };
        }

        /// <summary>
        /// Decrements the user's balance of the specified virtual currency by the stated amount
        /// </summary>
        public static async Task<PlayFabResult<ModifyUserVirtualCurrencyResult>> SubtractUserVirtualCurrencyAsync(SubtractUserVirtualCurrencyRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/SubtractUserVirtualCurrency", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<ModifyUserVirtualCurrencyResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<ModifyUserVirtualCurrencyResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            ModifyUserVirtualCurrencyResult result = resultData.data;

            return new PlayFabResult<ModifyUserVirtualCurrencyResult> { Result = result };
        }

        /// <summary>
        /// Opens a specific container (ContainerItemInstanceId), with a specific key (KeyItemInstanceId, when required), and returns the contents of the opened container. If the container (and key when relevant) are consumable (RemainingUses > 0), their RemainingUses will be decremented, consistent with the operation of ConsumeItem.
        /// </summary>
        public static async Task<PlayFabResult<UnlockContainerItemResult>> UnlockContainerInstanceAsync(UnlockContainerInstanceRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/UnlockContainerInstance", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UnlockContainerItemResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<UnlockContainerItemResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            UnlockContainerItemResult result = resultData.data;

            return new PlayFabResult<UnlockContainerItemResult> { Result = result };
        }

        /// <summary>
        /// Searches Player or Character inventory for any ItemInstance matching the given CatalogItemId, if necessary unlocks it using any appropriate key, and returns the contents of the opened container. If the container (and key when relevant) are consumable (RemainingUses > 0), their RemainingUses will be decremented, consistent with the operation of ConsumeItem.
        /// </summary>
        public static async Task<PlayFabResult<UnlockContainerItemResult>> UnlockContainerItemAsync(UnlockContainerItemRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/UnlockContainerItem", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UnlockContainerItemResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<UnlockContainerItemResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            UnlockContainerItemResult result = resultData.data;

            return new PlayFabResult<UnlockContainerItemResult> { Result = result };
        }

        /// <summary>
        /// Updates the key-value pair data tagged to the specified item, which is read-only from the client.
        /// </summary>
        public static async Task<PlayFabResult<EmptyResult>> UpdateUserInventoryItemCustomDataAsync(UpdateUserInventoryItemDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/UpdateUserInventoryItemCustomData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<EmptyResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<EmptyResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            EmptyResult result = resultData.data;

            return new PlayFabResult<EmptyResult> { Result = result };
        }

        /// <summary>
        /// Informs the PlayFab match-making service that the user specified has left the Game Server Instance
        /// </summary>
        public static async Task<PlayFabResult<NotifyMatchmakerPlayerLeftResult>> NotifyMatchmakerPlayerLeftAsync(NotifyMatchmakerPlayerLeftRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/NotifyMatchmakerPlayerLeft", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<NotifyMatchmakerPlayerLeftResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<NotifyMatchmakerPlayerLeftResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            NotifyMatchmakerPlayerLeftResult result = resultData.data;

            return new PlayFabResult<NotifyMatchmakerPlayerLeftResult> { Result = result };
        }

        /// <summary>
        /// Validates a Game Server session ticket and returns details about the user
        /// </summary>
        public static async Task<PlayFabResult<RedeemMatchmakerTicketResult>> RedeemMatchmakerTicketAsync(RedeemMatchmakerTicketRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/RedeemMatchmakerTicket", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<RedeemMatchmakerTicketResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<RedeemMatchmakerTicketResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            RedeemMatchmakerTicketResult result = resultData.data;

            return new PlayFabResult<RedeemMatchmakerTicketResult> { Result = result };
        }

        /// <summary>
        /// Sets the state of the indicated Game Server Instance
        /// </summary>
        public static async Task<PlayFabResult<SetGameServerInstanceStateResult>> SetGameServerInstanceStateAsync(SetGameServerInstanceStateRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/SetGameServerInstanceState", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<SetGameServerInstanceStateResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<SetGameServerInstanceStateResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            SetGameServerInstanceStateResult result = resultData.data;

            return new PlayFabResult<SetGameServerInstanceStateResult> { Result = result };
        }

        /// <summary>
        /// Awards the specified users the specified Steam achievements
        /// </summary>
        public static async Task<PlayFabResult<AwardSteamAchievementResult>> AwardSteamAchievementAsync(AwardSteamAchievementRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/AwardSteamAchievement", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<AwardSteamAchievementResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<AwardSteamAchievementResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            AwardSteamAchievementResult result = resultData.data;

            return new PlayFabResult<AwardSteamAchievementResult> { Result = result };
        }

        /// <summary>
        /// Logs a custom analytics event
        /// </summary>
        public static async Task<PlayFabResult<LogEventResult>> LogEventAsync(LogEventRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/LogEvent", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<LogEventResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<LogEventResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            LogEventResult result = resultData.data;

            return new PlayFabResult<LogEventResult> { Result = result };
        }

        /// <summary>
        /// Writes a character-based event into PlayStream.
        /// </summary>
        public static async Task<PlayFabResult<WriteEventResponse>> WriteCharacterEventAsync(WriteServerCharacterEventRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/WriteCharacterEvent", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<WriteEventResponse> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<WriteEventResponse>>(new JsonTextReader(new StringReader(resultRawJson)));

            WriteEventResponse result = resultData.data;

            return new PlayFabResult<WriteEventResponse> { Result = result };
        }

        /// <summary>
        /// Writes a player-based event into PlayStream.
        /// </summary>
        public static async Task<PlayFabResult<WriteEventResponse>> WritePlayerEventAsync(WriteServerPlayerEventRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/WritePlayerEvent", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<WriteEventResponse> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<WriteEventResponse>>(new JsonTextReader(new StringReader(resultRawJson)));

            WriteEventResponse result = resultData.data;

            return new PlayFabResult<WriteEventResponse> { Result = result };
        }

        /// <summary>
        /// Writes a title-based event into PlayStream.
        /// </summary>
        public static async Task<PlayFabResult<WriteEventResponse>> WriteTitleEventAsync(WriteTitleEventRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/WriteTitleEvent", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<WriteEventResponse> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<WriteEventResponse>>(new JsonTextReader(new StringReader(resultRawJson)));

            WriteEventResponse result = resultData.data;

            return new PlayFabResult<WriteEventResponse> { Result = result };
        }

        /// <summary>
        /// Adds users to the set of those able to update both the shared data, as well as the set of users in the group. Only users in the group (and the server) can add new members.
        /// </summary>
        public static async Task<PlayFabResult<AddSharedGroupMembersResult>> AddSharedGroupMembersAsync(AddSharedGroupMembersRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/AddSharedGroupMembers", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<AddSharedGroupMembersResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<AddSharedGroupMembersResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            AddSharedGroupMembersResult result = resultData.data;

            return new PlayFabResult<AddSharedGroupMembersResult> { Result = result };
        }

        /// <summary>
        /// Requests the creation of a shared group object, containing key/value pairs which may be updated by all members of the group. When created by a server, the group will initially have no members.
        /// </summary>
        public static async Task<PlayFabResult<CreateSharedGroupResult>> CreateSharedGroupAsync(CreateSharedGroupRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/CreateSharedGroup", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<CreateSharedGroupResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<CreateSharedGroupResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            CreateSharedGroupResult result = resultData.data;

            return new PlayFabResult<CreateSharedGroupResult> { Result = result };
        }

        /// <summary>
        /// Deletes a shared group, freeing up the shared group ID to be reused for a new group
        /// </summary>
        public static async Task<PlayFabResult<EmptyResult>> DeleteSharedGroupAsync(DeleteSharedGroupRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/DeleteSharedGroup", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<EmptyResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<EmptyResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            EmptyResult result = resultData.data;

            return new PlayFabResult<EmptyResult> { Result = result };
        }

        /// <summary>
        /// Retrieves data stored in a shared group object, as well as the list of members in the group. The server can access all public and private group data.
        /// </summary>
        public static async Task<PlayFabResult<GetSharedGroupDataResult>> GetSharedGroupDataAsync(GetSharedGroupDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetSharedGroupData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetSharedGroupDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetSharedGroupDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetSharedGroupDataResult result = resultData.data;

            return new PlayFabResult<GetSharedGroupDataResult> { Result = result };
        }

        /// <summary>
        /// Removes users from the set of those able to update the shared data and the set of users in the group. Only users in the group can remove members. If as a result of the call, zero users remain with access, the group and its associated data will be deleted.
        /// </summary>
        public static async Task<PlayFabResult<RemoveSharedGroupMembersResult>> RemoveSharedGroupMembersAsync(RemoveSharedGroupMembersRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/RemoveSharedGroupMembers", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<RemoveSharedGroupMembersResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<RemoveSharedGroupMembersResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            RemoveSharedGroupMembersResult result = resultData.data;

            return new PlayFabResult<RemoveSharedGroupMembersResult> { Result = result };
        }

        /// <summary>
        /// Adds, updates, and removes data keys for a shared group object. If the permission is set to Public, all fields updated or added in this call will be readable by users not in the group. By default, data permissions are set to Private. Regardless of the permission setting, only members of the group (and the server) can update the data.
        /// </summary>
        public static async Task<PlayFabResult<UpdateSharedGroupDataResult>> UpdateSharedGroupDataAsync(UpdateSharedGroupDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/UpdateSharedGroupData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UpdateSharedGroupDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<UpdateSharedGroupDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            UpdateSharedGroupDataResult result = resultData.data;

            return new PlayFabResult<UpdateSharedGroupDataResult> { Result = result };
        }

        /// <summary>
        /// Executes a CloudScript function, with the 'currentPlayerId' variable set to the specified PlayFabId parameter value.
        /// </summary>
        public static async Task<PlayFabResult<ExecuteCloudScriptResult>> ExecuteCloudScriptAsync(ExecuteCloudScriptServerRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/ExecuteCloudScript", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<ExecuteCloudScriptResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<ExecuteCloudScriptResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            ExecuteCloudScriptResult result = resultData.data;

            return new PlayFabResult<ExecuteCloudScriptResult> { Result = result };
        }

        /// <summary>
        /// This API retrieves a pre-signed URL for accessing a content file for the title. A subsequent  HTTP GET to the returned URL will attempt to download the content. A HEAD query to the returned URL will attempt to  retrieve the metadata of the content. Note that a successful result does not guarantee the existence of this content -  if it has not been uploaded, the query to retrieve the data will fail. See this post for more information:  https://community.playfab.com/hc/en-us/community/posts/205469488-How-to-upload-files-to-PlayFab-s-Content-Service
        /// </summary>
        public static async Task<PlayFabResult<GetContentDownloadUrlResult>> GetContentDownloadUrlAsync(GetContentDownloadUrlRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetContentDownloadUrl", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetContentDownloadUrlResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetContentDownloadUrlResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetContentDownloadUrlResult result = resultData.data;

            return new PlayFabResult<GetContentDownloadUrlResult> { Result = result };
        }

        /// <summary>
        /// Deletes the specific character ID from the specified user.
        /// </summary>
        public static async Task<PlayFabResult<DeleteCharacterFromUserResult>> DeleteCharacterFromUserAsync(DeleteCharacterFromUserRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/DeleteCharacterFromUser", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<DeleteCharacterFromUserResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<DeleteCharacterFromUserResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            DeleteCharacterFromUserResult result = resultData.data;

            return new PlayFabResult<DeleteCharacterFromUserResult> { Result = result };
        }

        /// <summary>
        /// Lists all of the characters that belong to a specific user. CharacterIds are not globally unique; characterId must be evaluated with the parent PlayFabId to guarantee uniqueness.
        /// </summary>
        public static async Task<PlayFabResult<ListUsersCharactersResult>> GetAllUsersCharactersAsync(ListUsersCharactersRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetAllUsersCharacters", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<ListUsersCharactersResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<ListUsersCharactersResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            ListUsersCharactersResult result = resultData.data;

            return new PlayFabResult<ListUsersCharactersResult> { Result = result };
        }

        /// <summary>
        /// Retrieves a list of ranked characters for the given statistic, starting from the indicated point in the leaderboard
        /// </summary>
        public static async Task<PlayFabResult<GetCharacterLeaderboardResult>> GetCharacterLeaderboardAsync(GetCharacterLeaderboardRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetCharacterLeaderboard", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetCharacterLeaderboardResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetCharacterLeaderboardResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetCharacterLeaderboardResult result = resultData.data;

            return new PlayFabResult<GetCharacterLeaderboardResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the details of all title-specific statistics for the specific character
        /// </summary>
        public static async Task<PlayFabResult<GetCharacterStatisticsResult>> GetCharacterStatisticsAsync(GetCharacterStatisticsRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetCharacterStatistics", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetCharacterStatisticsResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetCharacterStatisticsResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetCharacterStatisticsResult result = resultData.data;

            return new PlayFabResult<GetCharacterStatisticsResult> { Result = result };
        }

        /// <summary>
        /// Retrieves a list of ranked characters for the given statistic, centered on the requested user
        /// </summary>
        public static async Task<PlayFabResult<GetLeaderboardAroundCharacterResult>> GetLeaderboardAroundCharacterAsync(GetLeaderboardAroundCharacterRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetLeaderboardAroundCharacter", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetLeaderboardAroundCharacterResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetLeaderboardAroundCharacterResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetLeaderboardAroundCharacterResult result = resultData.data;

            return new PlayFabResult<GetLeaderboardAroundCharacterResult> { Result = result };
        }

        /// <summary>
        /// Retrieves a list of all of the user's characters for the given statistic.
        /// </summary>
        public static async Task<PlayFabResult<GetLeaderboardForUsersCharactersResult>> GetLeaderboardForUserCharactersAsync(GetLeaderboardForUsersCharactersRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetLeaderboardForUserCharacters", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetLeaderboardForUsersCharactersResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetLeaderboardForUsersCharactersResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetLeaderboardForUsersCharactersResult result = resultData.data;

            return new PlayFabResult<GetLeaderboardForUsersCharactersResult> { Result = result };
        }

        /// <summary>
        /// Grants the specified character type to the user. CharacterIds are not globally unique; characterId must be evaluated with the parent PlayFabId to guarantee uniqueness.
        /// </summary>
        public static async Task<PlayFabResult<GrantCharacterToUserResult>> GrantCharacterToUserAsync(GrantCharacterToUserRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GrantCharacterToUser", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GrantCharacterToUserResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GrantCharacterToUserResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GrantCharacterToUserResult result = resultData.data;

            return new PlayFabResult<GrantCharacterToUserResult> { Result = result };
        }

        /// <summary>
        /// Updates the values of the specified title-specific statistics for the specific character
        /// </summary>
        public static async Task<PlayFabResult<UpdateCharacterStatisticsResult>> UpdateCharacterStatisticsAsync(UpdateCharacterStatisticsRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/UpdateCharacterStatistics", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UpdateCharacterStatisticsResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<UpdateCharacterStatisticsResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            UpdateCharacterStatisticsResult result = resultData.data;

            return new PlayFabResult<UpdateCharacterStatisticsResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the title-specific custom data for the user which is readable and writable by the client
        /// </summary>
        public static async Task<PlayFabResult<GetCharacterDataResult>> GetCharacterDataAsync(GetCharacterDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetCharacterData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetCharacterDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetCharacterDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetCharacterDataResult result = resultData.data;

            return new PlayFabResult<GetCharacterDataResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the title-specific custom data for the user's character which cannot be accessed by the client
        /// </summary>
        public static async Task<PlayFabResult<GetCharacterDataResult>> GetCharacterInternalDataAsync(GetCharacterDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetCharacterInternalData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetCharacterDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetCharacterDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetCharacterDataResult result = resultData.data;

            return new PlayFabResult<GetCharacterDataResult> { Result = result };
        }

        /// <summary>
        /// Retrieves the title-specific custom data for the user's character which can only be read by the client
        /// </summary>
        public static async Task<PlayFabResult<GetCharacterDataResult>> GetCharacterReadOnlyDataAsync(GetCharacterDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/GetCharacterReadOnlyData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<GetCharacterDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<GetCharacterDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            GetCharacterDataResult result = resultData.data;

            return new PlayFabResult<GetCharacterDataResult> { Result = result };
        }

        /// <summary>
        /// Updates the title-specific custom data for the user's chjaracter which is readable and writable by the client
        /// </summary>
        public static async Task<PlayFabResult<UpdateCharacterDataResult>> UpdateCharacterDataAsync(UpdateCharacterDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/UpdateCharacterData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UpdateCharacterDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<UpdateCharacterDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            UpdateCharacterDataResult result = resultData.data;

            return new PlayFabResult<UpdateCharacterDataResult> { Result = result };
        }

        /// <summary>
        /// Updates the title-specific custom data for the user's character which cannot  be accessed by the client
        /// </summary>
        public static async Task<PlayFabResult<UpdateCharacterDataResult>> UpdateCharacterInternalDataAsync(UpdateCharacterDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/UpdateCharacterInternalData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UpdateCharacterDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<UpdateCharacterDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            UpdateCharacterDataResult result = resultData.data;

            return new PlayFabResult<UpdateCharacterDataResult> { Result = result };
        }

        /// <summary>
        /// Updates the title-specific custom data for the user's character which can only be read by the client
        /// </summary>
        public static async Task<PlayFabResult<UpdateCharacterDataResult>> UpdateCharacterReadOnlyDataAsync(UpdateCharacterDataRequest request)
        {
            if (PlayFabSettings.DeveloperSecretKey == null) throw new Exception ("Must have PlayFabSettings.DeveloperSecretKey set to call this method");

            object httpResult = await PlayFabHTTP.DoPost("/Server/UpdateCharacterReadOnlyData", request, "X-SecretKey", PlayFabSettings.DeveloperSecretKey);
            if(httpResult is PlayFabError)
            {
                PlayFabError error = (PlayFabError)httpResult;
                if (PlayFabSettings.GlobalErrorHandler != null)
                    PlayFabSettings.GlobalErrorHandler(error);
                return new PlayFabResult<UpdateCharacterDataResult> { Error = error, };
            }
            string resultRawJson = (string)httpResult;

            var serializer = JsonSerializer.Create(PlayFabUtil.JsonSettings);
            var resultData = serializer.Deserialize<PlayFabJsonSuccess<UpdateCharacterDataResult>>(new JsonTextReader(new StringReader(resultRawJson)));

            UpdateCharacterDataResult result = resultData.data;

            return new PlayFabResult<UpdateCharacterDataResult> { Result = result };
        }

    }
}
