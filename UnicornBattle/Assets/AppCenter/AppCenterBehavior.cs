// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.AppCenter.Unity;
using UnityEngine;
using System;
using System.Reflection;
using Microsoft.AppCenter.Unity.Internal;
using System.Linq;

[HelpURL("https://docs.microsoft.com/en-us/appcenter/sdk/crashes/unity")]
public class AppCenterBehavior : MonoBehaviour
{
    public static event Action InitializingServices;
    public static event Action InitializedAppCenterAndServices;
    public static event Action Started;

    private static AppCenterBehavior _instance;

    public AppCenterSettings Settings;

    private void Awake()
    {
        // Make sure that App Center have only one instance.
        if (_instance != null)
        {
            Debug.LogError("App Center Behavior should have only one instance!");
            DestroyImmediate(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
#if UNITY_WSA_10_0
        StartAppCenter();
#endif
    }

    private void Start()
    {
#if !UNITY_WSA_10_0
        StartAppCenter();
#endif
    }

#if UNITY_EDITOR
    public void Reset()
    {
        if (FindObjectsOfType<AppCenterBehavior>().Length > 1)
        {
            Debug.LogError("Only one game object with App Center Behaviour should exist.");
            DestroyImmediate(this);
        }
    }
#endif

    private void StartAppCenter()
    {
        if (Settings == null)
        {
            Debug.LogError("App Center isn't configured!");
            return;
        }
        var services = Settings.Services;
        PrepareEventHandlers(services);
        InvokeInitializingServices();
        AppCenter.SetWrapperSdk();
        AppCenter.CacheStorageSize(Settings.MaxStorageSize.Size);
        if (Settings.CustomLogUrl.UseCustomUrl)
        {
            AppCenter.CacheLogUrl(Settings.CustomLogUrl.Url);
        }
        var appSecret = AppCenter.ParseAndSaveSecretForPlatform(Settings.AppSecret);
        var advancedSettings = GetComponent<AppCenterBehaviorAdvanced>();
        if (IsStartFromAppCenterBehavior(advancedSettings))
        {
            AppCenter.LogLevel = Settings.InitialLogLevel;
            if (Settings.CustomLogUrl.UseCustomUrl)
            {
                AppCenter.SetLogUrl(Settings.CustomLogUrl.Url);
            }
            if (Settings.MaxStorageSize.UseCustomMaxStorageSize && Settings.MaxStorageSize.Size > 0)
            {
                AppCenterInternal.SetMaxStorageSize(Settings.MaxStorageSize.Size);
            }
            var startupType = GetStartupType(advancedSettings);
            if (startupType != StartupType.Skip)
            {
                var transmissionTargetToken = GetTransmissionTargetToken(advancedSettings);
                var appSecretString = GetAppSecretString(appSecret, transmissionTargetToken, startupType);
                if (string.IsNullOrEmpty(appSecretString))
                {
                    AppCenterInternal.Start(services);
                }
                else
                {
                    AppCenterInternal.Start(appSecretString, services);
                }
            }
        }
#if UNITY_IOS || UNITY_ANDROID
        else
        {
            foreach (var service in services)
            {
#if UNITY_IOS || UNITY_ANDROID
                // On iOS and Android we start crash service here, to give app an opportunity to assign handlers after crash and restart in Awake method
                var startCrashes = service.GetMethod("StartCrashes");
                if (startCrashes != null)
                    startCrashes.Invoke(null, null);
#endif
#if UNITY_IOS
                var startPush = service.GetMethod("StartPush");
                if (startPush != null)
                    startPush.Invoke(null, null);
#endif
            }
        }
#endif
        InvokeInitializedServices();
        if (Started != null)
        {
            Started.Invoke();
        }
    }

    private bool IsStartFromAppCenterBehavior(AppCenterBehaviorAdvanced advancedSettings)
    {
#if UNITY_IOS
        return advancedSettings != null && advancedSettings.SettingsAdvanced != null && advancedSettings.SettingsAdvanced.StartIOSNativeSDKFromAppCenterBehavior;
#elif UNITY_ANDROID
        return advancedSettings != null && advancedSettings.SettingsAdvanced != null && advancedSettings.SettingsAdvanced.StartAndroidNativeSDKFromAppCenterBehavior;
#else
        return true;
#endif
    }

    private StartupType GetStartupType(AppCenterBehaviorAdvanced advancedSettings)
    {
        return advancedSettings != null && advancedSettings.SettingsAdvanced != null ?
            advancedSettings.SettingsAdvanced.GetStartupType() :
            StartupType.AppCenter;
    }

    private string GetTransmissionTargetToken(AppCenterBehaviorAdvanced advancedSettings)
    {
        return advancedSettings != null && advancedSettings.SettingsAdvanced != null ?
            advancedSettings.SettingsAdvanced.TransmissionTargetToken :
            string.Empty;
    }

    private string GetAppSecretString(string appSecret, string transmissionTargetToken, StartupType startupType)
    {
#if UNITY_WSA_10_0
        return appSecret;
#else
        switch (startupType)
        {
            default:
            case StartupType.AppCenter:
                return appSecret;
            case StartupType.NoSecret:
                return string.Empty;
            case StartupType.OneCollector:
                return string.Format("target={0}", transmissionTargetToken);
            case StartupType.Both:
                return string.Format("appsecret={0};target={1}", appSecret, transmissionTargetToken);
        }
#endif
    }

    private static void PrepareEventHandlers(Type[] services)
    {
        foreach (var service in services)
        {
            var method = service.GetMethod("PrepareEventHandlers");
            if (method != null)
            {
                method.Invoke(null, null);
            }
        }
    }

    private static void InvokeInitializingServices()
    {
        if (InitializingServices != null)
        {
            InitializingServices.Invoke();
        }
    }

    private static void InvokeInitializedServices()
    {
        if (InitializedAppCenterAndServices != null)
        {
            InitializedAppCenterAndServices.Invoke();
        }
    }
}
