using System;
using PlayFab.ClientModels;
using UnicornBattle.Managers;
using UnicornBattle.Managers.Auth;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{

    public class RegistrationController : MonoBehaviour
    {
        public InputField userName;
        public InputField email;
        public InputField pass1;
        public InputField pass2;

        public Button register;
        public Button exit;
        public AccountStatusController controller;
        private Action<AddUsernamePasswordResult> callback;

        private PlayFabAuthManager Authentication
        {
            get { return (PlayFabAuthManager) MainManager.Instance.getAuthManager(); }
        }

        public void Init(Action<AddUsernamePasswordResult> cb = null)
        {
            callback = cb;
        }

        public void CloseRegistration()
        {
            gameObject.SetActive(false);
        }

        public void RegisterClicked()
        {
            if (null == Authentication) return;

            if (!string.IsNullOrEmpty(pass1.text) && !string.IsNullOrEmpty(pass2.text) && string.Equals(pass1.text, pass2.text))
            {
                DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.AddUsernamePassword);

                Authentication.AddUserNameAndPassword(
                    userName.text,
                    pass1.text,
                    email.text,
                    result =>
                    {
                        callback(result);
                        CloseRegistration();
                        PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.AddUsernamePassword);
                    },
                    (f) => { PF_Bridge.RaiseCallbackError(f, PlayFabAPIMethods.AddUsernamePassword); }
                );
            }
            else
            {
                PF_Bridge.RaiseCallbackError(GlobalStrings.REGISTER_FAIL_MSG, PlayFabAPIMethods.AddUsernamePassword);
            }
        }
    }
}