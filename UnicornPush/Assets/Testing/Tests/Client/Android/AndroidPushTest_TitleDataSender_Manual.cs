#if TESTING || !DISABLE_PLAYFABCLIENT_API && UNITY_ANDROID && !UNITY_EDITOR

using PlayFab.ClientModels;
using PlayFab.Internal;
using System.Collections.Generic;
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
    public class AndroidPushTest_TitleDataSender_Manual : UUnitTestCase
    {
        const string TITLE_ID = "A5F3";
        string androidPushSenderId;

        bool pushRegisterApiSuccessful;

        public override void ClassSetUp()
        {
            PlayFabSettings.TitleId = TITLE_ID;
            PlayFabPluginEventHandler.Init();
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

        [UUnitTest]
        public void Push_TitleDataSender_Manual(UUnitTestContext testContext)
        {
            var loginRequest = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true
                // We explicitly do NOT ask for the profile here, as we'll be manually triggering registration
            };
            PlayFabClientAPI.LoginWithCustomID(loginRequest, PlayFabUUnitUtils.ApiActionWrapper<LoginResult>(testContext, OnLoginSuccess), PlayFabUUnitUtils.ApiActionWrapper<PlayFabError>(testContext, SharedErrorCallback), testContext);
        }
        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("PlayFab: AndroidTest Logged in, getting Title data: " + PlayFabSettings.TitleId + " " + result.SessionTicket);
            GetTitleData(result.CustomData as UUnitTestContext);
        }

        void GetTitleData(UUnitTestContext testContext)
        {
            var getRequest = new GetTitleDataRequest
            {
                Keys = new List<string> { "AndroidPushSenderId" }
            };
            PlayFabClientAPI.GetTitleData(getRequest, PlayFabUUnitUtils.ApiActionWrapper<GetTitleDataResult>(testContext, OnGetTitleData), PlayFabUUnitUtils.ApiActionWrapper<PlayFabError>(testContext, SharedErrorCallback), testContext);
        }
        void OnGetTitleData(GetTitleDataResult result)
        {
            androidPushSenderId = result.Data["AndroidPushSenderId"];
            Debug.Log("PlayFab: Sender id: " + androidPushSenderId);
            PlayFabPluginEventHandler.Setup(androidPushSenderId);
            PlayFabPluginEventHandler.TriggerManualRegistration();
        }
    }
}

#endif
