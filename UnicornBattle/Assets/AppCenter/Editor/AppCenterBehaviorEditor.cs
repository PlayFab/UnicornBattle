// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

using UnityEditor;

[CustomEditor(typeof(AppCenterBehavior))]
public class AppCenterBehaviorEditor : Editor
{
    private Editor settingsEditor;

    public override void OnInspectorGUI()
    {
        // Load or create settings.
        var behaviour = (AppCenterBehavior) target;
        if (behaviour.Settings == null)
        {
            behaviour.Settings = AppCenterSettingsContext.SettingsInstance;
        }
        
        // Draw settings.
        if (settingsEditor == null)
        {
            settingsEditor = CreateEditor(behaviour.Settings);
        }
        settingsEditor.OnInspectorGUI();
    }
}
