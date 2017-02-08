using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// PlayerData contains all the PlayFab API calls that relate to manipulating and 
/// </summary>
public static class PF_PlayerData
{
    // Player Level Data:
    public static string PlayerId = string.Empty;
    public static bool showAccountOptionsOnLogin = true;
    public static bool isRegisteredForPush = false;
    public static UserAccountInfo accountInfo;
    public static Dictionary<string, UserDataRecord> UserData = new Dictionary<string, UserDataRecord>();

    // this is a sorted, collated structure built from playerInventory. By default, this will only grab items that are in the primary catalog
    public static Dictionary<string, InventoryCategory> inventoryByCategory = new Dictionary<string, InventoryCategory>();
    public static Dictionary<string, int> virtualCurrency;
    public static List<ItemInstance> playerInventory = new List<ItemInstance>();
    public static Dictionary<string, int> userStatistics = new Dictionary<string, int>();

    //aggregation of player characters
    public static List<CharacterResult> playerCharacters = new List<CharacterResult>();
    public static Dictionary<string, UB_CharacterData> playerCharacterData = new Dictionary<string, UB_CharacterData>();
    public static Dictionary<string, List<string>> characterAchievements = new Dictionary<string, List<string>>();
    public static Dictionary<string, Dictionary<string, int>> characterStatistics = new Dictionary<string, Dictionary<string, int>>();

    // Active Character Level Data:
    public static Dictionary<string, int> characterVirtualCurrency = new Dictionary<string, int>();
    public static List<ItemInstance> characterInventory = new List<ItemInstance>();
    public static Dictionary<string, InventoryCategory> characterInvByCategory = new Dictionary<string, InventoryCategory>();

    public static List<FriendInfo> playerFriends = new List<FriendInfo>();

    public static Dictionary<string, UB_AwardedOffer> pendingOffers = new Dictionary<string, UB_AwardedOffer>();

    public static List<ItemInstance> OfferContainers = new List<ItemInstance>();
    public static List<string> RedeemedOffers = new List<string>();

    public enum PlayerClassTypes { Bucephelous = 0, Nightmare = 1, PegaZeus = 2 }

    // The current character being played:
    public static UB_SavedCharacter activeCharacter = null;

