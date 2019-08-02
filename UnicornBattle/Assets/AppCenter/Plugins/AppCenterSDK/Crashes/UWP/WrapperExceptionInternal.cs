// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_WSA_10_0 && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;

namespace Microsoft.AppCenter.Unity.Crashes.Internal
{
    class WrapperExceptionInternal
    {
        public static object Create()
        {
            return new object();
        }

        public static void SetType(object exception, string type)
        {
        }

        public static void SetMessage(object exception, string message)
        {
        }

        public static void SetStacktrace(object exception, string stacktrace)
        {
        }

        public static void SetInnerException(object exception, object innerException)
        {
        }

        public static void SetWrapperSdkName(object exception, string sdkName)
        {
        }
    }
}
#endif