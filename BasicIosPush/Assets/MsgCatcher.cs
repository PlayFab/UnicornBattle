using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

// Despite the #Defines, this example is for iOS only
// This example is fairly pointless for Android because Android just wont work in Unity without some kind of client-side plugin, like FCM.

public class MsgCatcher : MonoBehaviour
{
    public string pushToken = null;
    public string playFabId = null;
    public string lastMsg = null;
	public string log = "";

    public void Start()
    {
        UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound, true);
        LoginToPlayFab();
    }

    public void Update()
	{
		UpdateToken ();
		UpdatePollNotifications ();
	}

	public void UpdateToken()
    {
		if (!string.IsNullOrEmpty(pushToken))
            return;
        byte[] token = UnityEngine.iOS.NotificationServices.deviceToken;
		if (token == null)
			return;
        pushToken = System.BitConverter.ToString(token).Replace("-", "").ToLower();
        RegisterForPush();
    }

	public void UpdatePollNotifications()
	{
		if (UnityEngine.iOS.NotificationServices.remoteNotifications != null) {
			foreach (var eachNotify in UnityEngine.iOS.NotificationServices.remoteNotifications) {
				lastMsg = eachNotify.alertBody;
			}
		}
	}

    public void OnGUI()
    {
		GUI.Label(new Rect(0, 0, Screen.width, 100), "PushToken: " + (string.IsNullOrEmpty(pushToken) ? "null" : pushToken));
		GUI.Label(new Rect(0, 100, Screen.width, 200), "PlayFabId: " + (string.IsNullOrEmpty(playFabId) ? "null" : playFabId));
		GUI.Label(new Rect(0, 200, Screen.width, 300), "Push Msg: " + (string.IsNullOrEmpty(lastMsg) ? "null" : lastMsg));
		GUI.Label(new Rect(0, 300, Screen.width, Screen.height - 300), string.IsNullOrEmpty(log) ? "null" : log);
    }

    private void OnPfFail(PlayFabError error)
    {
		log += "PlayFab: api error: " + error.GenerateErrorReport () + "\n";
    }


    private void LoginToPlayFab()
    {
#if UNITY_ANDROID
        var request = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true, };
        PlayFabClientAPI.LoginWithAndroidDeviceID(request, OnPfLogin, OnPfFail);
#elif UNITY_IOS
        var request = new LoginWithIOSDeviceIDRequest { DeviceId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true, };
        PlayFabClientAPI.LoginWithIOSDeviceID(request, OnPfLogin, OnPfFail);
#endif
    }

    private void OnPfLogin(LoginResult result)
    {
		log += "PlayFab: login successful" + "\n";
        playFabId = result.PlayFabId;
        RegisterForPush();
    }

    private void RegisterForPush()
    {
        if (string.IsNullOrEmpty(pushToken) || string.IsNullOrEmpty(playFabId))
            return;

#if UNITY_ANDROID
        var request = new AndroidDevicePushNotificationRegistrationRequest { DeviceToken = pushToken };
        PlayFabClientAPI.AndroidDevicePushNotificationRegistration(request, OnPfAndroidReg, OnPfFail);
#elif UNITY_IOS
        var request = new RegisterForIOSPushNotificationRequest { DeviceToken = pushToken };
        PlayFabClientAPI.RegisterForIOSPushNotification(request, OnPfIosReg, OnPfFail);
#endif
    }

    private void OnPfAndroidReg(AndroidDevicePushNotificationRegistrationResult result)
    {
		log += "PlayFab: Push Registration Successful" + "\n";
    }

    private void OnPfIosReg(RegisterForIOSPushNotificationResult result)
    {
		log += "PlayFab: Push Registration Successful" + "\n";
    }
}
