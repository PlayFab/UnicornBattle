// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using UnityEngine;

namespace Microsoft.AppCenter.Unity
{
    public partial class AppCenterTask : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get { return !IsComplete; }
        }
    }
}
