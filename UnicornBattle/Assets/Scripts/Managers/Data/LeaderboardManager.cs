using System;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace UnicornBattle.Managers
{
    /// <summary>
    /// LEADERBOARD MANAGER
    /// ===================
    /// Contains the data stored for the current Leaderboards from PlayFab
    /// 
    /// </summary>
    public class LeaderboardManager : DataManager
    {
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="p_manager">ref to main manager</param>
        public override void Initialize(MainManager p_manager)
        {
            m_currentTop10LB = new List<PlayerLeaderboardEntry>();
            m_friendsLB = new List<PlayerLeaderboardEntry>();

            base.Initialize(p_manager);
        }

        /// <summary>
        /// Refresh the Leaderboards for a single statistic
        /// Boards can only show one statistic at a time and always check Playfab for the most up-to-date leaderboards
        /// </summary>
        /// <param name="p_stat">Stat of the leaderboard</param>
        /// <param name="p_onSuccessCallback">Called on successful refresh</param>
        /// <param name="p_onFailureCallback">Called if an error occurred</param>
        public void RefreshLeaderboards(string p_stat, Action<string> p_onSuccessCallback, Action<string> p_onFailureCallback)
        {
            PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest { MaxResultsCount = k_MaximumLeaderboardEntries, StatisticName = p_stat },
                (GetLeaderboardResult result) =>
                {
                    m_currentTop10LB = new List<PlayerLeaderboardEntry>(result.Leaderboard);

                    PlayFabClientAPI.GetFriendLeaderboard(
                        new GetFriendLeaderboardRequest { MaxResultsCount = k_MaximumLeaderboardEntries, StatisticName = p_stat },
                        (GetLeaderboardResult result2) =>
                        {
                            m_friendsLB = new List<PlayerLeaderboardEntry>(result2.Leaderboard);
                            if (null != p_onSuccessCallback)
                                p_onSuccessCallback.Invoke(string.Empty);
                        },
                        (PlayFabError e) =>
                        {
                            if (null != p_onFailureCallback)
                                p_onFailureCallback.Invoke(e.ErrorMessage);
                        }
                    );
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// returns the player leaderboard as an array
        /// call Refresh() beforehand to get the most up-to-date data
        /// </summary>
        public PlayerLeaderboardEntry[] GetPlayerLeaderboard()
        {
            return m_currentTop10LB.ToArray();
        }

        /// <summary>
        /// returns the Friend Leaderboard as an array.
        /// call Refresh() beforehand to get the most up-to-date data
        /// </summary>
        public PlayerLeaderboardEntry[] GetFriendsLeaderboard()
        {
            return m_friendsLB.ToArray();
        }

        /// <summary>
        /// Refresh a character's leaderboard rank
        /// </summary>
        /// <param name="p_characterID"></param>
        /// <param name="p_stat">leaderboard stat to look up</param>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurred</param>
        public void RefreshMyCharacterLeaderboardRank(
            string p_characterID,
            string p_stat,
            Action<int> p_onSuccessCallback = null,
            Action<string> p_onFailureCallback = null)
        {

            var request = new GetLeaderboardAroundCharacterRequest
            {
            CharacterId = p_characterID,
            StatisticName = p_stat,
            MaxResultsCount = 1
            };

            PlayFabClientAPI.GetLeaderboardAroundCharacter(
                request,
                (GetLeaderboardAroundCharacterResult result) =>
                {
                    if (result.Leaderboard.Count > 0)
                    {
                        var entry = result.Leaderboard.First();

                        if (null != entry)
                        {
                            if (p_onSuccessCallback != null)
                                p_onSuccessCallback.Invoke(entry.Position);

                            return;
                        }
                    }
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke("Leaderboard error");
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Refresh the player's leaderboard rank around a single stat
        /// </summary>
        /// <param name="p_stat">leaderboard stat to look up</param>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurred</param>
        public void RefreshMyPlayerLeaderboardRank(string p_stat,
            Action<int> p_onSuccessCallback = null,
            Action<string> p_onFailureCallback = null)
        {

            PlayFabClientAPI.GetLeaderboardAroundPlayer(
                new GetLeaderboardAroundPlayerRequest { StatisticName = p_stat },
                result =>
                {
                    if (result.Leaderboard.Count > 0)
                    {
                        var entry = result.Leaderboard.First();

                        if (null != entry)
                        {
                            if (p_onSuccessCallback != null)
                                p_onSuccessCallback.Invoke(entry.Position);

                            return;
                        }
                    }
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke("Leaderboard error");
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        // private local cache
        private const int k_MaximumLeaderboardEntries = 10;
        private List<PlayerLeaderboardEntry> m_currentTop10LB = new List<PlayerLeaderboardEntry>();
        private List<PlayerLeaderboardEntry> m_friendsLB = new List<PlayerLeaderboardEntry>();
    }
}