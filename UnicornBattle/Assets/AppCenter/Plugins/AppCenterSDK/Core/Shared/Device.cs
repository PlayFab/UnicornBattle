// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.AppCenter.Unity
{
    public class Device
    {
        /// <summary>
        /// Name of the SDK.
        /// </summary>
        public string SdkName { get; private set; }

        /// <summary>
        /// Version of the SDK.
        /// </summary>
        public string SdkVersion { get; private set; }

        /// <summary>
        /// Device model (example: iPad2,3).
        /// </summary>
        public string Model { get; private set; }

        /// <summary>
        /// Device manufacturer (example: HTC).
        /// </summary>
        public string OemName { get; private set; }

        /// <summary>
        /// OS name (example: iOS).
        /// </summary>
        public string OsName { get; private set; }

        /// <summary>
        /// OS version (example: 9.3.0).
        /// </summary>
        public string OsVersion { get; private set; }

        /// <summary>
        /// OS build code (example: LMY47X).
        /// </summary>
        public string OsBuild { get; private set; }

        /// <summary>
        /// API level when applicable like in Android (example: 15).
        /// </summary>
        public int OsApiLevel { get; private set; }

        /// <summary>
        /// Language code (example: en_US).
        /// </summary>
        public string Locale { get; private set; }

        /// <summary>
        /// The offset in minutes from UTC for the device time zone, including daylight savings time.
        /// </summary>
        public int TimeZoneOffset { get; private set; }

        /// <summary>
        /// Screen size of the device in pixels (example: 640x480).
        /// </summary>
        public string ScreenSize { get; private set; }

        /// <summary>
        /// Application version name.
        /// </summary>
        public string AppVersion { get; private set; }

        /// <summary>
        /// Carrier name (for mobile devices).
        /// </summary>
        public string CarrierName { get; private set; }

        /// <summary>
        /// Carrier country code (for mobile devices).
        /// </summary>
        public string CarrierCountry { get; private set; }

        /// <summary>
        /// The app's build number, e.g. 42.
        /// </summary>
        public string AppBuild { get; private set; }

        /// <summary>
        /// The bundle identifier, package identifier, or namespace, depending on what the individual platforms use, .e.g com.microsoft.example.
        /// </summary>
        public string AppNamespace { get; private set; }

        public Device(string sdkName, string sdkVersion, string model, string oemName, string osName, string osVersion, string osBuild,
            int osApiLevel, string locale, int timeZoneOffset, string screenSize, string appVersion, string carrierName,
            string carrierCountry, string appBuild, string appNamespace)
        {
            SdkName = sdkName;
            SdkVersion = sdkVersion;
            Model = model;
            OemName = oemName;
            OsName = osName;
            OsVersion = osVersion;
            OsBuild = osBuild;
            OsApiLevel = osApiLevel;
            Locale = locale;
            TimeZoneOffset = timeZoneOffset;
            ScreenSize = screenSize;
            AppVersion = appVersion;
            CarrierName = carrierName;
            CarrierCountry = carrierCountry;
            AppBuild = appBuild;
            AppNamespace = appNamespace;
        }
    }
}
