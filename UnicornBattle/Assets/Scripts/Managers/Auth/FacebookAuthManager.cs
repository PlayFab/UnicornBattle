using System.Collections;
using System.Collections.Generic;
using PlayFab;
using UnicornBattle.Controllers;
using UnityEngine;
#if UNITY_ANDROID || UNITY_IOS
using Facebook.Unity;
#endif

namespace UnicornBattle.Managers.Auth
{
#if UNITY_ANDROID || UNITY_IOS

    /// <summary>
    /// FACEBOOK AUTHENTICATION MANAGER
    /// ===============================
    /// Wraps up Facebook authentication calls
    /// Inherits from PlayFabAuthManager -- you only need one AuthManager depending on if you are using Facebook or not
    /// </summary>
    public class FacebookAuthManager : PlayFabAuthManager
    {
        private readonly List<string> FacebookPermissionKeys = new List<string> { "public_profile", "email", "user_friends" };

        public FacebookAuthManager() : base()
        {
            FB.Init();
        }

        /// <summary>
        /// Login with Facebook token.
        /// </summary>
        /// <param name="token">Token obtained through the FB plugin. (works on mobile and FB canvas only)</param>
        public void LoginWithFacebook(System.Action<string> p_onSuccessCallback, System.Action<string> p_onFailureCallback)
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
                FB.LogInWithReadPermissions(FacebookPermissionKeys, (ILoginResult r) =>
                {
                    var fbAction = PlayFabClientAPI.IsClientLoggedIn() ? PlayFabAPIMethods.LinkFacebookId : PlayFabAPIMethods.LoginWithFacebook;

                    if (r.Error != null)
                    {
                        if (null != p_onFailureCallback)
                            p_onFailureCallback.Invoke("Facebook Error: " + r.Error);
                        //PF_Bridge.RaiseCallbackError("Facebook Error: " + r.Error, fbAction, MessageDisplayStyle.none);
                        return;
                    }
                    else if (!FB.IsLoggedIn)
                    {
                        if (null != p_onFailureCallback)
                            p_onFailureCallback.Invoke("Facebook Error: Login cancelled by Player");
                        //PF_Bridge.RaiseCallbackError("Facebook Error: Login cancelled by Player", fbAction, MessageDisplayStyle.none);
                        return;
                    }

                    // PlayFab Facebook Login
                    if (fbAction == PlayFabAPIMethods.LinkFacebookId)
                    {
                        LinkFaceBookToPlayFab(false, p_onSuccessCallback, p_onFailureCallback);
                    }
                    else
                    {
                        var request = new PlayFab.ClientModels.LoginWithFacebookRequest
                        {
                            AccessToken = AccessToken.CurrentAccessToken.TokenString,
                            TitleId = PlayFabSettings.TitleId,
                            CreateAccount = true
                        };

                        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);
                        PlayFabClientAPI.LoginWithFacebook(request,
                            (result) =>
                            {
                                OnLoginResult(result, p_onSuccessCallback);
                            },
                            (error) =>
                            {
                                if (error.Error == PlayFabErrorCode.AccountNotFound)
                                {
                                    p_onFailureCallback.Invoke("Account not found, please select a login method to continue.");
                                }
                                else
                                {
                                    OnLoginError(error, p_onFailureCallback);
                                }
                            }
                        );
                    }
                });
            }
        }

        public void LinkFaceBookToPlayFab(bool p_forceLink, System.Action<string> p_onSuccessCallback, System.Action<string> p_onFailureCallback)
        {
            var request = new PlayFab.ClientModels.LinkFacebookAccountRequest
            {
                AccessToken = AccessToken.CurrentAccessToken.TokenString,
                ForceLink = p_forceLink
            };

            PlayFabClientAPI.LinkFacebookAccount(
                request,
                (result) =>
                {
                    PlayerPrefs.SetInt("LinkedFacebook", 1);
                    PlayerPrefs.Save();
                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke(string.Empty);
                },
                error =>
                {
                    if (null == p_onFailureCallback)
                        p_onFailureCallback.Invoke(error.ErrorMessage);
                }
            );
        }

        public void UnlinkFacebook(System.Action<string> p_onSuccessCallback, System.Action<string> p_onFailureCallback)
        {
            var request = new PlayFab.ClientModels.UnlinkFacebookAccountRequest();

            PlayFabClientAPI.UnlinkFacebookAccount(request,
                result =>
                {
                    //Debug.Log("Unlinked Account.");
                    PlayerPrefs.SetInt("LinkedFacebook", 0);
                    PlayerPrefs.Save();
                    FB.LogOut();
                    if (null == p_onSuccessCallback)
                        p_onSuccessCallback.Invoke(string.Empty);
                },
                (error) =>
                {
                    if (null == p_onFailureCallback)
                        p_onFailureCallback.Invoke(error.ErrorMessage);
                }
            );

        }

        /// <summary>
        /// Called on a successful login attempt
        /// </summary>
        /// <param name="p_result">Result object returned from PlayFab server</param>
        /// <param name="p_callback">Callback after processing result</param>
        protected override void OnLoginResult(PlayFab.ClientModels.LoginResult p_result, System.Action<string> p_callback)
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

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.isEditor)
            {
                if (!FB.IsInitialized)
                    FB.Init();

                // usedManualFacebook is always false?  so removing this code
                // if (usedManualFacebook)
                // {
                //     LinkDeviceId();
                //     usedManualFacebook = false;
                // }
            }

            if (null != p_callback)
                p_callback.Invoke(m_sessionTicket);
        }

        /// <summary>
        /// Raises the login error event.
        /// </summary>
        /// <param name="p_error">Error.</param>
        protected override void OnLoginError(PlayFabError p_error, System.Action<string> p_callback) //PlayFabError
        {
            //clear the token if we had a fb login fail
            if (FB.IsInitialized || FB.IsLoggedIn)
                FB.LogOut();

            base.OnLoginError(p_error, p_callback);
        }
    }
#endif
}