// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace Microsoft.AppCenter.Unity.Crashes.Internal
{
    public class CrashesDelegate : AndroidJavaProxy
    {
        public static event Crashes.SendingErrorReportHandler SendingErrorReport;
        public static event Crashes.SentErrorReportHandler SentErrorReport;
        public static event Crashes.FailedToSendErrorReportHandler FailedToSendErrorReport;
        private static event Crashes.GetErrorAttachmentsHandler GetErrorAttachments;
        private static Crashes.UserConfirmationHandler shouldAwaitUserConfirmationHandler = null;
        private static Crashes.ShouldProcessErrorReportHandler shouldProcessErrorReportHandler = null;
        private static AndroidJavaClass _errorAttachmentLog = new AndroidJavaClass("com.microsoft.appcenter.crashes.ingestion.models.ErrorAttachmentLog");
        private static readonly CrashesDelegate instance = new CrashesDelegate();

        private CrashesDelegate() : base("com.microsoft.appcenter.crashes.CrashesListener")
        {
        }

        public static void SetDelegate()
        {
            var crashes = new AndroidJavaClass("com.microsoft.appcenter.crashes.Crashes");
            crashes.CallStatic("setListener", instance);
        }

        public void onBeforeSending(AndroidJavaObject report)
        {
            var handlers = SendingErrorReport;
            if (handlers != null)
            {
                var errorReport = JavaObjectsConverter.ConvertErrorReport(report);
                handlers.Invoke(errorReport);
            }
        }

        public void onSendingFailed(AndroidJavaObject report, AndroidJavaObject exception)
        {
            var handlers = FailedToSendErrorReport;
            if (handlers != null)
            {
                var errorReport = JavaObjectsConverter.ConvertErrorReport(report);
                var failCause = JavaObjectsConverter.ConvertException(exception);
                handlers.Invoke(errorReport, failCause);
            }
        }

        public void onSendingSucceeded(AndroidJavaObject report)
        {
            var handlers = SentErrorReport;
            if (handlers != null)
            {
                var errorReport = JavaObjectsConverter.ConvertErrorReport(report);
                handlers.Invoke(errorReport);
            }
        }

        public bool shouldProcess(AndroidJavaObject report)
        {
            var handler = shouldProcessErrorReportHandler;
            if (handler != null)
            {
                return handler.Invoke(JavaObjectsConverter.ConvertErrorReport(report));
            }
            return true;
        }

        public bool shouldAwaitUserConfirmation()
        {
            var handler = shouldAwaitUserConfirmationHandler;
            if (handler != null)
            {
                return handler.Invoke();
            }
            return false;
        }

        private AndroidJavaObject AttachmentWithText(string text, string fileName)
        {
            return _errorAttachmentLog.CallStatic<AndroidJavaObject>("attachmentWithText", text, fileName);
        }

        private AndroidJavaObject AttachmentWithBinary(byte[] text, string fileName, string contentType)
        {
            return _errorAttachmentLog.CallStatic<AndroidJavaObject>("attachmentWithBinary", text, fileName, contentType);
        }

        public AndroidJavaObject getErrorAttachments(AndroidJavaObject report)
        {
            if (GetErrorAttachments != null)
            {
                var logs = GetErrorAttachments(JavaObjectsConverter.ConvertErrorReport(report));
                var nativeLogs = new List<AndroidJavaObject>();
                foreach (var errorAttachmetLog in logs)
                {
                    AndroidJavaObject nativeLog = null;
                    if (errorAttachmetLog.Type == ErrorAttachmentLog.AttachmentType.Text)
                    {
                        nativeLog = AttachmentWithText(errorAttachmetLog.Text, errorAttachmetLog.FileName);
                    }
                    else
                    {
                        nativeLog = AttachmentWithBinary(errorAttachmetLog.Data, errorAttachmetLog.FileName, errorAttachmetLog.ContentType);
                    }
                    nativeLogs.Add(nativeLog);
                }

                var javaList = new AndroidJavaObject("java.util.LinkedList");
                if (nativeLogs.Count > 0)
                {
                    javaList.Call("addLast", nativeLogs[0]);
                }
                if (nativeLogs.Count > 1)
                {
                    javaList.Call("addLast", nativeLogs[1]);
                }
                return javaList;
            }
            else
            {
                return null;
            }
        }

        public static void SetShouldAwaitUserConfirmationHandler(Crashes.UserConfirmationHandler handler)
        {
            shouldAwaitUserConfirmationHandler = handler;
        }

        public static void SetShouldProcessErrorReportHandler(Crashes.ShouldProcessErrorReportHandler handler)
        {
            shouldProcessErrorReportHandler = handler;
        }

        public static void SetGetErrorAttachmentsHandler(Crashes.GetErrorAttachmentsHandler handler)
        {
            GetErrorAttachments = handler;
        }
    }
}
#endif
