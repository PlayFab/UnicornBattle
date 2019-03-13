// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#import <AppCenter/AppCenter.h>
#import "DeviceHelper.h"
#import "NSStringHelper.h"

const char* app_center_unity_device_sdk_name(void* device)
{
  MSDevice *deviceInfo = (__bridge MSDevice*)device;
  return appcenter_unity_ns_string_to_cstr([deviceInfo sdkName]);
}

const char* app_center_unity_device_sdk_version(void* device)
{
  MSDevice *deviceInfo = (__bridge MSDevice*)device;
  return appcenter_unity_ns_string_to_cstr([deviceInfo sdkVersion]);
}

const char* app_center_unity_device_model(void* device)
{
  MSDevice *deviceInfo = (__bridge MSDevice*)device;
  return appcenter_unity_ns_string_to_cstr([deviceInfo model]);
}

const char* app_center_unity_device_oem_name(void* device)
{
  MSDevice *deviceInfo = (__bridge MSDevice*)device;
  return appcenter_unity_ns_string_to_cstr([deviceInfo oemName]);
}

const char* app_center_unity_device_os_name(void* device)
{
  MSDevice *deviceInfo = (__bridge MSDevice*)device;
  return appcenter_unity_ns_string_to_cstr([deviceInfo osName]);
}

const char* app_center_unity_device_os_version(void* device)
{
  MSDevice *deviceInfo = (__bridge MSDevice*)device;
  return appcenter_unity_ns_string_to_cstr([deviceInfo osVersion]);
}

const char* app_center_unity_device_os_build(void* device)
{
  MSDevice *deviceInfo = (__bridge MSDevice*)device;
  return appcenter_unity_ns_string_to_cstr([deviceInfo osBuild]);
}

int app_center_unity_device_os_api_level(void* device)
{
  MSDevice *deviceInfo = (__bridge MSDevice*)device;
  return [[deviceInfo osApiLevel] intValue];
}

const char* app_center_unity_device_locale(void* device)
{
  MSDevice *deviceInfo = (__bridge MSDevice*)device;
  return appcenter_unity_ns_string_to_cstr([deviceInfo locale]);
}

int app_center_unity_device_time_zone_offset(void* device)
{
  MSDevice *deviceInfo = (__bridge MSDevice*)device;
  return [[deviceInfo timeZoneOffset] intValue];
}

const char* app_center_unity_device_screen_size(void* device)
{
  MSDevice *deviceInfo = (__bridge MSDevice*)device;
  return appcenter_unity_ns_string_to_cstr([deviceInfo screenSize]);
}

const char* app_center_unity_device_app_version(void* device)
{
  MSDevice *deviceInfo = (__bridge MSDevice*)device;
  return appcenter_unity_ns_string_to_cstr([deviceInfo appVersion]);
}

const char* app_center_unity_device_carrier_name(void* device)
{
  MSDevice *deviceInfo = (__bridge MSDevice*)device;
  return appcenter_unity_ns_string_to_cstr([deviceInfo carrierName]);
}

const char* app_center_unity_device_carrier_country(void* device)
{
  MSDevice *deviceInfo = (__bridge MSDevice*)device;
  return appcenter_unity_ns_string_to_cstr([deviceInfo carrierCountry]);
}

const char* app_center_unity_device_app_build(void* device)
{
  MSDevice *deviceInfo = (__bridge MSDevice*)device;
  return appcenter_unity_ns_string_to_cstr([deviceInfo appBuild]);
}

const char* app_center_unity_device_app_namespace(void* device)
{
  MSDevice *deviceInfo = (__bridge MSDevice*)device;
  return appcenter_unity_ns_string_to_cstr([deviceInfo appNamespace]);
}
