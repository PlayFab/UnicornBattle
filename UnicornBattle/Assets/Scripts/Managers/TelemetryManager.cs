using System.Collections.Generic;
using System.Collections.Specialized;
using PlayFab;
using PlayFab.ClientModels;

namespace UnicornBattle.Managers
{
    /// <summary>
    /// TELEMETRY MANAGER
    /// =================
    /// A simple Telemetry/Analytics Manager class
    /// - useful to send custom events to Playstream
    /// </summary>
    public static class TelemetryManager
    {
        /// <summary>
        /// Record a session-type custom event
        /// </summary>
        /// <param name="p_eventName">Name of the event in the form subject_verb_object</param>
        /// <param name="p_data">A list of key-value pairs of the data related to this event</param>
        public static void RecordSessionEvent(string p_eventName, Dictionary<string, object> p_data)
        {
            var request = new WriteClientCharacterEventRequest()
            {
                Body = p_data,
                EventName = p_eventName,
            };

            PlayFabClientAPI.WriteCharacterEvent(request, null, null);
        }

        /// <summary>
        /// Record a game-type custom event
        /// </summary>
        /// <param name="p_eventName">Name of the event in the form subject_verb_object</param>
        /// <param name="p_data">A list of key-value pairs of the data related to this event</param>
        public static void RecordGameEvent(string p_eventName, Dictionary<string, object> p_data)
        {
            var request = new WriteTitleEventRequest()
            {
                Body = p_data,
                EventName = p_eventName,
            };

            PlayFabClientAPI.WriteTitleEvent(request, null, null);
        }

        /// <summary>
        /// Record a player-type custom event
        /// </summary>
        /// <param name="p_eventName">Name of the event as a string</param>
        /// <param name="p_data">A list of key-value pairs of the data related to this event</param>
        public static void RecordPlayerEvent(string p_eventName, Dictionary<string, object> p_data)
        {
            var request = new WriteClientPlayerEventRequest()
            {
                Body = p_data,
                EventName = p_eventName,
            };

            PlayFabClientAPI.WritePlayerEvent(request, null, null);
        }

        /// <summary>
        /// Record a player-type custom event
        /// </summary>
        /// <param name="p_event">Name of the event as a TelemetryEvent</param>
        /// <param name="p_data">A list of key-value pairs of the data related to this event</param>
        public static void RecordPlayerEvent(TelemetryEvent p_event, Dictionary<string, object> p_data)
        {
            RecordPlayerEvent(p_event.ToString(), p_data);
        }

        public static void RecordScreenViewed(TelemetryScreenId id)
        {
            if (TelemetryScreenId.Untracked == id) return;
            RecordPlayerEvent(TelemetryEvent.Client_ScreenViewed, new Dictionary<string, object> { { "screen", id.ToString() } });
        }
    }

    //	Used for sending analytics data back into PlayFab
    public enum TelemetryEvent
    {
        none = 0,
        Client_LevelUp,
        Client_LevelComplete,
        Client_PlayerDied,
        Client_BossKill,
        Client_UnicornFreed,
        Client_StoreVisit,
        Client_SaleClicked,
        Client_BattleAborted,
        Client_RegisteredAccount,
        Client_FriendAdded,
        Client_AdWatched,
        Client_ScreenViewed,
    }

    public enum TelemetryScreenId
    {
        Untracked = 0,
        Main,
        Settings,
        Character,
        Store,
        Leaders,
        Inventory,
        BattleResults,
        LoadingScreen,
    }
}