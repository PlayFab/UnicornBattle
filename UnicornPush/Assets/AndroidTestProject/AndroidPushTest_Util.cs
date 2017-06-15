// #define TESTING

#if TESTING || !DISABLE_PLAYFABCLIENT_API && UNITY_ANDROID && !UNITY_EDITOR

using System;
using PlayFab.Android;
using PlayFab.Json;
using UnityEngine;

namespace PlayFab.UUnit
{
    public class AndroidPushTests_Util : UUnitTestCase
    {
        public override void SetUp(UUnitTestContext testContext)
        {
            PlayFabAndroidPushPlugin.Init();
        }

        public override void TearDown(UUnitTestContext testContext)
        {
            PlayFabAndroidPushPlugin.StopPlugin();
        }

        [UUnitTest]
        public void PfPush_JavaSerialization(UUnitTestContext testContext)
        {
            var javaPkg = new AndroidJavaObject("com.playfab.unityplugin.GCM.PlayFabNotificationPackage");
            var expectedPkgs = new[] {
                new PlayFabNotificationPackage("test message0", "test title", 10),
                new PlayFabNotificationPackage("test message1", "test title", 11),
                new PlayFabNotificationPackage("test message2", "test title", 12),
            };
            expectedPkgs[0].SetScheduleTime(DateTime.Now + TimeSpan.FromSeconds(10), ScheduleTypes.ScheduledLocal);
            expectedPkgs[1].SetScheduleTime(DateTime.UtcNow + TimeSpan.FromSeconds(10), ScheduleTypes.ScheduledUtc);
            expectedPkgs[2].SetScheduleTime(null, ScheduleTypes.None);

            for (var i = 0; i < expectedPkgs.Length; i++)
            {
                var expectedPkg = expectedPkgs[i];
                expectedPkg.ToJava(ref javaPkg);
                var javaScheduleType = javaPkg.Get<string>("ScheduleType");
                testContext.StringEquals(expectedPkg.ScheduleType.ToString(), javaScheduleType, "ScheduleType not assigned " + i + ", " + expectedPkg.ScheduleType + ", " + javaScheduleType);

                var actualPkg = PlayFabNotificationPackage.FromJava(javaPkg);

                testContext.StringEquals(expectedPkg.Message, actualPkg.Message, "Message mismatch " + i + ", " + expectedPkg.Message + ", " + actualPkg.Message);
                testContext.StringEquals(expectedPkg.Title, actualPkg.Title, "Title mismatch " + i + ", " + expectedPkg.Title + ", " + actualPkg.Title);
                testContext.IntEquals(expectedPkg.Id, actualPkg.Id, "Id mismatch " + i + ", " + expectedPkg.Id + ", " + actualPkg.Id);
                testContext.ObjEquals(expectedPkg.ScheduleType, actualPkg.ScheduleType, "ScheduleType mismatch " + i + ", " + expectedPkg.ScheduleType + ", " + actualPkg.ScheduleType);
                testContext.DateTimeEquals(expectedPkg.ScheduleDate, actualPkg.ScheduleDate, TimeSpan.FromSeconds(1), javaPkg.Get<string>("ScheduleDate"));
            }

            testContext.EndTest(UUnitFinishState.PASSED, null);
        }

        [UUnitTest]
        public void PfPush_JsonSerialization(UUnitTestContext testContext)
        {
            var tempJavaPkgObj = new AndroidJavaObject("com.playfab.unityplugin.GCM.PlayFabNotificationPackage");
            var javaPkgClass = new AndroidJavaClass("com.playfab.unityplugin.GCM.PlayFabNotificationPackage");
            var expectedPkgs = new[] {
                new PlayFabNotificationPackage("test message0", "test title1", 10),
                new PlayFabNotificationPackage("test message1", "test title2", 11),
                new PlayFabNotificationPackage("test message2", "test title3", 12),
            };
            expectedPkgs[0].SetScheduleTime(DateTime.Now + TimeSpan.FromSeconds(10), ScheduleTypes.ScheduledLocal);
            expectedPkgs[1].SetScheduleTime(DateTime.UtcNow + TimeSpan.FromSeconds(10), ScheduleTypes.ScheduledUtc);
            expectedPkgs[2].SetScheduleTime(null, ScheduleTypes.None);

            for (var i = 0; i < expectedPkgs.Length; i++)
            {
                var expectedPkg = expectedPkgs[i];
                expectedPkg.ToJava(ref tempJavaPkgObj);
                var firstJson = tempJavaPkgObj.Call<string>("toJson");
                var javaPkgObj = javaPkgClass.CallStatic<AndroidJavaObject>("fromJson", firstJson);
                var secondJson = javaPkgObj.Call<string>("toJson");
                testContext.StringEquals(firstJson, secondJson);

                PlayFabNotificationPackage actualPkg = null;;
                try
                {
                    actualPkg = JsonWrapper.DeserializeObject<PlayFabNotificationPackage>(secondJson);
                }
                catch (Exception)
                {
                    testContext.Fail("JSON PARSE FAILURE:\n1stJson: " + firstJson + "\n2ndJson:" + secondJson);
                }

                testContext.NotNull(actualPkg);
                testContext.StringEquals(expectedPkg.Message, actualPkg.Message, "Message mismatch " + i + ", " + expectedPkg.Message + ", " + actualPkg.Message + "\nExJson: " + secondJson + "\nAcJson:" + secondJson);
                testContext.StringEquals(expectedPkg.Title, actualPkg.Title, "Title mismatch " + i + ", " + expectedPkg.Title + ", " + actualPkg.Title + ", " + secondJson + ", " + secondJson);
                testContext.IntEquals(expectedPkg.Id, actualPkg.Id, "Id mismatch " + i + ", " + expectedPkg.Id + ", " + actualPkg.Id + ", " + secondJson + ", " + secondJson);
                testContext.ObjEquals(expectedPkg.ScheduleType, actualPkg.ScheduleType, "ScheduleType mismatch " + i + ", " + expectedPkg.ScheduleType + ", " + actualPkg.ScheduleType + ", " + secondJson + ", " + secondJson);
                testContext.DateTimeEquals(expectedPkg.ScheduleDate, actualPkg.ScheduleDate, TimeSpan.FromSeconds(1), "ScheduleDate mismatch " + i + ", " + expectedPkg.ScheduleDate + ", " + actualPkg.ScheduleDate + ", " + secondJson + ", " + secondJson);
            }

            testContext.EndTest(UUnitFinishState.PASSED, null);
        }
    }
}

#endif
