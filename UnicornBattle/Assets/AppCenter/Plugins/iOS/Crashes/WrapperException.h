// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#import "MSException.h"
#import <Foundation/Foundation.h>

// Don't need to return value because reference is kept by wrapper
extern "C" MSException* appcenter_unity_exception_create();
extern "C" void appcenter_unity_exception_set_type(MSException* exception, char* type);
extern "C" void appcenter_unity_exception_set_message(MSException* exception, char* message);
extern "C" void appcenter_unity_exception_set_stacktrace(MSException* exception, char* stacktrace);
extern "C" void appcenter_unity_exception_set_inner_exception(MSException* exception, MSException* innerException);
extern "C" void appcenter_unity_exception_set_wrapper_sdk_name(MSException* exception, char* sdkName);
