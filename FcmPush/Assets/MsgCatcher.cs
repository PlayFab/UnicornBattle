using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;

public class MsgCatcher : MonoBehaviour
{
    public string pushToken;
    public string playFabId;
    public string lastMsg;

    public void OnGUI()
    {
        GUI.Label(new Rect(0, 0, Screen.width, 200), pushToken);
        GUI.Label(new Rect(0, 200, Screen.width, Screen.height - 200), lastMsg);
    }

    private void OnPfFail(PlayFabError error)
    {
        Debug.Log("PlayFab: api error: " + error.GenerateErrorReport());
    }

    public void Start()
    {
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        LoginToPlayFab();
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

    private void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("PlayFab: Received Registration Token: " + token.Token);
        pushToken = token.Token;
        RegisterForPush();
    }

    private void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log("PlayFab: Received a new message from: " + e.Message.From);
        lastMsg = "";
        if (e.Message.Data != null)
        {
            lastMsg += "DATA: " + JsonWrapper.SerializeObject(e.Message.Data) + "\n";
            Debug.Log("PlayFab: Received a message with data:");
            foreach (var pair in e.Message.Data)
                Debug.Log("PlayFab data element: " + pair.Key + "," + pair.Value);
        }
        if (e.Message.Notification != null)
        {
            Debug.Log("PlayFab: Received a notification:");
            lastMsg += "TITLE: " + e.Message.Notification.Title + "\n";
            lastMsg += "BODY: " + e.Message.Notification.Body + "\n";
        }
    }
}
