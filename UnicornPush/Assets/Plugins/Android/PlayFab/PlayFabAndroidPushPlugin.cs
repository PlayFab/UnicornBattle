// #define TESTING

#if TESTING || !DISABLE_PLAYFABCLIENT_API && UNITY_ANDROID && !UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace PlayFab.Android
{
    /// <summary>
    /// Call Setup before login, if androidPushSenderId is available before login
    ///   -otherwise-
    /// Use Init before login, if you store androidPushSenderId in title data, and you need to log in before you can provide androidPushSenderId
    /// Call Setup after login with androidPushSenderId
    /// </summary>
    public class PlayFabAndroidPushPlugin : MonoBehaviour
    {
        public enum PushSetupStatus
        {
            GameObjectInitialized,
            AndroidPluginInitialized,
            RegistrationReady,
            TokenReceived,
            WaitingForManualTrigger,
            PlayFabRegisterApiSuccess,
            RegisteredWithPlayFab,
            ReceivedMessage,
            Unloading
        }

        /// <summary>
        /// Change this to true if you want debug level logging into Unity for PlayFab Android Push Notifications
        /// </summary>
        public static bool LogMessagesToUnity = true;
        /// <summary>
        /// Change this to true if you want to get a confirmation message after registering for push notifications
        /// </summary>
        public static bool SendConfirmationMessage = false;
        /// <summary>
        /// Change this to true if you want to get a confirmation message after registering for push notifications
        /// </summary>
        public static string ConfirmationMessage = "Push Notifications Registered!";
        /// <summary>
        /// Add a callback here to receive every push notification text when it arrives
        /// </summary>
        public static event Action<PlayFabNotificationPackage> OnGcmMessage;
        /// <summary>
        /// Add a callback here to receive a status update about every state-change in the push notification registration setup
        /// </summary>
        public static event Action<PushSetupStatus> OnGcmSetupStep;
        /// <summary>
        /// Add a callback here to receive every debug level log message
        /// </summary>
        public static event Action<string> OnGcmLog;

        #region Private Stuff
        private const string GAME_OBJECT_NAME = "_PlayFabGO"; // This name is defined in the Android Java Plugin, and shouldn't be changed

        private static string _androidPushSenderId = null; // Set when a developer calls Setup()
        private static HashSet<string> _androidPushTokens = null; // Existing push registrations - Retrieved at login if Profile information is requested properly
        private static string _myPushToken = null; // Set internally once java plugin is activated and working
        private static Action<string, bool, string> _registerForAndroidPushApi; // Set internally after any login
        private static PlayFabAndroidPushPlugin _singletonInstance;
        private static AndroidJavaClass _playFabGcmClass;
        private static AndroidJavaClass _androidPlugin;
        private static AndroidJavaClass _playServicesUtils;
        private static AndroidJavaClass _notificationSender;
        private static AndroidJavaClass _notificationPkgClass;
        private static AndroidJavaClass _clsUnity;
        private static AndroidJavaObject _objActivity;

        private static void LoadPlugin()
        {
            _playFabGcmClass = new AndroidJavaClass("com.playfab.unityplugin.GCM.PlayFabGoogleCloudMessaging");
            _playServicesUtils = new AndroidJavaClass("com.playfab.unityplugin.GCM.PlayServicesUtils");
            _notificationSender = new AndroidJavaClass("com.playfab.unityplugin.GCM.PlayFabNotificationSender");
            _notificationPkgClass = new AndroidJavaClass("com.playfab.unityplugin.GCM.PlayFabNotificationPackage");
            _clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            _objActivity = _clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
            _androidPlugin = new AndroidJavaClass("com.playfab.unityplugin.PlayFabUnityAndroidPlugin");
            _androidPlugin.CallStatic("initGCM", _androidPushSenderId, Application.productName); // Start the PlayFab push plugin service

            _singletonInstance.PostStatusMessage(PushSetupStatus.AndroidPluginInitialized);
        }
        #endregion Private Stuff

        #region PlayFabAndroidPushPlugin Setup Functions
        /// <summary>
        /// Init should be called before PlayFab Login, if you do not yet have the androidPushSenderId
        /// </summary>
        public static void Init()
        {
            if (_singletonInstance != null) // Check if we have already created this object.
                return;

            var playfabGo = GameObject.Find(GAME_OBJECT_NAME); // Create GameObject and place it in the scene.
            if (playfabGo == null)
            {
                playfabGo = new GameObject(GAME_OBJECT_NAME);
                DontDestroyOnLoad(playfabGo);
            }
            _singletonInstance = playfabGo.GetComponent<PlayFabAndroidPushPlugin>() ?? playfabGo.AddComponent<PlayFabAndroidPushPlugin>();
            _singletonInstance.PostStatusMessage(PushSetupStatus.GameObjectInitialized);
        }

        /// <summary>
        /// Setup should be called anytime before, or after PlayFab Login, when you have the androidPushSenderId
        /// </summary>
        public static void Setup(string androidPushSenderId)
        {
            Init();

            if (string.IsNullOrEmpty(_androidPushSenderId) || _androidPushSenderId == androidPushSenderId)
                _androidPushSenderId = androidPushSenderId; // Save the sender id for after login
            else
                throw new Exception("PlayFab Push Plugin Error: Cannot change the senderId once set");

            if (_registerForAndroidPushApi != null && !string.IsNullOrEmpty(_androidPushSenderId))
                LoadPlugin();
            else
                _singletonInstance.GCMLog("PlayFab: Android Push Ready, Log into PlayFab to activate Push Notifications");
        }

        public static void TriggerManualRegistration(bool? sendMessageNow = null, string message = null)
        {
            var msgSb = new StringBuilder();
            if (_registerForAndroidPushApi == null || _androidPushTokens == null)
                msgSb.Append("You must log in before calling TriggerManualRegistration()\n");
            if (string.IsNullOrEmpty(_androidPushSenderId))
                msgSb.Append("You must call Setup(androidPushSenderId) before calling TriggerManualRegistration()\n");
            if (string.IsNullOrEmpty(_myPushToken))
                msgSb.Append("Android Push Token not available\n");
            if (_androidPushTokens != null && _androidPushTokens.Contains(_myPushToken))
                msgSb.Append("Already registered, no need to call TriggerManualRegistration\n");

            while (msgSb.Length > 0 && msgSb[msgSb.Length - 1] == '\n')
                msgSb.Length -= 1;
            if (msgSb.Length > 0)
            {
                _singletonInstance.GCMLog(msgSb.ToString());
                return;
            }

            // Trigger manual registration
            if (sendMessageNow == null)
                sendMessageNow = SendConfirmationMessage;
            if (message == null)
                message = ConfirmationMessage;

            _registerForAndroidPushApi(_myPushToken, sendMessageNow.Value, message);
        }
        #endregion PlayFabAndroidPushPlugin Setup Functions

        #region Push Scheduling Functions
        /// <summary>
        /// Set up a push notification to display on this device
        /// </summary>
        public static void ScheduleNotification(string message, DateTime date, ScheduleTypes scheduleType = ScheduleTypes.ScheduledLocal, int id = 0)
        {
            var dateString = date.ToString(PlayFabNotificationPackage.SUPPORTED_PLUGIN_TIMESTAMP_FORMATS[0]);
            if (scheduleType == ScheduleTypes.ScheduledUtc)
                dateString = dateString + "Z";
            var package = _notificationSender.CallStatic<AndroidJavaObject>("createNotificationPackage", _objActivity, message, id);
            package.Call("setScheduleDate", dateString);
            _notificationSender.CallStatic("send", _objActivity, package);
        }

        public static void SendNotificationNow(string message)
        {
            var javaPackage = _notificationSender.CallStatic<AndroidJavaObject>("createNotificationPackage", _objActivity, message, 0);
            _notificationSender.CallStatic("send", _objActivity, javaPackage);
        }

        public static void SendNotification(PlayFabNotificationPackage package)
        {
            var javaPackage = _notificationSender.CallStatic<AndroidJavaObject>("createNotificationPackage", _objActivity, package.Message, package.Id);
            package.ToJava(ref javaPackage);
            _notificationSender.CallStatic("send", _objActivity, javaPackage);
        }

        public static void CancelNotification(string message)
        {
            var javaPackage = _notificationSender.CallStatic<AndroidJavaObject>("createNotificationPackage", _objActivity, message, 0);
            _notificationSender.CallStatic("cancelScheduledNotification", _objActivity, javaPackage);
        }

        public static void CancelNotification(int id)
        {
            var javaPackage = _notificationSender.CallStatic<AndroidJavaObject>("createNotificationPackage", _objActivity, "", id);
            _notificationSender.CallStatic("cancelScheduledNotification", _objActivity, javaPackage);
        }
        #endregion Push Scheduling Functions

        #region Direct Calls to Push Objects
        public static void StopPlugin()
        {
            // Shut down the Java Plugin
            if (_androidPlugin != null)
                _androidPlugin.CallStatic("stopPluginService");

            _singletonInstance.PostStatusMessage(PushSetupStatus.Unloading);
            // Clear my cached variables
            _androidPushSenderId = null; // Forget the senderId so we can set a new one
            _myPushToken = null; // Forget my token for this particular sender
            _androidPushTokens = null; // Forget my other registered tokens
            _registerForAndroidPushApi = null; // Lose my reference to the SDK callback
            // Clear any client callbacks
            OnGcmMessage = null;
            OnGcmSetupStep = null;
            OnGcmLog = null;
            // Clear the Java objects
            _playFabGcmClass = null;
            _androidPlugin = null;
            _playServicesUtils = null;
            _notificationSender = null;
            _clsUnity = null;
            _objActivity = null;
        }

        /// <summary>
        /// By default:
        ///   Every message is delivered to the OnGcmMessage callback if the app is running and in focus
        ///   Every message will also display in the device notification bar
        /// It is possible to hide the notification from the device, if it is properly delivered to OnGcmMessage, by calling
        ///   AlwaysShowOnNotificationBar(false)
        /// </summary>
        public static void AlwaysShowOnNotificationBar(bool alwaysShow = true)
        {
            _androidPlugin.CallStatic("alwaysShowOnNotificationBar", alwaysShow);
        }

        public static bool IsPlayServicesAvailable()
        {
            return _playServicesUtils != null && _playServicesUtils.CallStatic<bool>("isPlayServicesAvailable");
        }
        #endregion Direct Calls to Push Objects

        #region Internal Unity Monobehavior-Messaging Hooks
        private void OnPlayFabLogin(Action<string, bool, string> registerForAndroidPushApi)
        {
            _registerForAndroidPushApi = registerForAndroidPushApi; // Once the java push plugin has done its work, this is the API call which registers for push
            if (!string.IsNullOrEmpty(_androidPushSenderId))
                LoadPlugin();
            else
                GCMLog("PlayFab: Android Push Ready, call PlayFabPluginEventHandler.Setup() to activate Push Notifications");
        }

        private void SetPushRegistrations(HashSet<string> androidPushTokens) // androidPushTokens should not be null
        {
            _androidPushTokens = androidPushTokens;
        }

        private void OnRegisterApiSuccess(string token)
        {
            _myPushToken = token;
            if (_androidPushTokens == null)
                _androidPushTokens = new HashSet<string>();
            _androidPushTokens.Add(_myPushToken);
            PostStatusMessage(PushSetupStatus.PlayFabRegisterApiSuccess);
        }

        private void OnPushRegistrationApiSuccess()
        {
            _registerForAndroidPushApi = null;
            _androidPushSenderId = null;
            PostStatusMessage(PushSetupStatus.RegisteredWithPlayFab);
        }

        private void GCMRegistrationReady(string status)
        {
            bool statusParam;
            if (bool.TryParse(status, out statusParam) && statusParam)
                _playFabGcmClass.CallStatic("getToken");
            PostStatusMessage(PushSetupStatus.RegistrationReady);
        }

        private void GCMRegistered(string token)
        {
            _myPushToken = token;
            PostStatusMessage(PushSetupStatus.TokenReceived);

            // Determine setup failure
            if (string.IsNullOrEmpty(_myPushToken))
            {
                GCMLog("PlayFab: Android Push setup failed, with empty token");
                return;
            }

            // Determine if Login did not provide existing tokens
            if (_androidPushTokens == null)
            {
                GCMLog("PlayFab: Push Registration couldn't be determined, If unregistered, manually trigger ASDF()");
                PostStatusMessage(PushSetupStatus.WaitingForManualTrigger);
                return;
            }

            // Search the tokens to determine if we're already registered
            if (!_androidPushTokens.Contains(_myPushToken))
                _registerForAndroidPushApi(_myPushToken, SendConfirmationMessage, ConfirmationMessage);
            else
                GCMLog("PlayFab: Android Push setup success, already registered");
        }

        private void GCMRegisterError(string error)
        {
            if (LogMessagesToUnity)
                Debug.LogError("PlayFab GCM ERROR: " + error);
            if (OnGcmLog != null)
                OnGcmLog(error);
        }

        private void GCMLog(string message)
        {
            if (LogMessagesToUnity)
                Debug.Log("PlayFab GCM MESSAGE: " + message);
            if (OnGcmLog != null)
                OnGcmLog(message);
        }

        private void GCMMessageReceived(string json)
        {
            Debug.Log("PlayFabGCM: Unity GCMMessageReceived, json: " + json);
            var pkg = _notificationPkgClass.CallStatic<AndroidJavaObject>("fromJson", json);
            Debug.Log("PlayFabGCM: Unity GCMMessageReceived, pkg: " + pkg.Get<string>("Message"));
            var message = PlayFabNotificationPackage.FromJava(pkg);
            Debug.Log("PlayFabGCM: Unity GCMMessageReceived, msg: " + message.Message);
            if (OnGcmMessage != null)
                OnGcmMessage(message);
            PostStatusMessage(PushSetupStatus.ReceivedMessage);
        }

        private void PostStatusMessage(PushSetupStatus status)
        {
            if (OnGcmSetupStep != null)
                OnGcmSetupStep(status);
        }
        #endregion Internal Unity Monobehavior-Event Hooks
    }

    public enum ScheduleTypes
    {
        None,
        ScheduledUtc, // Corresponds to DATE_UTC_FORMAT above
        ScheduledLocal // Corresponds to DATE_LOCAL_FORMAT above
    }

    /// <summary>
    /// c# wrapper that matches the Java native com.playfab.unityplugin.GCM.PlayFabNotificationPackage
    /// </summary>
    [Serializable]
    public class PlayFabNotificationPackage
    {
        public static readonly string[] SUPPORTED_PLUGIN_TIMESTAMP_FORMATS = { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-dd HH:mm:ssZ" };

        public DateTime? ScheduleDate;
        public ScheduleTypes ScheduleType;
        public string Sound;                // do not set this to use the default device sound; otherwise the sound you provide needs to exist in Android/res/raw/_____.mp3, .wav, .ogg
        public string Title;                // title of this message
        public string Icon;                 // to use the default app icon use app_icon, otherwise send the name of the custom image. Image must be in Android/res/drawable/_____.png, .jpg
        public string Message;              // the actual message to transmit (this is what will be displayed in the notification area)
        public string CustomData;           // arbitrary key value pairs for game specific usage
        public int Id = 0;

        public PlayFabNotificationPackage() { }

        public PlayFabNotificationPackage(string message, string title = null, int id = 0, DateTime? scheduleDate = null, ScheduleTypes scheduleType = ScheduleTypes.None, string customData = null)
        {
            Message = message;
            Title = title;
            Id = id;
            SetScheduleTime(scheduleDate, scheduleType);
            CustomData = customData;
        }

        public void SetScheduleTime(DateTime? scheduleDate, ScheduleTypes scheduleType)
        {
            ScheduleDate = scheduleDate;
            ScheduleType = scheduleType;
        }

        public static PlayFabNotificationPackage FromJava(AndroidJavaObject package)
        {
            var output = new PlayFabNotificationPackage
            {
                Id = package.Get<int>("Id"),
                ScheduleType = (ScheduleTypes)Enum.Parse(typeof(ScheduleTypes), package.Get<string>("ScheduleType")),
                Title = package.Get<string>("Title"),
                Message = package.Get<string>("Message"),
                Icon = package.Get<string>("Icon"),
                Sound = package.Get<string>("Sound"),
                CustomData = package.Get<string>("CustomData"),
                ScheduleDate = null // Assigned below
            };

            DateTime temp;
            if (DateTime.TryParseExact(package.Get<string>("ScheduleDate"), SUPPORTED_PLUGIN_TIMESTAMP_FORMATS, CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind, out temp))
                output.ScheduleDate = temp;

            return output;
        }

        public void ToJava(ref AndroidJavaObject package)
        {
            package.Set("Id", Id);
            package.Set("ScheduleType", ScheduleType.ToString());
            package.Set("Title", Title);
            package.Set("Message", Message);
            package.Set("Icon", Icon);
            package.Set("Sound", Sound);
            package.Set("CustomData", CustomData);

            if (ScheduleDate == null)
                package.Call("setScheduleDate", null);
            else
            {
                var dateString = ScheduleDate.Value.ToString(SUPPORTED_PLUGIN_TIMESTAMP_FORMATS[0]);
                if (ScheduleType == ScheduleTypes.ScheduledUtc)
                    dateString = dateString + "Z";
                package.Call("setScheduleDate", dateString);
            }
        }
    }
}
#endif
