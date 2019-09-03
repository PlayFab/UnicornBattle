// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.AppCenter.Unity.Crashes.Models
{
    public class Exception
    {
        public string Message { get; private set; }
        public string StackTrace { get; private set; }

        public Exception(string message, string stackTrace)
        {
            Message = message;
            StackTrace = stackTrace;
        }
    }
}
