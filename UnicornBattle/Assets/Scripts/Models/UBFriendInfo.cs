using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;

namespace UnicornBattle.Models
{
    public class UBFriendInfo
    {
        public UserFacebookInfo FacebookInfo;
        /// <summary>
        /// PlayFab unique identifier for this friend.
        /// </summary>
        public string FriendPlayFabId;
        /// <summary>
        /// Tags which have been associated with this friend.
        /// </summary>
        public List<string> Tags;
        /// <summary>
        /// Title-specific display name for this friend.
        /// </summary>
        public string TitleDisplayName;
        /// <summary>
        /// PlayFab unique username for this friend.
        /// </summary>
        public string Username;


        public UBFriendInfo() { }

        public UBFriendInfo( FriendInfo p_copy )
        {
            FacebookInfo = p_copy.FacebookInfo;
            FriendPlayFabId = p_copy.FriendPlayFabId;
            Tags = new List<string>( p_copy.Tags );
            TitleDisplayName = p_copy.TitleDisplayName;
            Username = p_copy.Username;
        }
    }
}
