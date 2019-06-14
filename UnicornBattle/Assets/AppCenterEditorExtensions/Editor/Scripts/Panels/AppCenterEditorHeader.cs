using UnityEngine;
using UnityEditor;

namespace AppCenterEditor
{
    public class AppCenterEditorHeader : Editor
    {
        public static void DrawHeader(float progress = 0f)
        {
            if (AppCenterEditorHelper.uiStyle == null)
                return;
            using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleHeaderWrapper")))
            {
                //using Begin Vertical as our container.
                using (new AppCenterGuiFieldHelper.UnityHorizontal(GUILayout.Height(52)))
                {
                    EditorGUILayout.LabelField("", AppCenterEditorHelper.uiStyle.GetStyle("acLogo"), GUILayout.MaxHeight(60), GUILayout.Width(60));
                    EditorGUILayout.LabelField("App Center", AppCenterEditorHelper.uiStyle.GetStyle("gpStyleGray2"), GUILayout.MinHeight(52));

                    //end the vertical container
                }
            }

            ProgressBar.Draw();
        }
    }
}
