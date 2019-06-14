using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AppCenterEditor
{
    public static class AppCenterGuiFieldHelper
    {

        /// <summary>
        /// A disposable wrapper for Verticals, to ensure they're paired properly, and to make the code visually block together within them
        /// </summary>
        public class UnityHorizontal : IDisposable
        {
            public UnityHorizontal(params GUILayoutOption[] options)
            {
                EditorGUILayout.BeginHorizontal(options);
            }

            public UnityHorizontal(GUIStyle style, params GUILayoutOption[] options)
            {
                EditorGUILayout.BeginHorizontal(style, options);
            }

            public void Dispose()
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// A disposable wrapper for Horizontals, to ensure they're paired properly, and to make the code visually block together within them
        /// </summary>
        public class UnityVertical : IDisposable
        {
            public UnityVertical(params GUILayoutOption[] options)
            {
                EditorGUILayout.BeginVertical(options);
            }

            public UnityVertical(GUIStyle style, params GUILayoutOption[] options)
            {
                EditorGUILayout.BeginVertical(style, options);
            }

            public void Dispose()
            {
                EditorGUILayout.EndVertical();
            }
        }
    }
}
