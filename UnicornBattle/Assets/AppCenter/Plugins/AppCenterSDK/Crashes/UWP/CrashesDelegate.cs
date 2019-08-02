﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_WSA_10_0 && !UNITY_EDITOR

namespace Microsoft.AppCenter.Unity.Crashes.Internal
{
    public class CrashesDelegate
    {
#pragma warning disable 0067
        public static event Crashes.SendingErrorReportHandler SendingErrorReport;

        public static event Crashes.SentErrorReportHandler SentErrorReport;

        public static event Crashes.FailedToSendErrorReportHandler FailedToSendErrorReport;
#pragma warning restore 0067

        public static void SetDelegate()
        {
        }

        public static void SetShouldProcessErrorReportHandler(Crashes.ShouldProcessErrorReportHandler handler)
        {
        }

        public static void SetGetErrorAttachmentsHandler(Crashes.GetErrorAttachmentsHandler handler)
        {
        }

        public static void SetSentErrorReportHandler(Crashes.SentErrorReportHandler handler)
        {
        }

        public static void SetFailedToSendErrorReportHandler(Crashes.FailedToSendErrorReportHandler handler)
        {
        }
    }
}
#endif
