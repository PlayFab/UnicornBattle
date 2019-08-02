// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_ANDROID && !UNITY_EDITOR

using UnityEngine;
using System.Threading;
using System;

namespace Microsoft.AppCenter.Unity
{
    public class UnityAppCenterConsumer<T> : AndroidJavaProxy
    {
        internal Action<T> CompletionCallback { get; set; }

        internal UnityAppCenterConsumer() : base("com.microsoft.appcenter.utils.async.AppCenterConsumer")
        {
        }

        void accept(T t)
        {
            if (CompletionCallback != null)
            {
                CompletionCallback(t);
            }
        }
    }
}
#endif
