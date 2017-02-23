using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AccountStatusController : MonoBehaviour
{
    public Text displayName;
    public Text accountStatus;
    public Image facebookPicture;
    public Button setDisplayName;
    public Button registerAccount;
    public Button registerPush;
    public Button showOnLogin;
    public Button linkToFaceBook;
    public Button continueBtn;
    public Button resetPassword;
    public Sprite checkedBox;
    public Sprite uncheckedBox;
    public RegistrationController rc;

    private string pushToken = "";
    private bool changedLoginState = false;

    void Start()
    {
        Debug.Log("AccountStatusController: Start \"" + PF_GameData.AndroidPushSenderId + "\"");
#if UNITY_ANDROID && !UNITY_EDITOR
		PlayFabGoogleCloudMessaging._RegistrationReadyCallback += OnGCMReady;
		PlayFabGoogleCloudMessaging._RegistrationCallback += OnGCMRegistration;
#endif
        PF_Bridge.OnPlayfabCallbackSuccess += OnTitleDataSet;
        CheckPushStatus();
    }

    void OnDestroy()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		PlayFabGoogleCloudMessaging._RegistrationReadyCallback -= OnGCMReady;
		PlayFabGoogleCloudMessaging._RegistrationCallback -= OnGCMRegistration;
#endif
        PF_Bridge.OnPlayfabCallbackSuccess -= OnTitleDataSet;
    }

    private void OnTitleDataSet(string details, PlayFabAPIMethods method, MessageDisplayStyle style)
    {
        if (method == PlayFabAPIMethods.GetTitleData_General)
            CheckPushStatus();
    }

#if UNITY_ANDROID && !UNITY_EDITOR
	public void OnGCMReady(bool status)
	{
		Debug.Log("AccountStatusController: GCM Ready!");
		PlayFabGoogleCloudMessaging.GetToken();
	}
	
	public void OnGCMRegistration(string token, string error)
    {
        Debug.Log("AccountStatusController: GCM Token Received: " + token);
        
		if(!string.IsNullOrEmpty(error))
			Debug.Log("GCM Error: " + error);
		else if (token != null)
			pushToken = token;
        CheckPushStatus();
	}
#endif

    private void CheckPushStatus()
    {
        registerPush.interactable = !string.IsNullOrEmpty(PF_GameData.AndroidPushSenderId) && !string.IsNullOrEmpty(pushToken);
        if (!string.IsNullOrEmpty(PF_GameData.AndroidPushSenderId))
            PlayFabAndroidPlugin.Init(PF_GameData.AndroidPushSenderId);
    }

    private void SetCheckBox(Image image, bool isChecked)
    {
        image.overrideSprite = isChecked ? checkedBox : uncheckedBox;
    }

    public void Init()
    {
        if (PF_PlayerData.accountInfo == null)
            return;

        changedLoginState = false;
        displayName.text = PF_PlayerData.accountInfo.TitleInfo.DisplayName;
        if (string.IsNullOrEmpty(PF_PlayerData.accountInfo.Username) || string.IsNullOrEmpty(PF_PlayerData.accountInfo.PrivateInfo.Email))
        {
            accountStatus.color = Color.red;
            accountStatus.text = GlobalStrings.ACT_STATUS_UNREG_MSG;
            registerAccount.gameObject.SetActive(true);
            resetPassword.gameObject.SetActive(false);
        }
        else
        {
            accountStatus.color = Color.green;
            accountStatus.text = GlobalStrings.ACT_STATUS_REG_MSG;
            registerAccount.gameObject.SetActive(false);
            resetPassword.gameObject.SetActive(true);
        }

        SetCheckBox(showOnLogin.GetComponent<Image>(), PF_PlayerData.showAccountOptionsOnLogin);

        if (PF_PlayerData.accountInfo.FacebookInfo != null)
        {
            var btnText = linkToFaceBook.GetComponentInChildren<Text>();
            btnText.text = GlobalStrings.UNLINK_FB_BTN_MSG;

            UnityAction<Texture2D> afterGetPhoto = tx =>
            {
                facebookPicture.overrideSprite = Sprite.Create(tx, new Rect(0, 0, 128, 128), new Vector2());
            };

            StartCoroutine(FacebookHelperClass.GetPlayerProfilePhoto(FetchWebAsset, afterGetPhoto));
        }
        else
        {
            var btnText = linkToFaceBook.GetComponentInChildren<Text>();
            btnText.text = GlobalStrings.LINK_FB_BTN_MSG;
            facebookPicture.overrideSprite = null;
        }

#if UNITY_IPHONE
		UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound, true);
