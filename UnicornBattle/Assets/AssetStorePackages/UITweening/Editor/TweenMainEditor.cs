using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

namespace UnityEngine.UI
{
    [CustomEditor(typeof(TweenMain), true)]
    public class TweenMainEditor : Editor
    {
        private SerializedProperty
            style,
            method,
            functionCurve,
            delay,
            duration,
            ignoreTimeScale,
            onFinished;

        private bool tweenerFoldOut = true;

        void OnEnable()
        {
            style = serializedObject.FindProperty("style");
            method = serializedObject.FindProperty("method");
            functionCurve = serializedObject.FindProperty("functionCurve");
            ignoreTimeScale = serializedObject.FindProperty("ignoreTimeScale");
            delay = serializedObject.FindProperty("delay");
            duration = serializedObject.FindProperty("duration");
            onFinished = serializedObject.FindProperty("OnFinished");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            BaseTweenerProperties();
        }

        protected void BaseTweenerProperties()
        {
            serializedObject.Update();

            tweenerFoldOut = EditorGUILayout.Foldout(tweenerFoldOut, "TweenSettings:");
            if (tweenerFoldOut)
            {
                BeginContents();
                EditorGUILayout.PropertyField(style, new GUIContent("Tween Style"));
                EditorGUILayout.PropertyField(method);
                EditorGUILayout.PropertyField(functionCurve, new GUIContent("Curve"), GUILayout.Height(52f));
                EditorGUILayout.PropertyField(ignoreTimeScale, new GUIContent("Ignore TimeScale"));
                EditorGUILayout.PropertyField(delay);
                EditorGUILayout.PropertyField(duration);
                EndContents();
            }

            EditorGUILayout.PropertyField(onFinished, new GUIContent("OnFinished"));
            serializedObject.ApplyModifiedProperties();
        }

        protected void BeginContents()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
            GUILayout.Space(10f);
            GUILayout.BeginVertical();
            GUILayout.Space(2f);
        }

        protected void EndContents()
        {
            GUILayout.Space(3f);
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
}
