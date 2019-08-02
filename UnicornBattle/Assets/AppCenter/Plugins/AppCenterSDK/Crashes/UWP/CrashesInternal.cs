// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_WSA_10_0 && !UNITY_EDITOR
using Microsoft.AppCenter.Unity.Crashes.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Unity.Crashes;

namespace Microsoft.AppCenter.Unity.Crashes.Internal
{
    class CrashesInternal
    {
        public static void AddNativeType(List<Type> nativeTypes)
        {
        }

        public static void TrackException(object exception)
        {
        }

        public static void TrackException(object exception, IDictionary<string, string> properties)
        {
        }

        public static AppCenterTask SetEnabledAsync(bool enabled)
        {
            return AppCenterTask.FromCompleted();
        }

        public static AppCenterTask<bool> IsEnabledAsync()
        {
            return AppCenterTask<bool>.FromCompleted(false);
        }

        public static void GenerateTestCrash()
        {
        }

        public static AppCenterTask<bool> HasCrashedInLastSessionAsync()
        {
            return AppCenterTask<bool>.FromCompleted(false);
        }

        public static AppCenterTask<ErrorReport> GetLastSessionCrashReportAsync()
        {
            return AppCenterTask<ErrorReport>.FromCompleted(null);
        }

        public static void DisableMachExceptionHandler()
        {
        }

        private class Crashes
        {
        }

        public static void SetUserConfirmationHandler(Unity.Crashes.Crashes.UserConfirmationHandler handler)
        {
        }

        public static void NotifyWithUserConfirmation(Unity.Crashes.Crashes.ConfirmationResult answer)
        {
        }

        public static void StartCrashes()
        {
        }
    }
}
#endif