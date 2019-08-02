// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections;
using UnityEngine;

namespace Microsoft.AppCenter.Unity.Internal.Utils
{
#if UNITY_WSA_10_0
    using WSAApplication = UnityEngine.WSA.Application;
#endif

    public class UnityCoroutineHelper : MonoBehaviour
    {
        private static UnityCoroutineHelper Instance
        {
            get
            {
                var instance = FindObjectOfType<UnityCoroutineHelper>();
                if (instance == null)
                {
                    var gameObject = new GameObject("App Center Helper")
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                    DontDestroyOnLoad(gameObject);
                    instance = gameObject.AddComponent<UnityCoroutineHelper>();
                }
                return instance;
            }
        }

#if UNITY_WSA_10_0
        public static void StartCoroutine(Func<IEnumerator> coroutine)
        {
            if (WSAApplication.RunningOnAppThread())
            {
                Instance.StartCoroutine(coroutine());
            }
            else
            {
                WSAApplication.InvokeOnAppThread(() =>
                {
                    Instance.StartCoroutine(coroutine());
                }, false);
            }
        }
#else
        public static void StartCoroutine(Func<IEnumerator> coroutine)
        {
            Instance.StartCoroutine(coroutine());
        }
#endif
    }
}