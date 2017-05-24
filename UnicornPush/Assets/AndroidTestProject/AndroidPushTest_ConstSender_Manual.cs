#define TESTING

#if TESTING || !DISABLE_PLAYFABCLIENT_API && UNITY_ANDROID && !UNITY_EDITOR

using PlayFab.ClientModels;
using System;
using UnityEngine;

namespace PlayFab.UUnit
{
    public class AndroidPushTests_ConstSender_Manual : UUnitTestCase
    {
        const string TitleId = "A5F3";
        const string AndroidPushSenderId = "494923569376";
        bool _pushRegisterApiSuccessful;
        const int msgDelay = 5; // Can't test anything if this is above 15, but it should be enough to measure

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
                // Test normal message behavior
                PlayFabAndroidPushPlugin.SendNotificationNow("CS-M Test Message");
                PlayFabAndroidPushPlugin.SendNotificationNow(new PlayFabAndroidPushPlugin.PlayFabNotificationPackage("Obj Message", "Obj Title"));
                PlayFabAndroidPushPlugin.ScheduleNotification("CS-M UTC Scheduled Test Message", DateTime.UtcNow + TimeSpan.FromSeconds(msgDelay), PlayFabAndroidPushPlugin.ScheduleTypes.ScheduledUtc);
                PlayFabAndroidPushPlugin.ScheduleNotification("CS-M Local Scheduled Test Message", DateTime.Now + TimeSpan.FromSeconds(msgDelay), PlayFabAndroidPushPlugin.ScheduleTypes.ScheduledLocal);
                var scheduledMessage = new PlayFabAndroidPushPlugin.PlayFabNotificationPackage("Scheduled Message", "Scheduled Title");
                scheduledMessage.SetScheduleTime(DateTime.UtcNow + TimeSpan.FromSeconds(msgDelay), PlayFabAndroidPushPlugin.ScheduleTypes.ScheduledUtc);
                PlayFabAndroidPushPlugin.ScheduleNotification(scheduledMessage);
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

        [UUnitTest]
        public void Push_ConstSender_Manual(UUnitTestContext testContext)
        {
            var loginRequest = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true
                // We explicitly do NOT ask for the profile here, as we'll be manually triggering registration
            };
            PlayFabClientAPI.LoginWithCustomID(loginRequest, PlayFabUUnitUtils.ApiActionWrapper<LoginResult>(testContext, OnLoginSuccess), PlayFabUUnitUtils.ApiActionWrapper<PlayFabError>(testContext, SharedErrorCallback));
        }
        private void OnLoginSuccess(LoginResult result)
        {
            PlayFabAndroidPushPlugin.TriggerManualRegistration();
        }
    }
}

#endif
