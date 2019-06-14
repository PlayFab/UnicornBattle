// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

using System;

namespace Microsoft.AppCenter.Unity.Crashes
{
    public class ErrorReport
    {
        public ErrorReport(string id, DateTimeOffset appStartTime, DateTimeOffset appErrorTime, Models.Exception exception, Device device, string threadName)
        {
            Id = id;
            AppStartTime = appStartTime;
            AppErrorTime = appErrorTime;
            Exception = exception;
            Device = device;
            ThreadName = threadName;
        }

        public ErrorReport(string id, DateTimeOffset appStartTime, DateTimeOffset appErrorTime, Models.Exception exception, int processId, string reporterKey,
                           string reporterSignal, bool isAppKill, Device device)
        {
            Id = id;
            AppStartTime = appStartTime;
            AppErrorTime = appErrorTime;
            Exception = exception;
            ProcessId = processId;
            ReporterKey = reporterKey;
            ReporterSignal = reporterSignal;
            IsAppKill = isAppKill;
            Device = device;
        }

        /// <summary>
        /// Gets the report identifier.
        /// </summary>
        /// <value>UUID for the report.</value>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the app start time.
        /// </summary>
        /// <value>Date and time the app started</value>
        public DateTimeOffset AppStartTime { get; private set; }

        /// <summary>
        /// Gets the app error time.
        /// </summary>
        /// <value>Date and time the error occured</value>
        public DateTimeOffset AppErrorTime { get; private set; }

        /// <summary>
        /// Gets the device that the crashed app was being run on.
        /// </summary>
        /// <value>Device information at the crash time.</value>
        public Device Device { get; private set; }

        /// <summary>
        /// Gets the model exception associated with the error.
        /// </summary>
        /// <value>The exception.</value>
        public Models.Exception Exception { get; private set; }

        /// <summary>
        /// Gets the thread name.
        /// </summary>
        /// <value>The thread name.</value>
        public string ThreadName { get; private set; }

        /// <summary>
        /// Gets the process identifier.
        /// </summary>
        /// <value>Process Id.</value>
        public int ProcessId { get; private set; }

        /// <summary>
        /// Gets the reporter key.
        /// </summary>
        /// <value>Reporter Key.</value>
        public string ReporterKey { get; private set; }

        /// <summary>
        /// Gets the reporter signal.
        /// </summary>
        /// <value>Reporter Signal.</value>
        public string ReporterSignal { get; private set; }

        /// <summary>
        /// True if the details represent an app kill instead of a crash.
        /// </summary>
        /// <value>True if the details represent an app kill instead of a crash</value>
        public bool IsAppKill { get; private set; }
    }
}
