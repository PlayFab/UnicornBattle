// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.AppCenter.Unity.Crashes.Internal;
using Microsoft.AppCenter.Unity.Internal.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Microsoft.AppCenter.Unity.Crashes
{
#if UNITY_IOS || UNITY_ANDROID
    using RawType = System.IntPtr;
#else
    using RawType = System.Type;
#endif

    public class Crashes
    {
        // Used by App Center Unity Editor Extensions: https://github.com/Microsoft/AppCenter-SDK-Unity-Extension
        public const string CrashesSDKVersion = "1.4.0";
        private static bool _reportUnhandledExceptions = false;
        private static readonly object _objectLock = new object();

#if UNITY_ANDROID
        private static Queue<Exception> _unhandledExceptions = new Queue<Exception>();
#endif

        public static void PrepareEventHandlers()
        {
            AppCenterBehavior.InitializingServices += Initialize;
            AppCenterBehavior.InitializedAppCenterAndServices += HandleAppCenterInitialized;
        }

        public static void Initialize()
        {
            CrashesDelegate.SetDelegate();
        }

        public static void AddNativeType(List<RawType> nativeTypes)
        {
            CrashesInternal.AddNativeType(nativeTypes);
        }

        public static void TrackError(Exception exception, IDictionary<string, string> properties = null)
        {
            if (exception != null)
            {
                var exceptionWrapper = CreateWrapperException(exception);
                if (properties == null || properties.Count == 0)
                {
                    CrashesInternal.TrackException(exceptionWrapper.GetRawObject());
                }
                else
                {
                    CrashesInternal.TrackException(exceptionWrapper.GetRawObject(), properties);
                }
            }
        }

        public static void OnHandleLog(string logString, string stackTrace, LogType type)
        {
            if (LogType.Assert == type || LogType.Exception == type || LogType.Error == type)
            {
                var exception = CreateWrapperException(logString, stackTrace, type);
                CrashesInternal.TrackException(exception.GetRawObject());
            }
        }

#if !UNITY_WSA_10_0
        public static void OnHandleUnresolvedException(object sender, UnhandledExceptionEventArgs args)
        {
            if (args == null || args.ExceptionObject == null)
            {
                return;
            }
            var exception = args.ExceptionObject as Exception;
            if (exception != null)
            {
                Debug.Log("Unhandled exception: " + exception.ToString());
#if UNITY_ANDROID
                lock (_unhandledExceptions)
                {
                    _unhandledExceptions.Enqueue(exception);
                }
                UnityCoroutineHelper.StartCoroutine(SendUnhandledExceptionReports);
#else
                var exceptionWrapper = CreateWrapperException(exception);
                CrashesInternal.TrackException(exceptionWrapper.GetRawObject());
#endif
            }
        }
#endif

        public static AppCenterTask<bool> IsEnabledAsync()
        {
            return CrashesInternal.IsEnabledAsync();
        }

        public static AppCenterTask SetEnabledAsync(bool enabled)
        {
            return CrashesInternal.SetEnabledAsync(enabled);
        }

        public static void GenerateTestCrash()
        {
            CrashesInternal.GenerateTestCrash();
        }

        public static AppCenterTask<bool> HasCrashedInLastSessionAsync()
        {
            return CrashesInternal.HasCrashedInLastSessionAsync();
        }

        public static void DisableMachExceptionHandler()
        {
            CrashesInternal.DisableMachExceptionHandler();
        }

        public static AppCenterTask<ErrorReport> GetLastSessionCrashReportAsync()
        {
            return CrashesInternal.GetLastSessionCrashReportAsync();
        }

        /// <summary>
        /// Report unhandled exceptions, automatically captured by Unity, as handled errors
        /// </summary>
        /// <param name="enabled">Specify true to enable reporting of unhandled exceptions, automatically captured by Unity, as handled errors; otherwise, false.</param>
        public static void ReportUnhandledExceptions(bool enabled)
        {
            if (_reportUnhandledExceptions == enabled)
            {
                return;
            }
            _reportUnhandledExceptions = enabled;
            if (enabled)
            {
                SubscribeToUnhandledExceptions();
            }
            else
            {
                UnsubscribeFromUnhandledExceptions();
            }
        }

        public static bool IsReportingUnhandledExceptions()
        {
            return _reportUnhandledExceptions;
        }

#if ENABLE_IL2CPP
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
        public delegate bool UserConfirmationHandler();

        public static UserConfirmationHandler ShouldAwaitUserConfirmation
        {
            set
            {
                CrashesInternal.SetUserConfirmationHandler(value);
            }
        }

        public enum ConfirmationResult { DontSend, Send, AlwaysSend };

        public static void NotifyUserConfirmation(ConfirmationResult answer)
        {
            CrashesInternal.NotifyWithUserConfirmation(answer);
        }

#if ENABLE_IL2CPP
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
        public delegate bool ShouldProcessErrorReportHandler(ErrorReport errorReport);

        public static ShouldProcessErrorReportHandler ShouldProcessErrorReport
        {
            set
            {
                CrashesDelegate.SetShouldProcessErrorReportHandler(value);
            }
        }

#if ENABLE_IL2CPP
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
        public delegate ErrorAttachmentLog[] GetErrorAttachmentsHandler(ErrorReport errorReport);

        public static GetErrorAttachmentsHandler GetErrorAttachments
        {
            set
            {
                CrashesDelegate.SetGetErrorAttachmentsHandler(value);
            }
        }

#if ENABLE_IL2CPP
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
        public delegate void SendingErrorReportHandler(ErrorReport errorReport);

        public static event SendingErrorReportHandler SendingErrorReport
        {
            add
            {
                lock (_objectLock)
                {
                    CrashesDelegate.SendingErrorReport += value;
                }
            }
            remove
            {
                lock (_objectLock)
                {
                    CrashesDelegate.SendingErrorReport -= value;
                }
            }
        }

#if ENABLE_IL2CPP
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
        public delegate void SentErrorReportHandler(ErrorReport errorReport);

        public static event SentErrorReportHandler SentErrorReport
        {
            add
            {
                lock (_objectLock)
                {
                    CrashesDelegate.SentErrorReport += value;
                }
            }
            remove
            {
                lock (_objectLock)
                {
                    CrashesDelegate.SentErrorReport -= value;
                }
            }
        }

#if ENABLE_IL2CPP
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
        public delegate void FailedToSendErrorReportHandler(ErrorReport errorReport, Models.Exception exception);

        public static event FailedToSendErrorReportHandler FailedToSendErrorReport
        {
            add
            {
                lock (_objectLock)
                {
                    CrashesDelegate.FailedToSendErrorReport += value;
                }
            }
            remove
            {
                lock (_objectLock)
                {
                    CrashesDelegate.FailedToSendErrorReport -= value;
                }
            }
        }

        public static void StartCrashes()
        {
            CrashesInternal.StartCrashes();
        }

        private static void SubscribeToUnhandledExceptions()
        {
#if !UNITY_EDITOR && !UNITY_WSA_10_0
            Application.logMessageReceived += OnHandleLog;
            System.AppDomain.CurrentDomain.UnhandledException += OnHandleUnresolvedException;
#endif
        }

        private static void UnsubscribeFromUnhandledExceptions()
        {
#if !UNITY_EDITOR && !UNITY_WSA_10_0
            Application.logMessageReceived -= OnHandleLog;
            System.AppDomain.CurrentDomain.UnhandledException -= OnHandleUnresolvedException;
#endif
        }

        private static void HandleAppCenterInitialized()
        {
            if (_reportUnhandledExceptions)
            {
                SubscribeToUnhandledExceptions();
            }
        }

#if UNITY_ANDROID
        private static IEnumerator SendUnhandledExceptionReports()
        {
            yield return null; // ensure that code is executed in main thread
            while (true)
            {
                Exception exception = null;
                lock (_unhandledExceptions)
                {
                    if (_unhandledExceptions.Count > 0)
                    {
                        exception = _unhandledExceptions.Dequeue();
                    }
                    else
                    {
                        yield break;
                    }
                }
                if (exception != null)
                {
                    var exceptionWrapper = CreateWrapperException(exception);
                    CrashesInternal.TrackException(exceptionWrapper.GetRawObject());
                }
                yield return null; // report remaining exceptions on next frames
            }
        }
#endif

        private static WrapperException CreateWrapperException(Exception exception)
        {
            var exceptionWrapper = new WrapperException();
            exceptionWrapper.SetWrapperSdkName(GetExceptionWrapperSdkName());
            exceptionWrapper.SetStacktrace(exception.StackTrace);
            exceptionWrapper.SetMessage(exception.Message);
            exceptionWrapper.SetType(exception.GetType().ToString());

            if (exception.InnerException != null)
            {
                var innerExceptionWrapper = CreateWrapperException(exception.InnerException).GetRawObject();
                exceptionWrapper.SetInnerException(innerExceptionWrapper);
            }

            return exceptionWrapper;
        }

        private static WrapperException CreateWrapperException(string logString, string stackTrace, LogType type)
        {
            var exception = new WrapperException();
            exception.SetWrapperSdkName(GetExceptionWrapperSdkName());

            string sanitizedLogString = logString.Replace("\n", " ");
            exception.SetMessage(sanitizedLogString);
            exception.SetType(type.ToString());

            string[] stacktraceLines = stackTrace.Split('\n');
            string stackTraceString = "";
            foreach (string line in stacktraceLines)
            {
                if (line.Length > 0)
                {
                    stackTraceString += "at " + line + "\n";
                }
            }
            exception.SetStacktrace(stackTraceString);

            return exception;
        }

        private static string GetExceptionWrapperSdkName()
        {
            //return WrapperSdk.Name;
            return "appcenter.xamarin"; // fix stack traces are not showing up in the portal UI
        }
    }
}