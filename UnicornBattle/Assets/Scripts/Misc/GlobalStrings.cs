using System.Collections.Generic;

public static class GlobalStrings
{
    // Strings displayed to the user
    public const string FB_LOGIN_MSG = "\n Facebook Login Found";
    public const string DEVICE_ID_LOGIN_MSG = "\n Device ID Found";
    public const string DEVICE_ID_CREATE_MSG = "\n No Login Found (Creating New Account with Device ID)";
    public const string TEST_LOGIN_MSG = "\n Unsupported Platform (Using Facebook in Test Mode)";
    public const string TEST_LOGIN_PROMPT = "Developer Login";
    public const string AUTO_STATUS_MSG = "Checking for Supported Platforms";
    public const string STATUS_PROMPT = "Status";
    public const string NET_FAIL_MSG = "Your device has poor or no internet connectivity. Please check your connection and try again.";
    public const string REGISTER_FAIL_MSG = "You must fill out all fields, and passwords must match.";
    public const string ENCOUNTER_AMBUSH_MSG = "You were ambushed by: {0}";
    public const string ENCOUNTER_ENEMY_MSG = "You encountered: {0}";
    public const string ENCOUNTER_HERO_MSG = "You found someone in distress: {0}";
    public const string ENCOUNTER_VENDOR_MSG = "You found a vendor: {0}";
    public const string LOGOUT_BTN_TXT = "Return to Log In";
    public const string CREATE_BTN_TXT = "Create New Account";
    public const string PAGE_NUMBER_MSG = "Page {0} of {1}";
    public const string ACT_STATUS_REG_MSG = "Registered";
    public const string ACT_STATUS_UNREG_MSG = "Unregistered";
    public const string LINK_FB_BTN_MSG = "Link to Facebook";
    public const string UNLINK_FB_BTN_MSG = "Unlink from Facebook";
    public const string LOGOUT_BTN_MSG = "Logged Out";
    public const string LOADING_MSG = "Loading";
    public const string COMPLETE_MSG = "Complete!";
    public const string GRANT_CATALOG_ERR_MSG = "Granted item not found in catalog.";
    public const string UNLOCKED_MSG = "Unlocked";
    public const string NO_EVENTS_MSG = "There are no sales or events today. Please check again tomorrow!";
    public const string INV_WINDOW_TITLE = "Inventory ({0})";

    public const string DEL_CHAR_PROMPT = "Delete Character";
    public const string DEL_CHAR_MSG = "Are you sure you want to permanently delete {0}?";
    public const string PUSH_NOTIFY_PROMPT = "Push Notification";
    public const string PUSH_NOTIFY_MSG = "Please verify push notifications are disabled in your device's settings.";
    public const string TOGGLE_PROMPT = "Reminder";
    public const string TOGGLE_MSG = "You can access these options any time from the settings menu.";
    public const string CONFIRM_UNLINK_PROMPT = "Unlink Facebook";
    public const string CONFIRM_UNLINK_MSG = "Your account is not registered. Unlinking your account could make data retrieval for this character impossible.";
    public const string RECOVER_EMAIL_PROMPT = "Confirm Action";
    public const string RECOVER_EMAIL_MSG = "You should receive an email soon with instructions on how to reset your password.";
    public const string QUIT_LEVEL_PROMPT = "Quit Quest?";
    public const string QUIT_LEVEL_MSG = "Quitting before the completion of a quest will forfeit all found items and experience gains. Are you sure you want to quit?";
    public const string LOGOUT_PROMPT = "Log Out?";
    public const string LOGOUT_MSG = "Are you sure you want to log out?";
    public const string DISPLAY_NAME_PROMPT = "Set Display Name";
    public const string DISPLAY_NAME_MSG = "Set the name that you want others to see on the leaderboards (alpha-numeric characters only).";
    public const string CHAR_NAME_PROMPT = "Set Character Name";
    public const string CHAR_NAME_MSG = "Set your character's name (alpha-numeric only).";
    public const string FRIEND_TAG_PROMPT = "Set Custom Friend Tags";
    public const string FRIEND_TAG_MSG = "Tag your friends with custom notes.";
    public const string ADD_FRIEND_PROMPT = "Add Friend";
    public const string ADD_FRIEND_MSG = "Enter your friends {0}";

