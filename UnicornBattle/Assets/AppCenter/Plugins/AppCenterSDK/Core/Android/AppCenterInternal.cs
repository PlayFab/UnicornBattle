// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_ANDROID && !UNITY_EDITOR
using Assets.AppCenter.Plugins.Android.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.AppCenter.Unity.Internal
{
    class AppCenterInternal
    {
        private static AndroidJavaClass _appCenter = new AndroidJavaClass("com.microsoft.appcenter.AppCenter");

        public static void SetLogLevel(int logLevel)
        {
            _appCenter.CallStatic("setLogLevel", logLevel);
        }

        public static int GetLogLevel()
        {
            return _appCenter.CallStatic<int>("getLogLevel");
        }

        public static bool IsConfigured()
        {
            return _appCenter.CallStatic<bool>("isConfigured");
        }

        public static void SetLogUrl(string logUrl)
        {
            _appCenter.CallStatic("setLogUrl", logUrl);
        }

        public static void SetUserId(string userId)
        {
            _appCenter.CallStatic("setUserId", userId);
        }

        public static string GetSdkVersion()
        {
            return _appCenter.CallStatic<string>("getSdkVersion");
        }

        public static AppCenterTask SetEnabledAsync(bool enabled)
        {
            var future = _appCenter.CallStatic<AndroidJavaObject>("setEnabled", enabled);
            return new AppCenterTask(future);
        }

        public static AppCenterTask<bool> IsEnabledAsync()
        {
            var future = _appCenter.CallStatic<AndroidJavaObject>("isEnabled");
            return new AppCenterTask<bool>(future);
        }

        public static AppCenterTask<string> GetInstallIdAsync()
        {
            AndroidJavaObject future = _appCenter.CallStatic<AndroidJavaObject>("getInstallId");
            var javaUUIDtask = new AppCenterTask<AndroidJavaObject>(future);
            var stringTask = new AppCenterTask<string>();
            javaUUIDtask.ContinueWith(t =>
            {
                var installId = t.Result == null ? null : t.Result.Call<string>("toString");
                stringTask.SetResult(installId);
            });
            return stringTask;
        }

        public static void SetCustomProperties(AndroidJavaObject properties)
        {
            _appCenter.CallStatic("setCustomProperties", properties);
        }

        private static AndroidJavaObject GetAndroidApplication()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            return activity.Call<AndroidJavaObject>("getApplication");
        }

        public static void SetWrapperSdk(string wrapperSdkVersion,
                                         string wrapperSdkName,
                                         string wrapperRuntimeVersion,
                                         string liveUpdateReleaseLabel,
                                         string liveUpdateDeploymentKey,
                                         string liveUpdatePackageHash)
        {
            var wrapperSdkObject = new AndroidJavaObject("com.microsoft.appcenter.ingestion.models.WrapperSdk");
            wrapperSdkObject.Call("setWrapperSdkVersion", wrapperSdkVersion);
            wrapperSdkObject.Call("setWrapperSdkName", wrapperSdkName);
            wrapperSdkObject.Call("setWrapperRuntimeVersion", wrapperRuntimeVersion);
            wrapperSdkObject.Call("setLiveUpdateReleaseLabel", liveUpdateReleaseLabel);
            wrapperSdkObject.Call("setLiveUpdateDeploymentKey", liveUpdateDeploymentKey);
            wrapperSdkObject.Call("setLiveUpdatePackageHash", liveUpdatePackageHash);
            _appCenter.CallStatic("setWrapperSdk", wrapperSdkObject);
        }

        public static void Start(string appSecret, Type[] services)
        {
            var nativeServiceTypes = ServicesToNativeTypes(services);
            var rawAppSecretString = AndroidJNI.NewStringUTF(appSecret);
            var startMethod = AndroidJNI.GetStaticMethodID(_appCenter.GetRawClass(), "start", "(Landroid/app/Application;Ljava/lang/String;[Ljava/lang/Class;)V");
            AndroidJNI.CallStaticVoidMethod(_appCenter.GetRawClass(), startMethod, new jvalue[]
            {
                new jvalue { l = GetAndroidApplication().GetRawObject() },
                new jvalue { l = rawAppSecretString },
                new jvalue { l = nativeServiceTypes }
            });
        }

        public static void Start(Type[] services)
        {
            var nativeServiceTypes = ServicesToNativeTypes(services);
            var startMethod = AndroidJNI.GetStaticMethodID(_appCenter.GetRawClass(), "start", "(Landroid/app/Application;[Ljava/lang/Class;)V");
            AndroidJNI.CallStaticVoidMethod(_appCenter.GetRawClass(), startMethod, new jvalue[]
            {
                new jvalue { l = GetAndroidApplication().GetRawObject() },
                new jvalue { l = nativeServiceTypes }
            });
        }

        public static void Start(Type service)
        {
            var nativeServiceTypes = ServicesToNativeTypes(new[] { service });
            var startMethod = AndroidJNI.GetStaticMethodID(_appCenter.GetRawClass(), "start", "([Ljava/lang/Class;)V");
            AndroidJNI.CallStaticVoidMethod(_appCenter.GetRawClass(), startMethod, new jvalue[]
            {
                new jvalue { l = nativeServiceTypes }
            });
        }

        public static void StartFromLibrary(IntPtr servicesArray)
        {
            var startMethod = AndroidJNI.GetStaticMethodID(_appCenter.GetRawClass(), "startFromLibrary", "(Landroid/content/Context;[Ljava/lang/Class;)V");
            AndroidJNI.CallStaticVoidMethod(_appCenter.GetRawClass(), startMethod, new jvalue[]
            {
                new jvalue { l = AndroidUtility.GetAndroidContext().GetRawObject() },
                new jvalue { l = servicesArray }
            });
        }

        public static IntPtr ServicesToNativeTypes(Type[] services)
        {
            var nativeTypes = new List<IntPtr>();
            foreach (var serviceType in services)
            {
                serviceType.GetMethod("AddNativeType").Invoke(null, new object[] { nativeTypes });
            }
            var classClass = AndroidJNI.FindClass("java/lang/Class");
            var array = AndroidJNI.NewObjectArray(nativeTypes.Count, classClass, classClass);
            for (var i = 0; i < nativeTypes.Count; i++)
            {
                AndroidJNI.SetObjectArrayElement(array, i, nativeTypes[i]);
            }
            return array;
        }

        public static void SetMaxStorageSize(long size)
        {
            var future = _appCenter.CallStatic<AndroidJavaObject>("setMaxStorageSize", size);
            new AppCenterTask<bool>(future).ContinueWith(task =>
            {
                if (!task.Result)
                {
                    Debug.Log("Failed to set maximum storage size");
                }
            });
        }
    }
}
#endif
