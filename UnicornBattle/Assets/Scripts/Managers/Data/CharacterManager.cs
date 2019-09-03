using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using UnicornBattle.Models;
using UnityEngine;

namespace UnicornBattle.Managers
{
    public enum CharacterClassTypes { Bucephelous = 0, Nightmare = 1, PegaZeus = 2 }

    /// <summary>
    /// CHARACTER MANAGER
    /// =================
    /// Manages the Character data for the game
    ///  - separate from player manager because not all games use characters that need their own separate data from the player
    ///  - this class encapsulates all specific player data and general game data used for characters
    ///  - refresh methods refresh the local data, getters protect and return the local data
    /// </summary>
    public class CharacterManager : DataManager, IPlayerDataRefreshable, ITitleDataLoadable
    {
        /// <summary>
        /// Initialize the Manager
        /// </summary>
        /// <param name="p_manager"></param>
        public override void Initialize(MainManager p_manager)
        {
            m_playerCharacters = new List<CharacterResult>();
            m_playerCharacterData = new Dictionary<string, UBCharacterData>();
            m_characterAchievements = new Dictionary<string, List<string>>();
            m_characterStatistics = new Dictionary<string, Dictionary<string, int>>();

            m_Classes = new Dictionary<string, UBCharacterClassDetail>();
            m_CharacterLevelRamp = new Dictionary<string, int>();

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
            System.Action<string> p_onSuccessCallback,
            System.Action<string> p_onFailureCallback)
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

