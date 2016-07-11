using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab.Json;
using PlayFab;

public class CharacterPicker : MonoBehaviour {
	public int maxCharacterSlots = 0;
	public Transform defaultSlotUI;
	
	public ArrowPickerController pPicker;
	public CharacterAchievementsController pickedPonyDetails;
	
	public Transform slots;

	public Sprite defaultSprite;
	public Sprite selectedSprite;
	
	public Button confirmButton;
	
	public CharacterSlot selectedSlot;
	public List<CharacterSlot> slotObjects = new List<CharacterSlot>();
	
	private bool isPlayerCharatersLoaded = false;
	private bool isTitleDataLoaded = false;
	private bool isPlayerInventoryLoaded = false;
	private bool isCharacterDataLoaded = false;
    private bool isUserStatsLoaded = false;

	#region Monobehaviour Methods
	void OnEnable()
	{
		PF_Bridge.OnPlayfabCallbackSuccess += HandleCallbackSuccess;
        PF_PlayerData.ClearActiveCharacter();
		this.maxCharacterSlots = 0;
	}
    
    void OnDisable()
	{
		PF_Bridge.OnPlayfabCallbackSuccess -= HandleCallbackSuccess;
	}
	
	public void HandleCallbackSuccess(string details, PlayFabAPIMethods method, MessageDisplayStyle style)
	{
		switch(method)
		{
			case PlayFabAPIMethods.GetTitleData:
            case PlayFabAPIMethods.GetAccountInfo:
				this.isTitleDataLoaded |= method == PlayFabAPIMethods.GetTitleData;
				this.isPlayerInventoryLoaded |= method == PlayFabAPIMethods.GetAccountInfo;

                int extraCount = 0;
                InventoryCategory temp;
                if (PF_PlayerData.inventoryByCategory != null && PF_PlayerData.inventoryByCategory.TryGetValue("ExtraCharacterSlot", out temp))
                    extraCount = temp.count;
                maxCharacterSlots = PF_GameData.StartingCharacterSlots + extraCount;
				break;
				
			case PlayFabAPIMethods.GetAllUsersCharacters:
				this.isPlayerCharatersLoaded = true;
				PF_PlayerData.GetCharacterData();
				PF_PlayerData.GetCharacterStatistics();
				break;
				
			case PlayFabAPIMethods.DeleteCharacter:
				ResetDataChecks();
				GameController.CharacterSelectDataRefresh();
				break;
				
			case PlayFabAPIMethods.GrantCharacterToUser:
				ResetDataChecks();
				GameController.CharacterSelectDataRefresh();
				break;
				
			case PlayFabAPIMethods.GetCharacterReadOnlyData:
				isCharacterDataLoaded = true;
				break;

            case PlayFabAPIMethods.GetUserStatistics:
                isUserStatsLoaded = true;
                break;
		}
		CheckToInit();
		
	}
	#endregion
	
	void CheckToInit()
	{
		if (this.isPlayerCharatersLoaded && this.isTitleDataLoaded && this.isPlayerInventoryLoaded && this.isCharacterDataLoaded && this.isUserStatsLoaded)
		{
			if(this.maxCharacterSlots > 0)
			{
				Debug.Log("CharSelect Check Passed!");
				Init();
			}
		}
	}
	
	void Init()
	{
		ResetDataChecks();
		HideButtons();

		CharacterSlot[] children = (CharacterSlot[])slots.GetComponentsInChildren<CharacterSlot>();

		if(this.maxCharacterSlots > children.Length)
		{
			for(int z = 0; z < maxCharacterSlots - children.Length; z++)
			{
				Transform slot = Instantiate(this.defaultSlotUI);
				slot.SetParent(slots, false);
				
				CharacterSlot s = (CharacterSlot)slot.GetComponent<CharacterSlot>();
				
				s.Init();
				this.slotObjects.Add(s);
			}
		}
		
		foreach(var slot in this.slotObjects)
		{
			slot.ClearSlot();
		}
		
		
		int slotCount = 0;
		foreach(var character in PF_PlayerData.playerCharacters)
		{
			foreach(var slot in this.slotObjects)
			{
				if(slot.saved == null)
				{
					//Fill slot 
					if(PF_GameData.Classes.ContainsKey(character.CharacterType))
					{
						slot.FillSlot(new UB_SavedCharacter()
							{ 
								baseClass = PF_GameData.Classes[character.CharacterType],
								characterDetails = character,
								characterData = PF_PlayerData.playerCharacterData.ContainsKey(character.CharacterId) ? PF_PlayerData.playerCharacterData[character.CharacterId] : null
							});
						slotCount++;
						break;
					}
				}
			}
		}

		// -1 for using index
		if (slotCount < this.maxCharacterSlots) 
			for(int z = slotCount; z < this.maxCharacterSlots; z++)
				this.slotObjects[z].ClearSlot();

		for(int z = this.maxCharacterSlots; z < this.slotObjects.Count; z++)
			this.slotObjects[z].LockSlot();

		if(this.selectedSlot != null && this.selectedSlot.saved != null )
			ShowPickedPonyDetails();
		else
		 	HidePickedPonyDetails();

		SelectSlot (this.slotObjects [0]);

		this.confirmButton.onClick.RemoveAllListeners();
        this.confirmButton.onClick.AddListener(Continue);

	}
	
