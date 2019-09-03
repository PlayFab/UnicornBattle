// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#ifndef CRASHES_UNITY_H
#define CRASHES_UNITY_H

#import <AppCenterCrashes/AppCenterCrashes.h>

extern "C" void* appcenter_unity_crashes_get_type();
extern "C" void appcenter_unity_crashes_track_model_exception(MSException* exception);
extern "C" void appcenter_unity_crashes_track_model_exception_with_properties(MSException* exception, char** keys, char** values, int count);
extern "C" void appcenter_unity_crashes_set_enabled(bool isEnabled);
extern "C" bool appcenter_unity_crashes_is_enabled();
extern "C" void appcenter_unity_crashes_generate_test_crash();
extern "C" bool appcenter_unity_crashes_has_crashed_in_last_session();
extern "C" void appcenter_unity_crashes_disable_mach_exception_handler();
extern "C" void app_center_unity_crashes_set_user_confirmation_handler(void* userConfirmationHandler);
extern "C" void* appcenter_unity_crashes_last_session_crash_report();
extern "C" void appcenter_unity_crashes_set_user_confirmation_handler(bool(* userConfirmationHandler)());
extern "C" void appcenter_unity_crashes_notify_with_user_confirmation(int userConfirmation);
extern "C" void appcenter_unity_start_crashes();
extern "C" void* app_center_unity_crashes_get_error_attachment_log_text(char* text, char* fileName);
extern "C" void* app_center_unity_crashes_get_error_attachment_log_binary(const void* data, int size, char* fileName, char* contentType);
extern "C" void* app_center_unity_create_error_attachments_array(MSErrorAttachmentLog* errorAttachment0, MSErrorAttachmentLog* errorAttachment1);

#endif
