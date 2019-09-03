// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

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

    public class WrapperException
    {
        private readonly RawType _rawObject;

        internal RawType GetRawObject()
        {
            return _rawObject;
        }

        public WrapperException()
        {
            _rawObject = WrapperExceptionInternal.Create();
        }

        public void SetType(string type)
        {
            WrapperExceptionInternal.SetType(_rawObject, type);
        }

        public void SetMessage(string message)
        {
            WrapperExceptionInternal.SetMessage(_rawObject, message);
        }

        public void SetStacktrace(string stacktrace)
        {
            WrapperExceptionInternal.SetStacktrace(_rawObject, stacktrace);
        }

        public void SetInnerException(RawType innerException)
        {
            WrapperExceptionInternal.SetInnerException(_rawObject, innerException);
        }

        public void SetWrapperSdkName(string sdkName)
        {
            WrapperExceptionInternal.SetWrapperSdkName(_rawObject, sdkName);
        }
    }
}