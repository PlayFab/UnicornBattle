// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.AppCenter.Unity;
using UnityEditor;
using UnityEditor.Build;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

#if UNITY_2018_1_OR_NEWER
public class AppCenterPreBuild : IPreprocessBuildWithReport
#else
public class AppCenterPreBuild : IPreprocessBuild
#endif
{
    public int callbackOrder { get { return 0; } }

#if UNITY_2018_1_OR_NEWER
    public void OnPreprocessBuild(BuildReport report)
    {
        OnPreprocessBuild(report.summary.platform, report.summary.outputPath);
    }
#endif

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        if (target == BuildTarget.Android)
        {
            var settings = AppCenterSettingsContext.SettingsInstance;
            if (settings.UsePush && AppCenter.Push != null)
            {
                FirebaseDependency.SetupPush();
            }
            AddStartupCode(new AppCenterSettingsMakerAndroid());
        }
        else if (target == BuildTarget.iOS)
        {
            AddStartupCode(new AppCenterSettingsMakerIos());
        }
    }

    private void AddStartupCode(IAppCenterSettingsMaker settingsMaker)
    {
        var settings = AppCenterSettingsContext.SettingsInstance;
        var advancedSettings = AppCenterSettingsContext.SettingsInstanceAdvanced;
        settingsMaker.SetAppSecret(settings);
        settingsMaker.SetLogLevel((int)settings.InitialLogLevel);
        if (settings.CustomLogUrl.UseCustomUrl)
        {
            settingsMaker.SetLogUrl(settings.CustomLogUrl.Url);
        }
        if (settings.MaxStorageSize.UseCustomMaxStorageSize && settings.MaxStorageSize.Size > 0)
        {
            settingsMaker.SetMaxStorageSize(settings.MaxStorageSize.Size);
        }
        if (settings.UsePush && settingsMaker.IsPushAvailable())
        {
            settingsMaker.StartPushClass();
            if (settings.EnableFirebaseAnalytics)
            {
                settingsMaker.EnableFirebaseAnalytics();
            }
        }
        if (settings.UseAnalytics && settingsMaker.IsAnalyticsAvailable())
        {
            settingsMaker.StartAnalyticsClass();
        }
        if (settings.UseCrashes && settingsMaker.IsCrashesAvailable())
        {
            settingsMaker.StartCrashesClass();
        }
        if (settings.UseDistribute && settingsMaker.IsDistributeAvailable())
        {
            if (settings.CustomApiUrl.UseCustomUrl)
            {
                settingsMaker.SetApiUrl(settings.CustomApiUrl.Url);
            }
            if (settings.CustomInstallUrl.UseCustomUrl)
            {
                settingsMaker.SetInstallUrl(settings.CustomInstallUrl.Url);
            }
            if (settings.EnableDistributeForDebuggableBuild)
            {
                settingsMaker.SetShouldEnableDistributeForDebuggableBuild();
            }
            settingsMaker.StartDistributeClass();
        }
        if (advancedSettings != null)
        {
            var startupType = settingsMaker.IsStartFromAppCenterBehavior(advancedSettings) ? StartupType.Skip : advancedSettings.GetStartupType();
            settingsMaker.SetStartupType((int)startupType);
            settingsMaker.SetTransmissionTargetToken(advancedSettings.TransmissionTargetToken);
        }
        else
        {
            settingsMaker.SetStartupType((int)StartupType.AppCenter);
        }
        settingsMaker.CommitSettings();
    }
}
