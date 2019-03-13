// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#import "AppCenterUnity.h"
#import "Utility/NSStringHelper.h"
#import <AppCenter/AppCenter.h>
#import <Foundation/Foundation.h>

NSMutableArray<Class>* get_services_array(void** services, int count) {
  NSMutableArray<Class>* servicesArray = [NSMutableArray new];
  for (int i = 0; i < count; i++) {
    [servicesArray addObject:(Class)CFBridgingRelease(services[i])];
  }
  return servicesArray;
}

void appcenter_unity_set_log_level(int logLevel)
{
  [MSAppCenter setLogLevel:(MSLogLevel)logLevel];
}

int appcenter_unity_get_log_level()
{
  return (int)MSAppCenter.logLevel;
}

bool appcenter_unity_is_configured()
{
  return [MSAppCenter isConfigured];
}

void appcenter_unity_set_log_url(const char* logUrl)
{
  [MSAppCenter setLogUrl:appcenter_unity_cstr_to_ns_string(logUrl)];
}

void appcenter_unity_set_user_id(char* userId)
{
  [MSAppCenter setUserId: appcenter_unity_cstr_to_ns_string(userId)];
}

void appcenter_unity_set_enabled(bool isEnabled)
{
  [MSAppCenter setEnabled:isEnabled];
}

void appcenter_unity_start(const char* appSecret, void** services, int count) {
  NSMutableArray<Class>* servicesArray = get_services_array(services, count);
  [MSAppCenter start:appcenter_unity_cstr_to_ns_string(appSecret) withServices:servicesArray];
}

void appcenter_unity_start_no_secret(void** services, int count) {
  NSMutableArray<Class>* servicesArray = get_services_array(services, count);
  [MSAppCenter startWithServices:servicesArray];
}

void appcenter_unity_start_from_library(void** services, int count) {
  NSMutableArray<Class>* servicesArray = get_services_array(services, count);
  [MSAppCenter startFromLibraryWithServices:servicesArray];
}

bool appcenter_unity_is_enabled()
{
  return [MSAppCenter isEnabled];
}

const char* appcenter_unity_get_install_id()
{
  NSString *uuidString = [[MSAppCenter installId] UUIDString];
  return appcenter_unity_ns_string_to_cstr(uuidString);
}

const char* appcenter_unity_get_sdk_version()
{
  return appcenter_unity_ns_string_to_cstr([MSAppCenter sdkVersion]);
}

void appcenter_unity_set_custom_properties(MSCustomProperties* properties)
{
  [MSAppCenter setCustomProperties:properties];
}

void appcenter_unity_set_wrapper_sdk(const char* wrapperSdkVersion,
                                     const char* wrapperSdkName,
                                     const char* wrapperRuntimeVersion,
                                     const char* liveUpdateReleaseLabel,
                                     const char* liveUpdateDeploymentKey,
                                     const char* liveUpdatePackageHash)
{
  MSWrapperSdk *wrapperSdk = [[MSWrapperSdk alloc]
                              initWithWrapperSdkVersion:appcenter_unity_cstr_to_ns_string(wrapperSdkVersion)
                                         wrapperSdkName:appcenter_unity_cstr_to_ns_string(wrapperSdkName)
                                  wrapperRuntimeVersion:appcenter_unity_cstr_to_ns_string(wrapperRuntimeVersion)
                                 liveUpdateReleaseLabel:appcenter_unity_cstr_to_ns_string(liveUpdateReleaseLabel)
                                liveUpdateDeploymentKey:appcenter_unity_cstr_to_ns_string(liveUpdateDeploymentKey)
                                  liveUpdatePackageHash:appcenter_unity_cstr_to_ns_string(liveUpdatePackageHash)];
  [MSAppCenter setWrapperSdk:wrapperSdk];
}

void appcenter_unity_set_storage_size(long size, void(* completionHandler)(bool))
{
    [MSAppCenter setMaxStorageSize:size completionHandler:^void(BOOL result){
        completionHandler(result);
    }];
}
