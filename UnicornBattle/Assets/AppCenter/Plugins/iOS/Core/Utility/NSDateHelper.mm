// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#import "NSDateHelper.h"
#import <Foundation/Foundation.h>

void* appcenter_unity_ns_date_convert(long interval)
{
    return (void *)CFBridgingRetain([[NSDate alloc] initWithTimeIntervalSince1970:interval]);
}
