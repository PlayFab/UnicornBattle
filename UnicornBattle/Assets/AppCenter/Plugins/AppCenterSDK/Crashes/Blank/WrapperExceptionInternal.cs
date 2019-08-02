// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if (!UNITY_IOS && !UNITY_ANDROID && !UNITY_WSA_10_0) || UNITY_EDITOR
using Microsoft.AppCenter.Unity.Crashes.Internal;
using System;

namespace Microsoft.AppCenter.Unity.Crashes
{
#if UNITY_IOS
    using RawType = System.IntPtr;
#elif UNITY_ANDROID
    using RawType = UnityEngine.AndroidJavaObject;
#else
    using RawType = System.Object;
#endif

    public class WrapperExceptionInternal
    {
        public static RawType Create()
        {
            return default(RawType);
        }

        public static void SetType(RawType exception, string type)
        {
        }

        public static void SetMessage(RawType exception, string message)
        {
        }

        public static void SetStacktrace(RawType exception, string stacktrace)
        {
        }

        public static void SetInnerException(RawType exception, RawType innerException)
        {
        }

        public static void SetWrapperSdkName(RawType exception, string sdkName)
        {
        }
    }
}
#endif