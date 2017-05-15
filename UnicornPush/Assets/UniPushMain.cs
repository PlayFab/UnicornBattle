#if !DISABLE_PLAYFABCLIENT_API

using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Internal;
using UnityEngine;

public class UniPushMain : MonoBehaviour
{
    const string TITLE_ID = "A5F3";

    string playFabId = "";
    string androidPushSenderId = "";
    [Multiline(50)]
    string pushStatus = "";

    private void Start()
    {
        PlayFabSettings.TitleId = TITLE_ID;
        PlayFabPluginEventHandler.OnGcmSetupStep += OnGcmSetupStep;
        PlayFabPluginEventHandler.Init();
        DoLogin();
    }

    private void OnGcmSetupStep(PlayFabPluginEventHandler.PushSetupStatus status)
    {
        pushStatus += status.ToString() + "\n";
    }

    #region GUI
    const int SIZE_X = 150;
    const int SIZE_Y = 25;
    Rect GridRect(int x, int y, int sizeX, int sizeY)
    {
        return new Rect(x * SIZE_X, y * SIZE_Y, SIZE_X * sizeX, SIZE_Y * sizeY);
    }

    void OnGUI()
    {
        GUI.Label(GridRect(0, 0, 1, 1), "playFabId:"); GUI.TextField(GridRect(1, 0, 1, 1), playFabId);
        GUI.Label(GridRect(0, 1, 1, 1), "androidPushSenderId:"); GUI.TextField(GridRect(1, 1, 1, 1), androidPushSenderId);
        GUI.Label(GridRect(0, 2, 1, 1), "pushStatus:"); GUI.TextArea(GridRect(1, 2, 3, 20), pushStatus);
    }
    #endregion GUI

    #region PlayFab API Calls
    public static void OnSharedFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    void DoLogin()
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
        playFabId = result.PlayFabId;

        GetTitleData();
    }

    void GetTitleData()
    {
        var getRequest = new GetTitleDataRequest
        {
            Keys = new List<string> { "AndroidPushSenderId" }
        };
        PlayFabClientAPI.GetTitleData(getRequest, OnGetTitleData, OnSharedFailure);
    }
    void OnGetTitleData(GetTitleDataResult result)
    {
        androidPushSenderId = result.Data["AndroidPushSenderId"];
        PlayFabPluginEventHandler.Setup(androidPushSenderId);
    }
    #endregion PlayFab API Calls
}
#endif
