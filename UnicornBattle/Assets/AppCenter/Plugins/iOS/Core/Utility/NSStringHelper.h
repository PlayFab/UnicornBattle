// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#ifndef NS_STRING_HELPER_H
#define NS_STRING_HELPER_H

#import <Foundation/Foundation.h>

extern "C" const char* appcenter_unity_ns_string_to_cstr(NSString* nsstring);
extern "C" NSString* appcenter_unity_cstr_to_ns_string(const char* str);

#endif
