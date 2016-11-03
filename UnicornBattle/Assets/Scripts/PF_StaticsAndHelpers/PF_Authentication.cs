using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PlayFab.ClientModels;
using PlayFab;
using Facebook.Unity;

public class PF_Authentication {

	// used for device ID
	public static string android_id = string.Empty; // device ID to use with PlayFab login
	public static string ios_id = string.Empty; // device ID to use with PlayFab login
	public static string custom_id = string.Empty; // custom id for other platforms
	
	//tracked actions
	public static bool isLoggedOut = false;
	//public static bool hasLoggedInOnce = false;
	
	/* Communication is diseminated across these 4 events */
	//called after a successful login 
	public delegate void SuccessfulLoginHandler(string details, MessageDisplayStyle style);
	public static event SuccessfulLoginHandler OnLoginSuccess;
	
	//called after a login error or when logging out
	public delegate void FailedLoginHandler(string details, MessageDisplayStyle style);
	public static event FailedLoginHandler OnLoginFail;
	
	// regex pattern for validating email syntax
	private const string emailPattern = @"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$";
	public static bool usedManualFacebook = false;
	
	/// <summary>
	/// Informs lisenters when a successful login occurs
	/// </summary>
	/// <param name="details">Details - general string for additional details </param>
	/// <param name="style">Style - controls how the message should be handled </param>
	public static void RaiseLoginSuccessEvent(string details, MessageDisplayStyle style)
	{
		if(OnLoginSuccess != null)
		{
			OnLoginSuccess(details, style);
		}
	}
	
	/// <summary>
	/// Informs lisenters when a successful login occurs
	/// </summary>
	/// <param name="details">Details - general string for additional details </param>
	/// <param name="style">Style - controls how the message should be handled </param>
	public static void RaiseLoginFailedEvent(string details, MessageDisplayStyle style)
	{
		if(OnLoginFail != null)
		{
			OnLoginFail(details, style);
		}
	}
	

	#region Plugin Access & Helper Functions
	// Kicks off the Facebook login process
	public static void StartFacebookLogin(Action onInit = null)
	{
		if (FB.IsInitialized == true && onInit == null) {
			OnInitComplete ();
		} else if (FB.IsInitialized == true) {
			onInit ();
		} else if (onInit != null) {
			FB.Init(()=> { onInit();});
		}
		else 
		{
			FB.Init(OnInitComplete, OnHideUnity);
		}
	}


	#endregion
	
