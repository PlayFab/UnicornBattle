// #define TESTING

#if TESTING || !DISABLE_PLAYFABCLIENT_API && UNITY_ANDROID && !UNITY_EDITOR

using PlayFab.ClientModels;
using PlayFab.UUnit;
using System;
using UnityEngine;

namespace PlayFab.Android
{
    public class PushTest_ConstSender_AutoLogin : AndroidPushTest_Base
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

        // [UUnitTest] // This test won't pass until the profile can be returned at login
        void Push_ConstSender_AutoLogin(UUnitTestContext testContext)
        {
            var loginRequest = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
                // TODO: REQUIRED - ASK FOR PLAYER PROFILE
            };
            PlayFabClientAPI.LoginWithCustomID(loginRequest, PlayFabUUnitUtils.ApiActionWrapper<LoginResult>(testContext, null), PlayFabUUnitUtils.ApiActionWrapper<PlayFabError>(testContext, SharedErrorCallback));
            ActiveTick += PassOnSuccessfulRegistration;
        }
    }
}
#endif
