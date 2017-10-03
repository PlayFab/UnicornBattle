using Facebook.Unity;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
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
    private bool isFbSet = false;

    void Start()
    {
        Debug.Log("Start PlayFab Token Calls");
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        PF_Bridge.OnPlayfabCallbackSuccess += HandleCallbackSuccess;
        CheckPushStatus();
    }

    void OnDestroy()
    {
        Debug.Log("OnDestroy PlayFab Token Calls");
        Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
        PF_Bridge.OnPlayfabCallbackSuccess -= HandleCallbackSuccess;
    }

    private void HandleCallbackSuccess(string details, PlayFabAPIMethods method, MessageDisplayStyle style)
    {
        switch (method)
        {
            case PlayFabAPIMethods.GetTitleData_General: CheckPushStatus(); break;
        }
    }

    void Update()
    {
        if (isFbSet != FB.IsLoggedIn) // FB.IsLoggedIn doesn't update immediately, so you can't check it immediately after logout
            UpdateFacebookStatusButton();
        isFbSet = FB.IsLoggedIn;
        registerPush.interactable = !string.IsNullOrEmpty(pushToken);
    }

    private void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("PlayFab: Received Registration Token: " + token.Token);
        pushToken = token.Token;
        CheckPushStatus();
    }

    private void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        if (e.Message.Data != null)
            Debug.Log("PlayFab: Received a message with data");
        if (e.Message.Notification != null)
            Debug.Log("PlayFab: Received a notification");
    }

    private void CheckPushStatus()
    {
        if (string.IsNullOrEmpty(pushToken))
            return;

#if UNITY_ANDROID && !UNITY_EDITOR
        var androidRequest = new AndroidDevicePushNotificationRegistrationRequest { DeviceToken = pushToken };
        PlayFab.PlayFabClientAPI.AndroidDevicePushNotificationRegistration(androidRequest, null, null);
#elif UNITY_IPHONE && !UNITY_EDITOR
        var iosRequest = new RegisterForIOSPushNotificationRequest { DeviceToken = pushToken };
        PlayFab.PlayFabClientAPI.RegisterForIOSPushNotification(iosRequest, null, null);
#endif
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
        var isRegistered = PF_PlayerData.isPlayFabRegistered;
        accountStatus.color = isRegistered ? Color.green : Color.red;
        accountStatus.text = isRegistered ? GlobalStrings.ACT_STATUS_REG_MSG : GlobalStrings.ACT_STATUS_UNREG_MSG;
        registerAccount.gameObject.SetActive(!isRegistered);
        resetPassword.gameObject.SetActive(isRegistered);

        SetCheckBox(showOnLogin.GetComponent<Image>(), PF_PlayerData.showAccountOptionsOnLogin);
        UpdateFacebookStatusButton();

#if UNITY_IPHONE
		UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound, true);
        pushToken = UnityEngine.iOS.NotificationServices.deviceToken;
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

        if (FB.IsLoggedIn)
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
        if (FB.IsLoggedIn)
        {
            Action<bool> afterCheck = response =>
            {
                if (response)
                    PF_Authentication.UnlinkFbAccount();
            };

            if (!PF_PlayerData.isPlayFabRegistered)
                DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.CONFIRM_UNLINK_PROMPT, GlobalStrings.CONFIRM_UNLINK_MSG, afterCheck);
            else
                DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.CONFIRM_UNLINK_PROMPT, "Are you sure?", afterCheck);
        }
        else
        {
            PF_Authentication.StartFacebookLogin(); // This will do the linking automatically based on being logged in
        }
    }

    private void UpdateFacebookStatusButton()
    {
        Debug.Log("UpdateFacebookStatusButton: " + FB.IsLoggedIn);
        var txt = linkToFaceBook.GetComponentInChildren<Text>();
        txt.text = FB.IsLoggedIn ? GlobalStrings.UNLINK_FB_BTN_MSG : GlobalStrings.LINK_FB_BTN_MSG;

        facebookPicture.overrideSprite = null;
        if (FB.IsLoggedIn)
        {
            UnityAction<Texture2D> afterGetPhoto = tx =>
            {
                facebookPicture.overrideSprite = Sprite.Create(tx, new Rect(0, 0, 128, 128), Vector2.zero);
            };
            StartCoroutine(FacebookHelperClass.GetPlayerProfilePhoto(FetchWebAsset, afterGetPhoto));
        }
    }

    public void ShowRegistration()
    {
        Action<AddUsernamePasswordResult> afterRegistration = result =>
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
