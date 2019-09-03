// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_IOS && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;

namespace Microsoft.AppCenter.Unity.Crashes.Internal
{
    class WrapperExceptionInternal
    {
        public static IntPtr Create()
        {
            return appcenter_unity_exception_create();
        }

        public static void SetType(IntPtr exception, string type)
        {
            appcenter_unity_exception_set_type(exception, type);
        }

        public static void SetMessage(IntPtr exception, string message)
        {
            appcenter_unity_exception_set_message(exception, message);
        }

        public static void SetStacktrace(IntPtr exception, string stacktrace)
        {
            appcenter_unity_exception_set_stacktrace(exception, stacktrace);
        }

        public static void SetInnerException(IntPtr exception, IntPtr innerExcetion)
        {
            appcenter_unity_exception_set_inner_exception(exception, innerExcetion);
        }

        public static void SetWrapperSdkName(IntPtr exception, string sdkName)
        {
            appcenter_unity_exception_set_wrapper_sdk_name(exception, sdkName);
        }

        #region External

        [DllImport("__Internal")]
        private static extern IntPtr appcenter_unity_exception_create();

        [DllImport("__Internal")]
        private static extern void appcenter_unity_exception_set_type(IntPtr exception, string type);

        [DllImport("__Internal")]
        private static extern void appcenter_unity_exception_set_message(IntPtr exception, string message);

        [DllImport("__Internal")]
        private static extern void appcenter_unity_exception_set_stacktrace(IntPtr exception, string stacktrace);

        [DllImport("__Internal")]
        private static extern void appcenter_unity_exception_set_inner_exception(IntPtr exception, IntPtr innerExcetion);

        [DllImport("__Internal")]
        private static extern void appcenter_unity_exception_set_wrapper_sdk_name(IntPtr exception, string key);

        #endregion
    }
}
#endif