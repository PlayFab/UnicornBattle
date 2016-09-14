namespace PlayFab.Editor
{
    using UnityEngine;
    using System.Collections;
    using UnityEditor;

    public class PlayFabEditorMenu : Editor
    {
    #region panel variables
        internal enum MenuStates
        {
            Sdks = 0,
            Settings = 1,
            Data = 2,
            Help = 3,
            Logout = 4
        }

        internal static MenuStates _menuState = MenuStates.Sdks;
    #endregion

        public static void DrawMenu()
        {
            if (PlayFabEditorSDKTools.IsInstalled && PlayFabEditorSDKTools.isSdkSupported)
            {
                _menuState = (MenuStates) PlayFabEditorDataService.editorSettings.currentMainMenu;
            }

            var sdksButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            var settingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            var dataButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            var helpButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            var logoutButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");


            if (_menuState == MenuStates.Sdks)
            {
                sdksButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton_selected");
            }
            else
            {
                sdksButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            }

            if (_menuState == MenuStates.Settings)
            {
                settingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton_selected");
            }
            else
            {
                settingsButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            }

            if (_menuState == MenuStates.Logout)
            {
                logoutButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton_selected");
            }
            else
            {
                logoutButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            }

            if (_menuState == MenuStates.Data)
            {
                dataButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton_selected");
            }
            else
            {
                dataButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            }

            if (_menuState == MenuStates.Help)
            {
                helpButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton_selected");
            }
            else
            {
                helpButtonStyle = PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            }

            GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"), GUILayout.Height(25), GUILayout.ExpandWidth(true));
   
            GUILayout.Space(5);

            if (GUILayout.Button("SDK", sdksButtonStyle, GUILayout.MaxWidth(35)))
            {
                OnSdKsClicked();
            }


            if (PlayFabEditorSDKTools.IsInstalled && PlayFabEditorSDKTools.isSdkSupported)
            {
                if (GUILayout.Button("SETTINGS", settingsButtonStyle, GUILayout.MaxWidth(65)))
                {
                    OnSettingsClicked();
                }

                if (GUILayout.Button("DATA", dataButtonStyle, GUILayout.MaxWidth(45)))
                {
                    
                    OnDataClicked();
                }

            }

            if (GUILayout.Button("HELP", helpButtonStyle, GUILayout.MaxWidth(45)))
                {
                 
                    OnHelpClicked();
                   
                }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("LOGOUT", logoutButtonStyle, GUILayout.MaxWidth(55)))
            {
               
                OnLogoutClicked();
            }

            GUILayout.EndHorizontal();
        }



        public static void OnDataClicked()
        {
            _menuState = MenuStates.Data;

            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnMenuItemClicked, MenuStates.Data.ToString());

            PlayFabEditorDataService.editorSettings.currentMainMenu = (int)MenuStates.Data;
            PlayFabEditorDataService.SaveEditorSettings();
        }

        public static void OnHelpClicked()
        {
            _menuState = MenuStates.Help;

            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnMenuItemClicked, MenuStates.Help.ToString());
            PlayFabEditorDataService.editorSettings.currentMainMenu = (int)MenuStates.Help;      
            PlayFabEditorDataService.SaveEditorSettings();
        }

//        public static void OnServicesClicked()
//        {
//            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnMenuItemClicked, MenuStates.Services.ToString());
//            PlayFabEditorDataService.editorSettings.currentMainMenu = (int)MenuStates.Services;      
//            PlayFabEditorDataService.SaveEditorSettings();
//        }

        public static void OnSdKsClicked()
        {
            _menuState = MenuStates.Sdks;

            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnMenuItemClicked, MenuStates.Sdks.ToString());
            PlayFabEditorDataService.editorSettings.currentMainMenu = (int)MenuStates.Sdks;      
            PlayFabEditorDataService.SaveEditorSettings();
        }

        public static void OnSettingsClicked()
        {
            _menuState = MenuStates.Settings;

            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnMenuItemClicked, MenuStates.Settings.ToString());
            PlayFabEditorDataService.editorSettings.currentMainMenu = (int)MenuStates.Settings;      
            PlayFabEditorDataService.SaveEditorSettings();
        }

        public static void OnLogoutClicked()
        {
            _menuState = MenuStates.Logout;

            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnMenuItemClicked, MenuStates.Logout.ToString());

            PlayFabEditorAuthenticate.Logout();


            _menuState = MenuStates.Sdks;

            PlayFabEditorDataService.editorSettings.currentMainMenu = (int)MenuStates.Sdks;      
            PlayFabEditorDataService.SaveEditorSettings();
        }
    }
}