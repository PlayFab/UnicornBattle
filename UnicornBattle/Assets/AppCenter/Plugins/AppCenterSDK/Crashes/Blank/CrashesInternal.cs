// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if (!UNITY_IOS && !UNITY_ANDROID && !UNITY_WSA_10_0) || UNITY_EDITOR
using System.Collections.Generic;
using Microsoft.AppCenter.Unity.Crashes.Models;
using Microsoft.AppCenter.Unity.Crashes;

namespace Microsoft.AppCenter.Unity.Crashes.Internal
{
#if UNITY_IOS
    using NativeType = System.IntPtr;
    using RawType = System.IntPtr;
#elif UNITY_ANDROID
    using NativeType = System.IntPtr;
    using RawType = UnityEngine.AndroidJavaObject;
#else
    using NativeType = System.Type;
    using RawType = System.Object;
#endif

    class CrashesInternal
    {
        public static void AddNativeType(List<NativeType> nativeTypes)
        {
        }

        public static void TrackException(RawType exception)
        {
        }

        public static void TrackException(RawType exception, IDictionary<string, string> properties)
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

        public static void DisableMachExceptionHandler()
        {
        }

        public static AppCenterTask<ErrorReport> GetLastSessionCrashReportAsync()
        {
            return AppCenterTask<ErrorReport>.FromCompleted(null);
        }

        public static void SetUserConfirmationHandler(Crashes.UserConfirmationHandler handler)
        {
        }

        public static void NotifyWithUserConfirmation(Crashes.ConfirmationResult answer)
        {
        }

        public static void StartCrashes()
        {
        }
    }
}
#endif