    public const string FRIEND_SELECTOR_PROMPT = "Add Friend Using";
    public const string STAT_SELECTOR_PROMPT = "Select Stat";
    public const string CATEGORY_SELECTOR_PROMPT = "Select Category";
    public const string QUEST_SELECTOR_PROMPT = "Select Quest";

    // Programatic strings
    public const string PrimaryCatalogName = "CharacterClasses";
    public static readonly List<string> InitTitleKeys = new List<string> { "Achievements", "ActiveEventKeys", "AndroidPushSenderId", "CharacterLevelRamp", "Classes", "CommunityWebsite", "Events", "Levels", "MinimumInterstitialWait", "Spells", "StandardStores", "StartingCharacterSlots" };
    public const string DEFAULT_UB_TITLE_ID = "A5F3";
	public const string DEFAULT_ANDROID_PUSH_SENDER_ID = "494923569376";
    public const string FB_PREF_KEY = "LinkedFacebook";
    public const string DEVICE_PREF_KEY = "LastDeviceIdUsed";
    public const string ENCOUNTER_CREEP = "Creep";
    public const string ENCOUNTER_STORE = "Store";
    public const string ENCOUNTER_HERO = "Hero";
    public const string GOLD_CURRENCY = "AU";
    public const string GEM_CURRENCY = "GM";
    public const string HEART_CURRENCY = "HT";
    public const string REAL_MONEY_CURRENCY = "RM";

    // Events
    public const string QUEST_START_EVENT = "Starting Quest";
    public const string QUEST_COMPLETE_EVENT = "Quest Complete";
    public const string PLAYER_TURN_BEGIN_EVENT = "Player Turn Begins";
    public const string PLAYER_TURN_END_EVENT = "Player Turn Ends";
    public const string ENEMY_TURN_BEGIN_EVENT = "Enemy Turn Begins";
    public const string ENEMY_TURN_END_EVENT = "Enemy Turn Ends";
    public const string NEXT_ENCOUNTER_EVENT = "A new challenger";
    public const string BOSS_BATTLE_EVENT = "Starting Boss Battle";
    public const string ACT_COMPLETE_EVENT = "Act Complete";
    public const string PLAYER_DIED_EVENT = "Player Died";
    public const string PLAYER_RESPAWN_EVENT = "Respawn";
    public const string OUTRO_ENEMY_DIED_EVENT = "EnemyDied, OutroEncounter";
    public const string OUTRO_PLAYER_DEATH_EVENT = "Player, OutroEncounter";

    // Art
    public const string DEFAULT_ICON = "Default";
    public const string BRONZE_KEY_ICON = "BronzeKey";
    public const string DARKSTONE_LOCK_ICON = "DarkStone_Lock";
    public const string DARKSTONE_STAR_ICON = "DarkStone_Star";
    public const string SMOKE_EFFECT_1 = "CFXM_SmokePuffsAlt+Text Large";
    public const string SMOKE_EFFECT_2 = "CFXM_SmokePuffsAltLarge";

    public static readonly Dictionary<DialogCanvasController.InventoryFilters, string> INV_FILTER_DISPLAY_NAMES = new Dictionary<DialogCanvasController.InventoryFilters, string>()
    {
        {DialogCanvasController.InventoryFilters.AllItems, "All Items"},
        {DialogCanvasController.InventoryFilters.UsableInCombat, "Combat Items"},
        {DialogCanvasController.InventoryFilters.Keys, "Keys"},
        {DialogCanvasController.InventoryFilters.Containers, "Containers"},
    };
}
