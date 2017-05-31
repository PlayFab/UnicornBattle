#define TESTING

#if TESTING || !DISABLE_PLAYFABCLIENT_API && UNITY_ANDROID && !UNITY_EDITOR

using PlayFab.UUnit;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PlayFab.Android
{
    public class AndroidPushTest_Base : UUnitTestCase
    {
        protected Action<UUnitTestContext> ActiveTick;

        private const int MsgDelay = 5, TestExpire = 14; // Can't test anything if this is above 15, but it should be enough to measure an actual delay
        private bool _pushRegisterApiSuccessful;
        private bool _messagesTested;
        private readonly HashSet<string> _expectedMessages = new HashSet<string>();
        private readonly HashSet<string> _expectedCustom = new HashSet<string>();

        public override void SetUp(UUnitTestContext testContext)
        {
            _pushRegisterApiSuccessful = false;
            PlayFabAndroidPushPlugin.StopPlugin();
            testContext.False(PlayFabAndroidPushPlugin.IsPlayServicesAvailable(), "Play Services should not be available before setup");
            PlayFabAndroidPushPlugin.OnGcmSetupStep += OnGcmSetupStep;
            PlayFabAndroidPushPlugin.OnGcmMessage += OnGcmMessage;
            ActiveTick = null;
            _expectedMessages.Clear();
            _expectedCustom.Clear();
        }

        private void OnGcmSetupStep(PlayFabAndroidPushPlugin.PushSetupStatus status)
        {
            if (status == PlayFabAndroidPushPlugin.PushSetupStatus.PlayFabRegisterApiSuccess)
                _pushRegisterApiSuccessful = true;
        }

        private void OnGcmMessage(PlayFabNotificationPackage package)
        {
            Debug.Log("PlayFabGCM: Unity message package received: " + package.Message);
            _expectedMessages.Remove(package.Message);
            _expectedCustom.Remove(package.CustomData);
        }

        public override void Tick(UUnitTestContext testContext)
        {
            if (ActiveTick != null)
                ActiveTick(testContext);
        }

        public override void TearDown(UUnitTestContext testContext)
        {
            PlayFabAndroidPushPlugin.StopPlugin();
            testContext.False(PlayFabAndroidPushPlugin.IsPlayServicesAvailable(), "Play Services should not be available after shutdown");
            PlayFabClientAPI.ForgetClientCredentials();
        }

        public override void ClassTearDown()
        {
        }

        /// <summary>
        /// Utility for tests to pass as soon as registration is successful
        /// On the first successful registration, this will piggyback some tests that can only occur on a successful registration
        /// </summary>
        /// <param name="testContext"></param>
        protected void PassOnSuccessfulRegistration(UUnitTestContext testContext)
        {
            if (_pushRegisterApiSuccessful)
            {
                testContext.True(PlayFabAndroidPushPlugin.IsPlayServicesAvailable(), "This test should have made Play Services available");

                if (!_messagesTested)
                {
                    PlayFabAndroidPushPlugin.AlwaysShowOnNotificationBar(false); // For the purpose of this test, hide the notifications from the device notification tray
                    SendImmediateNotificationsAndWait();
                    SendScheduledNotificationsAndWait();
                    ActiveTick = WaitForExpectedMessages;
                }
                else
                {
                    testContext.EndTest(UUnitFinishState.PASSED, null);
                }
            }
        }

        /// <summary>
        /// Need this in practically every API test
        /// </summary>
        /// <param name="error"></param>
        protected void SharedErrorCallback(PlayFabError error)
        {
            // This error was not expected.  Report it and fail.
            ((UUnitTestContext)error.CustomData).Fail(error.GenerateErrorReport());
        }

        /// <summary>
        /// Test normal message behavior
        /// Send any immediate notifications and verify that all formats and inputs work
        /// The immediate messages still take a moment to complete
        /// </summary>
        private void SendImmediateNotificationsAndWait()
        {
            PlayFabAndroidPushPlugin.SendNotification(new PlayFabNotificationPackage { Id = 0, Message = null, ScheduleDate = null, Title = null, ScheduleType = ScheduleTypes.None, CustomData = null, Icon = null, Sound = null });
            // Don't expect this one to arrive, but it shouldn't crash anything
            PlayFabAndroidPushPlugin.SendNotificationNow("CS-M Test Message");
            _expectedMessages.Add("CS-M Test Message");
            PlayFabAndroidPushPlugin.SendNotification(new PlayFabNotificationPackage("Obj Message", "Obj Title"));
            _expectedMessages.Add("Obj Message");
        }

        /// <summary>
        /// Send several scheduled notifications and verify that everything about them occurs on schedule
        /// </summary>
        private void SendScheduledNotificationsAndWait()
        {
            PlayFabAndroidPushPlugin.ScheduleNotification("UTC Scheduled Test Message", DateTime.UtcNow + TimeSpan.FromSeconds(MsgDelay), ScheduleTypes.ScheduledUtc);
            _expectedMessages.Add("UTC Scheduled Test Message");
            PlayFabAndroidPushPlugin.ScheduleNotification("Local Scheduled Test Message", DateTime.Now + TimeSpan.FromSeconds(MsgDelay), ScheduleTypes.ScheduledLocal);
            _expectedMessages.Add("Local Scheduled Test Message");
            var scheduledMessage = new PlayFabNotificationPackage("Scheduled Message Obj", "Scheduled Title", 0, DateTime.UtcNow + TimeSpan.FromSeconds(MsgDelay), ScheduleTypes.ScheduledUtc, "test custom");
            PlayFabAndroidPushPlugin.SendNotification(scheduledMessage);
            _expectedMessages.Add("Scheduled Message Obj");
            _expectedCustom.Add("test custom");
        }

        private void WaitForExpectedMessages(UUnitTestContext testContext)
        {
            if (_expectedMessages.Count == 0 && _expectedCustom.Count == 0)
            {
                _messagesTested = true;
                testContext.EndTest(UUnitFinishState.PASSED, null);
            }
            if (DateTime.UtcNow > testContext.StartTime + TimeSpan.FromSeconds(TestExpire))
            {
                var sb = new StringBuilder();
                if (_expectedMessages.Count > 0)
                {
                    sb.Append(_expectedMessages.Count).Append(" Missing Messages: ");
                    foreach (var eachMsg in _expectedMessages)
                        sb.Append(eachMsg).Append(",");
                    sb.Length -= 1;
                }
                if (_expectedCustom.Count > 0)
                {
                    sb.Append(_expectedCustom.Count).Append("\nMissing Custom: ");
                    foreach (var eachMsg in _expectedCustom)
                        sb.Append(eachMsg).Append(",");
                    sb.Length -= 1;
                }
                testContext.Fail(sb.ToString());
            }
        }
    }
}

#endif
