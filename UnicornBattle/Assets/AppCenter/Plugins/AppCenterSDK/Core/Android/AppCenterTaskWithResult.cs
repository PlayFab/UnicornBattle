// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#if UNITY_ANDROID && !UNITY_EDITOR

using System.Threading;
using UnityEngine;

namespace Microsoft.AppCenter.Unity
{
    public partial class AppCenterTask<TResult>
    {
        private ManualResetEvent _completionEvent = new ManualResetEvent(false);
        private TResult _result;
        private UnityAppCenterConsumer<TResult> _consumer = new UnityAppCenterConsumer<TResult>();

        // This will block if it is called before the task is complete
        public TResult Result
        {
            get
            {
                // Locking in here is both unnecessary and can deadlock.
                _completionEvent.WaitOne();
                return _result;
            }
        }

        public AppCenterTask(AndroidJavaObject javaFuture)
        {
            _consumer.CompletionCallback = SetResult;
            javaFuture.Call("thenAccept", _consumer);
        }

        internal void SetResult(TResult result)
        {
            lock (_lockObject)
            {
                ThrowIfCompleted();
                _consumer.CompletionCallback = null;
                _result = result;
                _completionEvent.Set();
                CompletionAction();
            }
        }
    }
}
#endif
