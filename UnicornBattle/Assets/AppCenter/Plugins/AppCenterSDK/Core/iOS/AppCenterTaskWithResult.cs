// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_IOS && !UNITY_EDITOR

using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Microsoft.AppCenter.Unity
{
    public partial class AppCenterTask<TResult>
    {
        private TResult _result;
        public TResult Result
        {
            get
            {
                lock (_lockObject)
                {
                    return _result;
                }
            }
        }

        internal void SetResult(TResult result)
        {
            lock (_lockObject)
            {
                ThrowIfCompleted();
                _result = result;
                CompletionAction();
            }
        }
    }
}
#endif
