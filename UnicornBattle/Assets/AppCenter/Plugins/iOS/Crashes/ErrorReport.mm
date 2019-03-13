// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#import "../Core/Utility/NSStringHelper.h"
#import "ErrorReport.h"
#import <AppCenterCrashes/AppCenterCrashes.h>

const char* app_center_unity_crashes_error_report_incident_identifier(void* errorReport)
{
  MSErrorReport *report = (__bridge MSErrorReport*)errorReport;
  return appcenter_unity_ns_string_to_cstr([report incidentIdentifier]);
}

const char* app_center_unity_crashes_error_report_reporter_key(void* errorReport)
{
  MSErrorReport *report = (__bridge MSErrorReport*)errorReport;
  return appcenter_unity_ns_string_to_cstr([report reporterKey]);
}

const char* app_center_unity_crashes_error_report_signal(void* errorReport)
{
  MSErrorReport *report = (__bridge MSErrorReport*)errorReport;
  return appcenter_unity_ns_string_to_cstr([report signal]);
}

const char* app_center_unity_crashes_error_report_exception_name(void* errorReport)
{
  MSErrorReport *report = (__bridge MSErrorReport*)errorReport;
  return appcenter_unity_ns_string_to_cstr([report exceptionName]);
}

const char* app_center_unity_crashes_error_report_exception_reason(void* errorReport)
{
  MSErrorReport *report = (__bridge MSErrorReport*)errorReport;
  return appcenter_unity_ns_string_to_cstr([report exceptionReason]);
}

const char* app_center_unity_crashes_error_report_app_start_time(void* errorReport)
{
  MSErrorReport *report = (__bridge MSErrorReport*)errorReport;
  NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
  [dateFormatter setDateFormat:@"yyyy-MM-dd HH:mm:ss ZZZZZ"];
  NSString *dateStr = [dateFormatter stringFromDate:report.appStartTime];
  return appcenter_unity_ns_string_to_cstr(dateStr);
}

const char* app_center_unity_crashes_error_report_app_error_time(void* errorReport)
{
  MSErrorReport *report = (__bridge MSErrorReport*)errorReport;
  NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
  [dateFormatter setDateFormat:@"yyyy-MM-dd HH:mm:ss ZZZZZ"];
  NSString *dateStr = [dateFormatter stringFromDate:report.appErrorTime];
  return appcenter_unity_ns_string_to_cstr(dateStr);
}

extern "C" void* app_center_unity_crashes_error_report_device(void* errorReport)
{
  MSErrorReport *report = (__bridge MSErrorReport*)errorReport;
  return (__bridge void*)[report device];
}

extern "C" unsigned int app_center_unity_crashes_error_report_app_process_identifier(void* errorReport)
{
  MSErrorReport *report = (__bridge MSErrorReport*)errorReport;
  return [report appProcessIdentifier];
}

extern "C" bool app_center_unity_crashes_error_report_is_app_kill(void* errorReport)
{
  MSErrorReport *report = (__bridge MSErrorReport*)errorReport;
  return [report isAppKill];
}
