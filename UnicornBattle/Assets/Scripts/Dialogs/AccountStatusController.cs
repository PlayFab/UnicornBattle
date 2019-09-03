using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnicornBattle.Managers;
using UnicornBattle.Managers.Auth;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
#if UNITY_ANDROID || UNITY_IOS
using Facebook.Unity;
#endif

namespace UnicornBattle.Controllers
{

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

#pragma warning disable 0414
        private bool isFbSet = false;

#pragma warning disable 0414

        private PlayerManager m_PlayerManager;

#if UNITY_ANDROID || UNITY_IOS
        private FacebookAuthManager Authentication
        {
            get { return (FacebookAuthManager) MainManager.Instance.getAuthManager(); }
        }
#else
        private PlayFabAuthManager Authentication
        {
            get { return (PlayFabAuthManager) MainManager.Instance.getAuthManager(); }
        }
#endif

        private PlayerManager PlayerMgr
        {
            get
            {
                if (null == m_PlayerManager)
                    m_PlayerManager = MainManager.Instance.getPlayerManager();
                return m_PlayerManager;
            }
        }

        void Start()
        {
            //Debug.Log("Start PlayFab Token Calls");
#if UNITY_ANDROID || UNITY_IOS
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
#endif
            PF_Bridge.OnPlayfabCallbackSuccess += HandleCallbackSuccess;
            CheckPushStatus();
        }

        void OnDestroy()
        {
            //Debug.Log("OnDestroy PlayFab Token Calls");
#if UNITY_ANDROID || UNITY_IOS
            Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
#endif
            PF_Bridge.OnPlayfabCallbackSuccess -= HandleCallbackSuccess;
        }

        private void HandleCallbackSuccess(string details, PlayFabAPIMethods method, MessageDisplayStyle style)
        {
            switch (method)
            {
                case PlayFabAPIMethods.GetTitleData_General:
                    CheckPushStatus();
                    break;
            }
        }

        void Update()
        {

#if UNITY_ANDROID || UNITY_IOS
            if (isFbSet != FB.IsLoggedIn) // FB.IsLoggedIn doesn't update immediately, so you can't check it immediately after logout
                UpdateFacebookStatusButton();
            isFbSet = FB.IsLoggedIn;
#else
            isFbSet = false;
#endif

            registerPush.interactable = !string.IsNullOrEmpty(pushToken);
        }

#if UNITY_ANDROID || UNITY_IOS
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
#endif

        private void CheckPushStatus()
        {
            if (string.IsNullOrEmpty(pushToken))
                return;

#if UNITY_ANDROID && !UNITY_EDITOR
            var androidRequest = new AndroidDevicePushNotificationRegistrationRequest { DeviceToken = pushToken };
            PlayFab.PlayFabClientAPI.AndroidDevicePushNotificationRegistration(androidRequest, null, null);
#elif UNITY_IOS && !UNITY_EDITOR
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
            PlayerMgr.Refresh(false,
                (s) =>
                {
                    changedLoginState = false;
                    displayName.text = PlayerMgr.DisplayName;
                    var isRegistered = PlayerMgr.isPlayFabRegistered;
                    accountStatus.color = isRegistered ? Color.green : Color.red;
                    accountStatus.text = isRegistered ? GlobalStrings.ACT_STATUS_REG_MSG : GlobalStrings.ACT_STATUS_UNREG_MSG;
                    registerAccount.gameObject.SetActive(!isRegistered);
                    resetPassword.gameObject.SetActive(isRegistered);

                    SetCheckBox(showOnLogin.GetComponent<Image>(), PlayerMgr.showAccountOptionsOnLogin);
                    UpdateFacebookStatusButton();

#if UNITY_IOS
                    UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound, true);
                    pushToken = System.Text.Encoding.UTF8.GetString(UnityEngine.iOS.NotificationServices.deviceToken);
#endif

                    SetCheckBox(registerPush.GetComponent<Image>(), PlayerMgr.isRegisteredForPush);
                    CheckPushStatus();
                },
                (string error) =>
                {
                    return; // do not throw error message if failed
                }
            );
        }

        public void FetchWebAsset(string url, UnityAction<Texture2D> callback = null)
        {
            StartCoroutine(WebSpinner(url, callback));
        }

        public IEnumerator WebSpinner(string url, UnityAction<Texture2D> callback = null)
        {
            var uwr = new UnityWebRequest(url);
            uwr.downloadHandler = new DownloadHandlerTexture();
            yield return uwr.SendWebRequest();

            if (callback != null)
                callback(DownloadHandlerTexture.GetContent(uwr));
        }

        public void SetDisplayName()
        {
            Action<string> processResponse = displayNameStr =>
            {
                if (displayNameStr == null)
                    return;

                if (displayNameStr.Length < 3 || 20 < displayNameStr.Length)
                    return;

                DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.UpdateDisplayName);

                Authentication.UpdateDisplayName(displayNameStr,
                    result =>
                    {
                        PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.UpdateDisplayName);
                        this.displayName.text = result.DisplayName;
                        PlayerMgr.DisplayName = result.DisplayName;
                    },
                    (error) =>
                    {
                        Debug.LogError(error);
                    }
                );
            };

#if UNITY_ANDROID || UNITY_IOS
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
#else
            DialogCanvasController.RequestTextInputPrompt(GlobalStrings.DISPLAY_NAME_PROMPT, GlobalStrings.DISPLAY_NAME_MSG, processResponse);
