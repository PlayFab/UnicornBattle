using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayFab.Internal
{
    /// <summary>
    /// Call Setup before login, if androidPushSenderId is available before login
    ///   -otherwise-
    /// Use Init before login, if you store androidPushSenderId in title data, and you need to log in before you can provide androidPushSenderId
    /// Call Setup after login with androidPushSenderId
    /// </summary>
    public class PlayFabPluginEventHandler : MonoBehaviour
    {
        public enum PushSetupStatus
        {
            GameObjectInitialized,
            AndroidPluginInitialized,
            RegistrationReady,
            TokenReceived,
            RegisteredWithPlayFab,
            ReceivedMessage
        }

        private const string GAME_OBJECT_NAME = "_PlayFabGO"; // This name is defined in the Android Java Plugin, and shouldn't be changed

        public static bool LogMessagesToUnity = true;
        public static event Action<string> OnGcmMessage;
        public static event Action<PushSetupStatus> OnGcmSetupStep;

        private static string _androidPushSenderId;
        private static PlayFabPluginEventHandler _singletonInstance;
        private static AndroidJavaClass _playFabGcmClass;
        private static AndroidJavaClass _playFabPushCacheClass;
        private static AndroidJavaClass _androidPlugin;
        private static AndroidJavaClass _playServicesUtils;
        private static AndroidJavaClass _notificationSender;
        private static AndroidJavaClass _clsUnity;
        private static AndroidJavaObject _objActivity;
        private static Action<string> _registerForAndroidPushApi;

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
            _singletonInstance = playfabGo.GetComponent<PlayFabPluginEventHandler>() ?? playfabGo.AddComponent<PlayFabPluginEventHandler>();
            _singletonInstance.PostStatusMessage(PushSetupStatus.GameObjectInitialized);
        }

        /// <summary>
        /// Setup should be called anytime before, or after PlayFab Login, when you have the androidPushSenderId
        /// </summary>
        public static void Setup(string androidPushSenderId)
        {
            Init();

            _androidPushSenderId = androidPushSenderId; // Save the sender id for after login

            if (_registerForAndroidPushApi != null && !string.IsNullOrEmpty(_androidPushSenderId))
                LoadPlugin();
            else
                _singletonInstance.GCMLog("PlayFab: Android Push Ready, Log into PlayFab to activate Push Notifications");
        }

        private static void LoadPlugin()
        {
            _playFabGcmClass = new AndroidJavaClass("com.playfab.unityplugin.GCM.PlayFabGoogleCloudMessaging");
            _playFabPushCacheClass = new AndroidJavaClass("com.playfab.unityplugin.GCM.PlayFabPushCache");
            _androidPlugin = new AndroidJavaClass("com.playfab.unityplugin.PlayFabUnityAndroidPlugin");
            _playServicesUtils = new AndroidJavaClass("com.playfab.unityplugin.GCM.PlayServicesUtils");
            _notificationSender = new AndroidJavaClass("com.playfab.unityplugin.GCM.PlayFabNotificationSender");
            _clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            _objActivity = _clsUnity.GetStatic<AndroidJavaObject>("currentActivity");

            _androidPlugin.CallStatic("initGCM", _androidPushSenderId, Application.productName); // Start the PlayFab push plugin service
            _singletonInstance.PostStatusMessage(PushSetupStatus.AndroidPluginInitialized);
        }

        #region Push Scheduling Functions
        public static void ScheduleNotification(string notification, DateTime date)
        {
            var dateString = date.ToString("MM-dd-yyyy HH:mm:ss");
            var package = _notificationSender.CallStatic<AndroidJavaObject>("createNotificationPackage", _objActivity, notification);
            package.Call("SetScheduleDate", dateString);
            _notificationSender.CallStatic("Send", _objActivity, package);
        }

        public static void SendNotificationNow(string notification)
        {
            var package = _notificationSender.CallStatic<AndroidJavaObject>("createNotificationPackage", _objActivity, notification);
            _notificationSender.CallStatic("Send", _objActivity, package);
        }

        public static void CancelNotification(string notification)
        {
            var package = _notificationSender.CallStatic<AndroidJavaObject>("createNotificationPackage", _objActivity, notification);
            _notificationSender.CallStatic("CancelScheduledNotification", _objActivity, package);
        }
        #endregion Push Scheduling Functions

        #region Direct Calls to Push Objects
        public static void ClearPushCache()
        {
            _playFabPushCacheClass.CallStatic("clearPushCache");
        }

        public static string GetPushCacheData()
        {
            return _playFabPushCacheClass.CallStatic<string>("getPushCacheData");
        }

        public static void StopPlugin()
        {
            _androidPlugin.CallStatic("stopPluginService");
        }

        public static void UpdateRouting(bool routeToNotificationArea)
        {
            _androidPlugin.CallStatic("updateRouting", routeToNotificationArea);
        }

        public static bool IsPlayServicesAvailable()
        {
            return _playServicesUtils != null && _playServicesUtils.CallStatic<bool>("isPlayServicesAvailable");
        }

        public static List<PlayFabNotificationPackage> GetPushCache()
        {
            var pushCache = new List<PlayFabNotificationPackage>();
            var packages = _playFabPushCacheClass.CallStatic<List<AndroidJavaObject>>("getPushCache");
            if (packages == null)
                return pushCache;

            foreach (var package in packages)
            {
                pushCache.Add(new PlayFabNotificationPackage
                {
                    Id = package.Get<int>("Id"),
                    ScheduleType = package.Get<ScheduleTypes>("ScheduleType"),
                    ScheduleDate = package.Get<DateTime>("ScheduleDate"),
                    Title = package.Get<string>("Title"),
                    Message = package.Get<string>("Message"),
                    Icon = package.Get<string>("Icon"),
                    Sound = package.Get<string>("Sound"),
                    CustomData = package.Get<string>("CustomData"),
                    Delivered = package.Get<bool>("Delivered")
                });
            }
            return pushCache;
        }
        #endregion Direct Calls to Push Objects

        #region Internal Unity Monobehavior-Messaging Hooks
        private void OnPlayFabLogin(Action<string> registerForAndroidPushApi)
        {
            _registerForAndroidPushApi = registerForAndroidPushApi; // Once the java push plugin has done its work, this is the API call which registers for push
            if (!string.IsNullOrEmpty(_androidPushSenderId))
                LoadPlugin();
            else
                GCMLog("PlayFab: Android Push Ready, call PlayFabPluginEventHandler.Setup() to activate Push Notifications");
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
            if (!string.IsNullOrEmpty(token))
                _registerForAndroidPushApi(token);
            else
                GCMLog("PlayFab: Android Push setup failed, with token: " + token);
            PostStatusMessage(PushSetupStatus.TokenReceived);
        }

        private void GCMRegisterError(string message)
        {
            if (LogMessagesToUnity)
                Debug.LogError("PlayFab GCM MESSAGE: " + message);
            if (OnGcmMessage != null)
                OnGcmMessage(message);
        }

        private void GCMLog(string message)
        {
            if (LogMessagesToUnity)
                Debug.Log("PlayFab GCM MESSAGE: " + message);
        }

        private void GCMMessageReceived(string message)
        {
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
}

[Serializable]
public class PlayFabNotificationPackage
{ // c# wrapper that matches our native com.playfab.unityplugin.GCM.PlayFabNotificationPackage
    public DateTime ScheduleDate;
    public ScheduleTypes ScheduleType;
    public string Sound;                // do not set this to use the default device sound; otherwise the sound you provide needs to exist in Android/res/raw/_____.mp3, .wav, .ogg
    public string Title;                // title of this message
    public string Icon;                 // to use the default app icon use app_icon, otherwise send the name of the custom image. Image must be in Android/res/drawable/_____.png, .jpg
    public string Message;              // the actual message to transmit (this is what will be displayed in the notification area)
    public string CustomData;           // arbitrary key value pairs for game specific usage
    public int Id;
    public bool Delivered;
}

public enum ScheduleTypes
{
    None,
    ScheduledDate
}
