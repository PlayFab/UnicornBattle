using Facebook.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    #region Monobehaviour Methods
    void OnEnable()
    {
        PF_Authentication.OnLoginFail += HandleOnLoginFail;
        PF_Authentication.OnLoginSuccess += HandleOnLoginSuccess;
    }

    void OnDisable()
    {
        PF_Authentication.OnLoginFail -= HandleOnLoginFail;
        PF_Authentication.OnLoginSuccess -= HandleOnLoginSuccess;
    }

    void Start()
    {
        if (useDevLogin)
        {
            EnableDeveloperMode();
        }
        else if (PF_Authentication.isLoggedOut)
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

            if (PF_Authentication.CheckForSupportedMobilePlatform())
            {
                //1) check for login, attempt to login with device id (create account = false)
                if (PF_Authentication.GetDeviceId())
                {
                    DeviceIdDisplay.text = PF_Authentication.android_id ?? PF_Authentication.ios_id;
                }

                SigninWithDeviceID();
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
        signInWithFB.onClick.AddListener(PF_Authentication.StartFacebookLogin);

        signInWithDeviceID.onClick.RemoveAllListeners();
        signInWithDeviceID.onClick.AddListener(() => SigninWithDeviceID(true));
        Banner.text = GlobalStrings.LOGOUT_BTN_MSG;
    }

    public void DisableUserSelectMode()
    {
        signInWithDeviceID.gameObject.SetActive(false);
        signInWithFB.gameObject.SetActive(false);
        Banner.text = "";
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
    void SigninWithDeviceID(bool createAccount = false)
    {
        UnityAction accountNotFoundCallback = EnableUserSelectMode;

        PF_Authentication.LoginWithDeviceId(createAccount, accountNotFoundCallback);
    }

    void HandleOnLoginSuccess(string message, MessageDisplayStyle style)
    {
        if (message.Contains("SUCCESS"))
        {
            SceneController.Instance.RequestSceneChange(SceneController.GameScenes.CharacterSelect);
        }
    }

    void HandleOnLoginFail(string message, MessageDisplayStyle style)
    {
        Debug.Log(message);
        EnableUserSelectMode();
    }
    #endregion
}
