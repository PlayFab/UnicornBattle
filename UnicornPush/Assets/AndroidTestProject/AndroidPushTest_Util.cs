using System;
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
        public void PlayFabNotificationPackageSerialization(UUnitTestContext testContext)
        {
            var javaPkg = new AndroidJavaObject("com.playfab.unityplugin.GCM.PlayFabNotificationPackage");
            var expectedPkgs = new[] {
                new PlayFabAndroidPushPlugin.PlayFabNotificationPackage("test message0", "test title", 10),
                new PlayFabAndroidPushPlugin.PlayFabNotificationPackage("test message1", "test title", 11),
                new PlayFabAndroidPushPlugin.PlayFabNotificationPackage("test message2", "test title", 12),
            };
            expectedPkgs[0].SetScheduleTime(DateTime.Now + TimeSpan.FromSeconds(10), PlayFabAndroidPushPlugin.ScheduleTypes.ScheduledLocal);
            expectedPkgs[1].SetScheduleTime(DateTime.UtcNow + TimeSpan.FromSeconds(10), PlayFabAndroidPushPlugin.ScheduleTypes.ScheduledUtc);
            expectedPkgs[2].SetScheduleTime(null, PlayFabAndroidPushPlugin.ScheduleTypes.None);

            for (var i = 0; i < expectedPkgs.Length; i++)
            {
                var expectedPkg = expectedPkgs[i];
                expectedPkg.ToJava(ref javaPkg);
                var javaScheduleType = javaPkg.Get<string>("ScheduleType");
                testContext.StringEquals(expectedPkg.ScheduleType.ToString(), javaScheduleType, "ScheduleType not assigned " + i + ", " + expectedPkg.ScheduleType + ", " + javaScheduleType);

                var actualPkg = PlayFabAndroidPushPlugin.PlayFabNotificationPackage.FromJava(javaPkg);

                testContext.StringEquals(expectedPkg.Message, actualPkg.Message, "Message mismatch " + i + ", " + expectedPkg.Message + ", " + actualPkg.Message);
                testContext.StringEquals(expectedPkg.Title, actualPkg.Title, "Title mismatch " + i + ", " + expectedPkg.Title + ", " + actualPkg.Title);
                testContext.IntEquals(expectedPkg.Id, actualPkg.Id, "Id mismatch " + i + ", " + expectedPkg.Id + ", " + actualPkg.Id);
                testContext.ObjEquals(expectedPkg.ScheduleType, actualPkg.ScheduleType, "ScheduleType mismatch " + i + ", " + expectedPkg.ScheduleType + ", " + actualPkg.ScheduleType);
                testContext.DateTimeEquals(expectedPkg.ScheduleDate, actualPkg.ScheduleDate, TimeSpan.FromSeconds(1), "ScheduleDate mismatch " + i + ", " + expectedPkg.ScheduleDate + ", " + actualPkg.ScheduleDate);
            }

            testContext.EndTest(UUnitFinishState.PASSED, null);
        }
    }
}
