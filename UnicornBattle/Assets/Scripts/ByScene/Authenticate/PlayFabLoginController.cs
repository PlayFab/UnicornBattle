using UnityEngine;
using UnityEngine.UI;

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

    void OnEnable()
    {
        //		if(PlayFabLoginCalls.LoggedInUserInfo != null)
        //		{
        //			this.User.text = PlayFabLoginCalls.LoggedInUserInfo.Username;
        //		}
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
        //PF_Authentication.RequestSpinner();
        if (User.text.Contains("@"))
        {
            if (PF_Authentication.ValidateEmail(User.text))
            {
                PF_Authentication.LoginWithEmail(User.text, Password.text);
            }
        }
        else
        {
            PF_Authentication.LoginWithUsername(User.text, Password.text);
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
        PF_Authentication.RegisterNewPlayfabAccount(user.text, pass1.text, pass2.text, email.text);
    }

    void HandleCallbackSuccess(string message)
    {
        if (message.Contains("New Account Registered"))
        {
            Debug.Log("Account Created, logging in with new account.");
            PF_Authentication.LoginWithUsername(user.text, pass1.text);
        }
    }



}

