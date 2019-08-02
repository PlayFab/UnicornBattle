// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_ANDROID
using System;
using System.Globalization;
using UnityEngine;

namespace Microsoft.AppCenter.Unity.Internal.Utility
{
    public class JavaDateHelper
    {
        private const string DotNetDateFormat = "yyyy-MM-dd'T'HH:mm:ss.fffK";

        private static AndroidJavaObject _javaDateFormatter;
        private static AndroidJavaObject JavaDateFormatter
        {
            get
            {
                if (_javaDateFormatter == null)
                {
                    _javaDateFormatter = new AndroidJavaObject("java.text.SimpleDateFormat", "yyyy-MM-dd'T'HH:mm:ss.SSSXXX");
                }
                return _javaDateFormatter;
            }
        }

        public static AndroidJavaObject DateTimeConvert(DateTime date)
        {
            var dateString = date.ToString(DotNetDateFormat);
            return JavaDateFormatter.Call<AndroidJavaObject>("parse", dateString);
        }

        public static DateTimeOffset DateTimeConvert(AndroidJavaObject date)
        {
            // Unable to use DateTimeOffset.ParseExact(dateString, DotNetDateFormat, CultureInfo.InvariantCulture) here
            // because it throws "Invalid format string" exception
            var dateString = JavaDateFormatter.Call<string>("format", date);
            var dateTime = DateTime.ParseExact(dateString, DotNetDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            return new DateTimeOffset(dateTime);
        }
    }
}
#endif
