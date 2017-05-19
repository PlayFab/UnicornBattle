#define TESTING

#if TESTING || !DISABLE_PLAYFABCLIENT_API && UNITY_ANDROID && !UNITY_EDITOR

using PlayFab.ClientModels;
using PlayFab.Internal;
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
            PlayFabAndroidPushPlugin.Setup(AndroidPushSenderId);
            PlayFabAndroidPushPlugin.OnGcmSetupStep += OnGcmSetupStep;
            _pushRegisterApiSuccessful = false;
        }

        private void OnGcmSetupStep(PlayFabAndroidPushPlugin.PushSetupStatus status)
        {
            if (status == PlayFabAndroidPushPlugin.PushSetupStatus.PlayFabRegisterApiSuccess)
            {
                _pushRegisterApiSuccessful = true;
                PlayFabAndroidPushPlugin.ScheduleNotification("CS-AL Scheduled Test Message", DateTime.Now + TimeSpan.FromSeconds(30));
                PlayFabAndroidPushPlugin.ScheduleNotification("Canceled message - should not see", DateTime.Now + TimeSpan.FromSeconds(30));
                PlayFabAndroidPushPlugin.CancelNotification("Canceled message - should not see");
            }
        }

        public override void Tick(UUnitTestContext testContext)
        {
            if (_pushRegisterApiSuccessful)
                testContext.EndTest(UUnitFinishState.PASSED, null);
        }

        public override void ClassTearDown()
        {
            PlayFabAndroidPushPlugin.Unload();
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
