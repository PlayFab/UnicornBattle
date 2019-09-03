// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

namespace Microsoft.AppCenter.Unity
{
    /// <summary>
    /// AppCenterTask&lt;TResult&gt; extends the functionality of AppCenterTask
    /// to support return values.
    /// </summary>
    /// <typeparam name="TResult">The return type of the task.</typeparam>
    /// <seealso cref="AppCenterTask"/>
    public partial class AppCenterTask<TResult> : AppCenterTask
    {
        public AppCenterTask() : base()
        {
        }

        /// <summary>
        /// Adds a callback that will be invoked once the task is complete. If
        /// the task is already complete, it is invoked immediately after being set.
        /// </summary>
        /// <param name="continuationAction">Callback to be invoked after task completion.</param>
        public void ContinueWith(Action<AppCenterTask<TResult>> continuationAction)
        {
            base.ContinueWith(task => continuationAction(this));
        }

        /// <summary>
        /// Returns an already completed task with a given result.
        /// </summary>
        /// <returns>The completed task.</returns>
        internal static AppCenterTask<TResult> FromCompleted(TResult result)
        {
            var task = new AppCenterTask<TResult>();
            task.SetResult(result);
            return task;
        }
    }
}
