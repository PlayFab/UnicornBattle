// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AppCenter.Unity;
using UnityEngine;

[Serializable]
public class AppCenterSettings : ScriptableObject
{
    [AppSecret("iOS App Secret")]
    public string iOSAppSecret = "";

    [AppSecret]
    public string AndroidAppSecret = "";

    [AppSecret]
    public string UWPAppSecret = "";

    [Tooltip("App Center Analytics helps you understand user behavior and customer engagement to improve your app.")]
    public bool UseAnalytics = true;

    [Tooltip("App Center Crashes will automatically generate a crash log every time your app crashes.")]
    public bool UseCrashes = true;

    [Tooltip("App Center Distribute will let your users install a new version of the app when you distribute it via the App Center.")]
    public bool UseDistribute = true;

    public CustomUrlProperty CustomApiUrl = new CustomUrlProperty("API");

    public CustomUrlProperty CustomInstallUrl = new CustomUrlProperty("Install");

    [Tooltip("By default, App Center Distribute is disabled for debuggable builds. Check this to enable it.")]
    public bool EnableDistributeForDebuggableBuild = false;

    [Tooltip("App Center Push enables you to send push notifications to users of your app from the App Center portal.")]
    public bool UsePush = true;

    [Tooltip("By default, App Center Push disables Firebase Analytics. Use this option to enable it. This only applies to Android applications.")]
    public bool EnableFirebaseAnalytics = false;

    public LogLevel InitialLogLevel = LogLevel.Info;

    public CustomUrlProperty CustomLogUrl = new CustomUrlProperty("Log");

    public MaxStorageSizeProperty MaxStorageSize = new MaxStorageSizeProperty();

    public string AppSecret
    {
        get
        {
            var appSecrets = new Dictionary<string, string>
            {
                { "ios", iOSAppSecret },
                { "android", AndroidAppSecret },
                { "uwp", UWPAppSecret }
            };
            return string.Concat(appSecrets
                .Where(i => !string.IsNullOrEmpty(i.Value))
                .Select(i => string.Format("{0}={1};", i.Key, i.Value))
                .ToArray());
        }
    }

    public Type[] Services
    {
        get
        {
            var services = new List<Type>();
            if (UseAnalytics)
            {
                services.Add(AppCenter.Analytics);
            }
            if (UseCrashes)
            {
                services.Add(AppCenter.Crashes);
            }
            if (UseDistribute)
            {
                services.Add(AppCenter.Distribute);
            }
            if (UsePush)
            {
                services.Add(AppCenter.Push);
            }
            return services.Where(i => i != null).ToArray();
        }
    }
}
