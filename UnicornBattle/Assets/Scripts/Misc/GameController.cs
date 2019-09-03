using System.Collections.Generic;
using UnicornBattle.Managers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnicornBattle.Controllers
{
    public class GameController : Singleton<GameController>
        {
            protected GameController() { } // guarantee this will be always a singleton only - can't use the constructor!

            public SceneController sceneController;
            public UploadToPlayFabContentService cdnController;
            public IAB_Controller iabController;

            public IconManager iconManager;

            public Color ClassColor1;
            public Color ClassColor2;
            public Color ClassColor3;

            public Transform sceneControllerPrefab;

            //private int pausedOnScene = 0;
            private string _pausedOnScene = string.Empty;
            private string _lostFocusedOnScene = string.Empty;

            ///////////////////////////////////////////////////////////////////////
            #region GAMEPLAY DATA
            public LevelItemHelper ActiveLevel { get; set; }
            public List<UBEncounter> ActiveEncounterList { get; set; }
            public UBQuest QuestProgress { get; set; }

            public void ClearQuestProgress()
            {
                ActiveLevel = null;
                ActiveEncounterList = new List<UBEncounter>();
                QuestProgress = new UBQuest();
            }

            #endregion
            ///////////////////////////////////////////////////////////////////////
            #region ACTIVE CHARACTER
            private UBSavedCharacter m_activeCharacter;

            public UBSavedCharacter ActiveCharacter
            {
                get
                {
                    return m_activeCharacter;
                }
            }

            public void SetActiveCharacter(string p_id)
            {
                var l_playerMgr = MainManager.Instance.getPlayerManager();
                if (null == l_playerMgr) return;

                var l_characterMgr = MainManager.Instance.getCharacterManager();
                if (null == l_characterMgr) return;

                m_activeCharacter = l_characterMgr.GetUBSavedCharacter(p_id);
                PlayerPrefs.SetString($"MRU_{l_playerMgr.PlayerID}", m_activeCharacter.CharacterId);
                PlayerPrefs.Save();
                m_activeCharacter.SetMaxVitals();
            }

            public void ClearActiveCharacter()
            {
                m_activeCharacter = null;

                var l_characterMgr = MainManager.Instance.getCharacterManager();
                l_characterMgr.ActiveCharacterWasCleared();
            }

            public void UpdateActiveCharacterData()
            {
                if (null == m_activeCharacter) return;

                string id = m_activeCharacter.CharacterId;

                CharacterManager l_characterMgr = MainManager.Instance.getCharacterManager();
                if (null == l_characterMgr) return;

                l_characterMgr.Refresh(true,
                    (s) =>
                    {
                        if (null != m_activeCharacter)
                        {
                            m_activeCharacter.UpdateCharacterData(l_characterMgr.GetUBCharacterData(id));
                            m_activeCharacter.RefillVitals();
                        }
                    },
                    null);

            }

            #endregion
            ///////////////////////////////////////////////////////////////////////
            void OnLevelLoad(Scene scene, LoadSceneMode mode)
            {
                if (sceneController == null)
                    return; // This seems like a critical error ...???

                if (scene.name.Contains("Authenticate")
                    && (sceneController.previousScene != SceneController.GameScenes.Null
                        || sceneController.previousScene != SceneController.GameScenes.Splash))
                {
                    DialogCanvasController.RequestInterstitial();

                    UnicornBattle.Managers.Auth.AuthenticationManager authManager = UnicornBattle.Managers.MainManager.Instance.getAuthManager();
                    Debug.Log("Device ID :" + authManager.GetDeviceId());
                }
                else if (scene.name.Contains("CharacterSelect"))
                {
                    DialogCanvasController.RequestInterstitial();
                    CharacterSelectDataRefresh();
                }
                else if (scene.name.Contains("Profile"))
                {
                    CharacterProfileDataRefresh();
                }
                else if (scene.name.Contains("Gameplay"))
                {
                    DialogCanvasController.RequestInterstitial();
                }
            }

            void OnEnable()
            {
                SceneManager.sceneLoaded += OnLevelLoad;
            }

            void OnDisable()
            {
                SceneManager.sceneLoaded -= OnLevelLoad;
            }

            public void OnOnApplicationPause(bool status)
            {
                if (status && SceneManager.GetActiveScene().buildIndex != 0)
                {
                    // application just got paused
                    Debug.Log("application just got paused");
                    _pausedOnScene = SceneManager.GetActiveScene().name;
                    //Supersonic.Agent.onPause();
                }
                else
                {
                    // application just resumed, go back to the previously used scene.
                    Debug.Log("application just resumed, go back to the previously used scene: " + _pausedOnScene);

                    if (SceneManager.GetActiveScene().name != _pausedOnScene)
                        SceneController.Instance.RequestSceneChange((SceneController.GameScenes) System.Enum.Parse(typeof(SceneController.GameScenes), _pausedOnScene));
                    //Supersonic.Agent.onResume();
                }
            }

            void OnApplicationFocus(bool status)
            {
                if (!status)
                {
                    // application just got paused
                    Debug.Log("application just lost focus");
                    _lostFocusedOnScene = SceneManager.GetActiveScene().name;
                    // Supersonic.Agent.onPause();
                }
                else if (status)
                {
                    // application just resumed, go back to the previously used scene.
                    Debug.Log("application just regained focus, go back to the previously used scene: " + _lostFocusedOnScene);

                    if (!string.IsNullOrEmpty(_lostFocusedOnScene) && SceneManager.GetActiveScene().name != _lostFocusedOnScene)
                        SceneController.Instance.RequestSceneChange((SceneController.GameScenes) System.Enum.Parse(typeof(SceneController.GameScenes), _lostFocusedOnScene));
                    // Supersonic.Agent.onResume();
                }
            }

            public void ProcessPush()
            {
                //Will contain the code for processing push notifications.
            }

            public static void CharacterSelectDataRefresh()
            {
                //DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetEvents);
                DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetTitleData_General);
                //DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetAccountInfo);
                //DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetOffersCatalog);

                MainManager.Instance.RefreshTitleData(
                    () =>
                    {
                        MainManager.Instance.RefreshAll(false,
                            (s) =>
                            {
                                PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetEvents);
                                PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetTitleNews);
                                PF_Bridge.RaiseCallbackSuccess("Player Account Info Loaded", PlayFabAPIMethods.GetAccountInfo);
                                PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetOffersCatalog);
                                PF_Bridge.RaiseCallbackSuccess("Player Characters Retrieved", PlayFabAPIMethods.GetAllUsersCharacters);
                                PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetUserStatistics);
                                PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetTitleData_General);
                                PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetCharacterReadOnlyData);
                            },
                            (e) =>
                            {
                                PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetTitleNews);
                                PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetEvents);
                                PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetAccountInfo);
                                PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.Generic);
                                PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetOffersCatalog);
                                PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetAllUsersCharacters);
                                PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetUserStatistics);
                                PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetTitleData_General);
                            }
                        );
                    },
                    (e) =>
                    {
                        PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetTitleData_General);
                    }
                );

                //         PF_GameData.GetEventData(
                //             (s) => { PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetEvents); },
                //             (e) => { PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetEvents); }
                //         );

                // //        PF_GameData.GetActiveEventData(); // now called directly from GetEventData to ensure ordering

                //         PF_GameData.GetTitleData(
                //             (s) => { PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetTitleData_General); },
                //             (e) => { PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetTitleData_General); }
                //         );

                // PF_GameData.GetTitleNews( 15,
                //     (s) => { PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetTitleNews); },
                //     (e) => { PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetTitleNews); }
                // );

                // var l_inventoryMgr = MainManager.Instance.getInventoryManager();
                // if(null == l_inventoryMgr) return;

                // l_inventoryMgr.GetCatalogInfo(
                //     (s) => { 

                //         // PF_PlayerData.RefreshUserAccountInfo(
                //         //     () => { PF_Bridge.RaiseCallbackSuccess("Player Account Info Loaded", PlayFabAPIMethods.GetAccountInfo); },
                //         //     (string error) => { PF_Bridge.RaiseCallbackError(error, PlayFabAPIMethods.GetAccountInfo); }
                //         // );
                //         //PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.Generic);
                //     },
                //     (e) => { PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.Generic); }
                // );

                // PF_GameData.GetOffersCatalog(           
                //     (s) => { PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetOffersCatalog); },
                //     (e) => { PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetOffersCatalog); }
                // );

                // var l_characterMgr = MainManager.Instance.getCharacterManager();
                // if(null == l_characterMgr) return;

                // l_characterMgr.Refresh( false,
                //     (string success) => {
                //         PF_Bridge.RaiseCallbackSuccess("Player Characters Retrieved", PlayFabAPIMethods.GetAllUsersCharacters);
                //     },
                //     (string errorMessage) => {
                //         PF_Bridge.RaiseCallbackError(errorMessage, PlayFabAPIMethods.GetAllUsersCharacters);
                //     }
                // );

                // PF_PlayerData.RefreshUserStats(
                //     () => {  
                //         PF_Bridge.RaiseCallbackSuccess("", PlayFabAPIMethods.GetUserStatistics);
                //     },
                //     (string errorMessage) => {
                //         PF_Bridge.RaiseCallbackError(errorMessage, PlayFabAPIMethods.GetUserStatistics);
                //     }
                // );
            }

            public void CharacterProfileDataRefresh()
            {
                DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetTitleData_General);

                MainManager.Instance.RefreshTitleData(
                    () =>
                    {
                        MainManager.Instance.RefreshAll(false,
                            (s) =>
                            {
                                PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetEvents);
                                PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetTitleData_General);
                                PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetTitleNews);
                                PF_Bridge.RaiseCallbackSuccess("Player Account Info Loaded", PlayFabAPIMethods.GetAccountInfo);
                                PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetOffersCatalog);
                                PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetCharacterReadOnlyData);
                            },
                            (e) =>
                            {
                                PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetTitleData_General);
                            }
                        );
                    },
                    (e) =>
                    {
                        PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetTitleData_General);
                    }
                );

                // DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetEvents);
                // PF_GameData.GetEventData(
                //     (s) => { PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetEvents); },
                //     (e) => { PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetEvents); }
                // );

                // DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetTitleData_General);
                // PF_GameData.GetTitleData(
                //     (s) => { PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetTitleData_General); },
                //     (e) => { PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetTitleData_General); }
                // );

                // PF_GameData.GetTitleNews( 15,
                //     (s) => { PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetTitleNews); },
                //     (e) => { PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetTitleNews); }
                // );

                // var l_inventoryMgr = MainManager.Instance.getInventoryManager();
                // if(null == l_inventoryMgr) return;

                // l_inventoryMgr.GetCatalogInfo(
                //     (s) => { 
                //         DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetAccountInfo);
                //         PF_PlayerData.RefreshUserAccountInfo(
                //             () => { PF_Bridge.RaiseCallbackSuccess("Player Account Info Loaded", PlayFabAPIMethods.GetAccountInfo); },
                //             (string error) => { PF_Bridge.RaiseCallbackError(error, PlayFabAPIMethods.GetAccountInfo); }
                //         );
                //         //PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.Generic);
                //     },
                //     (e) => { PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.Generic); }
                // );

                // DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetOffersCatalog);
                // PF_GameData.GetOffersCatalog(           
                //     (s) => { PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetOffersCatalog); },
                //     (e) => { PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetOffersCatalog); }
                // );

                // var l_characterMgr = MainManager.Instance.getCharacterManager();
                // if(null == l_characterMgr) return;

                // DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetCharacterReadOnlyData);

                // l_characterMgr.RefreshCharacterDataById(
                //     l_characterMgr.ActiveCharacter.CharacterId,
                //     (s) => { PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetCharacterReadOnlyData); },
                //     (e) => { PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetCharacterReadOnlyData); }
                // );
            }

            // Use this for initialization
            void Start()
            {
                DontDestroyOnLoad(transform.parent.gameObject);

                UBGamePlay.ClassColor1 = ClassColor1;
                UBGamePlay.ClassColor2 = ClassColor2;
                UBGamePlay.ClassColor3 = ClassColor3;

                //base.Awake();
                GameController[] go = GameObject.FindObjectsOfType<GameController>();
                if (go.Length == 1)
                {
                    gameObject.tag = "GameController";

                    var sceneCont = Instantiate(sceneControllerPrefab);
                    sceneCont.SetParent(transform, false);
                    sceneController = sceneCont.GetComponent<SceneController>();
                }
            }
        }
}