namespace PlayFab.Editor
{
    using System;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using System.Linq;

    [InitializeOnLoad]
    public class PlayFabEditorDataMenu : Editor
    {
#region panel variables
        public static TitleDataViewer tdViewer;

        public static MenuComponent menu = null;

        public enum DataMenuStates { TitleData }
        public static DataMenuStates currentState = DataMenuStates.TitleData;

        private static Vector2 scrollPos = Vector2.zero;

#endregion


#region draw calls
        public static void DrawDataPanel()
        {
            if(PlayFabEditorDataService.isDataLoaded)
            {
                if(menu != null)
                {
                    menu.DrawMenu();

                    switch(currentState)
                    {
                        case DataMenuStates.TitleData:
                            if(tdViewer == null && !string.IsNullOrEmpty(PlayFabEditorDataService.envDetails.selectedTitleId)) //&& !string.IsNullOrEmpty(PlayFabEditorDataService.envDetails.developerSecretKey)
                            {
                                tdViewer = ScriptableObject.CreateInstance<TitleDataViewer>();
                                foreach(var item in PlayFabEditorDataService.envDetails.titleData)
                                {
                                    tdViewer.items.Add(new KvpItem(item.Key, item.Value));
                                }
                            }
                            else if(!string.IsNullOrEmpty(PlayFabEditorDataService.envDetails.selectedTitleId) ) //&& !string.IsNullOrEmpty(PlayFabEditorDataService.envDetails.developerSecretKey))
                            {
                                if(tdViewer.items.Count == 0)
                                {
                                    foreach(var item in PlayFabEditorDataService.envDetails.titleData)
                                    {
                                        tdViewer.items.Add(new KvpItem(item.Key, item.Value));
                                    }
                                }
                                scrollPos = GUILayout.BeginScrollView(scrollPos, PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                                tdViewer.Draw();
                                GUILayout.EndScrollView();
                            }
                           
                        break;

                        default:
                            EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                                GUILayout.Label("Coming Soon!", PlayFabEditorHelper.uiStyle.GetStyle("titleLabel"), GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));
                            GUILayout.EndHorizontal();
                        break;
                       
                    }
                }
                else
                {
                    RegisterMenu();
                }
            }

        }
#endregion


#region unity loops
        static PlayFabEditorDataMenu()
        {
            if(!PlayFabEditor.IsEventHandlerRegistered(StateUpdateHandler))
            {
                PlayFabEditor.EdExStateUpdate += StateUpdateHandler;
            }

            RegisterMenu();
        }

        public void OnDestroy()
        {
            if(PlayFabEditor.IsEventHandlerRegistered(StateUpdateHandler))
            {
                PlayFabEditor.EdExStateUpdate -= StateUpdateHandler;
            }
        }
#endregion


#region menu and helper methods
        public static void RegisterMenu()
        {
            if ( menu == null)
            {
                menu = ScriptableObject.CreateInstance<MenuComponent>();
                menu.RegisterMenuItem("TITLE DATA", OnTitleDataClicked);
            }
        }

        public static void StateUpdateHandler(PlayFabEditor.EdExStates state, string status, string json)
        {
            switch(state)
            {
                case PlayFabEditor.EdExStates.OnMenuItemClicked:
                    // do things here
                break;
                case PlayFabEditor.EdExStates.OnLogout:
                    if(tdViewer != null)
                    {
                        tdViewer.items.Clear();
                    }
                break;
            }
        }

        public static void OnTitleDataClicked()
        {
            currentState = DataMenuStates.TitleData;
        }
   }
#endregion
}
