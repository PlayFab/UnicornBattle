// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#import <AppCenter/AppCenter.h>
#import <Foundation/Foundation.h>

// Don't need to return value because reference is kept by wrapper
extern "C" MSCustomProperties* appcenter_unity_custom_properties_create();
extern "C" void appcenter_unity_custom_properties_set_string(MSCustomProperties* properties, char* key, char* val);
extern "C" void appcenter_unity_custom_properties_set_number(MSCustomProperties* properties, char* key, NSNumber* val);
extern "C" void appcenter_unity_custom_properties_set_bool(MSCustomProperties* properties, char* key, bool val);
extern "C" void appcenter_unity_custom_properties_set_date(MSCustomProperties* properties, char* key, NSDate* val);
extern "C" void appcenter_unity_custom_properties_clear(MSCustomProperties* properties, char* key);
