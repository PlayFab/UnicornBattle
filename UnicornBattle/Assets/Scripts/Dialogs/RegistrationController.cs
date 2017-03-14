using System;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

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
        if (!string.IsNullOrEmpty(pass1.text) && !string.IsNullOrEmpty(pass2.text) && string.Equals(pass1.text, pass2.text))
        {
            PF_Authentication.AddUserNameAndPassword(userName.text, pass1.text, email.text, result =>
            {
                callback(result);
                CloseRegistration();
            });
        }
        else
        {
            PF_Bridge.RaiseCallbackError(GlobalStrings.REGISTER_FAIL_MSG, PlayFabAPIMethods.AddUsernamePassword, MessageDisplayStyle.error);
        }
    }
}
