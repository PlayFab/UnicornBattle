// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MaxStorageSizeProperty))]
public class MaxStorageSizePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var useLabel = new GUIContent("Use Custom Max Storage Size");
        var urlLabel = new GUIContent("Max Storage Size Bytes");
        position.height = EditorGUIUtility.singleLineHeight; // Though the property may have double height, each child should have half that height.
        property.Next(true);
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
        property.Next(true); // If "Use Custom Max Storage Size" is true, need to make room for the text field.
        var height = base.GetPropertyHeight(property, label);
        if (property.boolValue)
        {
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        return height;
    }
}
