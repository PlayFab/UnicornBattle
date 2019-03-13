// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#import "../Core/Utility/NSStringDictionaryHelper.h"
#import "MSException.h"
#import "CrashesUnity.h"
#import "CrashesDelegate.h"
#import <Foundation/Foundation.h>
#import <AppCenter/MSAppCenter.h>
#import "AppCenterCrashes/MSErrorAttachmentLog.h"
#import "NSStringHelper.h"

void* appcenter_unity_crashes_get_type()
{
  return (void *)CFBridgingRetain([MSCrashes class]);
}

void appcenter_unity_crashes_track_model_exception(MSException* exception)
{
  [MSCrashes trackModelException:exception];
}

void appcenter_unity_crashes_track_model_exception_with_properties(MSException* exception, char** keys, char** values, int count)
{
  NSDictionary<NSString*, NSString*> *properties = appcenter_unity_create_ns_string_dictionary(keys, values, count);
  [MSCrashes trackModelException:exception withProperties:properties];
}

void appcenter_unity_crashes_set_enabled(bool isEnabled)
{
  [MSCrashes setEnabled:isEnabled];
}

bool appcenter_unity_crashes_is_enabled()
{
  return [MSCrashes isEnabled];
}

void appcenter_unity_crashes_generate_test_crash()
{
  [MSCrashes generateTestCrash];
}

bool appcenter_unity_crashes_has_crashed_in_last_session()
{
  return [MSCrashes hasCrashedInLastSession];
}

void appcenter_unity_crashes_disable_mach_exception_handler()
{
  [MSCrashes disableMachExceptionHandler];
}

void appcenter_unity_crashes_set_user_confirmation_handler(bool(* userConfirmationHandler)())
{
    [MSCrashes setUserConfirmationHandler:^BOOL(NSArray<MSErrorReport *> *_Nonnull errorReports){
        return userConfirmationHandler();
    }];
}

void appcenter_unity_crashes_notify_with_user_confirmation(int userConfirmation)
{
    [MSCrashes notifyWithUserConfirmation:(MSUserConfirmation)userConfirmation];
}

void* appcenter_unity_crashes_last_session_crash_report()
{
    return (void *)CFBridgingRetain([MSCrashes lastSessionCrashReport]);
}

void appcenter_unity_start_crashes()
{
    [MSAppCenter startService:MSCrashes.class];
}

void* app_center_unity_crashes_get_error_attachment_log_text(char* text, char* fileName)
{
    return (void *)CFBridgingRetain(
                                    [[MSErrorAttachmentLog alloc] initWithFilename:appcenter_unity_cstr_to_ns_string(fileName) attachmentText:appcenter_unity_cstr_to_ns_string(text)]);
}

void* app_center_unity_crashes_get_error_attachment_log_binary(const void* data, int size, char* fileName, char* contentType)
{
    return (void *)CFBridgingRetain(
                                    [[MSErrorAttachmentLog alloc] initWithFilename:appcenter_unity_cstr_to_ns_string(fileName) attachmentBinary:[[NSData alloc] initWithBytes:data length:size] contentType:appcenter_unity_cstr_to_ns_string(contentType)]);
}

void* app_center_unity_create_error_attachments_array(MSErrorAttachmentLog* errorAttachment0, MSErrorAttachmentLog* errorAttachment1)
{
    NSArray<MSErrorAttachmentLog *>* errorAttachments = [NSArray arrayWithObjects:errorAttachment0, errorAttachment1, nil];
    return (void *)CFBridgingRetain(errorAttachments);
}
