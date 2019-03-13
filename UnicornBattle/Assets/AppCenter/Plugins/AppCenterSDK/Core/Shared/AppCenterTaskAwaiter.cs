// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#if NET_4_6 && !UNITY_EDITOR

using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Unity
{
    public partial class AppCenterTask
    {
        public TaskAwaiter GetAwaiter()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            ContinueWith(task => taskCompletionSource.SetResult(true));
            return ((Task)taskCompletionSource.Task).GetAwaiter();
        }
    }

    public partial class AppCenterTask<TResult>
    {
        public new TaskAwaiter<TResult> GetAwaiter()
        {
            var taskCompletionSource = new TaskCompletionSource<TResult>();
            ContinueWith(task => taskCompletionSource.SetResult(task.Result));
            return taskCompletionSource.Task.GetAwaiter();
        }
    }
}

#endif
