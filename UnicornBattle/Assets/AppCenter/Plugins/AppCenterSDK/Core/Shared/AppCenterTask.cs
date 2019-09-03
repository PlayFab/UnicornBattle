// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace Microsoft.AppCenter.Unity
{
    /// <summary>
    /// AppCenterTask provides a way of performing long-running
    /// tasks on any thread and invoking callbacks upon completion.
    /// </summary>
    public partial class AppCenterTask
    {
        private readonly List<Action<AppCenterTask>> _continuationActions = new List<Action<AppCenterTask>>();
        protected readonly object _lockObject = new object();

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Microsoft.AppCenter.Unity.AppCenterTask"/> is complete.
        /// </summary>
        /// <value><c>true</c> if it is complete; otherwise, <c>false</c>.</value>
        public bool IsComplete { get; private set; }

        public AppCenterTask()
        {
        }

        /// <summary>
        /// Adds a callback that will be invoked once the task is complete. If
        /// the task is already complete, it is invoked immediately after being set.
        /// </summary>
        /// <param name="continuationAction">Callback to be invoked after task completion.</param>
        public void ContinueWith(Action<AppCenterTask> continuationAction)
        {
            lock (_lockObject)
            {
                _continuationActions.Add(continuationAction);
                InvokeContinuationActions();
            }
        }

        /// <summary>
        /// Invokes callbacks and sets completion flag.
        /// </summary>
        protected virtual void CompletionAction()
        {
            lock (_lockObject)
            {
                IsComplete = true;
                InvokeContinuationActions();
            }
        }

        /// <summary>
        /// Throws an exception if the task has completed.
        /// </summary>
        protected void ThrowIfCompleted()
        {
            lock (_lockObject)
            {
                if (IsComplete)
                {
                    throw new InvalidOperationException("The task has already completed");
                }
            }
        }

        /// <summary>
        /// Returns an already completed task.
        /// </summary>
        /// <returns>The completed task.</returns>
        internal static AppCenterTask FromCompleted()
        {
            var task = new AppCenterTask();
            task.CompletionAction();
            return task;
        }

        private void InvokeContinuationActions()
        {
            // Save the actions and then invoke them; could have a deadlock
            // if one of the actions calls ContinueWith on another thread for
            // the same task object.
            var continuationActionsSnapshot = new List<Action<AppCenterTask>>();
            lock (_lockObject)
            {
                if (!IsComplete)
                {
                    return;
                }
                foreach (var action in _continuationActions)
                {
                    if (action != null)
                    {
                        continuationActionsSnapshot.Add(action);
                    }
                }
                _continuationActions.Clear();
            }
            foreach (var action in continuationActionsSnapshot)
            {
                action(this);
            }
        }
    }
}
