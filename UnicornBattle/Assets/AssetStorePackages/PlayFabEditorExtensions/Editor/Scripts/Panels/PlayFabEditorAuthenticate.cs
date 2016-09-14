using System.ComponentModel;
using System.Linq;
using PlayFab.Editor.EditorModels;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace PlayFab.Editor
{
    public class PlayFabEditorAuthenticate : UnityEditor.Editor
    {

#region panel variables
        private static string _userEmail = string.Empty;
        private static string _userPass = string.Empty;
        private static string _userPass2 = string.Empty;
        private static string _2faCode = string.Empty;
        private static string _studio = string.Empty;

        private static bool isInitialized = false;

        public enum PanelDisplayStates { Register, Login, TwoFactorPrompt }
        private static PanelDisplayStates activeState = PanelDisplayStates.Login;
#endregion


#region draw calls
        public static void DrawAuthPanels()
        {
            if(activeState == PanelDisplayStates.TwoFactorPrompt)
            {
                GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                    EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                        GUILayout.Label("Enter your 2-factor authorization code.", PlayFabEditorHelper.uiStyle.GetStyle("cGenTxt"), GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));
                    EditorGUILayout.EndHorizontal();

                   EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                            GUILayout.FlexibleSpace();
                            _2faCode = EditorGUILayout.TextField(_2faCode, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25), GUILayout.MinWidth(200));
                            GUILayout.FlexibleSpace();
                  EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"));

                        var buttonWidth = 100;
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("CONTINUE", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.MaxWidth(buttonWidth)))
                        {
                            OnContinueButtonClicked();
                            _2faCode = string.Empty;

                        }
                        GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"));

                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("CANCEL", PlayFabEditorHelper.uiStyle.GetStyle("textButton")))
                        {
                            activeState = PanelDisplayStates.Login;

                        }
                        GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                return;
            }
            

            if(!string.IsNullOrEmpty(PlayFabEditorDataService.accountDetails.email) && !isInitialized)
            {
                _userEmail = PlayFabEditorDataService.accountDetails.email;
                isInitialized = true;
            }
            else if(!isInitialized)
            {
                activeState = PanelDisplayStates.Register;
                isInitialized = true;
            }

            EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                GUILayout.Label("Welcome to PlayFab!", PlayFabEditorHelper.uiStyle.GetStyle("titleLabel"), GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));
            EditorGUILayout.EndHorizontal();

            if(activeState == PanelDisplayStates.Login)
            {
                // login mode, this state either logged out, or did not have auto-login checked.
                DrawLogin();

            }
            else if (activeState == PanelDisplayStates.Register)
            {
                // register mode 
                DrawRegister();
            }
            else
            {
                DrawRegister();
            }

                

            GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
            GUILayout.Label("Our editor extension provides an easy way to manage the PlayFab SDK. \n\nExisting users may LOG IN using their developer account. \n\nNew users must CREATE AN ACCOUNT.", PlayFabEditorHelper.uiStyle.GetStyle("cGenTxt"), GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));



            GUILayout.EndVertical();

            //capture enter input for login
            Event e = Event.current;
            if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Return)
            {
                switch(activeState)
                {
                    case PanelDisplayStates.Login:
                        OnLoginButtonClicked();
                    break;
                    case PanelDisplayStates.Register:
                        OnRegisterClicked();
                    break;
                    case PanelDisplayStates.TwoFactorPrompt:
                        OnContinueButtonClicked();
                    break;
                }
            }
        }

        public static void DrawLogin()
        {

            float labelWidth = 120;

            GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));

             using (FixedWidthLabel fwl = new FixedWidthLabel("EMAIL: "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _userEmail = EditorGUILayout.TextField(_userEmail, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
            }

            using (FixedWidthLabel fwl = new FixedWidthLabel("PASSWORD: "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _userPass = EditorGUILayout.PasswordField(_userPass, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
            }

            EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("labelStyle")); //var buttonRect = 


            if(GUILayout.Button("CREATE AN ACCOUNT", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MaxWidth(100) ))
            {
                activeState = PanelDisplayStates.Register;
            }

            var buttonWidth = 100;
            GUILayout.Space(EditorGUIUtility.currentViewWidth - buttonWidth * 2);

            if (GUILayout.Button("LOG IN", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.MaxWidth(buttonWidth)))
            {
               
                OnLoginButtonClicked();
            }


            GUILayout.EndHorizontal();


            GUILayout.EndVertical();



        }

        public static void DrawRegister()
        {
            float labelWidth = 150;

            GUILayout.BeginVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));

             using (FixedWidthLabel fwl = new FixedWidthLabel("EMAIL:"))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _userEmail = EditorGUILayout.TextField(_userEmail, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
            }

            using (FixedWidthLabel fwl = new FixedWidthLabel("PASSWORD:"))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _userPass = EditorGUILayout.PasswordField(_userPass, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
            }

            using (FixedWidthLabel fwl = new FixedWidthLabel("CONFIRM PASSWORD:  "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _userPass2 = EditorGUILayout.PasswordField(_userPass2, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
            }

            using (FixedWidthLabel fwl = new FixedWidthLabel("STUDIO NAME:  "))
            {
                GUILayout.Space(labelWidth - fwl.fieldWidth);
                _studio = EditorGUILayout.TextField(_studio, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
            }

            EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")); //var buttonRect = 

            if (GUILayout.Button("LOG IN", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinHeight(32)))
            {
                activeState = PanelDisplayStates.Login;
            }

            GUILayout.FlexibleSpace();

            if(GUILayout.Button("  CREATE AN ACCOUNT  ", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32)))
            {
               OnRegisterClicked();
            }

            GUILayout.EndHorizontal();


            GUILayout.EndVertical();
        }
#endregion

#region menu and helper methods
        public static bool IsAuthenticated()
        {
            return string.IsNullOrEmpty(PlayFabEditorDataService.accountDetails.devToken) ? false : true;
        }

        public static void Logout()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnLogout);

            PlayFabEditorApi.Logout(new LogoutRequest()
            {
                DeveloperClientToken = PlayFabEditorDataService.accountDetails.devToken
            }, null, PlayFabEditorHelper.SharedErrorCallback);

            _userPass = string.Empty;

            _userPass2 = string.Empty;


            activeState = PanelDisplayStates.Login;

            PlayFabEditorDataService.accountDetails.studios.Clear();

            PlayFabEditorDataService.accountDetails.devToken = string.Empty;

            PlayFabEditorDataService.SaveAccountDetails();

            PlayFabEditorDataService.envDetails.titleData.Clear();
            PlayFabEditorDataService.SaveEnvDetails();
        }

        private static void OnRegisterClicked()
        {
            if(_userPass != _userPass2)
            {
                Debug.LogError("PlayFab developer account passwords must match.");
                return;
            }
           

            PlayFabEditorApi.RegisterAccouint(new RegisterAccountRequest()
            {
                DeveloperToolProductName = PlayFabEditorHelper.EDEX_NAME,  
                DeveloperToolProductVersion = PlayFabEditorHelper.EDEX_VERSION, 
                Email = _userEmail,
                Password = _userPass,
                StudioName = _studio
            }, (result) =>
            {
                PlayFabEditorDataService.accountDetails.devToken = result.DeveloperClientToken;
                PlayFabEditorDataService.accountDetails.email = _userEmail;
                 
                PlayFabEditorApi.GetStudios(new GetStudiosRequest(), (getStudioResult) =>
                {
                    PlayFabEditorDataService.accountDetails.studios = getStudioResult.Studios.ToList();
                    PlayFabEditorDataService.SaveAccountDetails();
                }, PlayFabEditorHelper.SharedErrorCallback);

                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnLogin);
                PlayFabEditorDataService.SaveAccountDetails();
                PlayFabEditorMenu._menuState = PlayFabEditorMenu.MenuStates.Sdks;

 

            }, PlayFabEditorHelper.SharedErrorCallback);
        }

        private static void OnLoginButtonClicked()
        {

            PlayFabEditorApi.Login(new LoginRequest()
            {
                DeveloperToolProductName = PlayFabEditorHelper.EDEX_NAME,  
                DeveloperToolProductVersion = PlayFabEditorHelper.EDEX_VERSION,
                Email = _userEmail,
                Password = _userPass
            }, (result) =>
            {
                PlayFabEditorDataService.accountDetails.devToken = result.DeveloperClientToken;
                PlayFabEditorDataService.accountDetails.email = _userEmail;

                PlayFabEditorApi.GetStudios(new GetStudiosRequest(), (getStudioResult) =>
                {
                    PlayFabEditorDataService.accountDetails.studios = getStudioResult.Studios.ToList();
                    PlayFabEditorDataService.SaveAccountDetails();
                }, PlayFabEditorHelper.SharedErrorCallback);

                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnLogin);
                PlayFabEditorDataService.SaveAccountDetails();
                PlayFabEditorMenu._menuState = PlayFabEditorMenu.MenuStates.Sdks;

            }, (error) =>
            {
                if((int)error.Error == 1246 || error.ErrorMessage.Contains("TwoFactor"))
                {
                    // pop 2FA dialog
                    PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnWarning, "This account requires 2-Factor Authentication.");
                    activeState = PanelDisplayStates.TwoFactorPrompt;
                }
                else
                {
                    PlayFabEditorHelper.SharedErrorCallback(error);
                }
            });
        }

        private static void OnContinueButtonClicked()
        {

            PlayFabEditorApi.Login(new LoginRequest()
            {
                DeveloperToolProductName = PlayFabEditorHelper.EDEX_NAME,  
                DeveloperToolProductVersion = PlayFabEditorHelper.EDEX_VERSION,
                TwoFactorAuth = _2faCode,
                Email = _userEmail,
                Password = _userPass
            }, (result) =>
            {
                PlayFabEditorDataService.accountDetails.devToken = result.DeveloperClientToken;
                PlayFabEditorDataService.accountDetails.email = _userEmail;


                PlayFabEditorApi.GetStudios(new GetStudiosRequest(), (getStudioResult) =>
                {
                    PlayFabEditorDataService.accountDetails.studios = getStudioResult.Studios.ToList();
                    PlayFabEditorDataService.SaveAccountDetails();
                }, PlayFabEditorHelper.SharedErrorCallback);

                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnLogin);
                PlayFabEditorDataService.SaveAccountDetails();
                PlayFabEditorMenu._menuState = PlayFabEditorMenu.MenuStates.Sdks;

            }, PlayFabEditorHelper.SharedErrorCallback);
        }
#endregion
    }
}