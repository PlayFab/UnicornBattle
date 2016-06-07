using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UnityEngine.UI
{
    [CustomEditor(typeof(TweenSlider))]
    public class TweenSliderEditor : TweenMainEditor
    {
        public override void OnInspectorGUI()
        {
            TweenSlider self = (TweenSlider)target;

            EditorGUILayout.BeginHorizontal();
            self.from = EditorGUILayout.Slider("From", self.from, self.Sld.minValue, self.Sld.maxValue);
            if (GUILayout.Button("\u25C0", GUILayout.Width(24f)))
                self.FromCurrentValue();
            EditorGUILayout.EndHorizontal();
            self.fromOffset = EditorGUILayout.Toggle("Offset", self.fromOffset);

            EditorGUILayout.BeginHorizontal();
            self.to = EditorGUILayout.Slider("To", self.to, self.Sld.minValue, self.Sld.maxValue);
            if (GUILayout.Button("\u25C0", GUILayout.Width(24f)))
                self.ToCurrentValue();
            EditorGUILayout.EndHorizontal();
            self.toOffset = EditorGUILayout.Toggle("Offset", self.toOffset);

            BaseTweenerProperties();
        }
    }
}
