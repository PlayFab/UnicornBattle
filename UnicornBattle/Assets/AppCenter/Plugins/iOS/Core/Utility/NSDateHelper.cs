// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_IOS

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Microsoft.AppCenter.Unity.Internal.Utility
{
    public class NSDateHelper
    {
        public static IntPtr DateTimeConvert(DateTime date)
        {
            var unixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var timeSpan = date - unixStartTime;
            var interval = (long)(timeSpan.TotalSeconds);
            return appcenter_unity_ns_date_convert(interval);
        }

        [DllImport("__Internal")]
        private static extern IntPtr appcenter_unity_ns_date_convert(long interval);
    }
}
#endif
