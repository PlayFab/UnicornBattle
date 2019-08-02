// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_ANDROID
using UnityEngine;

namespace Microsoft.AppCenter.Unity.Internal.Utility
{
    public class JavaNumberHelper
    {
        public static AndroidJavaObject Convert(int val)
        {
            AndroidJavaObject javaInteger = new AndroidJavaObject("java.lang.Integer", val);
            return javaInteger;
        }

        public static AndroidJavaObject Convert(long val)
        {
            AndroidJavaObject javaLong = new AndroidJavaObject("java.lang.Long", val);
            return javaLong;
        }

        public static AndroidJavaObject Convert(float val)
        {
            AndroidJavaObject javaFloat = new AndroidJavaObject("java.lang.Float", val);
            return javaFloat;
        }

        public static AndroidJavaObject Convert(double val)
        {
            AndroidJavaObject javaDouble = new AndroidJavaObject("java.lang.Double", val);
            return javaDouble;
        }

        //TODO how to support decimal?
    }
}
#endif
