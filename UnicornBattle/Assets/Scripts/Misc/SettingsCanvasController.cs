using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Unity.Crashes;
using UnicornBattle.Managers;
using UnicornBattle.Managers.Auth;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
    public class SettingsCanvasController : Singleton<SettingsCanvasController>
        {
            protected SettingsCanvasController() { } // guarantee this will be always a singleton only - can't use the constructor!

            public RectTransform settingsButtonContainer;
            public Button openSettingsButton;
            public Button closeSettingsButton;

            public GameObject menuOverlayPanel;
            public bool showOpenCloseButton = true;

            //public string UBVersion = "v1.10.10";
            public Text displayVersion;
            public Text activeTitleId;

            public enum SettingButtonTypes { none = 0, returnToCharacterSelect, leaveBattle, logout, accountSettings, setTitleId, communityPortal, redeemCoupon, triggerCrash, returnToMainMenu }

            public List<SettingsButtonDetails> settingsButtons = new List<SettingsButtonDetails>();
            public List<SceneToSettingsMapping> settingsByScene = new List<SceneToSettingsMapping>();

            public AuthenticationManager Authentication
            {
                get { return MainManager.Instance.getAuthManager(); }
            }

            void OnLevelLoad(Scene scene, LoadSceneMode mode)
            {
                UpdateSettingsMenuButtons();
            }

            public override void Awake()
            {
                base.Awake();
                UpdateSettingsMenuButtons();
            }

            // Use this for initialization
            void Start()
            {
                //CloseSettingsMenu();
                this.activeTitleId.text = PlayFab.PlayFabSettings.TitleId;
                this.displayVersion.text = GlobalStrings.UB_VERSION;
                SettingsButtonDisplay(showOpenCloseButton);
            }

            void OnEnable()
            {
                SceneManager.sceneLoaded += OnLevelLoad;
            }

            void OnDisable()
            {
                SceneManager.sceneLoaded -= OnLevelLoad;
            }

            public void OpenCommunityPortal()
            {
                var l_gameDataMgr = MainManager.Instance.getGameDataManager();
                if (null == l_gameDataMgr) return;

                if (string.IsNullOrEmpty(l_gameDataMgr.CommunityWebsite))
                {
                    PF_Bridge.RaiseCallbackError("No URL was found for the Community Portal. Check TitleData.", PlayFabAPIMethods.Generic);
                    return;
                }

                Application.OpenURL(l_gameDataMgr.CommunityWebsite);
            }

            void UpdateSettingsMenuButtons()
            {
                var levelName = SceneManager.GetActiveScene().name;
                var activeSettings = this.settingsByScene.Find((zz) => { return zz.sceneName.Contains(levelName); });
                if (activeSettings != null)
                {
                    SettingsButtonDisplay(activeSettings.showOpenCloseButtons);
                    foreach (var button in settingsButtons)
                    {
                        var sceneObject = activeSettings.buttons.Find((zz) => { return button.buttonType == zz; });
                        //Debug.Log(sceneObject.ToString());
                        if (sceneObject != SettingButtonTypes.none)
                        {
                            button.prefab.gameObject.SetActive(true);
                        }
                        else
                        {
                            button.prefab.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Something went wrong, check the scene names mappings");
                }
            }

            private bool isSettingsMenuAnimating;
            private const float SETTINGS_MENU_DURATION = 0.5f;

            public void StartSettingsMenuAnimation(bool mode)
            {
                if (isSettingsMenuAnimating) return;

                isSettingsMenuAnimating = true;
                openSettingsButton.interactable = false;
                closeSettingsButton.interactable = false;

                var target = menuOverlayPanel;
                var duration = SETTINGS_MENU_DURATION;
                var topRight = new Vector3(Screen.width, Screen.height, 0);
                var center = topRight / 2.0f;
                var style = TweenMain.Style.Once;
                var method = TweenMain.Method.EaseIn;

                void Done()
                {
                    isSettingsMenuAnimating = false;
                    openSettingsButton.interactable = true;
                    closeSettingsButton.interactable = true;
                    ToggleOpenCloseButtons();
                    target.SetActive(mode);
                }

                if (mode)
                {
                    target.SetActive(true);
                    TweenPos.Tween(target, duration, topRight, center, style, method);
                    TweenScale.Tween(target, duration, Vector3.zero, Vector3.one, style, method, Done);
                    TelemetryManager.RecordScreenViewed(TelemetryScreenId.Settings);
                }
                else
                {
                    TweenPos.Tween(target, duration, center, topRight, style, method);
                    TweenScale.Tween(target, duration, Vector3.one, Vector3.zero, style, method, Done);
                }
            }

            public void ToggleOpenCloseButtons()
            {
                if (this.showOpenCloseButton)
                {
                    if (this.openSettingsButton.gameObject.activeSelf)
                    {
                        this.openSettingsButton.gameObject.SetActive(false);
                        this.closeSettingsButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        this.openSettingsButton.gameObject.SetActive(true);
                        this.closeSettingsButton.gameObject.SetActive(false);
                    }
                }
            }

            void SettingsButtonDisplay(bool mode)
            {
                showOpenCloseButton = mode;
                settingsButtonContainer.gameObject.SetActive(mode);
                this.openSettingsButton.gameObject.SetActive(mode);
            }

            public void ReturnToCharacterSelect()
            {
                GameController.Instance.ClearActiveCharacter();
                SceneController.Instance.ReturnToCharacterSelect();
                StartSettingsMenuAnimation(false);
            }

            public void ReturnToMainMenu()
            {
                SceneController.Instance.ReturnToMainMenu();
                StartSettingsMenuAnimation(false);
            }

            public void LeaveBattle()
            {
                Action<bool> processResponse = (bool response) =>
                {
                    if (response)
                    {
                    Dictionary<string, object> eventData = new Dictionary<string, object>()
                    { { "Current_Quest", GameController.Instance.ActiveLevel.levelName }, { "Character_ID", GameController.Instance.ActiveCharacter.CharacterId }
                        };
                        TelemetryManager.RecordPlayerEvent(TelemetryEvent.Client_BattleAborted, eventData);

                        StartSettingsMenuAnimation(false);
                        SceneController.Instance.RequestSceneChange(SceneController.GameScenes.Profile, .333f);
                    }
                };

                DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.QUIT_LEVEL_PROMPT, GlobalStrings.QUIT_LEVEL_MSG, processResponse);
            }

            public void ShowAccountSettings()
            {
                DialogCanvasController.RequestAccountSettings();
            }

            public void RedeemCoupon()
            {
                UnityAction<string> afterPrompt = (string response) =>
                {
                    if (!string.IsNullOrEmpty(response))
                    {

                        var l_storeMgr = MainManager.Instance.getStoreManager();
                        if (null != l_storeMgr)
                        {
                            l_storeMgr.RedeemCoupon(
                                response,
                                (s) => { PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.RedeemCoupon); },
                                (f) => { PF_Bridge.RaiseCallbackError(f, PlayFabAPIMethods.RedeemCoupon); }
                            );
                        }
                    }
                };

                DialogCanvasController.RequestTextInputPrompt("Redeem a Coupon Code", "Enter a valid code to redeem rewards.",
                    (string response) => { afterPrompt(response); }, "XXX-XXXX-XXX");
            }

            public void ForceCrash()
            {
                Crashes.GenerateTestCrash();
                // Application.ForceCrash(2);
                // Crashes.GenerateTestCrash();
            }

            public void SetTitleId()
            {
                string oldTitleID = PlayFab.PlayFabSettings.TitleId;

                DialogCanvasController.RequestTextInputPrompt(
                    "Set Title Id",
                    "Change the Title ID that this client connects to a different PlayFab Title.",
                    (string newTitleID) =>
                    {
                        // if not empty & not the same as the oldID
                        if (!string.IsNullOrEmpty(newTitleID) && oldTitleID != newTitleID)
                        {
                            PlayFabAuthManager pfAuth = (PlayFabAuthManager) Authentication;
                            if (null == pfAuth)
                            {
                                PF_Bridge.RaiseCallbackError("PlayFab Auth Manager has encountered an error.", PlayFabAPIMethods.Generic);
                                return;
                            }
                            PlayFab.PlayFabSettings.TitleId = newTitleID;

                            pfAuth.VerifyNewTitleId(
                                newTitleID,
                                (string result) =>
                                {
                                    // successful, show the new ID & save it to playerPrefs.
                                    this.activeTitleId.text = PlayFab.PlayFabSettings.TitleId;
                                    PlayerPrefs.SetString("TitleId", newTitleID);
                                    PlayerPrefs.Save();
                                    Logout();
                                },
                                (string error) =>
                                {
                                    // failed, reset back to the old ID.
                                    PlayFab.PlayFabSettings.TitleId = oldTitleID;

                                    PF_Bridge.RaiseCallbackError(
                                        "The Title ID entered is either (1) not valid -- " + error,
                                        PlayFabAPIMethods.Generic
                                    );
                                }
                            );
                        }
                    },
                    oldTitleID
                );
            }

            public void Logout()
            {
                Action<bool> processResponse = (bool response) =>
                {
                    if (response)
                    {
                        GameController.Instance.ClearActiveCharacter();

                        StartSettingsMenuAnimation(false);
                        //CloseSettingsMenu();
                        Authentication.Logout(
                            (string s) =>
                            {
                                SceneController.Instance.RequestSceneChange(SceneController.GameScenes.Authenticate, .333f);
                            },
                            (string e) =>
                            {
                                // OnLoginFail event
                                DialogCanvasController.Instance.HandleOnLoginFail(e, MessageDisplayStyle.none);
                                // moved from static class
                                //SceneController.Instance.RequestSceneChange(SceneController.GameScenes.Authenticate, .333f);
                            }
                        );
                    }
                };
                DialogCanvasController.RequestConfirmationPrompt(
                    GlobalStrings.LOGOUT_PROMPT,
                    GlobalStrings.LOGOUT_MSG,
                    processResponse);
            }
        }

    [Serializable]
    public class SceneToSettingsMapping
    {
        public string sceneName;
        public SceneController.GameScenes scene;
        public bool showOpenCloseButtons = true;
        public List<SettingsCanvasController.SettingButtonTypes> buttons = new List<SettingsCanvasController.SettingButtonTypes>();
    }

    [Serializable]
    public class SettingsButtonDetails
    {
        public string buttonName;
        public SettingsCanvasController.SettingButtonTypes buttonType;
        public Transform prefab;
    }
}