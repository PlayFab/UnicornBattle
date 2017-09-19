using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

// Despite the #Defines, this example is for iOS only
// This example is fairly pointless for Android because Android just wont work in Unity without some kind of client-side plugin, like FCM.

public class MsgCatcher : MonoBehaviour
{
    public string pushToken;
    public string playFabId;
    public string lastMsg;

    public void Start()
    {
        UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound, true);
        LoginToPlayFab();
    }

    public void Update()
    {
        if (pushToken != null)
            return;
        byte[] token = UnityEngine.iOS.NotificationServices.deviceToken;
        if (token == null)
            return;
        pushToken = System.BitConverter.ToString(token).Replace("-", "").ToLower();
        RegisterForPush();
    }

    public void OnGUI()
    {
        GUI.Label(new Rect(0, 0, Screen.width, 200), pushToken);
        GUI.Label(new Rect(0, 200, Screen.width, Screen.height - 200), lastMsg);
    }

    private void OnPfFail(PlayFabError error)
    {
        Debug.Log("PlayFab: api error: " + error.GenerateErrorReport());
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
        Debug.Log("PlayFab: login successful");
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
        Debug.Log("PlayFab: Push Registration Successful");
    }

    private void OnPfIosReg(RegisterForIOSPushNotificationResult result)
    {
        Debug.Log("PlayFab: Push Registration Successful");
    }
}
