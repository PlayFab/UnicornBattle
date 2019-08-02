// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#ifndef NS_NUMBER_HELPER_H
#define NS_NUMBER_HELPER_H

#import <Foundation/Foundation.h>

extern "C" void* appcenter_unity_nsnumber_convert_int(int val);
extern "C" NSNumber* appcenter_unity_nsnumber_convert_long(long val);
extern "C" NSNumber* appcenter_unity_nsnumber_convert_float(float val);
extern "C" NSNumber* appcenter_unity_nsnumber_convert_double(double val);

#endif
