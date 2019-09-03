// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_IOS
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

namespace Microsoft.AppCenter.Unity.Internal.Utility
{
    public class NSStringDictionaryHelper
    {
        public static Dictionary<string, string> NSDictionaryConvert(IntPtr nsDictionary)
        {
            var length = appcenter_unity_ns_dictionary_get_length(nsDictionary);
            var dictionary = new Dictionary<string, string>();
            for (int i = 0; i < length; ++i)
            {
                var key = appcenter_unity_ns_string_dictionary_get_key_at_idx(nsDictionary, i);
                var value = appcenter_unity_ns_string_dictionary_get_value_for_key(nsDictionary, key);
                dictionary[key] = value;
            }
            return dictionary;
        }

        [DllImport("__Internal")]
        private static extern string appcenter_unity_ns_string_dictionary_get_key_at_idx(IntPtr dictionary, int idx);

        [DllImport("__Internal")]
        private static extern string appcenter_unity_ns_string_dictionary_get_value_for_key(IntPtr dictionary, string key);

        [DllImport("__Internal")]
        private static extern uint appcenter_unity_ns_dictionary_get_length(IntPtr dictionary);
    }
}
#endif
