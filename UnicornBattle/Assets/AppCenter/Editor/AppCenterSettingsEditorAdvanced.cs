// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AppCenterSettingsAdvanced))]
public class AppCenterSettingsEditorAdvanced : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("TransmissionTargetToken"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("StartAndroidNativeSDKFromAppCenterBehavior"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("StartIOSNativeSDKFromAppCenterBehavior"), new GUIContent("Start iOS Native SDK From App Center Behavior"));
        //The following line can be useful if you want to be able to configure StartupType from AppCenter Behaviour Advanced.
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("AppCenterStartupType"));
        serializedObject.ApplyModifiedProperties();
    }
}
