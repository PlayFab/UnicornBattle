// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#import <AppCenter/AppCenter.h>

extern "C" void appcenter_unity_set_log_level(int logLevel);
extern "C" int appcenter_unity_get_log_level();
extern "C" bool appcenter_unity_is_configured();
extern "C" void appcenter_unity_set_log_url(const char* logUrl);
extern "C" void appcenter_unity_set_user_id(char* userId);
extern "C" void appcenter_unity_set_enabled(bool isEnabled);
extern "C" bool appcenter_unity_is_enabled();
extern "C" const char* appcenter_unity_get_sdk_version();
extern "C" const char* appcenter_unity_get_install_id();
extern "C" void appcenter_unity_start(const char* appSecret, void** services, int count);
extern "C" void appcenter_unity_start_no_secret(void** services, int count);
extern "C" void appcenter_unity_start_from_library(void** services, int count);
extern "C" void appcenter_unity_set_custom_properties(MSCustomProperties* properties);
extern "C" void appcenter_unity_set_wrapper_sdk(const char* wrapperSdkVersion,
                                                const char* wrapperSdkName,
                                                const char* wrapperRuntimeVersion,
                                                const char* liveUpdateReleaseLabel,
                                                const char* liveUpdateDeploymentKey,
                                                const char* liveUpdatePackageHash);
extern "C" void appcenter_unity_set_storage_size(long size, void(* completionHandler)(bool));
