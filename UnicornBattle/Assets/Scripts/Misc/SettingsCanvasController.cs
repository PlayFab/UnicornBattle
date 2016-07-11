using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

public class SettingsCanvasController : Singleton<SettingsCanvasController> {
	protected SettingsCanvasController () {} // guarantee this will be always a singleton only - can't use the constructor!
	
	public Button openSettingsButton;
	public Button closeSettingsButton;
	
	public Transform menuOverlayPanel;
	public Transform menuButtonsGroup;
	public bool showOpenCloseButton = true;
	
	public string UBVersion = "v1.10.10";
	public Text displayVersion;
	public Text activeTitleId;
	public string communityPortal = @"https:\\playfab.com";

	public enum SettingButtonTypes {none = 0, returnToCharacterSelect, leaveBattle, logout, accountSettings, setTitleId, communityPortal, redeemCoupon}

	public List<SettingsButtonDetails> settingsButtons = new List<SettingsButtonDetails>();
	public List<SceneToSettingsMapping> settingsByScene = new List<SceneToSettingsMapping>();
	
	//private bool isSetTitleIdOpen = false;
	
	void OnLevelWasLoaded(int index) 
	{
		UpdateSettingsMenuButtons();
	}
	
	public override void Awake()
	{
		base.Awake();
		UpdateSettingsMenuButtons();
	}
	
	// Use this for initialization
	void Start () 
	{
		//CloseSettingsMenu();
		this.displayVersion.text = this.UBVersion;
		SettingsButtonDisplay(showOpenCloseButton);
	}

	public void OpenCommunityPortal()
	{
		if (string.IsNullOrEmpty (PF_GameData.CommunityWebsite)) 
		{
			PF_Bridge.RaiseCallbackError("No URL was found for the Community Portal. Check TitleData.", PlayFabAPIMethods.Generic, MessageDisplayStyle.error);
			return;
		}

		Application.OpenURL(PF_GameData.CommunityWebsite);
	}


