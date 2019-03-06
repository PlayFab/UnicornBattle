using UnityEngine;
using UnityEngine.SceneManagement;

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

    void OnLevelLoad(Scene scene, LoadSceneMode mode)
    {
        if (sceneController == null)
            return; // This seems like a critical error ...???

        if (scene.name.Contains("Authenticate") && (sceneController.previousScene != SceneController.GameScenes.Null || sceneController.previousScene != SceneController.GameScenes.Splash))
        {
            DialogCanvasController.RequestInterstitial();
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
                SceneController.Instance.RequestSceneChange((SceneController.GameScenes)System.Enum.Parse(typeof(SceneController.GameScenes), _pausedOnScene));
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
                SceneController.Instance.RequestSceneChange((SceneController.GameScenes)System.Enum.Parse(typeof(SceneController.GameScenes), _lostFocusedOnScene));
            // Supersonic.Agent.onResume();
        }
    }

    public void ProcessPush()
    {
        //Will contain the code for processing push notifications.
    }

    public static void CharacterSelectDataRefresh()
    {
        //Debug.Log("Ran CharacterSelectDataRefresh");
        PF_GameData.GetEventData();
//        PF_GameData.GetActiveEventData(); // now called directly from GetEventData to ensure ordering
        PF_GameData.GetTitleData();
        PF_GameData.GetTitleNews();
        PF_GameData.GetCatalogInfo();
        PF_GameData.GetOffersCatalog();
        PF_PlayerData.GetPlayerCharacters();
        PF_PlayerData.GetUserStatistics();
    }

    public void CharacterProfileDataRefresh()
    {
        PF_PlayerData.GetCharacterDataById(PF_PlayerData.activeCharacter.characterDetails.CharacterId);
    }

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(transform.parent.gameObject);

        PF_GamePlay.ClassColor1 = ClassColor1;
        PF_GamePlay.ClassColor2 = ClassColor2;
        PF_GamePlay.ClassColor3 = ClassColor3;

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
