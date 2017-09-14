// <copyright file="MessagingDeps.cs" company="Google Inc.">
// Copyright (C) 2016 Google Inc. All Rights Reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;

/// <summary>
/// This file is used to define dependencies, and pass them along to a system
/// which can resolve dependencies.
/// </summary>
[InitializeOnLoad]
public class FirebaseMessagingDeps : AssetPostprocessor
{
    /// <summary>
    /// This is the entry point for "InitializeOnLoad". It will register the
    /// dependencies with the dependency tracking system.
    /// </summary>
    static FirebaseMessagingDeps()
    {
        SetupDeps();
    }

    static void SetupDeps()
    {
#if UNITY_ANDROID
        // Setup the resolver using reflection as the module may not be
        // available at compile time.
        Type playServicesSupport = Google.VersionHandler.FindClass(
            "Google.JarResolver", "Google.JarResolver.PlayServicesSupport");
        if (playServicesSupport == null) {
            return;
        }

        Google.VersionHandler.InvokeStaticMethod(
            Google.VersionHandler.FindClass(
                "Google.JarResolver",
                "GooglePlayServices.PlayServicesResolver"),
            "AddAutoResolutionFilePatterns",
            new object[] { new HashSet<Regex> { new Regex(".*Deps\\.cs$") } });

        object svcSupport = Google.VersionHandler.InvokeStaticMethod(
            playServicesSupport, "CreateInstance",
            new object[] {
                "FirebaseMessaging",
                EditorPrefs.GetString("AndroidSdkRoot"),
                "ProjectSettings"
            });

        Google.VersionHandler.InvokeInstanceMethod(
            svcSupport, "DependOn",
            new object[] {
                "com.google.firebase",
                "firebase-messaging",
                "11.2.0"
            },
            namedArgs: new Dictionary<string, object>() {
                { "packageIds",
                    new string[] {
                        "extra-google-m2repository",
                        "extra-android-m2repository"
                    }
                },
                { "repositories",
                    null
                }
            });
        Google.VersionHandler.InvokeInstanceMethod(
            svcSupport, "DependOn",
            new object[] {
                "com.google.firebase",
                "firebase-core",
                "11.2.0"
            },
            namedArgs: new Dictionary<string, object>() {
                { "packageIds",
                    new string[] {
                        "extra-google-m2repository",
                        "extra-android-m2repository"
                    }
                },
                { "repositories",
                    null
                }
            });
        Google.VersionHandler.InvokeInstanceMethod(
            svcSupport, "DependOn",
            new object[] {
                "com.google.firebase",
                "firebase-messaging-unity",
                "4.1.0"
            },
            namedArgs: new Dictionary<string, object>() {
                { "packageIds",
                    null
                },
                { "repositories",
                    new string[] {
                        "Assets/Firebase/m2repository"
                    }
                }
            });
#elif UNITY_IOS
        Type iosResolver = Google.VersionHandler.FindClass(
            "Google.IOSResolver", "Google.IOSResolver");
        if (iosResolver == null) {
            return;
        }
        Google.VersionHandler.InvokeStaticMethod(
            iosResolver, "AddPod",
            new object[] { "Firebase/Messaging" }, 
            new Dictionary<string, object>() { 
                { "version", "4.1.0" },
                { "minTargetSdk", null },
                { "sources", null }
            });
#endif
    }

    // Handle delayed loading of the dependency resolvers.
    private static void OnPostprocessAllAssets(
            string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromPath) {
        foreach (string asset in importedAssets) {
            if (asset.Contains("IOSResolver") ||
                asset.Contains("JarResolver")) {
                SetupDeps();
                break;
            }
        }
    }
}