	void UpdateSettingsMenuButtons()
	{
		string levelName = SceneManager.GetActiveScene().name;
		SceneToSettingsMapping activeSettings = this.settingsByScene.Find((zz) => { return zz.sceneName.Contains(levelName); });
		if(activeSettings != null)
		{
			foreach(var button in settingsButtons)
			{
				SettingButtonTypes sceneObject = activeSettings.buttons.Find((zz) => { return button.buttonType == zz; });
				//Debug.Log(sceneObject.ToString());
				if(sceneObject != SettingButtonTypes.none)
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
	
	
	
	public void OpenSettingsMenu()
	{
		// THIS BREAKS IF THE GO IS DISABLED!!!
		
		//Tween.Tween(this.menuOverlayPanel.gameObject, .001f, Quaternion.Euler(0,0,0) , Quaternion.Euler(0,0,15f), TweenMain.Style.PingPong, TweenMain.Method.EaseIn, null);

		
		//new Vector3(Screen.width/2, Screen.height/2, 0)
		menuOverlayPanel.gameObject.SetActive(true);
		TweenPos.Tween(this.menuOverlayPanel.gameObject, .001f, this.menuOverlayPanel.transform.position, new Vector3(0, Screen.height, 0), TweenMain.Style.Once, TweenMain.Method.Linear, null, Space.World);
		TweenScale.Tween(this.menuOverlayPanel.gameObject, .001f, new Vector3(1,1,1), new Vector3(0,0,0), TweenMain.Style.Once, TweenMain.Method.EaseIn, null);
		
		TweenPos.Tween(this.menuOverlayPanel.gameObject, .5f, new Vector3(0, 0, 0), new Vector3(Screen.width/2, Screen.height/2, 0), TweenMain.Style.Once, TweenMain.Method.EaseIn, null, Space.World);
		TweenScale.Tween(this.menuOverlayPanel.gameObject, .5f, new Vector3(0,0,0), new Vector3(1,1,1), TweenMain.Style.Once, TweenMain.Method.EaseIn, null);

		this.activeTitleId.text = PlayFab.PlayFabSettings.TitleId;

		ToggleOpenCloseButtons();
	}
	
	// NEED TO MAKE THIS A COROUTINE
	public void CloseSettingsMenu()
	{
		TweenPos.Tween(this.menuOverlayPanel.gameObject, .5f, new Vector3(Screen.width/2, Screen.height/2, 0), new Vector3(0, 0, 0), TweenMain.Style.Once, TweenMain.Method.EaseIn, null, Space.World);
		TweenScale.Tween(this.menuOverlayPanel.gameObject, .5f, new Vector3(1,1,1), new Vector3(0,0,0), TweenMain.Style.Once, TweenMain.Method.EaseIn, null);
		
		StartCoroutine(PF_GamePlay.Wait(.75f, () => 
		{
			menuOverlayPanel.gameObject.SetActive(false);
			ToggleOpenCloseButtons();
		}));
	}
	
	public void ToggleOpenCloseButtons()
	{
		if(this.showOpenCloseButton == true)
		{
			if(this.openSettingsButton.gameObject.activeSelf == true)
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
		if(this.showOpenCloseButton == true && mode)
		{
			this.openSettingsButton.gameObject.SetActive(true);
			this.closeSettingsButton.gameObject.SetActive(false);
		}
		else if(this.showOpenCloseButton == true)
		{
			this.openSettingsButton.gameObject.SetActive(true);
			this.closeSettingsButton.gameObject.SetActive(false);
		}
	}
	
	public void ReturnToCharacterSelect()
	{
		PF_PlayerData.activeCharacter = null;
		SceneController.Instance.ReturnToCharacterSelect();
		CloseSettingsMenu();
	}
	
	public void LeaveBattle()
	{
		Action<bool> processResponse = (bool response) => { 
			if(response == true)
			{
				Dictionary<string, object> eventData = new Dictionary<string, object>()
				{
					{ "Current_Quest", PF_GamePlay.ActiveQuest.levelName },
					{ "Character_ID", PF_PlayerData.activeCharacter.characterDetails.CharacterId }
				};
				PF_Bridge.LogCustomEvent(PF_Bridge.CustomEventTypes.Client_BattleAborted, eventData);
				
				CloseSettingsMenu();
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
			if(!string.IsNullOrEmpty(response))
			{
				PF_GamePlay.RedeemCoupon(response);
			}
		};
			
		DialogCanvasController.RequestTextInputPrompt("Redeem a Coupon Code", "Enter a valid code to redeem rewards.", (string response) => { afterPrompt(response); } , "XXX-XXXX-XXX" );
	}

	public void SetTitleId()
	{
		UnityAction<string> afterPrompt = (string response) =>
		{
			if(!string.IsNullOrEmpty(response))
			{
				this.activeTitleId.text = response;
				PlayFab.PlayFabSettings.TitleId = response;
				PlayerPrefs.SetString("TitleId", response);
			}
		};
			
		DialogCanvasController.RequestTextInputPrompt("Set Title Id", "This will update which PlayFab title this client connects to.", (string response) => { afterPrompt(response); } , PlayFab.PlayFabSettings.TitleId );
	}

	public void Logout()
	{
		Action<bool> processResponse = (bool response) => { 
			if(response == true)
			{
				PF_PlayerData.activeCharacter = null;
				CloseSettingsMenu();
				PF_Authentication.Logout();
			}
		}; 
        DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.LOGOUT_PROMPT, GlobalStrings.LOGOUT_MSG, processResponse);
	}	
}

[System.Serializable]
public class SceneToSettingsMapping
{
	public string sceneName;
	public SceneController.GameScenes scene;
	public bool showOpenCloseButtons = true;
	public List<SettingsCanvasController.SettingButtonTypes> buttons = new List<SettingsCanvasController.SettingButtonTypes>();
}

[System.Serializable]
public class SettingsButtonDetails
{
	public string buttonName;
	public SettingsCanvasController.SettingButtonTypes buttonType;
	public Transform prefab;
}
