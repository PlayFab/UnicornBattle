using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

namespace UnityEngine.UI
{
    [CustomEditor(typeof(TweenColor))]
    public class TweenColorEditor : TweenMainEditor
    {
        public override void OnInspectorGUI()
        {
            TweenColor self = (TweenColor)target;

            EditorGUILayout.BeginHorizontal();
            self.from = EditorGUILayout.ColorField("From", self.from);
            if (GUILayout.Button("\u25C0", GUILayout.Width(24f)))
                self.FromCurrentValue();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            self.to = EditorGUILayout.ColorField("To", self.to);
            if (GUILayout.Button("\u25C0", GUILayout.Width(24f)))
                self.ToCurrentValue();
            EditorGUILayout.EndHorizontal();

            BaseTweenerProperties();
        }
    }
}
