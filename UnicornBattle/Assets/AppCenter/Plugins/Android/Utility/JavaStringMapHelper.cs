// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_ANDROID
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.AppCenter.Unity.Internal.Utility
{
    public class JavaStringMapHelper
    {
        public static Dictionary<string, string> ConvertFromJava(AndroidJavaObject map)
        {
            var keySet = map.Call<AndroidJavaObject>("keySet");
            var keyArray = keySet.Call<AndroidJavaObject>("toArray");
            string[] keys = AndroidJNIHelper.ConvertFromJNIArray<string[]>(keyArray.GetRawObject());
            var dictionary = new Dictionary<string, string>();
            foreach (var key in keys)
            {
                var val = map.Call<string>("get", key);
                dictionary[key] = val;
            }
            return dictionary;
        }

        public static AndroidJavaObject ConvertToJava(IDictionary<string, string> properties)
        {
            if (properties == null)
            {
                return null;
            }
            string[] keys = properties.Keys.ToArray();
            string[] values = properties.Values.ToArray();
            int count = properties.Count;
            var javaMap = new AndroidJavaObject("java.util.HashMap");
            for (int i = 0; i < count; ++i)
            {
                javaMap.Call<AndroidJavaObject>("put", keys[i], values[i]);
            }
            return javaMap;
        }
    }
}
#endif
