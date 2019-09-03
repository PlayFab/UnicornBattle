using UnicornBattle.Managers;
using UnicornBattle.Managers.Auth;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
    /// <summary>
    /// View controller for the login with PlayFab account fields  
    /// </summary>
    public class PlayFabLoginController : MonoBehaviour
    {

        // login fields
        public Transform loginGroup;
        public InputField User;
        public InputField Password;
        public Button Login;

        //registration fields
        public Transform registerGroup;
        public InputField user;
        public InputField email;
        public InputField pass1;
        public InputField pass2;
        public Button register;

        // toggle button
        public Button createAccountBtn;

        private PlayFabAuthManager Authentication
        {
            get { return (PlayFabAuthManager) MainManager.Instance.getAuthManager(); }
        }

        void OnEnable()
        {
            this.Password.text = string.Empty;
        }

        void Start()
        {
            Login.onClick.AddListener(() => LogIn());
            register.onClick.AddListener(() => RegisterNewAccount());
            createAccountBtn.onClick.AddListener(() => ToggleDevRegisterFields());
        }

        public void LogIn()
        {
            if (!isUsernameValid(User.text))
            {
                // error
                DialogCanvasController.Instance.HandleOnLoginFail("Invalid Username", MessageDisplayStyle.error);
            }
            if (!isPasswordValid(Password.text))
            {
                // error
                DialogCanvasController.Instance.HandleOnLoginFail("Invalid Password", MessageDisplayStyle.error);
            }

            //PF_Authentication.RequestSpinner();
            if (User.text.Contains("@"))
            {
                if (AuthenticationManager.ValidateEmail(User.text))
                {
                    DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);

                    Authentication.LoginWithEmail(
                        User.text,
                        Password.text,
                        OnLoginSuccess,
                        OnLoginFailure);
                }
                else
                {
                    // error
                    DialogCanvasController.Instance.HandleOnLoginFail("Invalid Email", MessageDisplayStyle.error);
                }
            }
            else
            {
                DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);

                Authentication.LoginWithUsername(
                    User.text,
                    Password.text,
                    OnLoginSuccess,
                    OnLoginFailure);
            }
        }

        public void ToggleDevRegisterFields()
        {
            if (this.loginGroup.gameObject.activeSelf)
            {
                this.loginGroup.gameObject.SetActive(false);
                this.registerGroup.gameObject.SetActive(true);
                this.createAccountBtn.GetComponentInChildren<Text>().text = GlobalStrings.LOGOUT_BTN_TXT;
            }
            else
            {
                this.loginGroup.gameObject.SetActive(true);
                this.registerGroup.gameObject.SetActive(false);
                this.createAccountBtn.GetComponentInChildren<Text>().text = GlobalStrings.CREATE_BTN_TXT;
            }
        }

        public void RegisterNewAccount()
        {
            //PlayFabLoginCalls.RequestSpinner();
            if (user.text.Length == 0
                || pass1.text.Length == 0
                || pass2.text.Length == 0
                || email.text.Length == 0)
            {
                DialogCanvasController.Instance.HandleOnLoginFail("All fields are required.", MessageDisplayStyle.error);
                return;
            }

            var passwordCheck = AuthenticationManager.ValidatePassword(pass1.text, pass2.text);
            var emailCheck = AuthenticationManager.ValidateEmail(email.text);

            if (!passwordCheck)
            {
                DialogCanvasController.Instance.HandleOnLoginFail("Passwords must match and be longer than 5 characters.", MessageDisplayStyle.error);
                return;
            }
            if (!emailCheck)
            {
                DialogCanvasController.Instance.HandleOnLoginFail("Invalid Email format.", MessageDisplayStyle.error);
                return;
            }
            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);

            Authentication.RegisterNewAccount(
                user.text,
                pass1.text,
                email.text,
                OnRegisterSuccess,
                OnLoginFailure);
        }

        void OnLoginSuccess(string message)
        {
            PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GenericLogin);
            DialogCanvasController.Instance.HandleOnLoginSuccess(message, MessageDisplayStyle.error);
        }

        void OnLoginFailure(string message)
        {
            DialogCanvasController.Instance.HandleOnLoginFail(message, MessageDisplayStyle.error);
            Debug.LogWarning(message);
        }

        void OnRegisterSuccess(string message)
        {
            if (message.Contains("New Account Registered"))
            {
                //Debug.Log("Account Created, logging in with new account.");

                Authentication.LoginWithUsername(
                    user.text,
                    pass1.text,
                    OnLoginSuccess,
                    OnLoginFailure);
            }
        }

        void OnRegisterFailure(string message)
        {

        }

        bool isUsernameValid(string user)
        {
            if (user.Length == 0) return false;

            return true;
        }

        bool isPasswordValid(string pass)
        {
            if (pass.Length == 0) return false;

            return true;
        }
    }
}