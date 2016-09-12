using UnityEngine;
using System.Collections;
using UnityEditor;

namespace PlayFab.Editor
{
    public class PlayFabEditorHeader : UnityEditor.Editor
    {
        public static ProgressBar progressBar = new ProgressBar();

        public static void DrawHeader(float progress = 0f)
        {
            //using Begin Vertical as our container.
            GUILayout.BeginHorizontal(GUILayout.Height(52));

            //Set the image in the container
            if (EditorGUIUtility.currentViewWidth < 375)
            {
                GUILayout.Label("", PlayFabEditorHelper.uiStyle.GetStyle("pfLogo"), GUILayout.MaxHeight(40), GUILayout.Width(186));
            }
            else
            {
                GUILayout.Label("", PlayFabEditorHelper.uiStyle.GetStyle("pfLogo"), GUILayout.MaxHeight(50), GUILayout.Width(233));
            }

            float gmAnchor = EditorGUIUtility.currentViewWidth - 30;


                if (EditorGUIUtility.currentViewWidth > 375)
                {
                    gmAnchor = EditorGUIUtility.currentViewWidth - 140;
                    GUILayout.BeginArea(new Rect(gmAnchor, 10, 140, 42));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("GAME MANAGER", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MaxWidth(105)))
                    {
                        OnDashbaordClicked();
                    }
                }
                else
                {
                    GUILayout.BeginArea(new Rect(gmAnchor, 10, EditorGUIUtility.currentViewWidth * .25f, 42));
                    GUILayout.BeginHorizontal();
                }

                if (GUILayout.Button("", PlayFabEditorHelper.uiStyle.GetStyle("gmIcon")))
                    {
                        OnDashbaordClicked();
                    }
               GUILayout.EndHorizontal();
            GUILayout.EndArea();

            //end the vertical container
            GUILayout.EndHorizontal();

            ProgressBar.Draw();

        }


        private static void OnDashbaordClicked()
        {
            Help.BrowseURL(PlayFabEditorDataService.activeTitle != null ? PlayFabEditorDataService.activeTitle.GameManagerUrl : PlayFabEditorHelper.GAMEMANAGER_URL);
        }

    }
}



