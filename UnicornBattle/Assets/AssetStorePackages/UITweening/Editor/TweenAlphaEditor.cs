using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

namespace UnityEngine.UI
{
    [CustomEditor(typeof(TweenAlpha))]
    public class TweenAlphaEditor : TweenMainEditor
    {
        public override void OnInspectorGUI()
        {
            TweenAlpha self = (TweenAlpha)target;

            EditorGUILayout.BeginHorizontal();
            self.from = EditorGUILayout.Slider("From", self.from, 0f, 1f);
            if (GUILayout.Button("\u25C0", GUILayout.Width(24f)))
                self.FromCurrentValue();
            EditorGUILayout.EndHorizontal();
            self.fromOffset = EditorGUILayout.Toggle("Offset", self.fromOffset);

            EditorGUILayout.BeginHorizontal();
            self.to = EditorGUILayout.Slider("To", self.to, 0f, 1f);
            if (GUILayout.Button("\u25C0", GUILayout.Width(24f)))
                self.ToCurrentValue();
            EditorGUILayout.EndHorizontal();
            self.toOffset = EditorGUILayout.Toggle("Offset", self.toOffset);

            BaseTweenerProperties();
        }
    }
}
