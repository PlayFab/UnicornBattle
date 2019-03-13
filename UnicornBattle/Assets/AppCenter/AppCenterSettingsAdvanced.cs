// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.Reflection;
using Microsoft.AppCenter.Unity;
using UnityEngine;

[Serializable]
public class AppCenterSettingsAdvanced : ScriptableObject
{
    [AppSecret("Transmission Target Token")]
    public string TransmissionTargetToken = "";

    [Tooltip("Configure the way App Center is started. For more info on startup types refer to the documentation.")]
    public StartupType AppCenterStartupType = StartupType.Both;

    [Tooltip("Start Android native SDK from the App Center Behavior script instead of the native plugin")]
    public bool StartAndroidNativeSDKFromAppCenterBehavior = false;

    [Tooltip("Start iOS native SDK from the App Center Behavior script instead of the native plugin")]
    public bool StartIOSNativeSDKFromAppCenterBehavior = false;

    public StartupType GetStartupType()
    {
        return AppCenterStartupType == StartupType.Both && string.IsNullOrEmpty(TransmissionTargetToken) ?
            StartupType.AppCenter :
            AppCenterStartupType;
    }

    private static Assembly AppCenterAssembly
    {
        get
        {
#if !UNITY_EDITOR && UNITY_WSA_10_0
            return typeof(AppCenterSettingsAdvanced).GetTypeInfo().Assembly;
#else
            return Assembly.GetExecutingAssembly();
#endif
        }
    }
}
