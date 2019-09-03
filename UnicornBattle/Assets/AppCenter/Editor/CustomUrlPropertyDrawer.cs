// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CustomUrlProperty))]
public class CustomUrlPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.Next(true);
        var urlName = property.stringValue;
        var useLabel = new GUIContent("Use Custom " + urlName + " URL");
        var urlLabel = new GUIContent("Custom " + urlName + " URL");

        // Though the property may have double height, each child should have
        // half that height.
        position.height = EditorGUIUtility.singleLineHeight;
        property.Next(false);
        EditorGUI.PropertyField(position, property, useLabel);
        if (property.boolValue)
        {
            property.Next(false);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property, urlLabel);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // If "set custom log url" is true, need to make room for the text field.
        property.Next(true);
        property.Next(false);

        var height = base.GetPropertyHeight(property, label);
        if (property.boolValue)
        {
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        return height;
    }
}
