#if !DISABLE_PLAYFABCLIENT_API && UNITY_ANDROID && !UNITY_EDITOR

using PlayFab;
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
    public class AndroidPushTests_ConstSender_AutoLogin : UUnitTestCase
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
                pushRegisterApiSuccessful = true;
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

        // [UUnitTest] // This test won't pass until the profile can be returned at login
        public void RunTest(UUnitTestContext testContext)
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