	void ResetDataChecks()
	{
		this.isPlayerCharatersLoaded = false;
		this.isTitleDataLoaded = false;
		this.isPlayerInventoryLoaded = false;
		this.isCharacterDataLoaded = false;
		this.isUserStatsLoaded = false;
	}
	
	#region UI Control	
	public void SelectSlot(CharacterSlot go)
	{
		if (this.selectedSlot != null && go.gameObject != this.selectedSlot.gameObject) 
		{
			this.pPicker.DeselectArrows ();
			this.pickedPonyDetails.gameObject.SetActive (false);
		} 
		else if( this.selectedSlot == null )
		{
			this.pPicker.DeselectArrows ();
		}	
		
		foreach (Transform child in slots)
		{
			if(child.gameObject == go.gameObject)
			{
				child.GetComponent<CharacterSlot>().SelectSlot();
				this.selectedSlot = go;
				
				// need to adjust the arrrows here to be in sync with the saved chars
				if(this.selectedSlot.saved == null)
				{
					int rng = UnityEngine.Random.Range(0, 3);
					ArrowUI slot;
					
					if(rng == 0)
					{
						slot = this.pPicker.Arrow1;
					}
					else if (rng == 1)
					{
						slot = this.pPicker.Arrow2;
					}
					else
					{
						slot = this.pPicker.Arrow3;
					}
					
					this.pPicker.selectedSlot = null;
					this.pPicker.SelectSlot(slot);
					HidePickedPonyDetails();
				}
				else
				{
					PF_PlayerData.PlayerClassTypes ponyType = (PF_PlayerData.PlayerClassTypes)Enum.Parse (typeof(PF_PlayerData.PlayerClassTypes), this.selectedSlot.saved.baseClass.CatalogCode);
					
					UnityEvent afterSelect = new UnityEvent();
					afterSelect.AddListener(() => 
					                        {
						//ShowButtons();
						ShowPickedPonyDetails();
					});
					
					switch ((int)ponyType) 
					{
						case 0:
							pPicker.SelectArrow(0, afterSelect);
							pPicker.DisablePulsingButtons();
							break;
						case 1:
							pPicker.SelectArrow(1, afterSelect);
							pPicker.DisablePulsingButtons();
							break;
						case 2:
							pPicker.SelectArrow(2, afterSelect);
							pPicker.DisablePulsingButtons();
							break;
						default:
							Debug.LogWarning ("Unknown Class type detected...");
							break;
					}
				}
			}
			else
			{
				child.GetComponent<CharacterSlot>().DeselectSlot();
			}
		}
		
		if (selectedSlot == null || selectedSlot.saved != null)
			ArrowPickerController.instance.DisablePulsingButtons();
		else
			ArrowPickerController.instance.EnablePulsingButtons();
	}
	
	public void ShowPonyPicker()
	{
		this.pPicker.TurnOnArrows ();
	}
	

	public void ShowPickedPonyDetails()
	{
		this.pickedPonyDetails.gameObject.SetActive(true);
		this.pickedPonyDetails.Init ();
		
		TweenScale.Tween(this.confirmButton.gameObject, .5f, new Vector3(1,1,1), new Vector3(1.1f,1.1f,1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);
		
		
	}
	
	public void HidePickedPonyDetails()
	{
		this.pickedPonyDetails.gameObject.SetActive(false);
	}
	
	
	
	public void HidePonyPicker()
	{
		//this.pPicker.gameObject.SetActive(false);
	}
	
	public void HideButtons()
	{
		//this.confirmButton.gameObject.SetActive(false);
		//this.deleteButton.gameObject.SetActive(false);
	}
	
	public void ShowButtons()
	{
		//this.confirmButton.gameObject.SetActive(true);
		//	this.deleteButton.gameObject.SetActive(true);
	}
	
	public void DeleteCharacter()
	{
		Action<bool> processResponse = (bool response) => { 
			if(response == true && this.selectedSlot != null)
			{
				PF_PlayerData.DeleteCharacter(this.selectedSlot.saved.characterDetails.CharacterId);
				this.selectedSlot = null;
			}
		};
		
		DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.DEL_CHAR_PROMPT, string.Format(GlobalStrings.DEL_CHAR_MSG, this.selectedSlot.ponyName.text), processResponse);
	}
		
	public void PonyPicked(ArrowUI pony)
	{
		Action<string> processResponse = (string response) => { 
			if(response != null)
			{
				//ShowButtons();
				PF_PlayerData.CreateNewCharacter(response, pony.details);
			}
		};
		
		string[] randomNames = new string[] { "Sparkle", "Twinkle", "PowerHoof", "Nova", "Lightning", "ReignBow", "Charlie", "Balloonicorn", "Roach" };
		int rng = UnityEngine.Random.Range(0, randomNames.Length);
		
		DialogCanvasController.RequestTextInputPrompt(GlobalStrings.CHAR_NAME_PROMPT, GlobalStrings.CHAR_NAME_MSG, processResponse, randomNames[rng]);
	}
	
	
	public void BuyAdditionalSlots()
	{
		ResetDataChecks ();
		DialogCanvasController.RequestStore("Character Slot Store");
	}
	
	
	public void Continue()
	{
		PF_PlayerData.activeCharacter = this.selectedSlot.saved;
		PF_PlayerData.activeCharacter.SetMaxVitals();
		
		SceneController.Instance.RequestSceneChange(SceneController.GameScenes.Profile, 0f);
	}
	#endregion

	
	/// <summary>
	/// This method waits until all the needed data has been loaded, before updating the UI
	/// </summary>


}