    #region User Data
    public static void GetUserData(List<string> keys, UnityAction<GetUserDataResult> callback = null)
    {
        var request = new GetUserDataRequest
        {
            Keys = keys,
            PlayFabId = PlayerId,
        };

        //DialogCanvasController.RequestLoadingPrompt (PlayFabAPIMethods.GetUserData);
        PlayFabClientAPI.GetUserReadOnlyData(request, (GetUserDataResult result) =>
                                             {
                                                 if (callback != null)
                                                 {
                                                     callback(result);
                                                 }
                                                 PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetUserData, MessageDisplayStyle.none);

                                             }, PF_Bridge.PlayFabErrorCallback);

    }

    public static void UpdateUserData(Dictionary<string, string> updates, string permission = "Public", UnityAction<UpdateUserDataResult> callback = null)
    {
        var request = new UpdateUserDataRequest
        {
            Data = updates,
            Permission = (UserDataPermission)Enum.Parse(typeof(UserDataPermission), permission),
        };

        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.UpdateUserData);
        PlayFabClientAPI.UpdateUserData(request, (UpdateUserDataResult result) =>
                                        {
                                            if (callback != null)
                                            {
                                                callback(result);
                                            }
                                            PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.UpdateUserData, MessageDisplayStyle.none);

                                        }, PF_Bridge.PlayFabErrorCallback);
    }

    public static void GetUserInventory(Action callback = null)
    {
        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetUserInventory);
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), (GetUserInventoryResult result) =>
        {
            virtualCurrency = result.VirtualCurrency;
            playerInventory = result.Inventory;
            inventoryByCategory.Clear();

            if (PF_GameData.catalogItems.Count > 0)
            {
                foreach (var item in playerInventory)
                {
                    if (item.CatalogVersion == "Offers")
                    {
                        OfferContainers.Add(item);
                        continue;
                    }

                    if (!inventoryByCategory.ContainsKey(item.ItemId))
                    {
                        var catalogItem = PF_GameData.GetCatalogItemById(item.ItemId);
                        var items = playerInventory.FindAll((x) => { return x.ItemId.Equals(item.ItemId); });

                        try
                        {
                            if (catalogItem != null)
                            {
                                var customIcon = "Defaut";
                                // here we can process the custom data and apply the propper treatment (eg assign icons)
                                if (catalogItem.CustomData != null && catalogItem.CustomData != "null") //TODO update once the bug is fixed on the null value
                                {
                                    Dictionary<string, string> customAttributes = JsonWrapper.DeserializeObject<Dictionary<string, string>>(catalogItem.CustomData);
                                    if (customAttributes.ContainsKey("icon"))
                                    {
                                        customIcon = customAttributes["icon"];
                                    }
                                }

                                var icon = GameController.Instance.iconManager.GetIconById(customIcon);

                                if (catalogItem.Consumable.UsageCount > 0)
                                {
                                    inventoryByCategory.Add(item.ItemId, new InventoryCategory(item.ItemId, catalogItem, items, icon, true));
                                }
                                else
                                {
                                    inventoryByCategory.Add(item.ItemId, new InventoryCategory(item.ItemId, catalogItem, items, icon));
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning(item.ItemId + " -- " + e.Message);
                            continue;
                        }
                    }
                }

                if (OfferContainers.Count > 0)
                {
                    DialogCanvasController.RequestOfferPrompt();
                }
            }

            if (callback != null)
            {
                callback();
            }

            PF_Bridge.RaiseCallbackSuccess("", PlayFabAPIMethods.GetUserInventory, MessageDisplayStyle.none);
        }, PF_Bridge.PlayFabErrorCallback);
    }

    #endregion

    #region User Statistics
    public static void GetUserStatistics()
    {
        GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest();
        PlayFabClientAPI.GetPlayerStatistics(request, OnGetUserStatisticsSuccess, OnGetUserStatisticsError);
    }

    private static void OnGetUserStatisticsSuccess(GetPlayerStatisticsResult result)
    {
        //TODO update to use new 

        PF_Bridge.RaiseCallbackSuccess("", PlayFabAPIMethods.GetUserStatistics, MessageDisplayStyle.none);
        foreach (var each in result.Statistics)
            userStatistics[each.StatisticName] = each.Value;
    }

    private static void OnGetUserStatisticsError(PlayFabError error)
    {
        PF_Bridge.RaiseCallbackError(error.ErrorMessage, PlayFabAPIMethods.GetUserStatistics, MessageDisplayStyle.error);
    }

    public static void UpdateUserStatistics(Dictionary<string, int> updates)
    {
        var request = new UpdatePlayerStatisticsRequest();
        request.Statistics = new List<StatisticUpdate>();

        foreach (var eachUpdate in updates) // Copy the stats from the inputs to the request
        {
            int eachStat;
            userStatistics.TryGetValue(eachUpdate.Key, out eachStat);
            request.Statistics.Add(new StatisticUpdate { StatisticName = eachUpdate.Key, Value = eachUpdate.Value }); // Send the value to the server
            userStatistics[eachUpdate.Key] = eachStat + eachUpdate.Value; // Update the local cache so that future updates are using correct values
        }

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdateUserStatisticsSuccess, OnUpdateUserStatisticsError);
    }

    private static void OnUpdateUserStatisticsSuccess(UpdatePlayerStatisticsResult result)
    {
        PF_Bridge.RaiseCallbackSuccess("User Statistics Loaded", PlayFabAPIMethods.UpdateUserStatistics, MessageDisplayStyle.none);
        GetCharacterStatistics(); // Refresh stats that we just updated
    }

    private static void OnUpdateUserStatisticsError(PlayFabError error)
    {
        PF_Bridge.RaiseCallbackError(error.ErrorMessage, PlayFabAPIMethods.UpdateUserStatistics, MessageDisplayStyle.error);
    }
    #endregion

    #region User Account APIs
    public static void GetUserAccountInfo()
    {
        var request = new GetPlayerCombinedInfoRequest();
        request.InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetUserData = true, GetUserReadOnlyData = true, GetUserInventory = true, GetUserVirtualCurrency = true, GetUserAccountInfo = true, GetPlayerStatistics = true };

        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetAccountInfo);
        PlayFabClientAPI.GetPlayerCombinedInfo(request, OnGetUserAccountInfoSuccess, PF_Bridge.PlayFabErrorCallback);
    }

    private static void OnGetUserAccountInfoSuccess(GetPlayerCombinedInfoResult result)
    {
        playerInventory = result.InfoResultPayload.UserInventory;
        accountInfo = result.InfoResultPayload.AccountInfo;

        if (result.InfoResultPayload.UserData.ContainsKey("IsRegisteredForPush"))
        {
            if (result.InfoResultPayload.UserData["IsRegisteredForPush"].Value == "1")
            {
                isRegisteredForPush = true;
            }
            else
            {
                isRegisteredForPush = false;
            }
        }
        else
        {
            isRegisteredForPush = false;
        }

        if (result.InfoResultPayload.UserData.ContainsKey("ShowAccountOptionsOnLogin") && result.InfoResultPayload.UserData["ShowAccountOptionsOnLogin"].Value == "0")
        {
            showAccountOptionsOnLogin = false;
        }
        else //if (PF_Authentication.hasLoggedInOnce == false) 
        {
            //PF_Authentication.hasLoggedInOnce = true;
            DialogCanvasController.RequestAccountSettings();
        }

        if (result.InfoResultPayload.UserReadOnlyData.ContainsKey("RedeemedOffers"))
        {
            RedeemedOffers = JsonWrapper.DeserializeObject<List<string>>(result.InfoResultPayload.UserReadOnlyData["RedeemedOffers"].Value);
        }

        inventoryByCategory.Clear();

        if (PF_GameData.catalogItems.Count > 0)
        {
            foreach (var item in playerInventory)
            {
                if (item.CatalogVersion == "Offers")
                {
                    OfferContainers.Add(item);
                    continue;
                }

                if (!inventoryByCategory.ContainsKey(item.ItemId))
                {
                    var catalogItem = PF_GameData.GetCatalogItemById(item.ItemId);
                    var items = new List<ItemInstance>(playerInventory.FindAll((x) => { return x.ItemId.Equals(item.ItemId); }));
                    try
                    {
                        if (catalogItem != null)
                        {
                            string customIcon = "Defaut";
                            // here we can process the custom data and apply the propper treatment (eg assign icons)
                            if (catalogItem.CustomData != null && catalogItem.CustomData != "null") //TODO update once the bug is fixed on the null value
                            {
                                Dictionary<string, string> customAttributes = JsonWrapper.DeserializeObject<Dictionary<string, string>>(catalogItem.CustomData);
                                if (customAttributes.ContainsKey("icon"))
                                {
                                    customIcon = customAttributes["icon"];
                                }
                            }

                            Sprite icon = GameController.Instance.iconManager.GetIconById(customIcon);

                            if (catalogItem.Consumable.UsageCount > 0)
                            {
                                inventoryByCategory.Add(item.ItemId, new InventoryCategory(item.ItemId, catalogItem, items, icon, true));
                            }
                            else
                            {
                                inventoryByCategory.Add(item.ItemId, new InventoryCategory(item.ItemId, catalogItem, items, icon));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(item.ItemId + " -- " + e.Message);
                        continue;
                    }
                }
            }

            if (OfferContainers.Count > 0)
            {
                DialogCanvasController.RequestOfferPrompt();
            }
        }


        if (PF_Authentication.GetDeviceId(true))
        {
            Debug.Log("Mobile Device ID Found!");

            string deviceID = string.IsNullOrEmpty(PF_Authentication.android_id) ? PF_Authentication.ios_id : PF_Authentication.android_id;
            PlayerPrefs.SetString("LastDeviceIdUsed", deviceID);
        }
        else
        {
            Debug.Log("Custom Device ID Found!");

            if (string.IsNullOrEmpty(PF_Authentication.custom_id))
            {
                PlayerPrefs.SetString("LastDeviceIdUsed", PF_Authentication.custom_id);
            }
        }


        //			if(result.AccountInfo.FacebookInfo != null)
        //			{
        //				if(FB.IsInitialized != false && FB.IsLoggedIn == false)
        //				{
        //					FB.Login("public_profile,email,user_friends", (FBResult response) => 
        //					{
        //						if (response.Error != null)
        //						{
        //							PF_Bridge.RaiseCallbackError("Facebook Error: " + response.Error, PlayFabAPIMethods.LoginWithFacebook, MessageDisplayStyle.none);
        //						}
        //						else if (!FB.IsLoggedIn)
        //						{
        //							PF_Bridge.RaiseCallbackError("You canceled the Facebook session, without an active facebook session photos and other data will not be accessable.", PlayFabAPIMethods.LoginWithFacebook, MessageDisplayStyle.none);
        //						}	
        //					});
        //				}
        //
        //				Debug.Log("Facebook Linked Account!");
        //				PlayerPrefs.SetInt("LinkedFacebook", 1);
        //			}
        //			else
        //			{
        //				Debug.Log("Unlinked Account.");
        //				PlayerPrefs.SetInt("LinkedFacebook", 0);
        //			}


        virtualCurrency = result.InfoResultPayload.UserVirtualCurrency;
        PF_Bridge.RaiseCallbackSuccess("Player Account Info Loaded", PlayFabAPIMethods.GetAccountInfo, MessageDisplayStyle.none);
    }
    #endregion

    #region Character APIs
    public static void GetCharacterData()
    {
        playerCharacterData.Clear();
        characterAchievements.Clear();

        int remainingCallbacks = playerCharacters.Count;

        if (remainingCallbacks == 0)
        {
            PF_Bridge.RaiseCallbackSuccess("", PlayFabAPIMethods.GetCharacterReadOnlyData, MessageDisplayStyle.none);
            return;
        }
        else
        {
            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetCharacterReadOnlyData);
        }


        foreach (var character in playerCharacters)
        {
            var request = new GetCharacterDataRequest();
            request.PlayFabId = PlayerId;
            request.CharacterId = character.CharacterId;
            request.Keys = new List<string>() { "CharacterData", "Achievements" };

            PlayFabClientAPI.GetCharacterReadOnlyData(request, (result) =>
            {
                // OFFERS

                if (result.Data.ContainsKey("Achievements"))
                {
                    characterAchievements.Add(result.CharacterId, JsonWrapper.DeserializeObject<List<string>>(result.Data["Achievements"].Value));
                }


                if (result.Data.ContainsKey("CharacterData"))
                {
                    playerCharacterData.Add(result.CharacterId, JsonWrapper.DeserializeObject<UB_CharacterData>(result.Data["CharacterData"].Value));
                    remainingCallbacks--;
                    if (remainingCallbacks == 0)
                    {
                        PF_Bridge.RaiseCallbackSuccess("", PlayFabAPIMethods.GetCharacterReadOnlyData, MessageDisplayStyle.none);
                    }
                }

            }, PF_Bridge.PlayFabErrorCallback);

        }
    }

    public static void GetCharacterDataById(string characterId)
    {
        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetCharacterReadOnlyData);

        var request = new GetCharacterDataRequest();
        request.PlayFabId = PlayerId;
        request.CharacterId = characterId;
        request.Keys = new List<string> { "CharacterData" };

        PlayFabClientAPI.GetCharacterReadOnlyData(request, (result) =>
                                                  {
                                                      if (result.Data.ContainsKey("CharacterData"))
                                                      {
                                                          playerCharacterData[result.CharacterId] = JsonWrapper.DeserializeObject<UB_CharacterData>(result.Data["CharacterData"].Value);

                                                          PF_Bridge.RaiseCallbackSuccess("", PlayFabAPIMethods.GetCharacterReadOnlyData, MessageDisplayStyle.none);
                                                      }

                                                  }, PF_Bridge.PlayFabErrorCallback);
    }

    public static bool DoesCharacterHaveAchievement(string charId, string achvId)
    {
        List<string> achievements;
        characterAchievements.TryGetValue(charId, out achievements);
        if (achievements != null && achievements.Count > 0)
        {
            var check = achievements.FirstOrDefault((i) => { return string.Equals(i, achvId); });
            if (check != null)
            {
                return true;
            }
        }
        return false;
    }

    public static void GetCharacterStatistics()
    {
        var characterIDs = new List<string>();
        foreach (var each in playerCharacters)
        {
            characterIDs.Add(each.CharacterId);
        }

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetCharacterStatistics",
            FunctionParameter = new { CharacterId = characterIDs },
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnGetCharacterStatisticsSuccess, PF_Bridge.PlayFabErrorCallback);
    }

    private static void OnGetCharacterStatisticsSuccess(ExecuteCloudScriptResult result)
    {
        if (!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
            return;

        characterStatistics = JsonWrapper.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(result.FunctionResult.ToString());
        PF_Bridge.RaiseCallbackSuccess("", PlayFabAPIMethods.GetCharacterStatistics, MessageDisplayStyle.none);
    }

    public static void UpdateCharacterStatistics(string charID, Dictionary<string, int> updates)
    {
        Dictionary<string, int> activeStats;
        characterStatistics.TryGetValue(charID, out activeStats);

        if (activeStats != null)
        {
            foreach (var each in updates)
            {
                if (activeStats.ContainsKey(each.Key))
                {
                    activeStats[each.Key] += each.Value;
                }
                else
                {
                    activeStats.Add(each.Key, each.Value);
                }
            }

            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.UpdateCharacterStatistics);

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "UpdateCharacterStats",
                FunctionParameter = new { CharacterId = charID, CharacterStatistics = activeStats },
            };
            PlayFabClientAPI.ExecuteCloudScript(request, OnUpdateCharacterStatisticsSuccess, PF_Bridge.PlayFabErrorCallback);
        }
    }

    private static void OnUpdateCharacterStatisticsSuccess(ExecuteCloudScriptResult result)
    {
        if (!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
            return;

        Debug.Log("Stats Saved!");
        PF_Bridge.RaiseCallbackSuccess("", PlayFabAPIMethods.UpdateCharacterStatistics, MessageDisplayStyle.none);
    }

    public static void GetCharacterInventory(string characterId, Action callback = null)
    {
        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetCharacterInventory);

        var request = new GetCharacterInventoryRequest();
        request.CharacterId = characterId;

        PlayFabClientAPI.GetCharacterInventory(request, (GetCharacterInventoryResult result) =>
        {
            OnGetCharacterInventorySuccess(result);

            if (callback != null)
            {
                callback();
            }

        }, PF_Bridge.PlayFabErrorCallback);
    }

    private static void OnGetCharacterInventorySuccess(GetCharacterInventoryResult result)
    {
        PF_Bridge.RaiseCallbackSuccess("Character Inventory Loaded", PlayFabAPIMethods.GetCharacterInventory, MessageDisplayStyle.none);

        characterVirtualCurrency = result.VirtualCurrency;
        characterInventory = result.Inventory;
        characterInvByCategory.Clear();

        if (PF_GameData.catalogItems.Count == 0)
            return;

        foreach (var item in characterInventory)
        {
            if (characterInvByCategory.ContainsKey(item.ItemId))
                continue;

            var catalogItem = PF_GameData.GetCatalogItemById(item.ItemId);
            List<ItemInstance> items = new List<ItemInstance>(characterInventory.FindAll((x) => { return x.ItemId.Equals(item.ItemId); }));

            try
            {
                if (catalogItem != null)
                {
                    var customIcon = "Default";
                    // here we can process the custom data and apply the propper treatment (eg assign icons)
                    if (catalogItem.CustomData != null && catalogItem.CustomData != "null") //TODO update once the bug is fixed on the null value
                    {
                        Dictionary<string, string> customAttributes = JsonWrapper.DeserializeObject<Dictionary<string, string>>(catalogItem.CustomData);
                        string temp;
                        if (customAttributes.TryGetValue("icon", out temp))
                            customIcon = temp;
                    }

                    var icon = GameController.Instance.iconManager.GetIconById(customIcon);
                    characterInvByCategory.Add(item.ItemId, new InventoryCategory(item.ItemId, catalogItem, items, icon, catalogItem.Consumable.UsageCount > 0));
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(item.ItemId + " -- " + e.Message);
            }
        }
    }

    public static void GetPlayerCharacters()
    {
        var request = new ListUsersCharactersRequest();

        PlayFabClientAPI.GetAllUsersCharacters(request, OnGetPlayerCharactersSuccess, PF_Bridge.PlayFabErrorCallback);
    }

    private static void OnGetPlayerCharactersSuccess(ListUsersCharactersResult result)
    {
        playerCharacters = result.Characters;

        PF_Bridge.RaiseCallbackSuccess("Player Characters Retrieved", PlayFabAPIMethods.GetAllUsersCharacters, MessageDisplayStyle.none);
    }

    public static void CreateNewCharacter(string name, UB_ClassDetail details)
    {
        var request = new ExecuteCloudScriptRequest();
        request.FunctionName = "CreateCharacter";
        request.FunctionParameter = new { catalogCode = details.CatalogCode, characterName = name };
        PlayFabClientAPI.ExecuteCloudScript(request, OnCreateNewCharacterSuccess, PF_Bridge.PlayFabErrorCallback);
    }

    private static void OnCreateNewCharacterSuccess(ExecuteCloudScriptResult result)
    {
        if (!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
            return;

        if ((bool)result.FunctionResult)
        {
            PF_Bridge.RaiseCallbackSuccess("New Character Added", PlayFabAPIMethods.GrantCharacterToUser, MessageDisplayStyle.none);
        }
        else
        {
            PF_Bridge.RaiseCallbackError("Error Creating Character" + result.Logs.ToString(), PlayFabAPIMethods.GrantCharacterToUser, MessageDisplayStyle.error);
        }
    }

    public static void DeleteCharacter(string cid)
    {
        Action callback = () =>
        {
            var request = new ExecuteCloudScriptRequest();
            request.FunctionName = "DeleteCharacter";
            request.FunctionParameter = new { characterId = cid };
            PlayFabClientAPI.ExecuteCloudScript(request, OnDeleteCharacterSuccess, PF_Bridge.PlayFabErrorCallback);
        };

        callback();
    }

    private static void OnDeleteCharacterSuccess(ExecuteCloudScriptResult result)
    {
        if (!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
            return;

        if ((bool)result.FunctionResult)
        {
            PF_Bridge.RaiseCallbackSuccess("Character Deleted", PlayFabAPIMethods.DeleteCharacter, MessageDisplayStyle.none);
        }
        else
        {
            PF_Bridge.RaiseCallbackError("Error Deleting Character" + result.Logs.ToString(), PlayFabAPIMethods.DeleteCharacter, MessageDisplayStyle.none);
        }
    }

    public static void UpdateActiveCharacterData()
    {
        var id = activeCharacter.characterDetails.CharacterId;
        UB_CharacterData cData;
        playerCharacterData.TryGetValue(id, out cData);

        if (cData != null)
        {
            activeCharacter.characterData = cData;
        }

        activeCharacter.RefillVitals();
    }
    #endregion

    #region Friend APIs
    public static void GetFriendsList(UnityAction callback = null)
    {
        GetFriendsListRequest request = new GetFriendsListRequest();
        request.IncludeFacebookFriends = true;
        request.IncludeSteamFriends = false;

        //DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetFriendList);
        PlayFabClientAPI.GetFriendsList(request, (GetFriendsListResult result) =>
                                        {
                                            playerFriends = result.Friends;
                                            if (callback != null)
                                            {
                                                callback();
                                            }
                                            PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetFriendList, MessageDisplayStyle.none);
                                        }, PF_Bridge.PlayFabErrorCallback);
    }

    public enum AddFriendMethod { DisplayName, Email, Username, PlayFabID }

    public static void AddFriend(string input, AddFriendMethod method, UnityAction<bool> callback = null)
    {
        AddFriendRequest request = new AddFriendRequest();
        if (method == AddFriendMethod.DisplayName)
        {
            request.FriendTitleDisplayName = input;
        }
        else if (method == AddFriendMethod.Email)
        {
            request.FriendEmail = input;
        }
        else if (method == AddFriendMethod.Username)
        {
            request.FriendUsername = input;
        }
        else if (method == AddFriendMethod.PlayFabID)
        {
            request.FriendPlayFabId = input;
        }

        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.AddFriend);
        PlayFabClientAPI.AddFriend(request, (AddFriendResult result) =>
                                   {
                                       if (callback != null)
                                       {
                                           callback(result.Created);
                                       }
                                       PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.AddFriend, MessageDisplayStyle.none);
                                   }, PF_Bridge.PlayFabErrorCallback);
    }


    public static void RemoveFriend(string id, UnityAction callback = null)
    {
        RemoveFriendRequest request = new RemoveFriendRequest();
        request.FriendPlayFabId = id;

        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.RemoveFriend);
        PlayFabClientAPI.RemoveFriend(request, (RemoveFriendResult result) =>
                                      {
                                          if (callback != null)
                                          {
                                              callback();
                                          }
                                          PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.RemoveFriend, MessageDisplayStyle.none);
                                      }, PF_Bridge.PlayFabErrorCallback);
    }

    public static void SetFriendTags(string id, List<string> tags, UnityAction callback = null)
    {
        SetFriendTagsRequest request = new SetFriendTagsRequest();
        request.FriendPlayFabId = id;
        request.Tags = tags;

        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.SetFriendTags);
        PlayFabClientAPI.SetFriendTags(request, (SetFriendTagsResult result) =>
                                       {
                                           if (callback != null)
                                           {
                                               callback();
                                           }
                                           PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.SetFriendTags, MessageDisplayStyle.none);
                                       }, PF_Bridge.PlayFabErrorCallback);
    }
    #endregion

    #region misc
    public static void RedeemItemOffer(CatalogItem offer, string instanceToRemove, UnityAction<string> callback = null, bool onlyRemoveInstance = false)
    {
        if (onlyRemoveInstance)
        {
            // this offer has already been rewarded, need to remove from the player's invenetory.
            var request = new ExecuteCloudScriptRequest();
            request.FunctionName = "RemoveOfferItem";
            request.FunctionParameter = new { PFID = PlayerId, InstanceToRemove = instanceToRemove };

            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.ConsumeOffer);
            PlayFabClientAPI.ExecuteCloudScript(request, (ExecuteCloudScriptResult result) =>
            {
                if (!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
                    return;

                if (callback != null)
                {
                    callback(null);
                }
                PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.ConsumeOffer, MessageDisplayStyle.none);

            }, PF_Bridge.PlayFabErrorCallback);
        }
        else
        {
            // consume the item 
            ExecuteCloudScriptRequest removeReq = new ExecuteCloudScriptRequest();
            removeReq.FunctionName = "RemoveOfferItem";
            removeReq.FunctionParameter = new { PFID = PlayerId, InstanceToRemove = instanceToRemove };
            PlayFabClientAPI.ExecuteCloudScript(removeReq, (ExecuteCloudScriptResult result) =>
            {
                if (!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
                    return;
            }, PF_Bridge.PlayFabErrorCallback);

            // make the award
            ExecuteCloudScriptRequest awardRequest = new ExecuteCloudScriptRequest();
            awardRequest.FunctionName = "RedeemItemOffer";

            awardRequest.FunctionParameter = new { PFID = PlayerId, Offer = offer, SingleUse = offer.Tags.IndexOf("SingleUse") > -1 ? true : false };

            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.ConsumeOffer);
            PlayFabClientAPI.ExecuteCloudScript(awardRequest, (ExecuteCloudScriptResult result) =>
            {
                if (!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
                    return;

                if (callback != null)
                {
                    callback(result.FunctionResult.ToString());
                }
                PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.ConsumeOffer, MessageDisplayStyle.none);
            }, PF_Bridge.PlayFabErrorCallback);
        }
    }
    public static void SubtractLifeFromPlayer()
    {
        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.ExecuteCloudScript);
        var request = new ExecuteCloudScriptRequest();
        request.FunctionName = "SubtractLife";
        request.FunctionParameter = new { CharacterId = activeCharacter.characterDetails.CharacterId };
        PlayFabClientAPI.ExecuteCloudScript(request, (ExecuteCloudScriptResult result) =>
        {
            if (!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
                return;

            PF_Bridge.RaiseCallbackSuccess("", PlayFabAPIMethods.ExecuteCloudScript, MessageDisplayStyle.none);

        }, PF_Bridge.PlayFabErrorCallback);
    }

    public static void ClearActiveCharacter()
    {
        activeCharacter = null;
        if (characterAchievements != null) characterAchievements.Clear();
        if (characterInvByCategory != null) characterInvByCategory.Clear();
        if (characterInventory != null) characterInventory.Clear();
        if (characterStatistics != null) characterStatistics.Clear();
        if (characterVirtualCurrency != null) characterVirtualCurrency.Clear();
    }

    public static void LinkFBAccount(UnityAction callback = null)
    {
        Action<string> linkAction = (string token) =>
        {
            Action<LinkFacebookAccountResult> afterLink = (LinkFacebookAccountResult result) =>
            {
                Debug.Log("Facebook Linked Account!");
                PlayerPrefs.SetInt("LinkedFacebook", 1);

                if (callback != null)
                {
                    callback();
                }
                PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.LinkFacebookId, MessageDisplayStyle.none);
            };

            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.LinkFacebookId);

            LinkFacebookAccountRequest request = new LinkFacebookAccountRequest();
            request.AccessToken = token;
            PlayFabClientAPI.LinkFacebookAccount(request, (LinkFacebookAccountResult result) =>
                                                     {
                                                         afterLink(result);
                                                     }, (PlayFabError error) =>
                                                     {
                                                         if (error.ErrorMessage.Contains("already linked"))  // ew, gotta get better error codes
                                                         {
                                                             PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.LinkFacebookId, MessageDisplayStyle.none);
                                                             Action<bool> afterConfirm = (bool response) =>
                                                                 {
                                                                     if (response)
                                                                     {
                                                                         request.ForceLink = true;

                                                                         DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.LinkFacebookId);
                                                                         PlayFabClientAPI.LinkFacebookAccount(request, (LinkFacebookAccountResult result) =>
                                                                                                               {
                                                                                                                   afterLink(result);
                                                                                                               }, PF_Bridge.PlayFabErrorCallback);
                                                                     }
                                                                 };

                                                             DialogCanvasController.RequestConfirmationPrompt("Caution!", "Your current facebook account is already linked to another Unicorn Battle player. Do you want to force-bind your Facebook account to this player?", afterConfirm);

                                                         }
                                                         else
                                                         {
                                                             PF_Bridge.RaiseCallbackError(error.ErrorMessage, PlayFabAPIMethods.LinkFacebookId, MessageDisplayStyle.error);
                                                         }

                                                     });
        };


        if (FB.IsInitialized && FB.IsLoggedIn)
        {
            linkAction(AccessToken.CurrentAccessToken.TokenString);
        }
        else
        {
            Action<ILoginResult> afterFBLogin = (ILoginResult result) =>
           {
               if (result.Error != null)
               {
                   PF_Bridge.RaiseCallbackError("Facebook Error: " + result.Error, PlayFabAPIMethods.LinkFacebookId, MessageDisplayStyle.none);
               }
               else if (!FB.IsLoggedIn)
               {
                   PF_Bridge.RaiseCallbackError("Facebook Error: Login cancelled by Player", PlayFabAPIMethods.LinkFacebookId, MessageDisplayStyle.none);
               }
               else
               {
                   linkAction(AccessToken.CurrentAccessToken.TokenString);
               }
           };

            Action afterFBInit = () =>
            {
                //Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
                if (FB.IsLoggedIn == false)
                {
                    FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, (ILoginResult result) => { afterFBLogin(result); });
                }
                else
                {
                    linkAction(AccessToken.CurrentAccessToken.UserId);
                }
            };

            PF_Authentication.StartFacebookLogin(afterFBInit);
        }

    }

    public static void UnlinkFBAccount(UnityAction calback = null)
    {
        var request = new UnlinkFacebookAccountRequest();
        PlayFabClientAPI.UnlinkFacebookAccount(request, (UnlinkFacebookAccountResult result) =>
                                               {
                                                   accountInfo.FacebookInfo = null;
                                                   if (calback != null)
                                                   {
                                                       Debug.Log("Unlinked Account.");
                                                       PlayerPrefs.SetInt("LinkedFacebook", 0);

                                                       calback();
                                                   }
                                               }, PF_Bridge.PlayFabErrorCallback);
    }

    public static void RegisterForPushNotification(string pushToken = null, UnityAction callback = null)
    {
#if UNITY_EDITOR || UNITY_EDITOR_OSX
        if (callback != null)
        {
            callback();
            return;
        }
#endif

#if UNITY_IPHONE
			string hexToken = string.Empty;
			byte[] token = UnityEngine.iOS.NotificationServices.deviceToken;
			if(token != null)
			{
				RegisterForIOSPushNotificationRequest request = new RegisterForIOSPushNotificationRequest();
				request.DeviceToken = BitConverter.ToString(token).Replace("-", "").ToLower();
				
				hexToken = request.DeviceToken;
				Debug.Log (hexToken);
				
				DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.RegisterForPush);
				PlayFabClientAPI.RegisterForIOSPushNotification(request, (RegisterForIOSPushNotificationResult result) => 
				                                                {
					if(callback != null)
					{
						callback();
					}
					PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.RegisterForPush, MessageDisplayStyle.none);
				}, PF_Bridge.PlayFabErrorCallback);
			}
			else
			{
				Debug.Log("Push Token was null!");
			}
