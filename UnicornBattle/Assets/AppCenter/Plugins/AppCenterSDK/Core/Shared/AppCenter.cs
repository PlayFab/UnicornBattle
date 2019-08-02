// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Reflection;
using Microsoft.AppCenter.Unity.Internal;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Microsoft.AppCenter.Unity
{
#if UNITY_IOS || UNITY_ANDROID
    using ServiceType = System.IntPtr;
#else
    using ServiceType = System.Type;
#endif

    public class AppCenter
    {
        private static AppCenterTask<string> _secretTask = new AppCenterTask<string>();
        private static AppCenterTask<string> _logUrlTask = new AppCenterTask<string>();
        private static AppCenterTask<long> _storageSizeTask = new AppCenterTask<long>();

        public static LogLevel LogLevel
        {
            get { return (LogLevel)AppCenterInternal.GetLogLevel(); }
            set { AppCenterInternal.SetLogLevel((int)value); }
        }

        public static AppCenterTask SetEnabledAsync(bool enabled)
        {
            return AppCenterInternal.SetEnabledAsync(enabled);
        }

        public static void StartFromLibrary(Type[] servicesArray)
        {
            AppCenterInternal.StartFromLibrary(AppCenterInternal.ServicesToNativeTypes(servicesArray));
        }

        public static AppCenterTask<bool> IsEnabledAsync()
        {
            return AppCenterInternal.IsEnabledAsync();
        }

        /// <summary>
        /// Get the unique installation identifier for this application installation on this device.
        /// </summary>
        /// <remarks>
        /// The identifier is lost if clearing application data or uninstalling application.
        /// </remarks>
        public static AppCenterTask<Guid?> GetInstallIdAsync()
        {
            var stringTask = AppCenterInternal.GetInstallIdAsync();
            var guidTask = new AppCenterTask<Guid?>();
            stringTask.ContinueWith(t =>
            {
                var installId = !string.IsNullOrEmpty(t.Result) ? new Guid(t.Result) : (Guid?)null;
                guidTask.SetResult(installId);
            });
            return guidTask;
        }

        public static string GetSdkVersion()
        {
            return AppCenterInternal.GetSdkVersion();
        }

        public static AppCenterTask<string> GetLogUrlAsync()
        {
            if (_logUrlTask == null)
            {
                _logUrlTask = new AppCenterTask<string>();
            }
            return _logUrlTask;
        }

        public static AppCenterTask<long> GetStorageSizeAsync()
        {
            if (_storageSizeTask == null)
            {
                _storageSizeTask = new AppCenterTask<long>();
            }
            return _storageSizeTask;
        }

        /// <summary>
        /// Change the base URL (scheme + authority + port only) used to communicate with the backend.
        /// </summary>
        /// <param name="logUrl">Base URL to use for server communication.</param>
        public static void SetLogUrl(string logUrl)
        {
            AppCenterInternal.SetLogUrl(logUrl);
        }

        public static void CacheStorageSize(long storageSize)
        {
            if (_storageSizeTask != null)
            {
                _storageSizeTask.SetResult(storageSize);
            }
        }

        public static void CacheLogUrl(string logUrl)
        {
            if (_logUrlTask != null)
            {
                _logUrlTask.SetResult(logUrl);
            }
        }

        /// <summary>
        /// Check whether SDK has already been configured or not.
        /// </summary>
        public static bool Configured
        {
            get { return AppCenterInternal.IsConfigured(); }
        }

        /// <summary>
        /// Set the custom properties.
        /// </summary>
        /// <param name="customProperties">Custom properties object.</param>
        public static void SetCustomProperties(Unity.CustomProperties customProperties)
        {
            var rawCustomProperties = customProperties.GetRawObject();
            AppCenterInternal.SetCustomProperties(rawCustomProperties);
        }

        public static void SetWrapperSdk()
        {
            AppCenterInternal.SetWrapperSdk(WrapperSdk.WrapperSdkVersion, WrapperSdk.Name, WrapperSdk.WrapperRuntimeVersion, null, null, null);
        }

        /// <summary>
        // Gets cached secret.
        /// </summary>
        public static AppCenterTask<string> GetSecretForPlatformAsync()
        {
            if (_secretTask == null)
            {
                _secretTask = new AppCenterTask<string>();
            }
            return _secretTask;
        }

        // Gets the first instance of an app secret corresponding to the given platform name, or returns the string
        // as-is if no identifier can be found.
        public static string ParseAndSaveSecretForPlatform(string secrets)
        {
            var platformIdentifier = GetPlatformIdentifier();
            if (platformIdentifier == null)
            {
                // Return as is for unsupported platform.
                return secrets;
            }
            if (secrets == null)
            {
                // If "secrets" is null, return that and let the error be dealt
                // with downstream.
                return secrets;
            }

            // If there are no equals signs, then there are no named identifiers
            if (!secrets.Contains("="))
            {
                return secrets;
            }

            var platformIndicator = platformIdentifier + "=";
            var secretIdx = secrets.IndexOf(platformIndicator, StringComparison.Ordinal);
            if (secretIdx == -1)
            {
                // If the platform indicator can't be found, return the original
                // string and let the error be dealt with downstream.
                return secrets;
            }
            secretIdx += platformIndicator.Length;
            var platformSecret = string.Empty;

            while (secretIdx < secrets.Length)
            {
                var nextChar = secrets[secretIdx++];
                if (nextChar == ';')
                {
                    break;
                }

                platformSecret += nextChar;
            }
            if (_secretTask != null)
            {
                _secretTask.SetResult(platformSecret);
            }
            return platformSecret;
        }

        public static void SetUserId(string userId)
        {
            AppCenterInternal.SetUserId(userId);
        }

#if ENABLE_IL2CPP
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
        public delegate void SetMaxStorageSizeCompletionHandler(bool result);

        private static string GetPlatformIdentifier()
        {
#if UNITY_IOS
            return "ios";
#elif UNITY_ANDROID
            return "android";
#elif UNITY_WSA_10_0
            return "uwp";
#else
            return null;
#endif
        }

        public static Type Analytics
        {
            get { return AppCenterAssembly.GetType("Microsoft.AppCenter.Unity.Analytics.Analytics"); }
        }

        public static Type Crashes
        {
            get { return AppCenterAssembly.GetType("Microsoft.AppCenter.Unity.Crashes.Crashes"); }
        }

        public static Type Distribute
        {
            get { return AppCenterAssembly.GetType("Microsoft.AppCenter.Unity.Distribute.Distribute"); }
        }

        public static Type Push
        {
            get { return AppCenterAssembly.GetType("Microsoft.AppCenter.Unity.Push.Push"); }
        }

        private static Assembly AppCenterAssembly
        {
            get
            {
#if !UNITY_EDITOR && UNITY_WSA_10_0
            return typeof(AppCenterSettings).GetTypeInfo().Assembly;
#else
                return Assembly.GetExecutingAssembly();
#endif
            }
        }
    }
}