	#region PlayFab API calls
	/// <summary>
	/// Login with Facebook token.
	/// </summary>
	/// <param name="token">Token obtained through the FB plugin. (works on mobile and FB canvas only)</param>
	public static void LoginWithFacebook(string token, bool createAccount = false, UnityAction errCallback = null)
	{
		//LoginMethodUsed = LoginPathways.facebook;
		LoginWithFacebookRequest request = new LoginWithFacebookRequest();
		request.AccessToken = token;
		request.TitleId = PlayFabSettings.TitleId;
		request.CreateAccount = createAccount;

		DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);
		PlayFabClientAPI.LoginWithFacebook(request, OnLoginResult, (PlayFabError error) =>
		                                   {
			if(errCallback != null && error.Error == PlayFabErrorCode.AccountNotFound)
			{
				errCallback();
				PF_Bridge.RaiseCallbackError("Account not found, please select a login method to continue.", PlayFabAPIMethods.GenericLogin, MessageDisplayStyle.error);
			}
			else
			{
				OnLoginError(error);
			}
			
		});
	}
	
	
	/// <summary>
	/// Logins the with device identifier (iOS & Android only).
	/// </summary>
	public static void LoginWithDeviceId( bool createAcount, UnityAction errCallback )
	{
		Action<bool> processResponse = (bool response) => { 
			if(response == true && GetDeviceId())
			{
				if(!string.IsNullOrEmpty(android_id))
				{
					Debug.Log("Using Android Device ID: " + android_id);
					LoginWithAndroidDeviceIDRequest request = new LoginWithAndroidDeviceIDRequest();
					request.AndroidDeviceId = android_id;
					request.TitleId = PlayFabSettings.TitleId;
					request.CreateAccount = createAcount;
					
					DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);
					PlayFabClientAPI.LoginWithAndroidDeviceID(request, OnLoginResult, (PlayFabError error) =>
					{
						if(errCallback != null && error.Error == PlayFabErrorCode.AccountNotFound)
						{
							errCallback();
							PF_Bridge.RaiseCallbackError("Account not found, please select a login method to continue.", PlayFabAPIMethods.GenericLogin, MessageDisplayStyle.none);
						}
						else
						{
							OnLoginError(error);
						}
						
					});
				}
				else if (!string.IsNullOrEmpty(ios_id))
				{
					Debug.Log("Using IOS Device ID: " + ios_id);
					LoginWithIOSDeviceIDRequest request = new LoginWithIOSDeviceIDRequest();
					request.DeviceId = ios_id;
					request.TitleId = PlayFabSettings.TitleId;
					request.CreateAccount = createAcount;

					DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);
					PlayFabClientAPI.LoginWithIOSDeviceID(request, OnLoginResult, (PlayFabError error) =>
					{
						if(errCallback != null && error.Error == PlayFabErrorCode.AccountNotFound)
						{
							errCallback();
							PF_Bridge.RaiseCallbackError("Account not found, please select a login method to continue.", PlayFabAPIMethods.GenericLogin, MessageDisplayStyle.none);
						}
						else
						{
							OnLoginError(error);
						}
					});
				}
			}
			else
			{
				Debug.Log("Using custom device ID: " + custom_id);
				LoginWithCustomIDRequest request = new LoginWithCustomIDRequest();
				request.CustomId = custom_id;
				request.TitleId = PlayFabSettings.TitleId;
				request.CreateAccount = createAcount;
				
				DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);
				PlayFabClientAPI.LoginWithCustomID(request, OnLoginResult, (PlayFabError error) =>
				                                      {
					if(errCallback != null && error.Error == PlayFabErrorCode.AccountNotFound)
					{
						errCallback();
						PF_Bridge.RaiseCallbackError("Account not found, please select a login method to continue.", PlayFabAPIMethods.GenericLogin, MessageDisplayStyle.none);
					}
					else
					{
						OnLoginError(error);
					}
				});
			}
		}; 
		
		processResponse(true);
		//DialogCanvasController.RequestConfirmationPrompt("Login With Device ID", "Logging in with device ID has some issue. Are you sure you want to contine?", processResponse);
	
			
	}
	
	/// <summary>
	/// Registers the new PlayFab account.
	/// </summary>
	public static void RegisterNewPlayfabAccount(string user, string pass1, string pass2, string email )
	{
		if(user.Length == 0 || pass1.Length == 0 || pass2.Length ==0 || email.Length == 0)
		{
			if(OnLoginFail != null)
			{
				OnLoginFail("All fields are required.", MessageDisplayStyle.error);
			}
			return;
		}
		
		bool passwordCheck = ValidatePassword(pass1, pass2);
		bool emailCheck = ValidateEmail(email);
		
		if(!passwordCheck)
		{
			if(OnLoginFail != null)
			{
				OnLoginFail("Passwords must match and be longer than 5 characters.", MessageDisplayStyle.error);
			}
			return;
			
		}
		else if(!emailCheck)
		{
			if(OnLoginFail != null)
			{
				OnLoginFail("Invalid Email format.", MessageDisplayStyle.error);
			}
			return;
		}
		else
		{
			RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest();
			request.TitleId = PlayFabSettings.TitleId;
			request.Username = user;
			request.Email = email;
			request.Password = pass1;
			
//			if(!string.IsNullOrEmpty(PlayFabLoginCalls.publisher_id))
//			{
//				request.PublisherId = PlayFabLoginCalls.publisher_id;
//			}
			DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);
			PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterResult, OnLoginError);
		}
	}
	
	/// <summary>
	/// Login with PlayFab username.
	/// </summary>
	/// <param name="user">Username to use</param>
	/// <param name="pass">Password to use</param>
	public static void LoginWithUsername(string user, string password)
	{			
		if(user.Length>0 && password.Length>0)
		{
			//LoginMethodUsed = LoginPathways.pf_username;
			LoginWithPlayFabRequest request = new LoginWithPlayFabRequest();
			request.Username = user;
			request.Password = password;
			request.TitleId = PlayFabSettings.TitleId;
			
//			if(!string.IsNullOrEmpty(PlayFabLoginCalls.publisher_id))
//			{
//				request.PublisherId = PlayFabLoginCalls.publisher_id;
//			}
			DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);
			PlayFabClientAPI.LoginWithPlayFab(request, OnLoginResult, OnLoginError);
		}
		else
		{
			if(OnLoginFail != null)
			{
				OnLoginFail("User Name and Password cannot be blank.", MessageDisplayStyle.error);
			}
		}
	}
	
	/// <summary>
	/// Login using the email associated with a PlayFab account.
	/// </summary>
	/// <param name="user">User.</param>
	/// <param name="password">Password.</param>
	public static void LoginWithEmail(string user, string password)
	{		
		if(user.Length>0 && password.Length>0 && ValidateEmail(user))
		{
			//LoginMethodUsed = LoginPathways.pf_email;
			LoginWithEmailAddressRequest request = new LoginWithEmailAddressRequest();
			request.Email = user;
			request.Password = password;
			request.TitleId = PlayFabSettings.TitleId;
			
//			if(!string.IsNullOrEmpty(PlayFabLoginCalls.publisher_id))
//			{
//				request.PublisherId = PlayFabLoginCalls.publisher_id;
//			}
			DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericLogin);
			PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginResult, OnLoginError);
			
		}
		else
		{
			if(OnLoginFail != null)
			{
				OnLoginFail("Username or Password is invalid. Check credentails and try again", MessageDisplayStyle.error);
			}
		}
		
	}
	
	
	
	/// <summary>
	/// Gets the device identifier and updates the static variables
	/// </summary>
	/// <returns><c>true</c>, if device identifier was obtained, <c>false</c> otherwise.</returns>
	public static bool GetDeviceId(bool silent = false) // silent suppresses the error
	{
		if(CheckForSupportedMobilePlatform())
		{
			#if UNITY_ANDROID
			//http://answers.unity3d.com/questions/430630/how-can-i-get-android-id-.html
			AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
			AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
			android_id = clsSecure.CallStatic<string>("getString", objResolver, "android_id");
			#endif
			
			#if UNITY_IPHONE
			ios_id = UnityEngine.iOS.Device.vendorIdentifier;
			#endif
			return true;
		}
		else
		{
			custom_id = SystemInfo.deviceUniqueIdentifier;
			//LoginWithCustomIDRequest
//			if(OnLoginFail != null && silent == false)
//			{
//				OnLoginFail("Must be using android or ios platforms to use deveice id.", MessageDisplayStyle.error);
//			}
			return false;
		}
	}
	
	/// <summary>
	/// Check to see if our current platform is supported (iOS & Android)
	/// </summary>
	/// <returns><c>true</c>, for supported mobile platform, <c>false</c> otherwise.</returns>
	public static bool CheckForSupportedMobilePlatform()
	{
		if(Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
		{
			return false;
		}
		return true;
	}
	
	/// <summary>
	/// Logs the user out.
	/// </summary>
	public static void Logout()
	{
		if(OnLoginFail != null)
		{
			OnLoginFail("Logout", MessageDisplayStyle.none);
		}
		android_id = string.Empty;
		ios_id = string.Empty;
		custom_id = string.Empty;
		
		if(FB.IsInitialized == true && FB.IsLoggedIn == true)
		{
			CallFBLogout();
		}

		//TODO maybe not OK to delete all, but if it works out this is easy
		// hack, manually deleteing keys to work across android devices.
		PlayerPrefs.DeleteKey("LinkedFacebook");
		PlayerPrefs.DeleteKey("LastDeviceIdUsed");
		PlayerPrefs.DeleteKey("TitleId");

		
	
		PF_Authentication.isLoggedOut = true;

		//TODO make sure the delay here is long enough to shut down the active game systems
		SceneController.Instance.RequestSceneChange(SceneController.GameScenes.Authenticate, .333f);
	}
	
	/// <summary>
	/// Called on a successful login attempt
	/// </summary>
	/// <param name="result">Result object returned from PlayFab server</param>
	private static void OnLoginResult(PlayFab.ClientModels.LoginResult result) //LoginResult
	{	
		PF_PlayerData.PlayerId = result.PlayFabId;
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		string senderID = "295754311231";
		PlayFabGoogleCloudMessaging._RegistrationReadyCallback += AccountStatusController.OnGCMReady;
		PlayFabGoogleCloudMessaging._RegistrationCallback += AccountStatusController.OnGCMRegistration;
		PlayFabAndroidPlugin.Init(senderID);
		#endif
		
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.isEditor)
		{
			if(FB.IsInitialized == false)
			{
				//FB.Init(OnInitComplete, OnHideUnity);
				FB.Init();
			}
			
			if(PF_Authentication.usedManualFacebook == true)
			{
				LinkDeviceId();
				PF_Authentication.usedManualFacebook = false;
			}
		}
		
		Debug.Log("Session Ticket: " + result.SessionTicket);
		
		PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GenericLogin, MessageDisplayStyle.none);
		if(OnLoginSuccess != null)
		{
			OnLoginSuccess(string.Format("SUCCESS: {0}", result.SessionTicket ), MessageDisplayStyle.error);
		}
	}
	
	/// <summary>
	/// Raises the login error event.
	/// </summary>
	/// <param name="error">Error.</param>
	private static void OnLoginError(PlayFabError error) //PlayFabError
	{
		string errorMessage = string.Empty;
		if (error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Password"))
		{
			errorMessage = "Invalid Password";
		}
		else if (error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Username") || (error.Error == PlayFabErrorCode.InvalidUsername))
		{
			errorMessage = "Invalid Username";
		}
		else if (error.Error == PlayFabErrorCode.AccountNotFound)
		{
			errorMessage = "Account Not Found, you must have a linked PlayFab account. Start by registering a new account or using your device id";
		}
		else if (error.Error == PlayFabErrorCode.AccountBanned)
		{
			errorMessage = "Account Banned";
		}
		else if (error.Error == PlayFabErrorCode.InvalidUsernameOrPassword)
		{
			errorMessage = "Invalid Username or Password";
		}
		else
		{
			errorMessage = string.Format("Error {0}: {1}", error.HttpCode, error.ErrorMessage);
		}
		
		if(OnLoginFail != null)
		{
			OnLoginFail(errorMessage, MessageDisplayStyle.error);				
		}
		
		// reset these IDs (a hack for properly detecting if a device is claimed or not, we will have an API call for this soon)
		//PlayFabLoginCalls.android_id = string.Empty;
		//PlayFabLoginCalls.ios_id = string.Empty;
		
		//clear the token if we had a fb login fail
		if(FB.IsLoggedIn)
		{
			CallFBLogout();
		}
	}



	/// <summary>
	/// Calls the UpdateUserTitleDisplayName request API
	/// </summary>
	public static void UpdateDisplayName(string displayName, UnityAction<UpdateUserTitleDisplayNameResult> callback = null)
	{
		if(displayName.Length > 2 && displayName.Length < 21)
		{ 
			DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.UpdateDisplayName);
			UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest();
			request.DisplayName = displayName;
			
			PlayFabClientAPI.UpdateUserTitleDisplayName(request, (UpdateUserTitleDisplayNameResult result) => 
			{
				PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.UpdateDisplayName, MessageDisplayStyle.none);
				if(callback != null)
				{
					callback (result);
				}
			},  PF_Bridge.PlayFabErrorCallback);
		}
	}
	
	/// <summary>
	/// Adds the user name and password to a partial (guest) account
	/// </summary>
	/// <param name="user">User - username to use (must be unique)</param>
	/// <param name="pass">Pass - password to use for the account, (must be > 5 characters)</param>
	/// <param name="email">Email - email to use (must be unique)</param>
	public static void AddUserNameAndPassword(string user, string pass, string email, UnityAction<AddUsernamePasswordResult> callback = null)
	{
		AddUsernamePasswordRequest request = new AddUsernamePasswordRequest();
		request.Email = email;
		request.Password = pass;
		request.Username = user;

		DialogCanvasController.RequestLoadingPrompt (PlayFabAPIMethods.AddUsernamePassword);
		PlayFabClientAPI.AddUsernamePassword(request, (AddUsernamePasswordResult result) => 
		{
			if(callback != null)
			{
				callback (result);
			}
			PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.AddUsernamePassword, MessageDisplayStyle.none);

		}, PF_Bridge.PlayFabErrorCallback);
	}
	
	/// <summary>
	/// Triggers the backend to send an account recovery email to the account provided
	/// </summary>
	/// <param name="email">Email to match</param>
	public static void SendAccountRecoveryEmail(string email)
	{
		SendAccountRecoveryEmailRequest request = new SendAccountRecoveryEmailRequest();
		request.Email = email;
		request.TitleId = PlayFabSettings.TitleId;
		
		DialogCanvasController.RequestLoadingPrompt (PlayFabAPIMethods.SendAccountRecoveryEmail);
		PlayFabClientAPI.SendAccountRecoveryEmail(request, (SendAccountRecoveryEmailResult result) =>
		{
			PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.SendAccountRecoveryEmail, MessageDisplayStyle.none);
		},  PF_Bridge.PlayFabErrorCallback);
	}
	
	/// <summary>
	/// Links a mobile device to a PlayFab account via the unique device id (A device can only be linked to one account at a time)
	/// </summary>
	public static void LinkDeviceId()
	{
		if( GetDeviceId())
		{
			UnityAction<PlayFabError> onLinkError = (PlayFabError err) => 
			{
				PF_Bridge.RaiseCallbackError("Unable to link device: " + err.ErrorMessage, PlayFabAPIMethods.LinkDeviceID, MessageDisplayStyle.error);
			};
			
			if(!string.IsNullOrEmpty(android_id))
			{
				Debug.Log("Linking Android");
				LinkAndroidDeviceIDRequest request = new LinkAndroidDeviceIDRequest();
				request.AndroidDeviceId = android_id;
				
				PlayFabClientAPI.LinkAndroidDeviceID(request, null, (PlayFabError error) => 
				{
					onLinkError(error);
				});
			}
			else if (!string.IsNullOrEmpty(ios_id))
			{
				Debug.Log("Linking iOS");
				LinkIOSDeviceIDRequest request = new LinkIOSDeviceIDRequest();
				request.DeviceId = ios_id;
				
				PlayFabClientAPI.LinkIOSDeviceID(request, null, (PlayFabError error) => 
				{
					onLinkError(error);
				});
			}
		}
	}
	
	/// <summary>
	/// Unlinks a mobile device from a PlayFab account
	/// </summary>
	public static void UnlinkDeviceId()
	{
		if( GetDeviceId())
		{
//			if(!string.IsNullOrEmpty(android_id))
//			{
//				Debug.Log("Unlinking Android");
//				UnlinkAndroidDeviceIDRequest request = new UnlinkAndroidDeviceIDRequest();
//				PlayFabClientAPI.UnlinkAndroidDeviceID(request, OnUnlinkAndroidDeviceIdSuccess, OnPlayFabCallbackError);
//			}
//			else if (!string.IsNullOrEmpty(ios_id))
//			{
//				Debug.Log("Unlinking iOS");
//				UnlinkIOSDeviceIDRequest request = new UnlinkIOSDeviceIDRequest();
//				PlayFabClientAPI.UnlinkIOSDeviceID(request, OnUnlinkIosDeviceIdSuccess, OnPlayFabCallbackError);
//			}
		}
	}
	#endregion	
	
	#region pf_callbacks
	
	/// <summary>
	/// Called on a successful registration result
	/// </summary>
	/// <param name="result">Result object returned from the PlayFab server</param>
	private static void OnRegisterResult(RegisterPlayFabUserResult result){
		if(OnLoginSuccess != null)
		{
			OnLoginSuccess("New Account Registered", MessageDisplayStyle.none);
		}
	}
	
	#endregion
	
	
	#region helperfunctions
		/// <summary>
	/// Validates the email.
	/// </summary>
	/// <returns><c>true</c>, if email was validated, <c>false</c> otherwise.</returns>
	/// <param name="em">Email address</param>
	public static bool ValidateEmail(string em){
		return Regex.IsMatch(em, PF_Authentication.emailPattern);
	}
	
	/// <summary>
	/// Validates the password.
	/// </summary>
	/// <returns><c>true</c>, if password was validated, <c>false</c> otherwise.</returns>
	/// <param name="p1">P1, text from password field one</param>
	/// <param name="p2">P2, text from password field two</param>
	public static bool ValidatePassword(string p1, string p2){
		return ((p1 == p2) && p1.Length > 5); 
	}
		
	#endregion
	
	
	#region fb_helperfunctions
	
	// callback after FB.Init();
	public static void OnInitComplete()
	{
		Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
		if(FB.IsLoggedIn == false)
		{
			CallFBLogin();
		}
		else
		{
			LoginWithFacebook(AccessToken.CurrentAccessToken.TokenString);
		}
	}
	
	// Handler for OnHideUnity Events
	public static void OnHideUnity(bool isGameShown)
	{
		Debug.Log("Is game showing? " + isGameShown);
	}
	
	/// <summary>
	/// Calls FB login.
	/// </summary>
	public static void CallFBLogin()
	{
		FB.LogInWithReadPermissions(new List<string>(){"public_profile", "email", "user_friends"}, LoginCallback);
	}

	
	// callback called after a successful FB login.
	public static void LoginCallback(Facebook.Unity.ILoginResult result)
	{
		if (result.Error != null)
		{
			if(OnLoginFail != null)
			{
				OnLoginFail("Facebook Error: " + result.Error, MessageDisplayStyle.none);
			}
		}
		else if (!FB.IsLoggedIn)
		{
			if(OnLoginFail != null)
			{
				OnLoginFail("Facebook Error: Login cancelled by Player", MessageDisplayStyle.none);
			}
		}
		else
		{
			LoginWithFacebook(AccessToken.CurrentAccessToken.TokenString);
		}
	}
	
	public static void CallFBLogout()
	{
		FB.LogOut();
	}
	#endregion
}