            PlayFabClientAPI.GetAllUsersCharacters(
                new ListUsersCharactersRequest(),
                (ListUsersCharactersResult result) =>
                {

                    m_playerCharacters.Clear();
                    m_playerCharacterData.Clear();
                    m_characterAchievements.Clear();

                    if (result.Characters.Count == 0)
                    {
                        if (null != p_onSuccessCallback)
                            p_onSuccessCallback.Invoke("No Characters");
                        return;
                    }

                    foreach (var eachChar in result.Characters)
                        m_playerCharacters.Add(eachChar);

                    ChainGetNextCharacterData(0,
                        (s) =>
                        {
                            DataRefreshed();
                            if (null != p_onSuccessCallback)
                                p_onSuccessCallback.Invoke("data refreshed");
                        },
                        p_onFailureCallback
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
        /// Loads a list of PlayFab.ClientModels.CharacterResult into the manager's cache
        /// </summary>
        /// <param name="p_characterData"></param>
        public void LoadCharacterList(List<CharacterResult> p_characterData)
        {
            m_playerCharacters.Clear();
            foreach (var l_char in p_characterData)
            {
                m_playerCharacters.Add(l_char);
            }
        }

        /// <summary>
        /// Using a dictionary of JSON data, load it into the manager's cache
        /// - this method does not contact PlayFab for the data, use Refresh() to get data from PlayFab
        /// - you could use this method to load from your own stored data
        /// </summary>
        /// <param name="p_titleData"></param>
        public void LoadTitleData(Dictionary<string, string> p_titleData)
        {
            ExtractJsonTitleData(p_titleData, "CharacterLevelRamp", ref m_CharacterLevelRamp);
            ExtractJsonTitleData(p_titleData, "Classes", ref m_Classes);
            ExtractJsonTitleData(p_titleData, "StartingCharacterSlots", ref StartingCharacterSlots);
        }

        private void ChainGetNextCharacterData(int index,
            System.Action<string> p_onSuccessCallback,
            System.Action<string> p_onFailureCallback)
        {
            if (index >= m_playerCharacters.Count)
            {
                if (null != p_onSuccessCallback)
                    p_onSuccessCallback.Invoke(string.Empty);
                return;
            }

            RefreshCharacterDataById(
                m_playerCharacters[index].CharacterId,
                (s) => { ChainGetNextCharacterData(++index, p_onSuccessCallback, p_onFailureCallback); },
                p_onFailureCallback
            );
        }

        /// <summary>
        /// Refresh Character Data by ID
        /// </summary>
        /// <param name="p_characterId">character ID to request refreshed data</param>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurs</param>
        private void RefreshCharacterDataById(string p_characterId,
            System.Action<string> p_onSuccessCallback,
            System.Action<string> p_onFailureCallback)
        {
            var l_playerMgr = mainManager.getPlayerManager();
            if (null == l_playerMgr)
            {
                if (null != p_onFailureCallback)
                    p_onFailureCallback.Invoke("PlayerManager is null");
                return;
            }

            var l_JsonUtil = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);

            var l_request = new GetCharacterDataRequest
            {
                PlayFabId = l_playerMgr.PlayerID,
                CharacterId = p_characterId,
                Keys = new List<string> { "CharacterData", "Achievements" }
            };

            PlayFabClientAPI.GetCharacterReadOnlyData(
                l_request,
                (GetCharacterDataResult r) =>
                {
                    if (r.Data.ContainsKey("Achievements"))
                    {
                        var l_achievementList = l_JsonUtil.DeserializeObject<List<string>>(r.Data["Achievements"].Value);

                        if (m_characterAchievements.ContainsKey(r.CharacterId))
                            m_characterAchievements[r.CharacterId] = l_achievementList;
                        else
                            m_characterAchievements.Add(r.CharacterId, l_achievementList);
                    }
                    if (r.Data.ContainsKey("CharacterData"))
                    {
                        var l_charData = l_JsonUtil.DeserializeObject<UBCharacterData>(r.Data["CharacterData"].Value);

                        if (m_playerCharacterData.ContainsKey(r.CharacterId))
                            m_playerCharacterData[r.CharacterId] = new UBCharacterData(l_charData);
                        else
                            m_playerCharacterData.Add(r.CharacterId, new UBCharacterData(l_charData));
                    }

                    PlayFabClientAPI.GetCharacterStatistics(
                        new GetCharacterStatisticsRequest { CharacterId = p_characterId },
                        (GetCharacterStatisticsResult r2) =>
                        {
                            var l_charStats = new Dictionary<string, int>(r2.CharacterStatistics);

                            if (m_characterStatistics.ContainsKey(p_characterId))
                            {
                                m_characterStatistics[p_characterId] = l_charStats;
                            }
                            else
                            {
                                m_characterStatistics.Add(p_characterId, l_charStats);
                            }
                            if (null != p_onSuccessCallback)
                                p_onSuccessCallback.Invoke("success");
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
        /// Get Character Data by ID from the local cache
        /// </summary>
        /// <param name="p_characterId">The id of the character you want the data for</param>
        /// <returns>a UB_CharacterData object</returns>
        public UBSavedCharacter GetUBSavedCharacter(string p_characterId)
        {
            if (string.IsNullOrEmpty(p_characterId))
            {
                Debug.LogError(GetType().ToString() + ".GetUBSavedCharacter() : p_characterID is null or empty");
                return null;
            }
            CharacterResult l_character = m_playerCharacters.Where((r) => { return r.CharacterId == p_characterId; }).FirstOrDefault();

            if (null != l_character)
            {
                return new UBSavedCharacter(
                    p_result: l_character,
                    p_details: m_Classes[l_character.CharacterType],
                    p_data: m_playerCharacterData[l_character.CharacterId]
                );
            }

            return null;
        }

        ///  <summary>
        /// Checks if a character has an achievement
        /// </summary>
        /// <param name="p_charId"></param>
        /// <param name="p_achievementId"></param>
        /// <returns></returns>
        public bool DoesCharacterHaveAchievement(string p_charId, string p_achievementId)
        {
            if (string.IsNullOrEmpty(p_achievementId) || string.IsNullOrEmpty(p_charId))
                return false;

            List<string> achievements;
            m_characterAchievements.TryGetValue(p_charId, out achievements);
            if (achievements == null || achievements.Count == 0)
                return false;
            return achievements.Any(i => { return string.Equals(i, p_achievementId); });
        }

        /// <summary>
        /// Get a dictionary of all character statictics for a particular character id
        /// </summary>
        /// <param name="p_characterId">Id of the character</param>
        /// <returns>the Dictionary of all the stats; null if character id not found</returns>
        public Dictionary<string, int> GetCharacterStatistics(string p_characterId)
        {

            if (m_characterStatistics.ContainsKey(p_characterId))
            {
                return m_characterStatistics[p_characterId];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Update the server with new character statistics
        /// </summary>
        /// <param name="p_characterId">ID of the character to update</param>
        /// <param name="p_updates">Dictionary of values to post to the character's stats</param>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurs</param>
        public void UpdateCharacterStatistics(string p_characterId,
            Dictionary<string, int> p_updates,
            System.Action p_onSuccessCallback = null,
            System.Action<string> p_onFailureCallback = null)
        {
            Dictionary<string, int> l_activeStats = GetCharacterStatistics(p_characterId);

            foreach (var l_each in p_updates)
            {
                int temp;
                l_activeStats.TryGetValue(l_each.Key, out temp);
                l_activeStats[l_each.Key] = temp + l_each.Value;
            }

            var l_request = new ExecuteCloudScriptRequest
            {
                FunctionName = "SetCharacterStats",
                FunctionParameter = new { characterId = p_characterId, statistics = l_activeStats },
                GeneratePlayStreamEvent = true
            };
            PlayFabClientAPI.ExecuteCloudScript(
                l_request,
                (ExecuteCloudScriptResult result) =>
                {
                    if (null != result.Error)
                    {
                        if (null != p_onFailureCallback)
                            p_onFailureCallback.Invoke(result.Error.Message);
                    }
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
        /// Gets the Player Character List
        /// </summary>
        /// <returns>a list</returns>
        public UBSavedCharacter[] GetAllSavedCharacters()
        {
            List<UBSavedCharacter> l_savedChars = new List<UBSavedCharacter>();

            foreach (var l_character in m_playerCharacters)
            {
                l_savedChars.Add(new UBSavedCharacter(
                    p_result: l_character,
                    p_details: m_Classes[l_character.CharacterType],
                    p_data: m_playerCharacterData[l_character.CharacterId]
                ));
            }

            return l_savedChars.ToArray();
        }

        /// <summary>
        /// Does a character with p_id exist?
        /// </summary>
        /// <param name="p_id">The id of the character</param>
        /// <returns>TRUE, if the character exists; FALSE, otherwise</returns>
        public bool DoesCharacterExist(string p_id)
        {
            if (string.IsNullOrEmpty(p_id))
            {
                return false;
            }
            return m_playerCharacters.Any((c) => { return c.CharacterId == p_id; });
        }

        /// <summary>
        /// Create a new character on the server
        /// </summary>
        /// <param name="p_name">Character's name</param>
        /// <param name="p_catalogCode">Character's catalog code</param>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurs</param>
        public void CreateNewCharacter(string p_name,
            string p_catalogCode,
            System.Action<string> p_onSuccessCallback = null,
            System.Action<string> p_onFailureCallback = null)
        {
            var l_request = new ExecuteCloudScriptRequest
            {
            FunctionName = "CreateCharacter",
            FunctionParameter = new { catalogCode = p_catalogCode, characterName = p_name }
            };
            PlayFabClientAPI.ExecuteCloudScript(
                l_request,
                (ExecuteCloudScriptResult result) =>
                {
                    if (null != result.Error)
                    {
                        if (null != p_onFailureCallback)
                            p_onFailureCallback.Invoke(result.Error.Message);
                        return;
                    }

                    if ((bool) result.FunctionResult)
                    {
                        FlagAsDirty();
                        if (null != p_onSuccessCallback)
                            p_onSuccessCallback.Invoke("New Character Added");
                    }
                    else
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke("Error Creating Character: " + result.Logs.ToString());
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        ///  <summary>
        /// Deletes a character from the server
        /// </summary>
        /// <param name="p_charId">character's id</param>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurs</param>
        public void DeleteCharacter(string p_charId, System.Action<string> p_onSuccessCallback = null, System.Action<string> p_onFailureCallback = null)
        {
            var l_playerMgr = mainManager.getPlayerManager();
            if (null == l_playerMgr) return;

            var l_request = new ExecuteCloudScriptRequest
            {
                FunctionName = "DeleteCharacter",
                FunctionParameter = new { characterId = p_charId }
            };
            PlayFabClientAPI.ExecuteCloudScript(
                l_request,
                (ExecuteCloudScriptResult result) =>
                {
                    if (null != result.Error)
                    {
                        if (null != p_onFailureCallback)
                            p_onFailureCallback.Invoke(result.Error.Message);
                        return;
                    }
                    if ((bool) result.FunctionResult)
                    {
                        FlagAsDirty();

                        if (null != p_onSuccessCallback)
                            p_onSuccessCallback.Invoke("Character Deleted");
                    }
                    else
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke("Error Deleting Character: " + result.Logs.ToString());
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Clears the active character
        /// </summary>
        public void ActiveCharacterWasCleared()
        {
            if (m_characterAchievements != null) m_characterAchievements.Clear();
            if (m_characterStatistics != null) m_characterStatistics.Clear();
            FlagAsDirty();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_characterID"></param>
        /// <returns></returns>
        public UBCharacterData GetUBCharacterData(string p_characterID)
        {
            if (m_playerCharacterData.ContainsKey(p_characterID))
            {
                return m_playerCharacterData[p_characterID];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get the most recently used character ID
        /// </summary>
        /// <returns></returns>
        public string GetMostRecentlyUsedCharacterId()
        {
            var l_playerMgr = mainManager.getPlayerManager();
            if (null == l_playerMgr) return string.Empty;

            return PlayerPrefs.GetString($"MRU_{l_playerMgr.PlayerID}", string.Empty);
        }

        /// <summary>
        /// Gets the UBCharacterClassDetail for a specific class name
        /// </summary>
        /// <param name="p_className"></param>
        /// <returns></returns>
        public UBCharacterClassDetail GetCharacterClassDetail(string p_className)
        {
            if (m_Classes.ContainsKey(p_className))
            {
                return m_Classes[p_className];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get all character class details stored locally
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, UBCharacterClassDetail> GetAllCharacterClassDetails()
        {
            return m_Classes;
        }

        /// <summary>
        /// Count the number of character classes stored locally
        /// </summary>
        /// <returns></returns>
        public int CountCharacterClasses()
        {
            return m_Classes.Count;
        }

        /// <summary>
        /// get a level ramp by level name
        /// </summary>
        /// <param name="p_levelName">Name of the level</param>
        /// <returns></returns>
        public int GetLevelRamp(string p_levelName)
        {
            if (m_CharacterLevelRamp.ContainsKey(p_levelName))
                return m_CharacterLevelRamp[p_levelName];
            else
                return -1;
        }

        /// <summary>
        /// Get all level ramps stored locally
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetAllLevelRamps()
        {
            return m_CharacterLevelRamp;
        }

        // private character Data
        private List<CharacterResult> m_playerCharacters;
        private Dictionary<string, UBCharacterData> m_playerCharacterData;
        private Dictionary<string, List<string>> m_characterAchievements;
        private Dictionary<string, Dictionary<string, int>> m_characterStatistics;

        // Title Data
        private Dictionary<string, UBCharacterClassDetail> m_Classes;
        private Dictionary<string, int> m_CharacterLevelRamp;
        public int StartingCharacterSlots;
    }
}