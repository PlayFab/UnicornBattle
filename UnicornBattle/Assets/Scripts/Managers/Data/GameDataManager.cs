using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using UnicornBattle.Controllers;
using UnicornBattle.Models;

namespace UnicornBattle.Managers
{
    /// <summary>
    /// GAME DATA MANAGER
    /// =================
    /// Encapsulates all of the Unicorn Battle Game Data pulled from PlayFab's Title Data
    /// </summary>
    public class GameDataManager : DataManager, ITitleDataLoadable
    {
        public float MinimumInterstitialWait { get { return m_minimumInterstitialWait; } }
        public string CommunityWebsite { get { return m_communityWebsite; } }
        public string AndroidPushSenderId { get { return m_AndroidPushSenderId; } }

        public override void Initialize(MainManager p_manager)
        {
            m_Achievements = new Dictionary<string, UBAchievement>();
            m_Levels = new Dictionary<string, UBLevelData>();
            m_SpellDetails = new Dictionary<string, UBSpellDetail>();
            m_Encounters = new Dictionary<string, Dictionary<string, UBEncounterData>>();

            base.Initialize(p_manager);
        }

        /// <summary>
        /// Stores this classes relevent title data into its member data
        /// </summary>
        /// <param name="p_titleData">The title data Dictionary</param>
        public void LoadTitleData(Dictionary<string, string> p_titleData)
        {
            ExtractJsonTitleData(p_titleData, "Achievements", ref m_Achievements);
            ExtractJsonTitleData(p_titleData, "Levels", ref m_Levels);
            ExtractJsonTitleData(p_titleData, "Spells", ref m_SpellDetails);
            ExtractJsonTitleData(p_titleData, "MinimumInterstitialWait", ref m_minimumInterstitialWait);
            ExtractJsonTitleData(p_titleData, "UseCDN", ref m_UseCDN);
            if (m_UseCDN == 0)
            {
                GameController.Instance.cdnController.useCDN = false;
            }
            else
            {
                GameController.Instance.cdnController.useCDN = true;
            }

            m_AndroidPushSenderId = GlobalStrings.DEFAULT_ANDROID_PUSH_SENDER_ID;
            if (p_titleData.ContainsKey("AndroidPushSenderId"))
                m_AndroidPushSenderId = p_titleData["AndroidPushSenderId"];

            if (p_titleData.ContainsKey("CommunityWebsite"))
                m_communityWebsite = p_titleData["CommunityWebsite"];

            // cleanup
            foreach (var pair in m_Levels)
            {
                pair.Value.Name = pair.Key;
            }
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

            var request = new GetTitleDataRequest { Keys = GlobalStrings.InitTitleKeys };

            PlayFabClientAPI.GetTitleData(request,
                (GetTitleDataResult result) =>
                {
                    LoadTitleData(result.Data);

                    DataRefreshed();

                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke("Data refreshed");
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Get an array of all the Achievement data
        /// </summary>
        /// <returns>an array of UBAchievement objects, or NULL if no data</returns>
        public UBAchievement[] GetAllAchievements()
        {
            if (null != m_Achievements)
                return m_Achievements.Values.ToArray();
            else
                return null;
        }

        /// <summary>
        /// Get a single achievement given an ID
        /// </summary>
        /// <param name="p_id">The achievement ID</param>
        /// <returns>a UBAchievement object, or NULL if no data</returns>
        public UBAchievement GetAchievement(string p_id)
        {
            if (m_Achievements.ContainsKey(p_id))
                return m_Achievements[p_id];
            else
                return null;
        }

        /// <summary>
        /// Get Event Associated Level Names
        /// </summary>
        /// <param name="eventKey"></param>
        /// <returns></returns>
        public string[] GetEventAssociatedLevelNames(string eventKey)
        {
            var levelNames = new List<string>();
            foreach (var eachPair in m_Levels)
                if (eachPair.Value.RestrictedToEventKey == eventKey)
                    levelNames.Add(eachPair.Key);
            return levelNames.ToArray();
        }

        /// <summary>
        /// are there any levels associated with an event?
        /// </summary>
        /// <param name="p_eventKey">event key</param>
        /// <returns>true, if there is; false otherwise</returns>
        public bool HasAnyEventAssociatedLevels(string p_eventKey)
        {
            foreach (var l_level in m_Levels.Values)
            {
                if (l_level.RestrictedToEventKey == p_eventKey)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all Level Data
        /// </summary>
        /// <returns></returns>
        public UBLevelData[] GetAllLevelData()
        {
            return m_Levels.Values.ToArray();
        }

        /// <summary>
        /// Gets all level names
        /// </summary>
        /// <returns></returns>
        public string[] GetAllLevelNames()
        {
            return m_Levels.Keys.ToArray();
        }

        /// <summary>
        /// Gets UBSpellDetail for a particular spell
        /// </summary>
        /// <param name="p_spellName"></param>
        /// <returns></returns>
        public UBSpellDetail GetSpellDetail(string p_spellName)
        {
            if (m_SpellDetails.ContainsKey(p_spellName))
                return m_SpellDetails[p_spellName];
            else
                return null;
        }

        /// <summary>
        /// Get Encounters List (the main gameplay)
        /// </summary>
        /// <param name="encounters"></param>
        /// <param name="p_onSuccessCallback"></param>
        /// <param name="p_onFailureCallback"></param>
        public void RefreshEncounterLists(List<string> encounters,
            System.Action<string> p_onSuccessCallback,
            System.Action<string> p_onFailureCallback)
        {
            var request = new GetTitleDataRequest { Keys = encounters };
            var JsonUtil = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);

            PlayFabClientAPI.GetTitleData(
                request,
                (GetTitleDataResult result) =>
                {
                    m_Encounters.Clear();

                    foreach (var item in encounters)
                        if (result.Data.ContainsKey(item))
                            m_Encounters.Add(item, JsonUtil.DeserializeObject<Dictionary<string, UBEncounterData>>(result.Data[item]));

                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke("Encounters Loaded!");
                },
                (PlayFabError error) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(error.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Get the encounter pool
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        public Dictionary<string, UBEncounterData> getEncounterPool(string p_id)
        {
            if (m_Encounters.ContainsKey(p_id))
                return m_Encounters[p_id];
            else
                return new Dictionary<string, UBEncounterData>();
        }

        /// <summary>
        /// Are there any encounters?
        /// </summary>
        /// <returns>true, if there are encounters; false otherwise</returns>
        public bool HasAnyEncounters()
        {
            return m_Encounters.Any();
        }

        // private local data populated from PlayFab
        private float m_minimumInterstitialWait;
        private string m_communityWebsite;
        private int m_UseCDN;
        private string m_AndroidPushSenderId;
        private Dictionary<string, UBAchievement> m_Achievements;
        private Dictionary<string, UBLevelData> m_Levels;
        private Dictionary<string, UBSpellDetail> m_SpellDetails;
        private Dictionary<string, Dictionary<string, UBEncounterData>> m_Encounters;
    }
}