// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Reflection;

namespace Microsoft.AppCenter.Unity
{
    public static class WrapperSdk
    {
        private static string _wrapperRuntimeVersion;
        private static bool _hasAttemptedToGetRuntimeVersion;

        public const string Name = "appcenter.unity";
        public const string WrapperSdkVersion = "2.2.0";

        internal static string WrapperRuntimeVersion
        {
            get
            {
                // Use a flag instead of checking if _wrapperRuntimeVersion == null, because
                // GetWrapperRuntimeVersion() can return null
                return _hasAttemptedToGetRuntimeVersion ? _wrapperRuntimeVersion : (_wrapperRuntimeVersion = GetWrapperRuntimeVersion());
            }
        }

        private static string GetWrapperRuntimeVersion()
        {
            _hasAttemptedToGetRuntimeVersion = true;
            Type type = Type.GetType("Mono.Runtime");
            if (type != null)
            {
                MethodInfo displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
                if (displayName != null)
                {
                    return (string)displayName.Invoke(null, null);
                }
            }
            return null;
        }
    }
}