#endif

#if UNITY_ANDROID
        if (!string.IsNullOrEmpty(pushToken))
        {
            // success
            Debug.Log("GCM Init Success");
            var request = new AndroidDevicePushNotificationRegistrationRequest();
            request.DeviceToken = pushToken;

            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.RegisterForPush);
            PlayFabClientAPI.AndroidDevicePushNotificationRegistration(request, (AndroidDevicePushNotificationRegistrationResult result) =>
                                                                       {
                                                                           if (callback != null)
                                                                           {
                                                                               callback();
                                                                           }
                                                                           PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.RegisterForPush, MessageDisplayStyle.none);
                                                                       }, PF_Bridge.PlayFabErrorCallback);

        }
        else
        {
            // error happened
            Debug.Log("Push Token was null or empty: ");
        }
#endif
    }

    public static void TransferItemToPlayer(string sourceId, string instanceId, Action callback = null)
    {
        var request = new ExecuteCloudScriptRequest();
        request.FunctionName = "TransferItemToPlayer";
        request.FunctionParameter = new { sourceId = sourceId, instanceId = instanceId };
        PlayFabClientAPI.ExecuteCloudScript(request, (ExecuteCloudScriptResult result) =>
        {
            if (!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
                return;

            if (callback != null)
            {
                callback();
            }
        }, PF_Bridge.PlayFabErrorCallback);
    }

    public static void TransferItemToCharacter(string sourceId, string sourceType, string instanceId, string destId, Action callback = null)
    {
        var request = new ExecuteCloudScriptRequest();
        request.FunctionName = "TransferItemToCharacter";
        request.FunctionParameter = new { sourceId = sourceId, sourceType = sourceType, destId = destId, instanceId = instanceId };
        PlayFabClientAPI.ExecuteCloudScript(request, (ExecuteCloudScriptResult result) =>
        {
            if (!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
                return;

            if (callback != null)
            {
                callback();
            }
        }, PF_Bridge.PlayFabErrorCallback);
    }

    public static void TransferVcToPlayer(string sourceId, string cCode, int amount, Action callback = null)
    {
        var request = new ExecuteCloudScriptRequest();
        request.FunctionName = "TransferVcToPlayer";
        request.FunctionParameter = new { sourceId = sourceId, amount = amount, cCode = cCode };
        PlayFabClientAPI.ExecuteCloudScript(request, (ExecuteCloudScriptResult result) =>
        {
            if (!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
                return;

            if (callback != null)
            {
                callback();
            }
        }, PF_Bridge.PlayFabErrorCallback);
    }

    public static void TransferVCToCharacter(string sourceId, string sourceType, string cCode, int amount, string destId, Action callback = null)
    {
        var request = new ExecuteCloudScriptRequest();
        request.FunctionName = "TransferVCToCharacter";
        request.FunctionParameter = new { sourceId = sourceId, sourceType = sourceType, destId = destId, amount = amount, cCode = cCode };
        PlayFabClientAPI.ExecuteCloudScript(request, (ExecuteCloudScriptResult result) =>
        {
            if (!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
                return;

            if (callback != null)
            {
                callback();
            }
        }, PF_Bridge.PlayFabErrorCallback);
    }
    #endregion
}


