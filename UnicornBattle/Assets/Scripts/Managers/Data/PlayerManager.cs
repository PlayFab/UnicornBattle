using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnicornBattle.Models;
using UnityEngine;

namespace UnicornBattle.Managers
{
    /// <summary>
    /// PLAYER MANAGER
    /// ==============
    /// Manages all the local player/user data
    /// 
    /// </summary>
    public class PlayerManager : DataManager, IPlayerDataRefreshable
    {
        public string PlayerID { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string UserEmail { get; set; }
        public bool showAccountOptionsOnLogin { get; set; }
        public bool isRegisteredForPush { get; set; }

        public bool isPlayFabRegistered
        {
            get { return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(UserEmail); }
        }

        public override void Initialize(MainManager p_manager)
        {
            showAccountOptionsOnLogin = false;
            isRegisteredForPush = false;

            base.Initialize(p_manager);
        }

        /// <summary>
        /// Refresh the local cache with data from the PlayFab server
        /// - This method will only pull down from the server if the data is out of date OR if the data is dirty
        /// - To force the server to get new data, set p_forceRefresh to true
        /// </summary>
        /// <param name="p_forceRefresh">Should we force the call from the server?</param>
        /// <param name="p_onSuccessCallback">Called if successfully refreshed</param>
        /// <param name="p_onFailureCallback">Called if an error occurred</param>
        public void Refresh(bool p_forceRefresh,
            System.Action<string> p_onSuccessCallback = null,
            System.Action<string> p_onFailureCallback = null)
        {
            if (IsInitialized == false)
            {
                Initialize(MainManager.Instance);
            }
            if (p_forceRefresh == false)
            {
                if (IsDataCleanAndFresh)
                {
                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke("Data Fresh");
                    return;
                }
            }

            var l_InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetUserData = true,
                GetUserReadOnlyData = true,
                GetUserInventory = false,
                GetUserVirtualCurrency = false,
                GetUserAccountInfo = true,
                GetPlayerStatistics = true,
                GetCharacterList = true,
                GetTitleData = true,
            };

            PlayFabClientAPI.GetPlayerCombinedInfo(
                new GetPlayerCombinedInfoRequest { InfoRequestParameters = l_InfoRequestParameters },
                (GetPlayerCombinedInfoResult result) =>
                {
                    GetPlayerCombinedInfoResultPayload l_payload = result.InfoResultPayload;
                    if (null == l_payload)
                    {
                        Debug.LogError("No payload in GetplayerCombinedInfo");
                        if (null != p_onFailureCallback)
                            p_onFailureCallback.Invoke("No payload");
                    }

                    loadAccountInfo(l_payload.AccountInfo);
                    loadCharacterList(l_payload.CharacterList);
                    mainManager.LoadTitleData(l_payload.TitleData);
                    loadUserData(l_payload.UserData, l_payload.UserDataVersion);
                    loadPrivateUserData(l_payload.UserReadOnlyData, l_payload.UserReadOnlyDataVersion);
                    loadPlayerStatistics(l_payload.PlayerStatistics);
                    DataRefreshed();
                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke(string.Empty);

                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Update the player's public user data by sending it to the server.
        /// </summary/>
        /// <param name="p_onSuccessCallback">Called if successfully sent</param>
        /// <param name="p_onFailureCallback">Called if an error occurred</param>
        public void UpdatePublicUserData(System.Action<string> p_onSuccessCallback,
            System.Action<string> p_onFailureCallback)
        {
            Dictionary<string, string> l_updates = new Dictionary<string, string>
                { { "ShowAccountOptionsOnLogin", showAccountOptionsOnLogin ? "1" : "0" },
                    { "IsRegisteredForPush", isRegisteredForPush ? "1" : "0" },
                };

            var request = new UpdateUserDataRequest
            {
                Data = l_updates,
                Permission = UserDataPermission.Public,
            };

            PlayFabClientAPI.UpdateUserData(request,
                (UpdateUserDataResult result) =>
                {
                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke(string.Empty); // we don't need anything back
                },
                (PlayFabError error) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(error.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Update the player's private user data by sending it to the server.
        /// </summary>
        /// <param name="p_data">Key-Value pairs of the data to store</param>
        /// <param name="p_onSuccessCallback">Called if successfully sent</param>
        /// <param name="p_onFailureCallback">Called if an error occurred</param>
        public static void UpdatePrivateUserData(System.Action<string> p_onSuccessCallback, System.Action<string> p_onFailureCallback)
        {
            Dictionary<string, string> l_updates = new Dictionary<string, string>
                {

                };

            var request = new UpdateUserDataRequest
            {
                Data = l_updates,
                Permission = UserDataPermission.Private
            };
            PlayFabClientAPI.UpdateUserData(request,
                (result) =>
                {
                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke(string.Empty); // we don't need anything back
                },
                (error) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(error.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Post new user statistical data to the server
        /// </summary>
        /// <param name="p_data">Data to save/post to the server</param>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurred</param>
        public void UpdatePlayerStatistics(Dictionary<string, int> p_data,
            System.Action<string> p_onSuccessCallback = null,
            System.Action<string> p_onFailureCallback = null)
        {
            var l_statList = new List<StatisticUpdate>();
            foreach (var l_pairToUpdate in p_data) // Copy the stats from the inputs to the request
            {
                l_statList.Add(new StatisticUpdate
                {
                    StatisticName = l_pairToUpdate.Key,
                        Value = l_pairToUpdate.Value
                }); // Send the value to the server

                int l_currentStatValue;
                m_userStatistics.TryGetValue(l_pairToUpdate.Key, out l_currentStatValue);
                m_userStatistics[l_pairToUpdate.Key] = l_currentStatValue + l_pairToUpdate.Value; // Update the local cache so that future updates are using correct values
            }

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "SetPlayerStats",
                FunctionParameter = new { statistics = l_statList },
                GeneratePlayStreamEvent = false
            };
            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    if (null != result.Error)
                    {
                        if (null != p_onFailureCallback)
                        {
                            p_onFailureCallback.Invoke(result.Error.Message);
                        }
                        return;
                    }
                    FlagAsDirty();
                    var l_characterMgr = MainManager.Instance.getCharacterManager();
                    if (null == l_characterMgr) return;
                    l_characterMgr.Refresh(true, p_onSuccessCallback, p_onFailureCallback); // Refresh stats that we just updated
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Get a single User Statistic from the local cache.
        /// - returns the stat or zero if the stat does not exist
        /// </summary>
        /// <param name="p_statName">Name of the stat</param>
        /// <returns>The stat's value or zero if stat does not exist</returns>
        public int GetPlayerStatistic(string p_statName)
        {
            if (m_userStatistics.ContainsKey(p_statName))
            {
                return m_userStatistics[p_statName];
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Save Player Data to the server 
        /// - saves:
        ///   - activeCharacter (UBSavedCharacter)
        ///   - QuestProgress (UBQuest)
        ///   - LevelRamp
        /// </summary>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurs</param>
        public void SavePlayerData(
            UBSavedCharacter p_ActiveCharacter,
            UBQuest p_ActiveQuestProgress,
            Dictionary<string, int> p_levelRampList,
            System.Action<string> p_onSuccessCallback = null,
            System.Action<string> p_onFailureCallback = null)
        {

            if (null == p_ActiveCharacter || null == p_ActiveQuestProgress || null == p_levelRampList)
                return;

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "SaveProgress",
                FunctionParameter = new
                {
                CurrentPlayerData = new UBSavedCharacter(p_ActiveCharacter),
                QuestProgress = new UBQuest(p_ActiveQuestProgress),
                LevelRamp = new Dictionary<string, int>(p_levelRampList)
                },
            };
            PlayFabClientAPI.ExecuteCloudScript(
                request,
                (ExecuteCloudScriptResult result) =>
                {
                    if (null != result.Error)
                    {
                        if (null != p_onFailureCallback)
                            p_onFailureCallback.Invoke(result.Error.Message);
                        return;
                    }
                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke("Player Info Saved");
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Subtract a life from the player.
        /// - this method needs to update the server
        /// </summary>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurs</param>
        public void SubtractLifeFromPlayer(
            string p_ActiveCharacterID,
            System.Action<string> p_onSuccessCallback = null,
            System.Action<string> p_onFailureCallback = null)
        {
            var request = new ExecuteCloudScriptRequest
            {
            FunctionName = "SubtractLife",
            FunctionParameter = new { CharacterId = p_ActiveCharacterID }
            };
            PlayFabClientAPI.ExecuteCloudScript(
                request,
                (result) =>
                {
                    if (null != result.Error)
                    {
                        if (null != p_onFailureCallback)
                            p_onFailureCallback.Invoke(result.Error.Message);
                        return;
                    }

                    FlagAsDirty();
                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke(string.Empty);
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Register for Push Notification
        /// </summary>
        /// <param name="pushToken">device token</param>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurs</param>
        public void RegisterForPushNotification(string pushToken = null,
            System.Action<string> p_onSuccessCallback = null,
            System.Action<string> p_onFailureCallback = null)
        {
#if UNITY_IOS
            string hexToken = string.Empty;
            byte[] token = UnityEngine.iOS.NotificationServices.deviceToken;
            if (token != null)
            {
                RegisterForIOSPushNotificationRequest request = new RegisterForIOSPushNotificationRequest();
                request.DeviceToken = System.BitConverter.ToString(token).Replace("-", "").ToLower();

                hexToken = request.DeviceToken;
                //Debug.Log(hexToken);

                PlayFabClientAPI.RegisterForIOSPushNotification(
                    request,
                    result =>
                    {
                        if (null != p_onSuccessCallback)
                            p_onSuccessCallback.Invoke(string.Empty);
                    },
                    (PlayFabError e) =>
                    {
                        if (null != p_onFailureCallback)
                            p_onFailureCallback.Invoke(e.ErrorMessage);
                    }
                );
            }
            else
            {
                Debug.LogWarning("Push Token was null!");
            }
#endif

#if UNITY_ANDROID
            if (!string.IsNullOrEmpty(pushToken))
            {
                var request = new AndroidDevicePushNotificationRegistrationRequest { DeviceToken = pushToken };

                PlayFabClientAPI.AndroidDevicePushNotificationRegistration(
                    request,
                    result =>
                    {
                        if (null != p_onSuccessCallback)
                            p_onSuccessCallback.Invoke(string.Empty);
                    },
                    (PlayFabError e) =>
                    {
                        if (null != p_onFailureCallback)
                            p_onFailureCallback.Invoke(e.ErrorMessage);
                    }
                );
            }
            else
            {
                Debug.LogWarning("Push Token was null or empty: ");
            }
#endif

#if !UNITY_ANDROID && !UNITY_IOS
            if (null != p_onSuccessCallback)
            {
                p_onSuccessCallback.Invoke(string.Empty);
            }
            return;
#endif
        }

        ///  <summary>
        /// Refresh the local cache of User Statistics from the server
        /// </summary>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurred</param>
        public void RefreshUserStats(System.Action p_onSuccessCallback = null, System.Action<string> p_onFailureCallback = null)
        {
            PlayFabClientAPI.GetPlayerStatistics(
                new GetPlayerStatisticsRequest(),
                (GetPlayerStatisticsResult r) =>
                {
                    loadPlayerStatistics(r.Statistics);
                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke();
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        private void loadUserData(Dictionary<string, UserDataRecord> p_userData, uint p_dataVersion)
        {
            if (p_userData.ContainsKey("IsRegisteredForPush"))
                isRegisteredForPush = (p_userData["IsRegisteredForPush"].Value == "1");
            else
                isRegisteredForPush = false;

            if (p_userData.ContainsKey("ShowAccountOptionsOnLogin"))
                showAccountOptionsOnLogin = (p_userData["ShowAccountOptionsOnLogin"].Value == "1");
            else
                showAccountOptionsOnLogin = false;
        }

        private void loadPrivateUserData(Dictionary<string, UserDataRecord> p_userData, uint p_dataVersion)
        {

        }

        private void loadCharacterList(List<CharacterResult> p_characterData)
        {
            var l_charDataManager = mainManager.getCharacterManager();
            if (null == l_charDataManager) return;

            l_charDataManager.LoadCharacterList(p_characterData);
        }

        private void loadInventory(List<ItemInstance> p_inventory)
        {
            var l_inventory = mainManager.getInventoryManager();
            if (null == l_inventory) return;

            l_inventory.LoadInventory(p_inventory);
        }

        private void loadVirtualCurrency(Dictionary<string, int> p_currency)
        {
            var l_inventory = mainManager.getInventoryManager();
            if (null == l_inventory) return;

            l_inventory.LoadVirtualCurrency(p_currency);
        }

        private void loadAccountInfo(UserAccountInfo p_info)
        {
            Username = p_info.Username;
            UserEmail = p_info.PrivateInfo.Email;
            DisplayName = p_info.TitleInfo.DisplayName;
        }

        private void loadPlayerStatistics(List<StatisticValue> p_stats)
        {
            foreach (var l_item in p_stats)
            {
                if (m_userStatistics.ContainsKey(l_item.StatisticName))
                    m_userStatistics[l_item.StatisticName] = l_item.Value;
                else
                    m_userStatistics.Add(l_item.StatisticName, l_item.Value);
            }
        }

        // private local data
        private Dictionary<string, int> m_userStatistics = new Dictionary<string, int>();
        private Dictionary<string, string> m_userData = new Dictionary<string, string>();

    }
}