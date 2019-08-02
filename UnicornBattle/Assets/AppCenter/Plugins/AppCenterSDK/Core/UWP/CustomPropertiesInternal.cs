// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_WSA_10_0 && !UNITY_EDITOR
using System;

namespace Microsoft.AppCenter.Unity.Internal
{
    using UWPCustomProperties = Microsoft.AppCenter.CustomProperties;

    class CustomPropertiesInternal
    {
       public static object Create()
        {
            return new UWPCustomProperties();
        }

        public static void SetString(object properties, string key, string val)
        {
            var uwpProperties = properties as UWPCustomProperties;
            uwpProperties.Set(key, val);
        }

        public static void SetNumber(object properties, string key, int val)
        {
            var uwpProperties = properties as UWPCustomProperties;
            uwpProperties.Set(key, val);
        }

        public static void SetNumber(object properties, string key, long val)
        {
            var uwpProperties = properties as UWPCustomProperties;
            uwpProperties.Set(key, val);
        }

        public static void SetNumber(object properties, string key, float val)
        {
            var uwpProperties = properties as UWPCustomProperties;
            uwpProperties.Set(key, val);
        }

        public static void SetNumber(object properties, string key, double val)
        {
            var uwpProperties = properties as UWPCustomProperties;
            uwpProperties.Set(key, val);
        }

        public static void SetBool(object properties, string key, bool val)
        {
            var uwpProperties = properties as UWPCustomProperties;
            uwpProperties.Set(key, val);
        }

        public static void SetDate(object properties, string key, DateTime val)
        {
            var uwpProperties = properties as UWPCustomProperties;
            uwpProperties.Set(key, val);
        }

        public static void Clear(object properties, string key)
        {
            var uwpProperties = properties as UWPCustomProperties;
            uwpProperties.Clear(key);
        }
    }
}
#endif
