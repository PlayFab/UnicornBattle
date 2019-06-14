// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#if UNITY_ANDROID
using System.Text;
using Microsoft.AppCenter.Unity;
using Microsoft.AppCenter.Unity.Crashes;
using Microsoft.AppCenter.Unity.Crashes.Models;
using Microsoft.AppCenter.Unity.Internal.Utility;
using UnityEngine;

public class JavaObjectsConverter
{
    public static ErrorReport ConvertErrorReport(AndroidJavaObject errorReport)
    {
        if (errorReport == null)
        {
            return null;
        }
        try
        {
            var id = errorReport.Call<string>("getId");
            var threadName = errorReport.Call<string>("getThreadName");
            var startTime = JavaDateHelper.DateTimeConvert(errorReport.Call<AndroidJavaObject>("getAppStartTime"));
            var errorTime = JavaDateHelper.DateTimeConvert(errorReport.Call<AndroidJavaObject>("getAppErrorTime"));
            var exception = ConvertException(errorReport.Call<AndroidJavaObject>("getThrowable"));
            var device = ConvertDevice(errorReport.Call<AndroidJavaObject>("getDevice"));
            return new ErrorReport(id, startTime, errorTime, exception, device, threadName);
        }
        catch (System.Exception e)
        {
            Debug.LogErrorFormat("Failed to convert error report Java object to .Net object: {0}", e.ToString());
            return null;
        }
    }

    public static Exception ConvertException(AndroidJavaObject throwable)
    {
        var message = throwable.Call<string>("toString");
        var stackTrace = throwable.Call<AndroidJavaObject[]>("getStackTrace");
        var stackTraceString = new StringBuilder();
        foreach (var element in stackTrace)
        {
            stackTraceString.AppendLine("at " + element.Call<string>("toString"));
        }
        return new Exception(message, stackTraceString.ToString().TrimEnd());
    }

    private static Device ConvertDevice(AndroidJavaObject device)
    {
        return new Device(
            device.Call<string>("getSdkName"),
            device.Call<string>("getSdkVersion"),
            device.Call<string>("getModel"),
            device.Call<string>("getOemName"),
            device.Call<string>("getOsName"),
            device.Call<string>("getOsVersion"),
            device.Call<string>("getOsBuild"),
            GetIntValue(device, "getOsApiLevel"),
            device.Call<string>("getLocale"),
            GetIntValue(device, "getTimeZoneOffset"),
            device.Call<string>("getScreenSize"),
            device.Call<string>("getAppVersion"),
            device.Call<string>("getCarrierName"),
            device.Call<string>("getCarrierCountry"),
            device.Call<string>("getAppBuild"),
            device.Call<string>("getAppNamespace"));
    }

    private static int GetIntValue(AndroidJavaObject javaObject, string getterName)
    {
        var integer = javaObject.Call<AndroidJavaObject>(getterName);
        return integer.Call<int>("intValue");
    }
}
#endif
