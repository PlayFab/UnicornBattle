// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.AppCenter.Unity
{
    /// <summary>
    /// SDK Startup mode.
    /// </summary>
    public enum StartupType
    {
        /// <summary>
        /// The app starts with SDK using an appSecret that is targeting AppCenter. 
        /// If you send events in the app it will thus go to App Center portal. 
        /// </summary>
        AppCenter = 0,

        /// <summary>
        /// Only Analytics is usable, other modules appear disabled.
        /// </summary>
        OneCollector = 1,

        /// <summary>
        /// Events go to OneCollector, crashes go to AppCenter. 
        /// </summary>
        Both = 2,

        /// <summary>
        /// Sending an event does nothing and you have to use alternate transmission targets. 
        /// Only the Analytics module appears enabled.
        /// </summary>
        NoSecret = 3,

        /// <summary>
        /// Don’t start the SDK: like NoSecret, except the SDK is not even started when launching the application. 
        /// All modules will appear disabled.
        /// </summary>
        Skip = 4
    }
}
