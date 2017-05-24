#define TESTING

#if TESTING || !DISABLE_PLAYFABCLIENT_API && UNITY_ANDROID && !UNITY_EDITOR

using PlayFab.ClientModels;
using System;
using UnityEngine;

namespace PlayFab.UUnit
{
    public class PushTest_ConstSender_AutoLogin : UUnitTestCase
    {
        const string TitleId = "A5F3";
        const string AndroidPushSenderId = "494923569376";
        bool _pushRegisterApiSuccessful;

        public override void ClassSetUp()
        {
            PlayFabSettings.TitleId = TitleId;
            _pushRegisterApiSuccessful = false;
        }

        public override void SetUp(UUnitTestContext testContext)
        {
            testContext.False(PlayFabAndroidPushPlugin.IsPlayServicesAvailable(), "Play Services should not be available before setup");
            PlayFabAndroidPushPlugin.Setup(AndroidPushSenderId);
            PlayFabAndroidPushPlugin.OnGcmSetupStep += OnGcmSetupStep;
        }

        private void OnGcmSetupStep(PlayFabAndroidPushPlugin.PushSetupStatus status)
        {
            if (status == PlayFabAndroidPushPlugin.PushSetupStatus.PlayFabRegisterApiSuccess)
            {
                _pushRegisterApiSuccessful = true;
                PlayFabAndroidPushPlugin.SendNotificationNow("CS-AL Test Message");
                PlayFabAndroidPushPlugin.ScheduleNotification("CS-AL UTC Scheduled Test Message", DateTime.UtcNow + TimeSpan.FromSeconds(30), PlayFabAndroidPushPlugin.ScheduleTypes.ScheduledUtc);
                PlayFabAndroidPushPlugin.ScheduleNotification("Canceled UTC message - should not see", DateTime.UtcNow + TimeSpan.FromSeconds(30), PlayFabAndroidPushPlugin.ScheduleTypes.ScheduledUtc);
                PlayFabAndroidPushPlugin.CancelNotification("Canceled UTC message - should not see");
            }
        }

        public override void Tick(UUnitTestContext testContext)
        {
            if (_pushRegisterApiSuccessful)
                testContext.EndTest(UUnitFinishState.PASSED, null);
        }

        public override void TearDown(UUnitTestContext testContext)
        {
            testContext.True(PlayFabAndroidPushPlugin.IsPlayServicesAvailable(), "This test should have made Play Services available");
            PlayFabAndroidPushPlugin.StopPlugin();
            testContext.False(PlayFabAndroidPushPlugin.IsPlayServicesAvailable(), "Play Services should not be available after shutdown");
        }

        public override void ClassTearDown()
        {
            PlayFabClientAPI.ForgetClientCredentials();
        }

        private void SharedErrorCallback(PlayFabError error)
        {
            // This error was not expected.  Report it and fail.
            ((UUnitTestContext)error.CustomData).Fail(error.GenerateErrorReport());
        }

        // [UUnitTest] // This test won't pass until the profile can be returned at login
        void Push_ConstSender_AutoLogin(UUnitTestContext testContext)
        {
            var loginRequest = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
                // TODO: REQUIRED - ASK FOR PLAYER PROFILE
            };
            PlayFabClientAPI.LoginWithCustomID(loginRequest, PlayFabUUnitUtils.ApiActionWrapper<LoginResult>(testContext, OnLoginSuccess), PlayFabUUnitUtils.ApiActionWrapper<PlayFabError>(testContext, SharedErrorCallback));
        }
        private void OnLoginSuccess(LoginResult result)
        {
        }
    }
}
#endif
