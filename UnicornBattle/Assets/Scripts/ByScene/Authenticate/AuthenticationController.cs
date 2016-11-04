using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using PlayFab;
using Facebook.Unity;

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


    // Use this for initialization
    void Start()
    {
        if (PlayerPrefs.HasKey("TitleId"))
        {
            PlayFabSettings.TitleId = PlayerPrefs.GetString("TitleId");
        }
        else
        {
            PlayFabSettings.TitleId = GlobalStrings.DEFAULT_UB_TITLE_ID;
        }

        if (this.useDevLogin == true)
        {
            EnableDeveloperMode();
        }
        else if (PF_Authentication.isLoggedOut == true)
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
                    this.DeviceIdDisplay.text = PF_Authentication.android_id == null ? PF_Authentication.ios_id : PF_Authentication.android_id;
                }

                SigninWithDeviceID();
            }
            else
            {
                // this is only used by the unity editor.
                this.Status.text = GlobalStrings.TEST_LOGIN_MSG;
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
        this.developerLogin.gameObject.SetActive(true);
        this.Banner.text = GlobalStrings.TEST_LOGIN_PROMPT;
    }

    public void DisableDeveloperMode()
    {
        this.developerLogin.gameObject.SetActive(false);
        this.Banner.text = "";
    }

    public void EnableUserSelectMode()
    {
        DisableDeveloperMode();
        DisableAutoMode();
        signInWithDeviceID.gameObject.SetActive(true);
        signInWithFB.gameObject.SetActive(true);


        signInWithFB.onClick.RemoveAllListeners();
        signInWithFB.onClick.AddListener(() => SigninWithFB());

        signInWithDeviceID.onClick.RemoveAllListeners();
        signInWithDeviceID.onClick.AddListener(() => SigninWithDeviceID(true));
        this.Banner.text = GlobalStrings.LOGOUT_BTN_MSG;
    }

    public void DisableUserSelectMode()
    {
        signInWithDeviceID.gameObject.SetActive(false);
        signInWithFB.gameObject.SetActive(false);
        this.Banner.text = "";
    }

    public void EnableAutoMode()
    {
        DisableDeveloperMode();
        DisableUserSelectMode();

        this.Status.text = GlobalStrings.AUTO_STATUS_MSG;
        this.Status.gameObject.SetActive(true);

        this.Banner.text = GlobalStrings.STATUS_PROMPT;

    }

    public void DisableAutoMode()
    {
        this.Status.gameObject.SetActive(false);
        this.Banner.text = "";
    }
    #endregion

    #region start login methods & callbacks
    void SigninWithFB(bool testMode = false)
    {
        UnityAction afterFBInit = () =>
        {
            FB.ActivateApp();
            FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, (ILoginResult response) =>
               {
                   if (response.Error != null)
                   {
                       PF_Bridge.RaiseCallbackError("Facebook Error: " + response.Error, PlayFabAPIMethods.LoginWithFacebook, MessageDisplayStyle.none);
                   }
                   else if (!FB.IsLoggedIn)
                   {
                       PF_Bridge.RaiseCallbackError("You canceled the Facebook session, without an active facebook session photos and other data will not be accessable.", PlayFabAPIMethods.LoginWithFacebook, MessageDisplayStyle.none);
                   }
                   else
                   {
                       PF_Authentication.usedManualFacebook = testMode == true ? false : true;
                       PF_Authentication.LoginWithFacebook(AccessToken.CurrentAccessToken.TokenString, true);
                   }
               });
        };

        PF_Authentication.StartFacebookLogin(() =>
        {
            afterFBInit();
        });

    }

    void SigninWithDeviceID(bool createAccount = false)
    {
        UnityAction accountNotFoundCallback = () =>
        {
            EnableUserSelectMode();
        };

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
