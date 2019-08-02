using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnicornBattle.Models;

namespace UnicornBattle.Managers
{
    /// <summary>
    /// FRIENDS MANAGER
    /// ===============
    /// Encapsulates all the Friend data used in the game, and stores a local cache of data pulled from the server
    /// - Refresh methods refresh the local data, getters wil return the local data
    /// </summary>
    public class FriendsManager : DataManager, IPlayerDataRefreshable
    {
        public enum AddFriendMethod { DisplayName, Email, Username, PlayFabID }

        public override void Initialize(MainManager p_manager)
        {
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

            var l_request = new GetFriendsListRequest
            {
                IncludeFacebookFriends = true,
                IncludeSteamFriends = false
            };

            PlayFabClientAPI.GetFriendsList(
                l_request,
                (GetFriendsListResult result) =>
                {
                    m_playerFriends.Clear();
                    foreach (FriendInfo eachFriend in result.Friends)
                        m_playerFriends.Add(new UBFriendInfo(eachFriend));

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

        #region Friend APIs

        /// <summary>
        /// get the Friends List
        /// </summary>
        public UBFriendInfo[] GetFriendsArray()
        {
            return m_playerFriends.ToArray();
        }

        /// <summary>
        /// Attempt to add a new friend
        /// </summary>
        /// <param name="p_input">Information used to find the friend</param>
        /// <param name="p_method">Type of input</param>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurs</param>
        public void AddFriend(string p_input, AddFriendMethod p_method, Action p_onSuccessCallback = null, Action<string> p_onFailureCallback = null)
        {
            var l_request = new AddFriendRequest();
            switch (p_method)
            {
                case AddFriendMethod.DisplayName:
                    l_request.FriendTitleDisplayName = p_input;
                    break;
                case AddFriendMethod.Email:
                    l_request.FriendEmail = p_input;
                    break;
                case AddFriendMethod.Username:
                    l_request.FriendUsername = p_input;
                    break;
                case AddFriendMethod.PlayFabID:
                    l_request.FriendPlayFabId = p_input;
                    break;
            }

            PlayFabClientAPI.AddFriend(
                l_request,
                (AddFriendResult result) =>
                {
                    if (result.Created)
                    {

                        FlagAsDirty();
                        if (null != p_onSuccessCallback)
                            p_onSuccessCallback.Invoke();
                    }
                    else
                    {
                        if (null != p_onFailureCallback)
                            p_onFailureCallback.Invoke("Failed to add friend");
                    }
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Attempt to Remove a friend from the server
        /// </summary>
        /// <param name="id">ID of the friend to removed</param>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurs</param>
        public void RemoveFriend(string id, Action p_onSuccessCallback = null, Action<string> p_onFailureCallback = null)
        {
            var request = new RemoveFriendRequest { FriendPlayFabId = id };

            PlayFabClientAPI.RemoveFriend(
                request,
                (RemoveFriendResult result) =>
                {
                    FlagAsDirty();
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

        /// <summary>
        /// Set Friend tags
        /// </summary>
        /// <param name="p_FriendId">Friend ID</param>
        /// <param name="p_tags">Tags to add to the Friend ID</param>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurs</param>
        public void SetFriendTags(string p_FriendId,
            List<string> p_tags,
            Action p_onSuccessCallback = null,
            Action<string> p_onFailureCallback = null)
        {
            var request = new SetFriendTagsRequest
            {
            FriendPlayFabId = p_FriendId,
            Tags = p_tags
            };

            PlayFabClientAPI.SetFriendTags(
                request,
                (SetFriendTagsResult result) =>
                {
                    FlagAsDirty();
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
        #endregion

        private List<UBFriendInfo> m_playerFriends = new List<UBFriendInfo>();

    }
}