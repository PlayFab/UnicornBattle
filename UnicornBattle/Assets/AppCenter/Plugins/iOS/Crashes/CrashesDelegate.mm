// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#import "CrashesDelegate.h"
#import <AppCenterCrashes/AppCenterCrashes.h>
#import <Foundation/Foundation.h>

static bool (*shouldProcessErrorReport)(MSErrorReport *);
static NSArray<MSErrorAttachmentLog *>* (*getErrorAttachments)(MSErrorReport *);
static void (*sendingErrorReport)(MSErrorReport *);
static void (*sentErrorReport)(MSErrorReport *);
static void (*failedToSendErrorReport)(MSErrorReport *, NSError *);

// we need static instance var because we have weak reaf in native part
static UnityCrashesDelegate *unityCrashesDelegate = NULL;

void app_center_unity_crashes_set_delegate()
{
    unityCrashesDelegate = [[UnityCrashesDelegate alloc] init];
    [MSCrashes setDelegate:unityCrashesDelegate];
}

void app_center_unity_crashes_delegate_set_should_process_error_report_delegate(bool(*handler)(MSErrorReport *))
{
    shouldProcessErrorReport = handler;
}

void app_center_unity_crashes_delegate_set_get_error_attachments_delegate(NSArray<MSErrorAttachmentLog *> *(*handler)(MSErrorReport *))
{
    getErrorAttachments = handler;
}

void app_center_unity_crashes_delegate_set_sending_error_report_delegate(void(*handler)(MSErrorReport *))
{
    sendingErrorReport = handler;
}

void app_center_unity_crashes_delegate_set_sent_error_report_delegate(void(*handler)(MSErrorReport *))
{
    sentErrorReport = handler;
}

void app_center_unity_crashes_delegate_set_failed_to_send_error_report_delegate(void(*handler)(MSErrorReport *, NSError *))
{
    failedToSendErrorReport = handler;
}

@implementation UnityCrashesDelegate

-(BOOL)crashes:(MSCrashes *)crashes shouldProcessErrorReport:(MSErrorReport *)errorReport
{
    if (shouldProcessErrorReport)
    {
        return (*shouldProcessErrorReport)(errorReport);
    }
    else
    {
        return true;
    }
}

- (NSArray<MSErrorAttachmentLog *> *)attachmentsWithCrashes:(MSCrashes *)crashes forErrorReport:(MSErrorReport *)errorReport
{
    if (getErrorAttachments)
    {
        return (*getErrorAttachments)(errorReport);
    }
    else
    {
        return nil;
    }
}

- (void)crashes:(MSCrashes *)crashes willSendErrorReport:(MSErrorReport *)errorReport
{
    if (sendingErrorReport)
    {
        (*sendingErrorReport)(errorReport);
    }
}

- (void)crashes:(MSCrashes *)crashes didSucceedSendingErrorReport:(MSErrorReport *)errorReport
{
    if (sentErrorReport)
    {
        (*sentErrorReport)(errorReport);
    }
}

- (void)crashes:(MSCrashes *)crashes didFailSendingErrorReport:(MSErrorReport *)errorReport withError:(NSError *)error
{
    if (failedToSendErrorReport)
    {
        (*failedToSendErrorReport)(errorReport, error);
    }
}

@end
