using UnicornBattle.Managers;
using UnicornBattle.Managers.Auth;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
    public class AuthenticationController : MonoBehaviour
    {
        public bool useDevLogin = false;
        public Transform developerLogin;
        public Image UB;
        public Button signInWithFB;
        public Button signInWithDeviceID;
        public Text DeviceIdDisplay;
        public Text Banner;
        public Text Status;
        public Transform PlayIcon;

        private AuthenticationManager Authentication
        {
            get { return MainManager.Instance.getAuthManager(); }
        }

        #region Monobehaviour Methods

        void Start()
        {
            if (useDevLogin)
            {
                EnableDeveloperMode();
            }
            else if (Authentication.IsLoggedOut())
            {
                EnableUserSelectMode();
            }
            else
            {
                EnableAutoMode();
                /*
                    1) check for login, attempt to login with device id (create account = false)
                    2) check catch error (Account not found)
                    3) if error, show logged out options
                    4a) login with device id, create account = true
                    4b) login with facebook account, create account = false
                    5) catch error, (account not found)
                    6) login with facebook account, attempt to link device id to newly create fb account
                    7) silently fail if the device is already linked.
			    */

                if (Application.platform == RuntimePlatform.Android
                    || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    //1) check for login, attempt to login with device id (create account = false)
                    // if (PF_Authentication.GetDeviceId())
                    // {
                    //     DeviceIdDisplay.text = PF_Authentication.android_id ?? PF_Authentication.ios_id;
                    // }
                    DeviceIdDisplay.text = Authentication.GetDeviceId();
                    SignInWithDeviceID();
                }
                else
                {
                    // this is only used by the unity editor.
                    Status.text = GlobalStrings.TEST_LOGIN_MSG;
                    EnableUserSelectMode();
                }
            }
        }
        #endregion

        #region UI Control
        public void EnableDeveloperMode()
        {
            DisableAutoMode();
            DisableUserSelectMode();
            developerLogin.gameObject.SetActive(true);
            Banner.text = GlobalStrings.TEST_LOGIN_PROMPT;
        }

        public void DisableDeveloperMode()
        {
            developerLogin.gameObject.SetActive(false);
            Banner.text = "";
        }

        public void EnableUserSelectMode()
        {
            DisableDeveloperMode();
            DisableAutoMode();
            signInWithDeviceID.gameObject.SetActive(true);
            signInWithFB.gameObject.SetActive(true);

            signInWithFB.onClick.RemoveAllListeners();
            signInWithFB.onClick.AddListener(signInWithFacebook);

            signInWithDeviceID.onClick.RemoveAllListeners();
            signInWithDeviceID.onClick.AddListener(() => SignInWithDeviceID(true));
            Banner.text = GlobalStrings.LOGOUT_BTN_MSG;
        }

        public void DisableUserSelectMode()
        {
            signInWithDeviceID.gameObject.SetActive(false);
            signInWithFB.gameObject.SetActive(false);
            Banner.text = "";
        }

        void signInWithFacebook()
        {
#if UNITY_ANDROID || UNITY_IOS
            FacebookAuthManager l_fbAuth = (FacebookAuthManager) Authentication;
            if (null == l_fbAuth) return;

            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);

            l_fbAuth.LoginWithFacebook(HandleOnLoginSuccess, (string failure) => { });
#endif
        }

        public void EnableAutoMode()
        {
            DisableDeveloperMode();
            DisableUserSelectMode();

            Status.text = GlobalStrings.AUTO_STATUS_MSG;
            Status.gameObject.SetActive(true);

            Banner.text = GlobalStrings.STATUS_PROMPT;
        }

        public void DisableAutoMode()
        {
            Status.gameObject.SetActive(false);
            Banner.text = "";
        }
        #endregion

        #region start login methods & callbacks
        void SignInWithDeviceID(bool p_shouldCreateNewAccount = false)
        {
            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);

            Authentication.LoginWithDeviceId(
                p_shouldCreateNewAccount,
                HandleOnLoginSuccess,
                HandleOnLoginFail);
        }

        void HandleOnLoginSuccess(string message)
        {
            // callbacks for success
            PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GenericLogin);
            DialogCanvasController.Instance.HandleOnLoginSuccess(message, MessageDisplayStyle.error);

            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetAccountInfo);

            MainManager.Instance.RefreshAll(true,
                (s) =>
                {
                    var l_characterMgr = MainManager.Instance.getCharacterManager();
                    if (null == l_characterMgr) return;

                    string id = l_characterMgr.GetMostRecentlyUsedCharacterId();

                    if (l_characterMgr.DoesCharacterExist(id))
                    {
                        GameController.Instance.SetActiveCharacter(id);
                        SceneController.Instance.RequestSceneChange(SceneController.GameScenes.Profile);
                    }
                    else
                    {
                        SceneController.Instance.RequestSceneChange(SceneController.GameScenes.CharacterSelect);
                    }

                    PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetAccountInfo);
                    PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetTitleData_General);
                    PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetAllUsersCharacters);
                    PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetCharacterData);
                    PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetCharacterStatistics);
                    PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetCharacterReadOnlyData);
                },
                (f) =>
                {
                    Debug.LogError(f);
                    EnableUserSelectMode();
                }
            );
        }

        void HandleOnLoginFail(string message)
        {
            DialogCanvasController.Instance.HandleOnLoginFail(message, MessageDisplayStyle.error);
            EnableUserSelectMode();
            Debug.LogWarning(message);
        }

        #endregion
    }
}