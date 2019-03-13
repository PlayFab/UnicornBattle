// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#if UNITY_IOS && !UNITY_EDITOR
using AOT;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.AppCenter.Unity.Crashes.Internal
{
    class CrashesInternal
    {
        public static void AddNativeType(List<IntPtr> nativeTypes)
        {
            nativeTypes.Add(appcenter_unity_crashes_get_type());
        }

        public static void TrackException(IntPtr exception)
        {
            appcenter_unity_crashes_track_model_exception(exception);
        }

        public static void TrackException(IntPtr exception, IDictionary<string, string> properties)
        {
            appcenter_unity_crashes_track_model_exception_with_properties(exception, properties.Keys.ToArray(), properties.Values.ToArray(), properties.Count);
        }

        public static AppCenterTask SetEnabledAsync(bool isEnabled)
        {
            appcenter_unity_crashes_set_enabled(isEnabled);
            return AppCenterTask.FromCompleted();
        }

        public static AppCenterTask<bool> IsEnabledAsync()
        {
            var isEnabled = appcenter_unity_crashes_is_enabled();
            return AppCenterTask<bool>.FromCompleted(isEnabled);
        }

        public static void GenerateTestCrash()
        {
            appcenter_unity_crashes_generate_test_crash();
        }

        public static AppCenterTask<bool> HasCrashedInLastSessionAsync()
        {
            var hasCrashedInLastSession = appcenter_unity_crashes_has_crashed_in_last_session();
            return AppCenterTask<bool>.FromCompleted(hasCrashedInLastSession);
        }

        public static void DisableMachExceptionHandler()
        {
            appcenter_unity_crashes_disable_mach_exception_handler();
        }

        public static AppCenterTask<ErrorReport> GetLastSessionCrashReportAsync()
        {
            var errorReportPtr = appcenter_unity_crashes_last_session_crash_report();
            var errorReport = GetErrorReportFromIntPtr(errorReportPtr);
            return AppCenterTask<ErrorReport>.FromCompleted(errorReport);
        }

        public static void SetUserConfirmationHandler(Crashes.UserConfirmationHandler handler)
        {
            appcenter_unity_crashes_set_user_confirmation_handler(handler);
        }

        public static void NotifyWithUserConfirmation(Crashes.ConfirmationResult answer)
        {
            appcenter_unity_crashes_notify_with_user_confirmation((int)answer);
        }

        public static void StartCrashes()
        {
            appcenter_unity_start_crashes();
        }

        public static ErrorReport GetErrorReportFromIntPtr(IntPtr errorReportPtr)
        {
            if (errorReportPtr == IntPtr.Zero)
            {
                return null;
            }
            var procId = app_center_unity_crashes_error_report_app_process_identifier(errorReportPtr);
            var identifier = app_center_unity_crashes_error_report_incident_identifier(errorReportPtr);
            var exceptionName = app_center_unity_crashes_error_report_exception_name(errorReportPtr);
            var reporterKey = app_center_unity_crashes_error_report_reporter_key(errorReportPtr);
            var reporterSignal = app_center_unity_crashes_error_report_signal(errorReportPtr);
            var exceptionReason = app_center_unity_crashes_error_report_exception_reason(errorReportPtr);
            var startTime = app_center_unity_crashes_error_report_app_start_time(errorReportPtr);
            DateTimeOffset dtoStart = DateTimeOffset.UtcNow;
            if (startTime != null)
            {
                dtoStart = DateTimeOffset.Parse(startTime);
            }
            var errorTime = app_center_unity_crashes_error_report_app_error_time(errorReportPtr);
            DateTimeOffset dtoError = DateTimeOffset.UtcNow;
            if (errorTime != null)
            {
                dtoError = DateTimeOffset.Parse(errorTime);
            }
            var isAppKill = app_center_unity_crashes_error_report_is_app_kill(errorReportPtr);
            var exception = new Models.Exception(exceptionName + ": " + exceptionReason, "");
            var device = GetDevice(errorReportPtr);
            return new ErrorReport(identifier, dtoStart, dtoError, exception, procId, reporterKey, reporterSignal, isAppKill, device);
        }

        private static Device GetDevice(IntPtr errorReportPtr)
        {
            var device = app_center_unity_crashes_error_report_device(errorReportPtr);
            return DeviceHelper.Convert(device);
        }

#region External

        [DllImport("__Internal")]
        private static extern IntPtr appcenter_unity_crashes_get_type();

        [DllImport("__Internal")]
        private static extern void appcenter_unity_crashes_track_model_exception(IntPtr exception);

        [DllImport("__Internal")]
        private static extern void appcenter_unity_crashes_track_model_exception_with_properties(IntPtr exception, string[] keys, string[] values, int count);

        [DllImport("__Internal")]
        private static extern void appcenter_unity_crashes_set_enabled(bool isEnabled);

        [DllImport("__Internal")]
        private static extern bool appcenter_unity_crashes_is_enabled();

        [DllImport("__Internal")]
        private static extern void appcenter_unity_crashes_generate_test_crash();

        [DllImport("__Internal")]
        private static extern bool appcenter_unity_crashes_has_crashed_in_last_session();

        [DllImport("__Internal")]
        private static extern void appcenter_unity_crashes_disable_mach_exception_handler();

        [DllImport("__Internal")]
        private static extern IntPtr appcenter_unity_crashes_last_session_crash_report();

        [DllImport("__Internal")]
        private static extern void appcenter_unity_crashes_set_user_confirmation_handler(Crashes.UserConfirmationHandler handler);

        [DllImport("__Internal")]
        private static extern void appcenter_unity_crashes_notify_with_user_confirmation(int code);

        [DllImport("__Internal")]
        private static extern string app_center_unity_crashes_error_report_exception_name(IntPtr errorReport);

        [DllImport("__Internal")]
        private static extern int app_center_unity_crashes_error_report_app_process_identifier(IntPtr errorReport);

        [DllImport("__Internal")]
        private static extern string app_center_unity_crashes_error_report_incident_identifier(IntPtr errorReport);

        [DllImport("__Internal")]
        private static extern string app_center_unity_crashes_error_report_reporter_key(IntPtr errorReport);

        [DllImport("__Internal")]
        private static extern string app_center_unity_crashes_error_report_signal(IntPtr errorReport);

        [DllImport("__Internal")]
        private static extern string app_center_unity_crashes_error_report_exception_reason(IntPtr errorReport);

        [DllImport("__Internal")]
        private static extern string app_center_unity_crashes_error_report_app_start_time(IntPtr errorReport);

        [DllImport("__Internal")]
        private static extern string app_center_unity_crashes_error_report_app_error_time(IntPtr errorReport);

        [DllImport("__Internal")]
        private static extern bool app_center_unity_crashes_error_report_is_app_kill(IntPtr errorReport);

        [DllImport("__Internal")]
        private static extern IntPtr app_center_unity_crashes_error_report_device(IntPtr errorReport);

        [DllImport("__Internal")]
        private static extern void appcenter_unity_start_crashes();

#endregion
    }
}
#endif