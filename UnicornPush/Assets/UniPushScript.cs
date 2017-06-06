using PlayFab;
using PlayFab.Android;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UniPushScript : MonoBehaviour
{
    // YOU MUST REPLACE THESE VALUES WITH YOUR TITLE AND YOUR SENDER
    // This is an existing PlayFab title, which is already set up, but you should not use it for more than a single test notification
    private const string TITLE_ID = "A5F3"; // TODO: Use your own titleId
    private const string ANDROID_PUSH_SENDER_ID = "494923569376"; // TODO: Use your own FCM SenderId
    private string _lastMessageReceived = "waiting for message...";

    private void Start()
    {
        PlayFabSettings.TitleId = TITLE_ID;
        PlayFabAndroidPushPlugin.Setup(ANDROID_PUSH_SENDER_ID);
        PlayFabAndroidPushPlugin.OnGcmMessage += OnPushReceived;

        StartLoginSequence();
    }

    void OnGUI()
    {
        var style = new GUIStyle();
        style.fontSize = Screen.width / 30;
        style.normal.textColor = Color.white;
        style.wordWrap = true;
        if (GUI.Button(new Rect(0, 0, Screen.width, 250), "Switch To Test Scene", style))
            SceneManager.LoadScene(1); // Switch to the test scene
        GUI.Label(new Rect(0, 250, Screen.width, Screen.height-250), _lastMessageReceived, style);
    }

    void OnPushReceived(PlayFabNotificationPackage package)
    {
        _lastMessageReceived = JsonWrapper.SerializeObject(package);
    }

    private static void OnSharedFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    void StartLoginSequence()
    {
        var loginRequest = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
        };
        PlayFabClientAPI.LoginWithCustomID(loginRequest, OnLoginSuccess, OnSharedFailure);
    }
    void OnLoginSuccess(LoginResult result)
    {
        PlayFabAndroidPushPlugin.TriggerManualRegistration();
    }
}
