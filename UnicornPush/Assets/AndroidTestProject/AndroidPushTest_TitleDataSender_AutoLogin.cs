#define TESTING

#if TESTING || !DISABLE_PLAYFABCLIENT_API && UNITY_ANDROID && !UNITY_EDITOR

using PlayFab.ClientModels;
using PlayFab.UUnit;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayFab.Android
{
    public class PushTest_TitleDataSender_AutoLogin : AndroidPushTest_Base
    {
        const string TitleId = "A5F3";
        string _androidPushSenderId = "";

        public override void ClassSetUp()
        {
            base.ClassSetUp();
            PlayFabSettings.TitleId = TitleId;
        }

        public override void SetUp(UUnitTestContext testContext)
        {
            base.SetUp(testContext);
            PlayFabAndroidPushPlugin.Init();
        }

        public override void ClassTearDown()
        {
            base.ClassTearDown();
            PlayFabClientAPI.ForgetClientCredentials();
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
            ActiveTick += PassOnSuccessfulRegistration;
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
