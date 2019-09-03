// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_IOS && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;

namespace Microsoft.AppCenter.Unity
{
    public static class DeviceHelper
    {
        public static Device Convert(IntPtr devicePtr)
        {
            if (devicePtr == IntPtr.Zero)
            {
                return null;
            }
            var sdkName = app_center_unity_device_sdk_name(devicePtr);
            var sdkVersion = app_center_unity_device_sdk_version(devicePtr);
            var model = app_center_unity_device_model(devicePtr);
            var oemName = app_center_unity_device_oem_name(devicePtr);
            var osName = app_center_unity_device_os_name(devicePtr);
            var osVersion = app_center_unity_device_os_version(devicePtr);
            var osBuild = app_center_unity_device_os_build(devicePtr);
            var osApiLevel = app_center_unity_device_os_api_level(devicePtr);
            var locale = app_center_unity_device_locale(devicePtr);
            var timeZoneOffset = app_center_unity_device_time_zone_offset(devicePtr);
            var screenSize = app_center_unity_device_screen_size(devicePtr);
            var appVersion = app_center_unity_device_app_version(devicePtr);
            var carrierName = app_center_unity_device_carrier_name(devicePtr);
            var carrierCountry = app_center_unity_device_carrier_country(devicePtr);
            var appBuild = app_center_unity_device_app_build(devicePtr);
            var appNamespace = app_center_unity_device_app_namespace(devicePtr);
            return new Device(sdkName, sdkVersion, model, oemName, osName, osVersion, osBuild, osApiLevel,
                              locale, timeZoneOffset, screenSize, appVersion, carrierName, carrierCountry,
                              appBuild, appNamespace);
        }

        [DllImport("__Internal")]
        private static extern string app_center_unity_device_sdk_name(IntPtr device);

        [DllImport("__Internal")]
        private static extern string app_center_unity_device_sdk_version(IntPtr device);

        [DllImport("__Internal")]
        private static extern string app_center_unity_device_model(IntPtr device);

        [DllImport("__Internal")]
        private static extern string app_center_unity_device_oem_name(IntPtr device);

        [DllImport("__Internal")]
        private static extern string app_center_unity_device_os_name(IntPtr device);

        [DllImport("__Internal")]
        private static extern string app_center_unity_device_os_version(IntPtr device);

        [DllImport("__Internal")]
        private static extern string app_center_unity_device_os_build(IntPtr device);

        [DllImport("__Internal")]
        private static extern int app_center_unity_device_os_api_level(IntPtr device);

        [DllImport("__Internal")]
        private static extern string app_center_unity_device_locale(IntPtr device);

        [DllImport("__Internal")]
        private static extern int app_center_unity_device_time_zone_offset(IntPtr device);

        [DllImport("__Internal")]
        private static extern string app_center_unity_device_screen_size(IntPtr device);

        [DllImport("__Internal")]
        private static extern string app_center_unity_device_app_version(IntPtr device);

        [DllImport("__Internal")]
        private static extern string app_center_unity_device_carrier_name(IntPtr device);

        [DllImport("__Internal")]
        private static extern string app_center_unity_device_carrier_country(IntPtr device);

        [DllImport("__Internal")]
        private static extern string app_center_unity_device_app_build(IntPtr device);

        [DllImport("__Internal")]
        private static extern string app_center_unity_device_app_namespace(IntPtr device);
    }
}
#endif
