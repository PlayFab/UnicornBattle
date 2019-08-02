// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_ANDROID && !UNITY_EDITOR

using UnityEngine;

namespace Microsoft.AppCenter.Unity
{
    public partial class AppCenterTask
    {
        public AppCenterTask(AndroidJavaObject javaFuture)
        {
            var consumer = new UnityAppCenterConsumer<AndroidJavaObject>();
            consumer.CompletionCallback = t =>
            {
                CompletionAction();
            };
            javaFuture.Call("thenAccept", consumer);
        }
    }
}
#endif