#endif
        }

        public void TogglePushNotification()
        {

            if (!PlayerMgr.isRegisteredForPush)
            {
                DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.Generic);
                UnityAction afterPush = () =>
                {
                    changedLoginState = true;
                    PlayerMgr.isRegisteredForPush = true;
                    SetCheckBox(registerPush.GetComponent<Image>(), true);
                    Debug.Log("AccountStatusController: PUSH ENABLED!");
                    PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.Generic);
                };

                StartCoroutine(UBAnimator.Wait(1.5f, () =>
                {
                    DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.RegisterForPush);
                    PlayerMgr.RegisterForPushNotification(
                        pushToken,
                        (s) => { PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.RegisterForPush); },
                        (e) => { PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.RegisterForPush); }
                    );
                }));
            }
            else
            {
                Action<bool> processResponse = response =>
                {
                    if (!response)
                        return;

                    changedLoginState = true;
                    PlayerMgr.isRegisteredForPush = false;
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

                PlayerMgr.showAccountOptionsOnLogin = !PlayerMgr.showAccountOptionsOnLogin;
                changedLoginState = true;
                SetCheckBox(showOnLogin.GetComponent<Image>(), PlayerMgr.showAccountOptionsOnLogin);
            };

            if (PlayerMgr.showAccountOptionsOnLogin)
                DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.TOGGLE_PROMPT, GlobalStrings.TOGGLE_MSG, processResponse);
            else
                processResponse.Invoke(true);
        }

        public void ToggleFacebookLink()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (FB.IsLoggedIn)
            {
                Action<bool> afterCheck = response =>
                {
                    if (response)
                        Authentication.UnlinkFacebook(
                            success =>
                            {
                                PF_Bridge.RaiseCallbackSuccess(success, PlayFabAPIMethods.UnlinkFacebookId);
                            },
                            failure =>
                            {
                                PF_Bridge.RaiseCallbackError(failure, PlayFabAPIMethods.UnlinkFacebookId);
                            }
                        );
                };

                if (!PlayerMgr.isPlayFabRegistered)
                    DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.CONFIRM_UNLINK_PROMPT, GlobalStrings.CONFIRM_UNLINK_MSG, afterCheck);
                else
                    DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.CONFIRM_UNLINK_PROMPT, "Are you sure?", afterCheck);
            }
            else
            {
                DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.LinkFacebookId);

                Authentication.LinkFaceBookToPlayFab(false,
                    (string success) =>
                    {
                        Debug.Log("Facebook Linked Account!");
                        PF_Bridge.RaiseCallbackSuccess(success, PlayFabAPIMethods.LinkFacebookId);
                    },
                    (string failure) =>
                    {

                        if (!failure.Contains("already linked")) // ew, gotta get better error codes
                        {
                            PF_Bridge.RaiseCallbackError(failure, PlayFabAPIMethods.LinkFacebookId);
                            return;
                        }

                        // PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.LinkFacebookId, MessageDisplayStyle.none);
                        System.Action<bool> afterConfirm = (bool response) =>
                        {
                            if (!response)
                                return;

                            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.LinkFacebookId);
                            Authentication.LinkFaceBookToPlayFab(true,
                                (string success2) =>
                                {
                                    PF_Bridge.RaiseCallbackSuccess(success2, PlayFabAPIMethods.LinkFacebookId);
                                },
                                (string failure2) =>
                                {
                                    PF_Bridge.RaiseCallbackError(failure2, PlayFabAPIMethods.LinkFacebookId);
                                });
                        };

                        DialogCanvasController.RequestConfirmationPrompt("Caution!", "Your current facebook account is already linked to another Unicorn Battle player. Do you want to force-bind your Facebook account to this player?", afterConfirm);
                    }
                );
            }
#endif
        }

        private void UpdateFacebookStatusButton()
        {
#if UNITY_ANDROID || UNITY_IOS
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
#endif
        }

        public void ShowRegistration()
        {
            Action<AddUsernamePasswordResult> afterRegistration = result =>
            {
                PlayerMgr.Username = result.Username;
                PlayerMgr.UserEmail = "Pending Refresh";

                Dictionary<string, object> eventData = new Dictionary<string, object>()
                {
                    //pull email from RC due to it not being returned.
                    { "Username", result.Username }, { "Email", rc.email.text }
                };
                TelemetryManager.RecordPlayerEvent(TelemetryEvent.Client_RegisteredAccount, eventData);

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
            Action<bool> afterCheck = (response) =>
            {
                if (!response)
                    return;

                DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.SendAccountRecoveryEmail);

                string email = (string.IsNullOrEmpty(PlayerMgr.UserEmail) || PlayerMgr.UserEmail.Contains("Pending Refresh")) ? rc.email.text : PlayerMgr.UserEmail;

                Authentication.SendAccountRecoveryEmail(
                    email,
                    (r) =>
                    {
                        PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.SendAccountRecoveryEmail);
                    },
                    (e) =>
                    {
                        PF_Bridge.RaiseCallbackError(string.Empty, PlayFabAPIMethods.SendAccountRecoveryEmail);
                    }
                );
            };

            DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.RECOVER_EMAIL_PROMPT, GlobalStrings.RECOVER_EMAIL_MSG, afterCheck);
        }

        public void Continue()
        {
            if (changedLoginState)
            {
                var l_playerDataMgr = MainManager.Instance.getPlayerManager();
                if (null == l_playerDataMgr) return;

                DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.UpdateUserData);

                l_playerDataMgr.UpdatePublicUserData(onUpdateUserDataSuccess, onUpdateUserDataFailure);
            }
            gameObject.SetActive(false);
        }

        void onUpdateUserDataSuccess(string message)
        {
            PF_Bridge.RaiseCallbackSuccess(message, PlayFabAPIMethods.UpdateUserData);
        }

        void onUpdateUserDataFailure(string message)
        {
            PF_Bridge.RaiseCallbackError(message, PlayFabAPIMethods.UpdateUserData);
        }
    }
}