#endif

        SetCheckBox(registerPush.GetComponent<Image>(), PF_PlayerData.isRegisteredForPush);
        CheckPushStatus();
    }

    public void FetchWebAsset(string url, UnityAction<Texture2D> callback = null)
    {
        StartCoroutine(WebSpinner(url, callback));
    }

    public IEnumerator WebSpinner(string url, UnityAction<Texture2D> callback = null)
    {
        var www = new WWW(url);
        yield return www;

        if (callback != null)
            callback(www.texture);
    }

    public void SetDisplayName()
    {
        Action<string> processResponse = response =>
        {
            if (response == null)
                return;

            PF_Authentication.UpdateDisplayName(response, result =>
            {
                displayName.text = result.DisplayName;
                PF_PlayerData.accountInfo.TitleInfo.DisplayName = result.DisplayName;
            });
        };

        if (PF_PlayerData.accountInfo.FacebookInfo != null)
        {
            UnityAction<string> afterGetFbName = fbName =>
            {
                DialogCanvasController.RequestTextInputPrompt(GlobalStrings.DISPLAY_NAME_PROMPT, GlobalStrings.DISPLAY_NAME_MSG, processResponse, fbName);
            };
            FacebookHelperClass.GetFBUserName(afterGetFbName);
        }
        else
        {
            DialogCanvasController.RequestTextInputPrompt(GlobalStrings.DISPLAY_NAME_PROMPT, GlobalStrings.DISPLAY_NAME_MSG, processResponse);
        }
    }

    public void TogglePushNotification()
    {
        if (!PF_PlayerData.isRegisteredForPush)
        {
            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.Generic);
            UnityAction afterPush = () =>
            {
                changedLoginState = true;
                PF_PlayerData.isRegisteredForPush = true;
                SetCheckBox(registerPush.GetComponent<Image>(), true);
                Debug.Log("AccountStatusController: PUSH ENABLED!");
                PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.Generic, MessageDisplayStyle.none);
            };

            StartCoroutine(PF_GamePlay.Wait(1.5f, () =>
            {
                PF_PlayerData.RegisterForPushNotification(pushToken, afterPush);
            }));
        }
        else
        {
            Action<bool> processResponse = response =>
            {
                if (!response)
                    return;

                changedLoginState = true;
                PF_PlayerData.isRegisteredForPush = false;
                SetCheckBox(registerPush.GetComponent<Image>(), false);
                Debug.Log("AccountStatusController: PUSH DISABLED!");
            };

            DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.PUSH_NOTIFY_PROMPT, GlobalStrings.PUSH_NOTIFY_MSG, processResponse);
        }
    }

    public void ToggleShowOnLogin()
    {
        Action<bool> processResponse = response =>
        {
            if (!response)
                return;

            PF_PlayerData.showAccountOptionsOnLogin = !PF_PlayerData.showAccountOptionsOnLogin;
            changedLoginState = true;
            SetCheckBox(showOnLogin.GetComponent<Image>(), PF_PlayerData.showAccountOptionsOnLogin);
        };

        if (PF_PlayerData.showAccountOptionsOnLogin)
            DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.TOGGLE_PROMPT, GlobalStrings.TOGGLE_MSG, processResponse);
        else
            processResponse.Invoke(true);
    }

    public void ToggleFacebookLink()
    {
        if (PF_PlayerData.accountInfo.FacebookInfo != null)
        {
            Action<bool> afterCheck = response =>
            {
                if (!response)
                    return;

                UnityAction afterUnlink = () =>
                {
                    var txt = linkToFaceBook.GetComponentInChildren<Text>();
                    txt.color = Color.red;
                    txt.text = GlobalStrings.LINK_FB_BTN_MSG;
                    facebookPicture.overrideSprite = null;
                    PF_PlayerData.accountInfo.FacebookInfo = null;
                    changedLoginState = true;
                };

                PF_PlayerData.UnlinkFBAccount(afterUnlink);
            };

            DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.CONFIRM_UNLINK_PROMPT, GlobalStrings.CONFIRM_UNLINK_MSG, afterCheck);
        }
        else
        {
            // link
            UnityAction afterLink = () =>
            {
                var btnText = linkToFaceBook.GetComponentInChildren<Text>();
                btnText.text = GlobalStrings.UNLINK_FB_BTN_MSG;
                btnText.color = Color.green;

                changedLoginState = true;
                PF_PlayerData.accountInfo.FacebookInfo = new UserFacebookInfo();

                UnityAction<Texture2D> afterGetPhoto = tx =>
                {
                    facebookPicture.overrideSprite = Sprite.Create(tx, new Rect(0, 0, 128, 128), new Vector2());
                };

                StartCoroutine(FacebookHelperClass.GetPlayerProfilePhoto(FetchWebAsset, afterGetPhoto));
            };

            PF_PlayerData.LinkFBAccount(afterLink);
        }
    }

    public void ShowRegistration()
    {
        UnityAction<AddUsernamePasswordResult> afterRegistration = result =>
        {
            PF_PlayerData.accountInfo.Username = result.Username;
            PF_PlayerData.accountInfo.PrivateInfo.Email = "Pending Refresh";

            Dictionary<string, object> eventData = new Dictionary<string, object>()
            {
				//pull emailf from RC due to it not being returned.
				{ "Username", result.Username},
                { "Email", rc.email.text}
            };
            PF_Bridge.LogCustomEvent(PF_Bridge.CustomEventTypes.Client_RegisteredAccount, eventData);

            registerAccount.gameObject.SetActive(false);
            accountStatus.text = GlobalStrings.ACT_STATUS_REG_MSG;
            resetPassword.gameObject.SetActive(true);
            accountStatus.color = Color.green;
        };

        rc.gameObject.SetActive(true);
        rc.Init(afterRegistration);
    }


    public void SendRecoveryEmail()
    {
        Action<bool> afterCheck = response =>
        {
            if (!response)
                return;

            var email = string.IsNullOrEmpty(PF_PlayerData.accountInfo.PrivateInfo.Email) || PF_PlayerData.accountInfo.PrivateInfo.Email.Contains("Pending Refresh") ? rc.email.text : PF_PlayerData.accountInfo.PrivateInfo.Email;
            PF_Authentication.SendAccountRecoveryEmail(email);
        };

        DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.RECOVER_EMAIL_PROMPT, GlobalStrings.RECOVER_EMAIL_MSG, afterCheck);
    }

    public void Continue()
    {
        if (changedLoginState)
        {
            Dictionary<string, string> updates = new Dictionary<string, string> {
                { "ShowAccountOptionsOnLogin", PF_PlayerData.showAccountOptionsOnLogin ? "1" : "0" },
                { "IsRegisteredForPush", PF_PlayerData.isRegisteredForPush ? "1" : "0" },
            };
            PF_PlayerData.UpdateUserData(updates);
        }
        gameObject.SetActive(false);
    }
}
