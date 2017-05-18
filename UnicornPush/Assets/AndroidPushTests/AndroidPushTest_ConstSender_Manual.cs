#define TESTING

#if TESTING || !DISABLE_PLAYFABCLIENT_API && UNITY_ANDROID && !UNITY_EDITOR

using System;
using PlayFab.ClientModels;
using PlayFab.Internal;
using UnityEngine;

namespace PlayFab.UUnit
{
    /// <summary>
    /// A real system would potentially run only the client or server API, and not both.
    /// But, they still interact with eachother directly.
    /// The tests can't be independent for Client/Server, as the sequence of calls isn't really independent for real-world scenarios.
    /// The client logs in, which triggers a server, and then back and forth.
    /// For the purpose of testing, they each have pieces of information they share with one another, and that sharing makes various calls possible.
    /// </summary>
    public class AndroidPushTests_ConstSender_Manual : UUnitTestCase
    {
        const string TITLE_ID = "A5F3";
        const string androidPushSenderId = "494923569376";

        bool pushRegisterApiSuccessful;

        public override void ClassSetUp()
        {
            PlayFabSettings.TitleId = TITLE_ID;
            PlayFabPluginEventHandler.Setup(androidPushSenderId);
            PlayFabPluginEventHandler.OnGcmSetupStep += OnGcmSetupStep;
            pushRegisterApiSuccessful = false;
        }

        private void OnGcmSetupStep(PlayFabPluginEventHandler.PushSetupStatus status)
        {
            if (status == PlayFabPluginEventHandler.PushSetupStatus.PlayFabRegisterApiSuccess)
            {
                pushRegisterApiSuccessful = true;
                PlayFabPluginEventHandler.ScheduleNotification("Const Sender Scheduled Test Message", DateTime.Now + TimeSpan.FromSeconds(30));
            }
        }

        public override void Tick(UUnitTestContext testContext)
        {
            if (pushRegisterApiSuccessful)
                testContext.EndTest(UUnitFinishState.PASSED, null);
        }

        public override void ClassTearDown()
        {
            PlayFabPluginEventHandler.Unload();
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
            PlayFabPluginEventHandler.TriggerManualRegistration();
        }
    }
}

#endif
