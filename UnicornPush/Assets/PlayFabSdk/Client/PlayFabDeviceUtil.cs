#if !DISABLE_PLAYFABCLIENT_API
using System;
using PlayFab.ClientModels;
using UnityEngine;

namespace PlayFab.Internal
{
    public static class PlayFabDeviceUtil
    {
        private static GameObject _playFabAndroidPushGo;

        #region Make Attribution API call
        private static void DoAttributeInstall()
        {
            var attribRequest = new AttributeInstallRequest();
            switch (PlayFabSettings.AdvertisingIdType)
            {
                case PlayFabSettings.AD_TYPE_ANDROID_ID: attribRequest.Adid = PlayFabSettings.AdvertisingIdValue; break;
                case PlayFabSettings.AD_TYPE_IDFA: attribRequest.Idfa = PlayFabSettings.AdvertisingIdValue; break;
            }
            PlayFabClientAPI.AttributeInstall(attribRequest, OnAttributeInstall, null);
        }
        private static void OnAttributeInstall(AttributeInstallResult result)
        {
            // This is for internal testing.
            PlayFabSettings.AdvertisingIdType += "_Successful";
        }
        #endregion Make Attribution API call

        #region Make Push Registration API call
        private static void RegisterForPush_Android(string token)
        {
            var request = new AndroidDevicePushNotificationRegistrationRequest
            {
                SendPushNotificationConfirmation = true,
                ConfirmationMessage = "Push Registered",
                DeviceToken = token
            };
            PlayFabClientAPI.AndroidDevicePushNotificationRegistration(request, OnAndroidPushRegister, UniPushMain.OnSharedFailure);
        }
        private static void OnAndroidPushRegister(AndroidDevicePushNotificationRegistrationResult result)
        {
            Debug.Log("Android Push Registered");
        }
        #endregion Make Push Registration API call

        public static void OnPlayFabLogin(bool needsAttribution)
        {
            if (needsAttribution)
                SetDeviceAttribution();
            ActivatePush();
        }

        private static void SetDeviceAttribution()
        {
            if (PlayFabSettings.DisableAdvertising)
                return;
            if (PlayFabSettings.AdvertisingIdType != null && PlayFabSettings.AdvertisingIdValue != null)
                DoAttributeInstall();
            else
                GetAdvertIdFromUnity();
        }

        private static void ActivatePush()
        {
#if UNITY_ANDROID && (!UNITY_EDITOR || TESTING)
            Debug.Log("Triggering AP OnPlayFabLogin");
            _playFabAndroidPushGo = GameObject.Find("_PlayFabGO");
            if (_playFabAndroidPushGo != null)
                _playFabAndroidPushGo.BroadcastMessage("OnPlayFabLogin", (Action<string>)RegisterForPush_Android);
#endif // TODO: iOS
        }

        private static void GetAdvertIdFromUnity()
        {
            Application.RequestAdvertisingIdentifierAsync(
                (advertisingId, trackingEnabled, error) =>
                {
                    PlayFabSettings.DisableAdvertising = !trackingEnabled;
                    if (!trackingEnabled)
                        return;
#if UNITY_ANDROID
                    PlayFabSettings.AdvertisingIdType = PlayFabSettings.AD_TYPE_ANDROID_ID;
#elif UNITY_IOS
                    PlayFabSettings.AdvertisingIdType = PlayFabSettings.AD_TYPE_IDFA;
#endif
                    PlayFabSettings.AdvertisingIdValue = advertisingId;
                    DoAttributeInstall();
                }
            );
        }
    }
}
#endif
