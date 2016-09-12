using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using PlayFab.Internal;


public class GameController : Singleton<GameController> {
	protected GameController () {} // guarantee this will be always a singleton only - can't use the constructor!
	
	public SceneController sceneController;
	public UploadToPlayFabContentService cdnController;
	public IAB_Controller iabController;

	public IconManager iconManager;
	
	public Color ClassColor1;
	public Color ClassColor2;
	public Color ClassColor3;
	
	public List<string> TitleDataKeys = new List<string>();
	
	public Transform sceneControllerPrefab;
	
	//private int pausedOnScene = 0;
	private string pausedOnScene = string.Empty;
	private string lostFocusedOnScene = string.Empty;



	
	void OnLevelWasLoaded(int index) 
	{	
		if(sceneController != null)
		{
			string levelName = SceneManager.GetActiveScene().name;
			//Debug.Log ("GAME CONTROLLER ON LEVEL LOADED: " +  Application.loadedLevelName);

			if(levelName.Contains("Authenticate") && (sceneController.previousScene != SceneController.GameScenes.Null || sceneController.previousScene != SceneController.GameScenes.Splash))
			{
				//Debug.Log("MADE IT THROUGH THE IF STATEMENT");
				DialogCanvasController.RequestInterstitial();
			}
			else if(levelName.Contains("CharacterSelect"))
			{
				DialogCanvasController.RequestInterstitial();
				CharacterSelectDataRefresh();
			}
			else if(levelName.Contains("Profile"))
			{
                CharacterProfileDataRefresh();
			}
			else if(levelName.Contains("Gameplay"))
			{
				DialogCanvasController.RequestInterstitial();
			}
		}
	}
	
	
	public void OnOnApplicationPause(bool status)
	{
		if(status == true && SceneManager.GetActiveScene().buildIndex != 0)
		{
			// application just got paused
			Debug.Log("application just got paused");
			this.pausedOnScene = SceneManager.GetActiveScene().name;
		}
		else
		{
			// application just resumed, go back to the previously used scene.
			Debug.Log("application just resumed, go back to the previously used scene: " + this.pausedOnScene);
			
			if(SceneManager.GetActiveScene().name != this.pausedOnScene)
			{
				SceneController.Instance.RequestSceneChange((SceneController.GameScenes)System.Enum.Parse(typeof(SceneController.GameScenes), this.pausedOnScene));
			}
			
		}
		
	}
	
	void OnApplicationFocus(bool status) {
		if(status == false)
		{
			// application just got paused
			Debug.Log("application just lost focus");
			this.lostFocusedOnScene = SceneManager.GetActiveScene().name;
		}
		else if(status == true)
		{
			// application just resumed, go back to the previously used scene.
			Debug.Log("application just regained focus, go back to the previously used scene: " + this.lostFocusedOnScene);
			
			if(!string.IsNullOrEmpty(this.lostFocusedOnScene) && SceneManager.GetActiveScene().name != this.lostFocusedOnScene)
			{
				SceneController.Instance.RequestSceneChange((SceneController.GameScenes)System.Enum.Parse(typeof(SceneController.GameScenes), this.lostFocusedOnScene));
			}
		}
	}
		
	public void ProcessPush()
	{
		//Will contain the code for processing push notifications.
	}	
		
	public static void CharacterSelectDataRefresh()
	{
		//Debug.Log("Ran CharacterSelectDataRefresh");
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
		PF_PlayerData.GetCharacterInventory(PF_PlayerData.activeCharacter.characterDetails.CharacterId);
	}
									
	// Use this for initialization
	void Start () {
		
		DontDestroyOnLoad( this.transform.parent.gameObject);
		
		PF_GamePlay.ClassColor1 = this.ClassColor1;
		PF_GamePlay.ClassColor2 = this.ClassColor2;
		PF_GamePlay.ClassColor3 = this.ClassColor3;
		
		//base.Awake();
		GameController[] go = GameObject.FindObjectsOfType<GameController>();
		if(go.Length == 1)
		{
			this.gameObject.tag = "GameController";
			//DontDestroyOnLoad(this.gameObject);
			
			Transform sceneCont = Instantiate(this.sceneControllerPrefab);
			sceneCont.SetParent(this.transform, false);
			this.sceneController = sceneCont.GetComponent<SceneController>();
		}		

		//TODO -- add in code to listen for and process scene change events.
		//SceneManager.sceneLoaded += OnLevelWasLoaded;

	}
}
