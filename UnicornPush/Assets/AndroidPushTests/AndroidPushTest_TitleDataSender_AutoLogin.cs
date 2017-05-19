#define TESTING

#if TESTING || !DISABLE_PLAYFABCLIENT_API && UNITY_ANDROID && !UNITY_EDITOR

using PlayFab.ClientModels;
using PlayFab.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayFab.UUnit
{
    public class PushTest_TitleDataSender_AutoLogin : UUnitTestCase
    {
        const string TitleId = "A5F3";
        string _androidPushSenderId = "";
        bool _pushRegisterApiSuccessful;

        public override void ClassSetUp()
        {
            PlayFabSettings.TitleId = TitleId;
            PlayFabAndroidPushPlugin.Init();
            PlayFabAndroidPushPlugin.OnGcmSetupStep += OnGcmSetupStep;
            _pushRegisterApiSuccessful = false;
        }

        private void OnGcmSetupStep(PlayFabAndroidPushPlugin.PushSetupStatus status)
        {
            if (status == PlayFabAndroidPushPlugin.PushSetupStatus.PlayFabRegisterApiSuccess)
            {
                _pushRegisterApiSuccessful = true;
                PlayFabAndroidPushPlugin.ScheduleNotification("TS-AL Scheduled Test Message", DateTime.Now + TimeSpan.FromSeconds(30));
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
        public void Push_TitleDataSender_AutoLogin(UUnitTestContext testContext)
        {
            var loginRequest = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
                // TODO: REQUIRED - ASK FOR PLAYER PROFILE
            };
            PlayFabClientAPI.LoginWithCustomID(loginRequest, PlayFabUUnitUtils.ApiActionWrapper<LoginResult>(testContext, OnLoginSuccess), PlayFabUUnitUtils.ApiActionWrapper<PlayFabError>(testContext, SharedErrorCallback), testContext);
        }
        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("PlayFab: AndroidTest Logged in, getting Title data: " + PlayFabSettings.TitleId + " " + result.SessionTicket);
            GetTitleData(result.CustomData as UUnitTestContext);
        }

        private void GetTitleData(UUnitTestContext testContext)
        {
            var getRequest = new GetTitleDataRequest
            {
                Keys = new List<string> { "AndroidPushSenderId" }
            };
            PlayFabClientAPI.GetTitleData(getRequest, PlayFabUUnitUtils.ApiActionWrapper<GetTitleDataResult>(testContext, OnGetTitleData), PlayFabUUnitUtils.ApiActionWrapper<PlayFabError>(testContext, SharedErrorCallback), testContext);
        }
        private void OnGetTitleData(GetTitleDataResult result)
        {
            _androidPushSenderId = result.Data["AndroidPushSenderId"];
            Debug.Log("PlayFab: Sender id: " + _androidPushSenderId);
            PlayFabAndroidPushPlugin.Setup(_androidPushSenderId);
        }
    }
}
#endif
