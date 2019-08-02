using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Events;

namespace UnicornBattle.Managers.Auth
{
    /// <summary>
    /// PLAYFAB AUTHENTICATION MANAGER
    /// ==============================
    /// wraps up PlayFab's authentication, letting the player to log in and out, and get the necessary auth tokens
    /// </summary>

    public class PlayFabAuthManager : AuthenticationManager
    {
        protected string m_deviceID;
        protected string m_sessionTicket;

        public PlayFabAuthManager() : base() { }

        /// <summary>
        /// Is the User Logged In?
        /// </summary>
        /// <returns><c>true</c>, if logged in; <c>false</c> otherwise</returns>
        public override bool IsLoggedIn()
        {
            return PlayFabClientAPI.IsClientLoggedIn();
        }

        public override bool IsLoggedOut()
        {
            return string.IsNullOrEmpty(m_sessionTicket);
        }

        /// <summary>
        /// Gets the device identifier
        /// </summary>
        /// <returns>the device identifier</returns>
        public override string GetDeviceId()
        {
            if (string.IsNullOrEmpty(m_deviceID))
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
#if UNITY_ANDROID
                        //http://answers.unity3d.com/questions/430630/how-can-i-get-android-id-.html
                        AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                        AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
                        AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
                        AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
                        m_deviceID = clsSecure.CallStatic<string>("getString", objResolver, "android_id");
#endif
                        break;
                    case RuntimePlatform.IPhonePlayer:
#if UNITY_IOS
                        m_deviceID = UnityEngine.iOS.Device.vendorIdentifier;
#endif
                        break;
                    default:
                        m_deviceID = SystemInfo.deviceUniqueIdentifier;
                        break;
                }
                PlayerPrefs.SetString("LastDeviceIdUsed", m_deviceID);
                PlayerPrefs.Save();
            }
            return m_deviceID;
        }

        /// <summary>
        /// Logins the with device identifier (iOS & Android only).
        /// </summary>
        public override void LoginWithDeviceId(
            bool p_shouldCreateNewAccount,
            System.Action<string> p_onSuccessCallback,
            System.Action<string> p_onFailureCallback)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    Debug.Log("Using Android Device ID: " + GetDeviceId());

                    PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest
                        {
                            AndroidDeviceId = GetDeviceId(),
                                TitleId = PlayFabSettings.TitleId,
                                CreateAccount = p_shouldCreateNewAccount
                        },
                        (LoginResult r) =>
                        {
                            OnLoginResult(r, p_onSuccessCallback);
                        },
                        (PlayFabError e) =>
                        {
                            OnLoginError(e, p_onFailureCallback);
                        });
                    break;

                case RuntimePlatform.IPhonePlayer:
                    Debug.Log("Using IOS Device ID: " + GetDeviceId());
                    PlayFabClientAPI.LoginWithIOSDeviceID(new LoginWithIOSDeviceIDRequest
                        {
                            DeviceId = GetDeviceId(),
                                TitleId = PlayFabSettings.TitleId,
                                CreateAccount = p_shouldCreateNewAccount
                        },
                        (LoginResult r) =>
                        {
                            OnLoginResult(r, p_onSuccessCallback);
                        },
                        (PlayFabError e) =>
                        {
                            OnLoginError(e, p_onFailureCallback);
                        });

                    break;

                default:
                    Debug.Log("Using custom device ID: " + GetDeviceId());
                    PlayFabClientAPI.LoginWithCustomID(
                        new LoginWithCustomIDRequest
                        {
                            CustomId = GetDeviceId(),
                                TitleId = PlayFabSettings.TitleId,
                                CreateAccount = p_shouldCreateNewAccount
                        },
                        (LoginResult r) =>
                        {
                            OnLoginResult(r, p_onSuccessCallback);
                        },
                        (PlayFabError e) =>
                        {
                            OnLoginError(e, p_onFailureCallback);
                        });

                    break;
            }
        }

        /// <summary>
        /// Login with PlayFab username.
        /// </summary>
        /// <param name="p_user">Username to use</param>
        /// <param name="pass">Password to use</param>
        public override void LoginWithUsername(
            string p_user,
            string p_password,
            System.Action<string> p_onSuccessCallback,
            System.Action<string> p_onFailureCallback)
        {
            PlayFabClientAPI.LoginWithPlayFab(
                new LoginWithPlayFabRequest
                {
                    Username = p_user,
                        Password = p_password,
                        TitleId = PlayFabSettings.TitleId
                },
                (LoginResult r) =>
                {
                    OnLoginResult(r, p_onSuccessCallback);
                },
                (PlayFabError e) =>
                {
                    OnLoginError(e, p_onFailureCallback);
                });

        }

        /// <summary>
        /// Login using the email associated with a PlayFab account.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="password">Password.</param>
        public override void LoginWithEmail(
            string p_email,
            string p_password,
            System.Action<string> p_onSuccessCallback,
            System.Action<string> p_onFailureCallback)
        {
            PlayFabClientAPI.LoginWithEmailAddress(
                new LoginWithEmailAddressRequest
                {
                    Email = p_email,
                        Password = p_password,
                        TitleId = PlayFabSettings.TitleId
                },
                (LoginResult r) =>
                {
                    OnLoginResult(r, p_onSuccessCallback);
                },
                (PlayFabError e) =>
                {
                    OnLoginError(e, p_onFailureCallback);
                });
        }

        /// <summary>
        /// Registers the new PlayFab account.
        /// </summary>
        public override void RegisterNewAccount(
            string p_username,
            string p_password,
            string p_email,
            System.Action<string> p_onSuccessCallback,
            System.Action<string> p_onFailureCallback)
        {

            var request = new RegisterPlayFabUserRequest
            {
                TitleId = PlayFabSettings.TitleId,
                Username = p_username,
                Email = p_email,
                Password = p_password
            };

            PlayFabClientAPI.RegisterPlayFabUser(
                request,
                (RegisterPlayFabUserResult r) =>
                {
                    OnRegisterResult(r, p_onSuccessCallback);
                },
                (PlayFabError e) =>
                {
                    OnLoginError(e, p_onFailureCallback);
                });

        }

        /// <summary>
        /// Logs the user out.
        /// </summary>
        public override void Logout(System.Action<string> p_onSuccessCallback,
            System.Action<string> p_onFailureCallback)
        {
            m_sessionTicket = string.Empty;

            PlayFabClientAPI.ForgetAllCredentials();
            PlayerPrefs.DeleteKey("LinkedFacebook");
            PlayerPrefs.DeleteKey("LastDeviceIdUsed");
            PlayerPrefs.Save();

            if (null != p_onSuccessCallback)
            {
                p_onSuccessCallback.Invoke("Logout"); // because it was like that before
            }
        }

        /// <summary>
        /// Calls the UpdateUserTitleDisplayName request API
        /// </summary>
        public void UpdateDisplayName(
            string displayName,
            System.Action<UpdateUserTitleDisplayNameResult> p_onSuccessCallback,
            System.Action<PlayFabError> p_onFailureCallback)
        {
            var request = new UpdateUserTitleDisplayNameRequest { DisplayName = displayName };
            PlayFabClientAPI.UpdateUserTitleDisplayName(
                request,
                p_onSuccessCallback,
                p_onFailureCallback);
        }

        /// <summary>
        /// Adds the user name and password to a partial (guest) account
        /// </summary>
        /// <param name="user">User - username to use (must be unique)</param>
        /// <param name="pass">Pass - password to use for the account, (must be > 5 characters)</param>
        /// <param name="email">Email - email to use (must be unique)</param>
        public void AddUserNameAndPassword(
            string user,
            string pass,
            string email,
            System.Action<AddUsernamePasswordResult> p_onSuccessCallback,
            System.Action<string> p_onFailureCallback)
        {
            var request = new AddUsernamePasswordRequest
            {
                Email = email,
                Password = pass,
                Username = user
            };

            PlayFabClientAPI.AddUsernamePassword(
                request,
                p_onSuccessCallback,
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Triggers the backend to send an account recovery email to the account provided
        /// </summary>
        /// <param name="email">Email to match</param>
        public override void SendAccountRecoveryEmail(string email,
            System.Action<string> p_onSuccessCallback,
            System.Action<string> p_onFailureCallback)
        {
            var request = new SendAccountRecoveryEmailRequest
            {
                Email = email,
                TitleId = PlayFabSettings.TitleId
            };

            PlayFabClientAPI.SendAccountRecoveryEmail(
                request,
                (SendAccountRecoveryEmailResult r) =>
                {
                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke(string.Empty);
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Links a mobile device to a PlayFab account via the unique device id (A device can only be linked to one account at a time)
        /// </summary>
        public override void LinkDeviceId()
        {
            UnityAction<PlayFabError> onLinkError = error =>
            {
                PF_Bridge.RaiseCallbackError("Unable to link device: " + error.ErrorMessage, PlayFabAPIMethods.LinkDeviceID);
            };

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    Debug.Log("Linking Android");

                    PlayFabClientAPI.LinkAndroidDeviceID(
                        new LinkAndroidDeviceIDRequest { AndroidDeviceId = GetDeviceId() },
                        null,
                        error => { onLinkError(error); });
                    break;
                case RuntimePlatform.IPhonePlayer:
                    Debug.Log("Linking iOS");

                    PlayFabClientAPI.LinkIOSDeviceID(
                        new LinkIOSDeviceIDRequest { DeviceId = GetDeviceId() },
                        null,
                        error => { onLinkError(error); });
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Unlinks a mobile device from a PlayFab account
        /// </summary>
        public override void UnlinkDeviceId()
        {
            // if (!GetDeviceId())
            //     return;

            //if (!string.IsNullOrEmpty(android_id))
            //{
            //    Debug.Log("Unlinking Android");
            //    UnlinkAndroidDeviceIDRequest request = new UnlinkAndroidDeviceIDRequest();
            //    PlayFabClientAPI.UnlinkAndroidDeviceID(request, OnUnlinkAndroidDeviceIdSuccess, OnPlayFabCallbackError);
            //}
            //else if (!string.IsNullOrEmpty(ios_id))
            //{
            //    Debug.Log("Unlinking iOS");
            //    UnlinkIOSDeviceIDRequest request = new UnlinkIOSDeviceIDRequest();
            //    PlayFabClientAPI.UnlinkIOSDeviceID(request, OnUnlinkIosDeviceIdSuccess, OnPlayFabCallbackError);
            //}
        }

        public void VerifyNewTitleId(string p_id,
            System.Action<string> p_onSuccessCallback,
            System.Action<string> p_onFailureCallback)
        {
            LoginWithDeviceId(false, p_onSuccessCallback, p_onFailureCallback);
        }

        /// <summary>
        /// Called on a successful login attempt
        /// </summary>
        /// <param name="p_result">Result object returned from PlayFab server</param>
        /// <param name="p_callback">Callback after processing result</param>
        protected virtual void OnLoginResult(LoginResult p_result, System.Action<string> p_callback)
        {
            var l_playerMgr = MainManager.Instance.getPlayerManager();
            if (null == l_playerMgr) return;

            l_playerMgr.PlayerID = p_result.PlayFabId;
            m_sessionTicket = p_result.SessionTicket;

            PF_PubSub.currentEntity = new PlayFab.Sockets.Models.EntityKey()
            {
                Type = p_result.EntityToken.Entity.Type,
                Id = p_result.EntityToken.Entity.Id
            };
            PF_PubSub.InitializePubSub();

            if (null != p_callback)
                p_callback.Invoke(m_sessionTicket);
        }

        /// <summary>
        /// Called on a successful registration result
        /// </summary>
        protected virtual void OnRegisterResult(RegisterPlayFabUserResult result, System.Action<string> p_callback)
        {
            if (p_callback != null)
                p_callback.Invoke("New Account Registered");
        }

        /// <summary>
        /// Raises the login error event.
        /// </summary>
        /// <param name="p_error">Error.</param>
        protected virtual void OnLoginError(PlayFabError p_error, System.Action<string> p_callback) //PlayFabError
        {
            string errorMessage;
            if (p_error.Error == PlayFabErrorCode.InvalidParams && p_error.ErrorDetails.ContainsKey("Password"))
                errorMessage = "Invalid Password";
            else if (p_error.Error == PlayFabErrorCode.InvalidParams && p_error.ErrorDetails.ContainsKey("Username") || (p_error.Error == PlayFabErrorCode.InvalidUsername))
                errorMessage = "Invalid Username";
            else if (p_error.Error == PlayFabErrorCode.AccountNotFound)
                errorMessage = "Account Not Found, you must have a linked PlayFab account. Start by registering a new account or using your device id";
            else if (p_error.Error == PlayFabErrorCode.AccountBanned)
                errorMessage = "Account Banned";
            else if (p_error.Error == PlayFabErrorCode.InvalidUsernameOrPassword)
                errorMessage = "Invalid Username or Password";
            else
                errorMessage = string.Format("Error {0}: {1}", p_error.HttpCode, p_error.ErrorMessage);

            if (null != p_callback)
                p_callback.Invoke(errorMessage);
            // if (OnLoginFail != null)
            //     OnLoginFail(errorMessage, MessageDisplayStyle.error);

            // reset these IDs (a hack for properly detecting if a device is claimed or not, we will have an API call for this soon)
            //PlayFabLoginCalls.android_id = string.Empty;
            //PlayFabLoginCalls.ios_id = string.Empty;

        }

    }

}