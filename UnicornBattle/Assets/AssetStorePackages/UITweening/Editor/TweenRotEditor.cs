using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

namespace UnityEngine.UI
{
    [CustomEditor(typeof(TweenRot))]
    public class TweenRotEditor : TweenMainEditor
    {
        public override void OnInspectorGUI()
        {
            TweenRot self = (TweenRot)target;

            EditorGUILayout.BeginHorizontal();
            self.from = EditorGUILayout.Vector3Field("From", self.from);
            if (GUILayout.Button("\u25C0", GUILayout.Width(24f)))
                self.FromCurrentValue();
            EditorGUILayout.EndHorizontal();
            self.fromOffset = EditorGUILayout.Toggle("Offset", self.fromOffset);

            EditorGUILayout.BeginHorizontal();
            self.to = EditorGUILayout.Vector3Field("To", self.to);
            if (GUILayout.Button("\u25C0", GUILayout.Width(24f)))
                self.ToCurrentValue();
            EditorGUILayout.EndHorizontal();
            self.toOffset = EditorGUILayout.Toggle("Offset", self.toOffset);

            BaseTweenerProperties();
        }
    }
}
