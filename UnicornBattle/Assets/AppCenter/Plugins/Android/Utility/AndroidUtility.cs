using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.AppCenter.Plugins.Android.Utility
{
    class AndroidUtility
    {
        private static AndroidJavaObject _context;
        private const string PREFS_NAME = "AppCenterUserPrefs";

        public static AndroidJavaObject GetAndroidContext()
        {
            if (_context != null)
            {
                return _context;
            }
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            _context = activity.Call<AndroidJavaObject>("getApplicationContext");
            return _context;
        }

        public static void SetPreferenceInt(string prefKey, int prefValue)
        {
            AndroidJavaObject context = GetAndroidContext();
            AndroidJavaObject sharedPreferences = context.Call<AndroidJavaObject>("getSharedPreferences", new object[] { PREFS_NAME, 0 });
            AndroidJavaObject editor = sharedPreferences.Call<AndroidJavaObject>("edit");
            editor = editor.Call<AndroidJavaObject>("putInt", new object[] { prefKey, prefValue });
            editor.Call("apply");
        }

        public static void SetPreferenceString(string prefKey, string prefValue)
        {
            AndroidJavaObject context = GetAndroidContext();
            AndroidJavaObject sharedPreferences = context.Call<AndroidJavaObject>("getSharedPreferences", new object[] { PREFS_NAME, 0 });
            AndroidJavaObject editor = sharedPreferences.Call<AndroidJavaObject>("edit");
            editor = editor.Call<AndroidJavaObject>("putString", new object[] { prefKey, prefValue });
            editor.Call("apply");
        }
    }
}
