// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.AppCenter.Unity
{
    /// <summary>
    /// Log level threshold for logs emitted by the SDK.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// SDK emits all possible level of logs.
        /// </summary>
        Verbose = 2,

        /// <summary>
        /// SDK emits debug, info, warn, error and assert logs.
        /// </summary>
        Debug = 3,

        /// <summary>
        /// SDK emits info, warn, error, and assert logs.
        /// </summary>
        Info = 4,

        /// <summary>
        /// SDK emits warn, error, and assert logs.
        /// </summary>
        Warn = 5,

        /// <summary>
        /// SDK error and assert logs.
        /// </summary>
        Error = 6,

        /// <summary>
        /// Only assert logs are emitted by SDK.
        /// </summary>
        Assert = 7,

        /// <summary>
        /// No log is emitted by SDK.
        /// </summary>
#if UNITY_IOS
        None = 99
#else
        None = 8
#endif
    }
}
