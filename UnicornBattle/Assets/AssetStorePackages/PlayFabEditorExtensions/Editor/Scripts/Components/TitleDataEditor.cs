namespace PlayFab.Editor
{
    using UnityEditor;
    using UnityEngine;
    using System.Collections;


    public class TitleDataEditor : EditorWindow {
        public static TitleDataEditor myInstance;
        public static TitleDataEditor myWindow;

        public string key = string.Empty;
        public string Value = string.Empty;

        public string displayTitle = "";
        public Vector2 scrollPos = Vector2.zero;


        public static void  ShowWindow (TitleDataEditor tdInstance) {
           // myInstance tdInstance;
            myInstance = tdInstance;
         
            myWindow = EditorWindow.GetWindow<TitleDataEditor>();
            myWindow.key = tdInstance.key;
            myWindow.Value = string.IsNullOrEmpty(tdInstance.Value) ? "ENTER A VALUE" : tdInstance.Value;

            myWindow.Show();
        }
        
        void OnGUI () {
            // The actual window code goes here
            EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                GUILayout.Label(string.Format("Editing: {0}", key), PlayFabEditorHelper.uiStyle.GetStyle("orTitle"), GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));
            EditorGUILayout.EndHorizontal();

            scrollPos = GUILayout.BeginScrollView(scrollPos, PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                Value = GUILayout.TextArea(Value, PlayFabEditorHelper.uiStyle.GetStyle("editTxt"));
            GUILayout.EndScrollView();


            EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Save",  PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxWidth(200)))
                {
                    for(int z = 0; z < PlayFabEditorDataMenu.tdViewer.items.Count; z++)
                    {
                        if(PlayFabEditorDataMenu.tdViewer.items[z].Key == key)
                        {
                            PlayFabEditorDataMenu.tdViewer.items[z].Value = Value;
                            PlayFabEditorDataMenu.tdViewer.items[z].isDirty = true;
                        }
                    }
                    Close();

                }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            Repaint();
        }

        public void LoadData(string k, string v)
        {
            key = k;
            Value = v;
        }

    }
}