// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#if UNITY_ANDROID && !UNITY_EDITOR
using Microsoft.AppCenter.Unity.Crashes.Models;
using Microsoft.AppCenter.Unity.Crashes;
using Microsoft.AppCenter.Unity.Internal.Utility;
using Microsoft.AppCenter.Unity.Internal;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;

namespace Microsoft.AppCenter.Unity.Crashes.Internal
{
    class CrashesInternal
    {
        private static AndroidJavaClass _crashes = new AndroidJavaClass("com.microsoft.appcenter.crashes.Crashes");
        private static AndroidJavaClass _wrapperSdkExceptionManager = new AndroidJavaClass("com.microsoft.appcenter.crashes.WrapperSdkExceptionManager");

        public static void AddNativeType(List<IntPtr> nativeTypes)
        {
            nativeTypes.Add(AndroidJNI.FindClass("com/microsoft/appcenter/crashes/Crashes"));
        }

        public static void TrackException(AndroidJavaObject exception)
        {
            _wrapperSdkExceptionManager.CallStatic("trackException", exception);
        }

        public static void TrackException(AndroidJavaObject exception, IDictionary<string, string> properties)
        {
            var propertiesMap = JavaStringMapHelper.ConvertToJava(properties);
            _wrapperSdkExceptionManager.CallStatic("trackException", exception, propertiesMap);
        }

        public static AppCenterTask SetEnabledAsync(bool isEnabled)
        {
            var future = _crashes.CallStatic<AndroidJavaObject>("setEnabled", isEnabled);
            return new AppCenterTask(future);
        }

        public static AppCenterTask<bool> IsEnabledAsync()
        {
            var future = _crashes.CallStatic<AndroidJavaObject>("isEnabled");
            return new AppCenterTask<bool>(future);
        }

        public static void GenerateTestCrash()
        {
            // The call to the "generateTestCrash" method from native SDK wouldn't work in this
            // case because it just throws an exception which Unity automatically catches and logs,
            // without crashing the application
            // _crashes.CallStatic("generateTestCrash");

            if (Debug.isDebugBuild)
            {
                new Thread(() =>
                {
                    AndroidJNI.FindClass("Test/crash/generated/by/SDK");
                }).Start();
            }
        }

        public static AppCenterTask<bool> HasCrashedInLastSessionAsync()
        {
            var future = _crashes.CallStatic<AndroidJavaObject>("hasCrashedInLastSession");
            return new AppCenterTask<bool>(future);
        }

        public static AppCenterTask<ErrorReport> GetLastSessionCrashReportAsync()
        {
            var future = _crashes.CallStatic<AndroidJavaObject>("getLastSessionCrashReport");
            var javaTask = new AppCenterTask<AndroidJavaObject>(future);
            var errorReportTask = new AppCenterTask<ErrorReport>();
            javaTask.ContinueWith(t =>
            {
                var errorReport = JavaObjectsConverter.ConvertErrorReport(t.Result);
                errorReportTask.SetResult(errorReport);
            });
            return errorReportTask;
        }

        public static void DisableMachExceptionHandler()
        {
        }

        public static void SetUserConfirmationHandler(Crashes.UserConfirmationHandler handler)
        {
            CrashesDelegate.SetShouldAwaitUserConfirmationHandler(handler);
        }

        public static void NotifyWithUserConfirmation(Crashes.ConfirmationResult answer)
        {
            _crashes.CallStatic("notifyUserConfirmation", ToJavaConfirmationResult(answer));
        }

        public static void StartCrashes()
        {
            AppCenterInternal.Start(AppCenter.Crashes);
        }

        private static int ToJavaConfirmationResult(Crashes.ConfirmationResult answer)
        {
            // Java values: SEND=0, DONT_SEND=1, ALWAYS_SEND=2
            // Crashes.ConfirmationResult values: SEND=1, DONT_SEND=0, ALWAYS_SEND=2
            switch (answer)
            {
                case Crashes.ConfirmationResult.Send:
                    return _crashes.GetStatic<int>("SEND");
                case Crashes.ConfirmationResult.AlwaysSend:
                    return _crashes.GetStatic<int>("ALWAYS_SEND");
                default:
                    return _crashes.GetStatic<int>("DONT_SEND");
            }
        }
    }
}
#endif
