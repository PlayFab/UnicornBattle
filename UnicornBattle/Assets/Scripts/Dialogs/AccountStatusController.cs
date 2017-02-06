using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using PlayFab;

using PlayFab.ClientModels;

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

	public static string pushToken = string.Empty;
	
	private bool isRegisteredForPush = false;
	private bool isShownOnLogin = true;
	private bool changedLoginState = false;

	void Awake()
	{

	}
	
	void OnDestory()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		PlayFabGoogleCloudMessaging._RegistrationReadyCallback -= AccountStatusController.OnGCMReady;
		PlayFabGoogleCloudMessaging._RegistrationCallback -= AccountStatusController.OnGCMRegistration;
		#endif
	}
	
	
	
	#if UNITY_ANDROID && !UNITY_EDITOR
	public static void OnGCMReady(bool status)
	{
		Debug.Log("GCM Ready!");
		PlayFabGoogleCloudMessaging.GetToken();
	}
	
	public static void OnGCMRegistration(string token, string error)
    {
        Debug.Log(string.Format("GCM Token Received: {0}", token));
        
		if(!string.IsNullOrEmpty(error))
		{
			Debug.Log(string.Format("GCM Error: {0}", error));
		}
		else if (token != null)
		{
			pushToken = token;
		}
	}
	#endif
	
	public void Init()
	{
		if (PF_PlayerData.accountInfo != null) 
		{
			this.changedLoginState = false;
			displayName.text = PF_PlayerData.accountInfo.TitleInfo.DisplayName;
			if( string.IsNullOrEmpty(PF_PlayerData.accountInfo.Username) || string.IsNullOrEmpty(PF_PlayerData.accountInfo.PrivateInfo.Email))
			{
				accountStatus.color = Color.red;
                accountStatus.text = GlobalStrings.ACT_STATUS_UNREG_MSG;
				this.registerAccount.gameObject.SetActive(true);
				this.resetPassword.gameObject.SetActive(false);
			} 
			else
			{
				accountStatus.color = Color.green;
                accountStatus.text = GlobalStrings.ACT_STATUS_REG_MSG;
				this.registerAccount.gameObject.SetActive(false);
				this.resetPassword.gameObject.SetActive(true);
			}
		


			if(PF_PlayerData.showAccountOptionsOnLogin)
			{
				this.isShownOnLogin = true;
				this.showOnLogin.GetComponent<Image>().overrideSprite = this.checkedBox;
			}
			else
			{
				this.isShownOnLogin = false;
				this.showOnLogin.GetComponent<Image>().overrideSprite = this.uncheckedBox;
			}


			if(PF_PlayerData.accountInfo.FacebookInfo != null)
			{
				Text btnText = this.linkToFaceBook.GetComponentInChildren<Text>();
                btnText.text = GlobalStrings.UNLINK_FB_BTN_MSG;
				//btnText.color = Color.green;


				UnityAction<Texture2D> afterGetPhoto = (Texture2D tx) =>
				{
					facebookPicture.overrideSprite = Sprite.Create(tx, new Rect(0,0, 128, 128), new Vector2()); 
				};

				StartCoroutine(FacebookHelperClass.GetPlayerProfilePhoto( FetchWebAsset, afterGetPhoto));

			}
			else
			{
				Text btnText = this.linkToFaceBook.GetComponentInChildren<Text>();
                btnText.text = GlobalStrings.LINK_FB_BTN_MSG;
				//btnText.color = Color.red;
				facebookPicture.overrideSprite = null;
			}
			
			
			#if UNITY_IPHONE
				UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound, true);
			#endif
			
			if(PF_PlayerData.isRegisteredForPush)
			{
				this.isRegisteredForPush = true;
				this.registerPush.GetComponent<Image> ().overrideSprite = this.checkedBox;
			}
			else
			{
				this.isRegisteredForPush = false;
				this.registerPush.GetComponent<Image>().overrideSprite = this.uncheckedBox;	
			}
		}
	}


	
	// used to get the FB picture. Probably a better way to do this.
	public void FetchWebAsset(string url, UnityAction<Texture2D> callback = null)
	{
		StartCoroutine (WebSpinner (url, callback));
	}
	
	// used to get the FB picture. Probably a better way to do this.
	public IEnumerator WebSpinner(string url, UnityAction<Texture2D> callback = null)
	{
		WWW www = new WWW(url);
		yield return www;
		
		if (callback != null) 
		{
			callback (www.texture);
		}
	}


	public void SetDisplayName()
	{
		Action<string> processResponse = (string response) => { 
			if(response != null)
			{
				PF_Authentication.UpdateDisplayName(response, (UpdateUserTitleDisplayNameResult result) => 
				{
					this.displayName.text = result.DisplayName;
					PF_PlayerData.accountInfo.TitleInfo.DisplayName = result.DisplayName;
				});
			}
		};
		
		if(PF_PlayerData.accountInfo.FacebookInfo != null)
		{
			UnityAction<string> afterGetFBName = (string name) =>
			{
                DialogCanvasController.RequestTextInputPrompt(GlobalStrings.DISPLAY_NAME_PROMPT, GlobalStrings.DISPLAY_NAME_MSG, processResponse, name);
			};
			
			FacebookHelperClass.GetFBUserName(afterGetFBName);
			
			
		}
		else
		{
            DialogCanvasController.RequestTextInputPrompt(GlobalStrings.DISPLAY_NAME_PROMPT, GlobalStrings.DISPLAY_NAME_MSG, processResponse);
		}

	}

	public void TogglePushNotification()
	{

		if (this.isRegisteredForPush == false) 
		{	
			DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.Generic);
			UnityAction afterPush = () =>
			{
				
				this.changedLoginState = true;
				this.isRegisteredForPush = true;
				PF_PlayerData.isRegisteredForPush = true;
				this.registerPush.GetComponent<Image> ().overrideSprite = this.checkedBox;
				Debug.Log("PUSH ENABLED!");
				PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.Generic, MessageDisplayStyle.none);
			};
			
			StartCoroutine(PF_GamePlay.Wait(1.5f, ()=> 
			{
				PF_PlayerData.RegisterForPushNotification(pushToken, afterPush);
			}));
		}
		else 
		{
			Action<bool> processResponse = (bool response) => { 
				if (response) 
				{
					this.changedLoginState = true;
					this.isRegisteredForPush = false;
					PF_PlayerData.isRegisteredForPush = false;
					this.registerPush.GetComponent<Image> ().overrideSprite = this.uncheckedBox;
					Debug.Log("PUSH DISABLED!");
				}
			};

            DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.PUSH_NOTIFY_PROMPT, GlobalStrings.PUSH_NOTIFY_MSG, processResponse);
		}
	}




	public void ToggleShowOnLogin()
	{
		Action<bool> processResponse = (bool response) => { 
			if (response) {
				this.isShownOnLogin = !this.isShownOnLogin;	
				this.changedLoginState = true;

				if (this.isShownOnLogin) {
					this.showOnLogin.GetComponent<Image> ().overrideSprite = this.checkedBox;
				} else {
					this.showOnLogin.GetComponent<Image> ().overrideSprite = this.uncheckedBox;
				}
			}
		};

		if (this.isShownOnLogin) 
		{
            DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.TOGGLE_PROMPT, GlobalStrings.TOGGLE_MSG, processResponse);
		} 
		else 
		{
			processResponse.Invoke(true);
		}
	}


	public void ToggleFacebookLink()
	{
		if (PF_PlayerData.accountInfo.FacebookInfo != null) 
		{
			Action<bool> afterCheck = (bool response) =>
			{
				if(response)
				{
					//unlink
					UnityAction afterUnlink = () =>
					{
						Text txt = this.linkToFaceBook.GetComponentInChildren<Text>();
						txt.color = Color.red;
                        txt.text = GlobalStrings.LINK_FB_BTN_MSG;
						this.facebookPicture.overrideSprite = null;
						Debug.Log("NEED ACCOUNT REFRESH???");
						PF_PlayerData.accountInfo.FacebookInfo = null;
						this.changedLoginState = true;
					};

					PF_PlayerData.UnlinkFBAccount(afterUnlink);
				}
			};

            DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.CONFIRM_UNLINK_PROMPT, GlobalStrings.CONFIRM_UNLINK_MSG, afterCheck);
		}
		else
		{
			// link
			UnityAction afterLink = () =>
			{
				Text btnText = this.linkToFaceBook.GetComponentInChildren<Text>();
                btnText.text = GlobalStrings.UNLINK_FB_BTN_MSG;
				btnText.color = Color.green;
				

				Debug.Log("NEED ACCOUNT REFRESH???");
				this.changedLoginState = true;
				PF_PlayerData.accountInfo.FacebookInfo = new UserFacebookInfo();
				
				UnityAction<Texture2D> afterGetPhoto = (Texture2D tx) =>
				{
					facebookPicture.overrideSprite = Sprite.Create(tx, new Rect(0,0, 128, 128), new Vector2()); 
				};
				
				StartCoroutine(FacebookHelperClass.GetPlayerProfilePhoto( FetchWebAsset, afterGetPhoto));
			};

			PF_PlayerData.LinkFBAccount(afterLink);
		}
	}

	public void ShowRegistration()
	{
		UnityAction<AddUsernamePasswordResult> afterRegistration = (AddUsernamePasswordResult result) =>
		{
			PF_PlayerData.accountInfo.Username = result.Username;
			PF_PlayerData.accountInfo.PrivateInfo.Email = "Pending Refresh";

			Dictionary<string, object> eventData = new Dictionary<string, object>()
			{
				//pull emailf from RC due to it not being returned.
				{ "Username", result.Username},
				{ "Email", this.rc.email.text} 
			};
			PF_Bridge.LogCustomEvent(PF_Bridge.CustomEventTypes.Client_RegisteredAccount, eventData);
			
			this.registerAccount.gameObject.SetActive (false);
            this.accountStatus.text = GlobalStrings.ACT_STATUS_REG_MSG;
			this.resetPassword.gameObject.SetActive(true);
			this.accountStatus.color = Color.green;
		};

		this.rc.gameObject.SetActive (true);
		this.rc.Init (afterRegistration);
	}


	public void SendRecoveryEmail()
	{
		Action<bool> afterCheck = (bool response) =>
		{
			if(response)
			{
				var email =  string.IsNullOrEmpty(PF_PlayerData.accountInfo.PrivateInfo.Email) || PF_PlayerData.accountInfo.PrivateInfo.Email.Contains("Pending Refresh") ? this.rc.email.text : PF_PlayerData.accountInfo.PrivateInfo.Email;
				PF_Authentication.SendAccountRecoveryEmail(email);
			}
		};

        DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.RECOVER_EMAIL_PROMPT, GlobalStrings.RECOVER_EMAIL_MSG, afterCheck);
	}

	public void Continue()
	{
		if (changedLoginState) 
		{
			PF_PlayerData.showAccountOptionsOnLogin = this.isShownOnLogin;
			
			Dictionary<string, string> updates = new Dictionary<string, string>();
			updates.Add("ShowAccountOptionsOnLogin", this.isShownOnLogin ? "1" : "0");
			updates.Add("IsRegisteredForPush", this.isRegisteredForPush ? "1" : "0");
			
			PF_PlayerData.UpdateUserData(updates); 	
		}
		this.gameObject.SetActive (false);
	}

}
