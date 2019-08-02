// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using UnityEditor;

[CustomEditor(typeof(AppCenterBehaviorAdvanced))]
public class AppCenterBehaviorEditorAdvanced : Editor
{
    private Editor settingsEditorAdvanced;

    public override void OnInspectorGUI()
    {
        // Load or create settings.
        var behaviour = (AppCenterBehaviorAdvanced) target;
        if (behaviour.SettingsAdvanced == null)
        {
            behaviour.SettingsAdvanced = AppCenterSettingsContext.CreateSettingsInstanceAdvanced();
        }
        
        // Draw settings.
        if (settingsEditorAdvanced == null)
        {
            settingsEditorAdvanced = CreateEditor(behaviour.SettingsAdvanced);
        }
        settingsEditorAdvanced.OnInspectorGUI();
    }

    public void OnDestroy()
    {
        // If the component is removed from GameObject then remove the related asset.
        if (!target && FindObjectsOfType<AppCenterBehaviorAdvanced>().Length == 0)
        {
            AppCenterSettingsContext.DeleteSettingsInstanceAdvanced();
        }
    }
}
