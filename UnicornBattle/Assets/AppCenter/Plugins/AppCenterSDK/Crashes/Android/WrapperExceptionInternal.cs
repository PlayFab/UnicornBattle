// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Microsoft.AppCenter.Unity.Crashes.Internal
{
    class WrapperExceptionInternal
    {
        public static AndroidJavaObject Create()
        {
            return new AndroidJavaObject("com.microsoft.appcenter.crashes.ingestion.models.Exception");
        }

        public static void SetType(AndroidJavaObject exception, string type)
        {
            exception.Call("setType", type);
        }

        public static void SetMessage(AndroidJavaObject exception, string message)
        {
            exception.Call("setMessage", message);
        }

        public static void SetStacktrace(AndroidJavaObject exception, string stacktrace)
        {
            exception.Call("setStackTrace", stacktrace);
        }

        public static void SetInnerException(AndroidJavaObject exception, AndroidJavaObject innerException)
        {
            var javaList = new AndroidJavaObject("java.util.LinkedList");
            javaList.Call("addLast", innerException);
            exception.Call("setInnerExceptions", javaList);
        }

        public static void SetWrapperSdkName(AndroidJavaObject exception, string sdkName)
        {
            exception.Call("setWrapperSdkName", sdkName);
        }
    }
}
#endif