// #define TESTING

#if TESTING || !DISABLE_PLAYFABCLIENT_API && UNITY_ANDROID && !UNITY_EDITOR

using PlayFab.Android;
using PlayFab.ClientModels;
using PlayFab.UUnit;
using System;
using UnityEngine;

namespace PlayFab.Android
{
    public class AndroidPushTests_ConstSender : AndroidPushTest_Base
    {
        const string TitleId = "A5F3";
        const string AndroidPushSenderId = "494923569376";

        public override void ClassSetUp()
        {
            base.ClassSetUp();
            PlayFabSettings.TitleId = TitleId;
        }

        public override void SetUp(UUnitTestContext testContext)
        {
            base.SetUp(testContext);
            PlayFabAndroidPushPlugin.Setup(AndroidPushSenderId);
        }

        public override void ClassTearDown()
        {
            base.ClassTearDown();
            PlayFabClientAPI.ForgetClientCredentials();
        }

        [UUnitTest]
        public void Push_ConstSender(UUnitTestContext testContext)
        {
            var loginRequest = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true
            };
            PlayFabClientAPI.LoginWithCustomID(loginRequest, PlayFabUUnitUtils.ApiActionWrapper<LoginResult>(testContext, OnLoginSuccess), PlayFabUUnitUtils.ApiActionWrapper<PlayFabError>(testContext, SharedErrorCallback));
            ActiveTick += PassOnSuccessfulRegistration;
        }
        private void OnLoginSuccess(LoginResult result)
        {
            PlayFabAndroidPushPlugin.TriggerManualRegistration();
        }
    }
}

#endif
