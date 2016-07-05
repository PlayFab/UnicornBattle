using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class ProfileUIController : MonoBehaviour {
	public Transform[] UiObjects;
	public Image[] colorize;

	private bool isCharacterInventoryLoaded = false;
	private bool isCharacterDataLoaded = false;
	
	
	void OnEnable()
	{
		PF_Bridge.OnPlayFabCallbackError += HandleCallbackError;
		PF_Bridge.OnPlayfabCallbackSuccess += HandleCallbackSuccess;
	}
	
	void OnDisable()
	{
		PF_Bridge.OnPlayFabCallbackError -= HandleCallbackError;	
		PF_Bridge.OnPlayfabCallbackSuccess -= HandleCallbackSuccess;
	}
	
	public void HandleCallbackError(string details, PlayFabAPIMethods method, MessageDisplayStyle style)
	{	
		
	}
	
	public void HandleCallbackSuccess(string details, PlayFabAPIMethods method, MessageDisplayStyle style)
	{
		switch(method)
		{
			case PlayFabAPIMethods.GetCharacterReadOnlyData:
				this.isCharacterDataLoaded = true;
				break;
			case PlayFabAPIMethods.GetCharacterInventory:
				this.isCharacterInventoryLoaded = true;
				break;
		}
		CheckToContinue();
	}
	
	void CheckToContinue()
	{
		if(this.isCharacterDataLoaded && this.isCharacterInventoryLoaded)
		{
			PF_PlayerData.PlayerClassTypes ponyType = (PF_PlayerData.PlayerClassTypes)Enum.Parse (typeof(PF_PlayerData.PlayerClassTypes), PF_PlayerData.activeCharacter.baseClass.CatalogCode);
			
			switch ((int)ponyType) {
			case 0:
				foreach (var item in this.colorize) {
					item.color = PF_GamePlay.ClassColor1;
				}
				break;
			case 1:
				foreach (var item in this.colorize) {
					item.color = PF_GamePlay.ClassColor2;
				}
				break;
			case 2:
				foreach (var item in this.colorize) {
					item.color = PF_GamePlay.ClassColor3;
				}
				break;
			default:
				Debug.LogWarning ("Unknown Class type detected...");
				break;
			}

			PF_PlayerData.UpdateActiveCharacterData();
			foreach(Transform each in this. UiObjects)
			{
				each.gameObject.SetActive(true); //<---- BOOM Null Ref
				each.BroadcastMessage("Init", SendMessageOptions.DontRequireReceiver);
			}
			ResetDataChecks();
		}
	}
	
	void ResetDataChecks()
	{
		this.isCharacterDataLoaded = false;
		this.isCharacterInventoryLoaded = false;
	}
	

	public void OpenInventory()
	{
		DialogCanvasController.RequestInventoryPrompt (null, DialogCanvasController.InventoryFilters.AllItems);
	}

//	public void SwitchToInventoryScene()
//	{
//		SceneController.Instance.RequestSceneChange(SceneController.GameScenes.Store, .333f);
//	}
//	
	public void SwitchToSocialScene()
	{
		DialogCanvasController.RequestSocialPrompt();
	}


    public void OpenStore()
    {
            //Debug.Log("" + response);
            //DialogCanvasController.RequestStore(PF_GameData.StandardStores[0]);

    }

	public void OpenStorePicker()
	{
        DialogCanvasController.RequestStore(PF_GameData.StandardStores[0]);

        /*
		UnityAction<int> afterSelect = ( int  response) => 
		{
			//Debug.Log("" + response);
			DialogCanvasController.RequestStore(PF_GameData.StandardStores[response]);
		};
		
		if(PF_GameData.StandardStores.Count > 0)
		{
			List<string> options  = PF_GameData.StandardStores;
			DialogCanvasController.RequestSelectorPrompt("Select a Store:", options, afterSelect);
		}
		else
		{
			Debug.Log("No StandardStores found in TitleData");
		}
         */
	}
}
