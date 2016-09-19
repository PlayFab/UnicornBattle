using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using PlayFab.ClientModels;
using PlayFab;
using PlayFab.Json;
using UnityEngine.Events;


public class FloatingInventoryController : MonoBehaviour {
	public Text StoreName;
	public Text pageDisplay;
	public Button nextPage;
	public Button prevPage;

	public GameObject[] transUiElements;

	public enum InventoryMode { Character = 0, Player = 1 }
	public InventoryMode activeMode = InventoryMode.Character;
	public bool showTransUi = true;

	public List<string> currenciesInUse;
	public DialogCanvasController.InventoryFilters activeFilter;
	
	public Transform ItemGrid;
	public List<InventoryDisplayItem> inventory = new List<InventoryDisplayItem>();
	public InventoryToggle toggleGroup;

	
	private int currentPage = 1;
	private int pageCount = 1;
	private int itemsPerPage = 12;
	
	private StorePicker sPicker;
	private Dictionary<string, InventoryCategory> itemsToDisplay = new Dictionary<string, InventoryCategory>();
	private Action<string> callbackAfterUse;
	
	//public StoreDisplayItem selectedItem
	public FloatingInventory_SelectedItem selectedItem;
	
	public InventoryCurrencyBarController Currencies;

	//TODO add in filter for things like:
	// only items on display
	// only containers
	// all items
	public void Init(Action<string> callback = null, DialogCanvasController.InventoryFilters filter = DialogCanvasController.InventoryFilters.AllItems, bool enableTransUi = true, InventoryMode displayMode = InventoryMode.Character)
	{
		this.activeFilter = filter;
		this.activeMode = displayMode;

		this.toggleGroup.characterButtonTx.text = PF_PlayerData.activeCharacter.characterDetails.CharacterName;

		if(callback != null)
		{
			this.callbackAfterUse = callback;
		}

		this.showTransUi = enableTransUi;
		if(enableTransUi)
		{
			ToggleTransUiOn();
		}
		else
		{
			ToggleTransUiOff();
		}

		//only displaying the main currencies (Gold & Gems) for now
		this.currenciesInUse.Clear ();
		this.currenciesInUse.Add("AU");
		this.currenciesInUse.Add("GM");

		if(displayMode == InventoryMode.Player)
		{
			if(PF_PlayerData.inventoryByCategory != null && PF_PlayerData.virtualCurrency != null)
			{
				this.itemsToDisplay = PF_PlayerData.inventoryByCategory;
				this.Currencies.Init (PF_PlayerData.virtualCurrency);
			}
			else
			{
				ResetItemTiles();
				return;
			}
		}
		else
		{
			if(PF_PlayerData.characterInvByCategory != null && PF_PlayerData.characterVirtualCurrency != null)
			{
				this.itemsToDisplay = PF_PlayerData.characterInvByCategory;
				this.Currencies.Init (PF_PlayerData.characterVirtualCurrency);
			}
			else
			{
				ResetItemTiles();
				return;
			}
		}

		//reset
		ResetItemTiles();

        string filterName;
        GlobalStrings.INV_FILTER_DISPLAY_NAMES.TryGetValue(filter, out filterName);
        this.StoreName.text = string.Format(GlobalStrings.INV_WINDOW_TITLE, filterName);

        nextPage.interactable = (pageCount > 1);
		prevPage.interactable = false;

		int count = 0;
		foreach(var kvp in this.itemsToDisplay)
		{
			if(count >= this.itemsPerPage)
				break;
			
			bool addItem = false;
			if(filter == DialogCanvasController.InventoryFilters.Containers)
			{
				if(string.Equals(kvp.Value.catalogRef.ItemClass, filter.ToString()))
				{
					addItem = true;
				}
				else
				{
					continue;
				}
			}
			else if(filter == DialogCanvasController.InventoryFilters.Keys)
			{
				if(string.Equals(kvp.Value.catalogRef.ItemClass, filter.ToString()))
				{
					addItem = true;
				}
				else
				{
					continue;
				}
			}
			else if(filter == DialogCanvasController.InventoryFilters.UsableInCombat)
			{
				if(string.Equals(kvp.Value.catalogRef.ItemClass, filter.ToString()))
				{
					addItem = true;
				}
				else
				{
					continue;
				}
			}
			else if(filter == DialogCanvasController.InventoryFilters.AllItems)
			{
				addItem = true;
			}
			
			if(addItem == true)
			{
				this.inventory[count].Init();
				this.inventory[count].SetButton(kvp.Value.icon, kvp.Value);
				count++;
			}
		}
		this.gameObject.SetActive (true);
	}
	
	
	public void ShowSelectedItem()
	{
		this.selectedItem.gameObject.SetActive(true);
	}	
	
	public void HideSelectedItem()
	{
		DeselectButtons();
		
		if(this.selectedItem.itemData != null)
		{
			this.selectedItem.itemData = null;
		}
		
		this.selectedItem.gameObject.SetActive(false);
		
	}
	
	public void DeselectButtons()
	{
		foreach(var item in this.inventory)
		{
			if(item != null)
			{
				item.Deselect();
			}
		}
	}
	
	public void CloseInventory()
	{
		// get this to close down and also close the tint.	
		// get a confirmation here
		this.gameObject.SetActive (false);
	}
	
	public void ItemClicked(InventoryDisplayItem item)
	{
		if(this.selectedItem.itemData != null)
		{
			this.selectedItem.itemData.Deselect();
		}
		
		this.selectedItem.RefreshSelected (item);
		ShowSelectedItem();
	}
	
	
	// start here after lunch! -- dict changes
	public void NextPage()
	{
		HideSelectedItem();
		int nextPage = currentPage+1;
		int lowerBound = this.itemsPerPage * currentPage;
		int upperBound = lowerBound + this.itemsPerPage;
		
        this.pageDisplay.text = string.Format(GlobalStrings.PAGE_NUMBER_MSG, nextPage, pageCount);
        
		int uiIndex = 0;
		for(int z = lowerBound; z < upperBound; z++, uiIndex++)
		{
			if(z < itemsToDisplay.Count)
			{
				var kvp = this.itemsToDisplay.ElementAt(z);
		
				
				this.inventory[uiIndex].Init();
				this.inventory[uiIndex].SetButton(kvp.Value.icon, kvp.Value);
			}
			else
			{
				this.inventory[uiIndex].ClearButton();
			}
		}

		this.prevPage.interactable = true;
		
		if(pageCount > nextPage)
		{
			this.nextPage.interactable = true;
		}
		else
		{
			this.nextPage.interactable = false;
		}
		this.currentPage++;
	}
	
	public void PrevPage()
	{
		HideSelectedItem();
		int prevPage = currentPage-1;
		int lowerBound = (this.itemsPerPage * prevPage) - this.itemsPerPage;
		int upperBound = lowerBound + this.itemsPerPage;
		
        this.pageDisplay.text = string.Format(GlobalStrings.PAGE_NUMBER_MSG, prevPage, pageCount);
		
		int uiIndex = 0;
		for(int z = lowerBound; z < upperBound; z++, uiIndex++)
		{
			if(z < itemsToDisplay.Count)
			{
				var kvp = this.itemsToDisplay.ElementAt(z);
				
				
				this.inventory[uiIndex].Init();
				this.inventory[uiIndex].SetButton(kvp.Value.icon, kvp.Value);
			}
			else
			{
				this.inventory[uiIndex].ClearButton();
			}
		}
		
		this.nextPage.interactable = true;
		
		if(prevPage > 1)
		{
			this.prevPage.interactable = true;
		}
		else
		{
			this.prevPage.interactable = false;
		}
		currentPage--;
	}
	
	
	
	public void UseItem() // not possible outside of battle
	{
		if(this.callbackAfterUse != null)
		{
			this.callbackAfterUse(selectedItem.itemData.category.catalogRef.ItemId);
			CloseInventory();
		}
	}
	
	public void UnlockContainer()
	{
		if(this.activeMode == InventoryMode.Character)
		{
			DialogCanvasController.RequestItemViewer( new List<string>() { selectedItem.itemData.category.catalogRef.ItemId } );
		}
		else
		{
			DialogCanvasController.RequestItemViewer( new List<string>() { selectedItem.itemData.category.catalogRef.ItemId }, true);
		}
	}


	public void RefreshInventory()
	{
		if(this.activeMode == InventoryMode.Character)
		{
			PF_PlayerData.GetUserInventory();
			DialogCanvasController.RequestInventoryPrompt(null, this.activeFilter, this.showTransUi, FloatingInventoryController.InventoryMode.Character);
		}
		else
		{
			PF_PlayerData.GetCharacterInventory(PF_PlayerData.activeCharacter.characterDetails.CharacterId);
			DialogCanvasController.RequestInventoryPrompt(null, this.activeFilter, this.showTransUi, FloatingInventoryController.InventoryMode.Player);
		}
	}

	public void ViewCharacterInventory(bool force = false)
	{
		if(this.activeMode == InventoryMode.Character && force == false)
			return;

		// if a change, update radio button.
		this.toggleGroup.playerRadio.overrideSprite = null;
		this.toggleGroup.characterRadio.overrideSprite = this.toggleGroup.toggleOn;

		Init(null, this.activeFilter, this.showTransUi, FloatingInventoryController.InventoryMode.Character);
	}

	public void ViewPlayerInventory(bool force = false)
	{
		if(this.activeMode == InventoryMode.Player && force == false)
			return;
	
		// if a change, update radio button.
		this.toggleGroup.characterRadio.overrideSprite = null;
		this.toggleGroup.playerRadio.overrideSprite = this.toggleGroup.toggleOn;

		Init(null, this.activeFilter, this.showTransUi, FloatingInventoryController.InventoryMode.Player);
	}

	public void TransferItem()
	{
		if(this.selectedItem.itemData.category.itemId == "ExtraCharacterSlot")
		{
			return;
		}

		//if player, show picker for valid characters
		//if character, show picker for valid characters + Player
		List<string> transOptions = new List<string>();

		if(this.activeMode == InventoryMode.Character)
		{
			transOptions.Add("Player");
		}

		foreach(var c in PF_PlayerData.playerCharacters)
		{
			if(this.activeMode == InventoryMode.Character && c.CharacterId == PF_PlayerData.activeCharacter.characterDetails.CharacterId)
			{
				//Probably better logic for this, and I should feel sad (but i dont). 
			}
			else
			{
				transOptions.Add(c.CharacterName);
			}
		}

		UnityAction<int> afterSelect = (int response)=>
		{
			var item = this.selectedItem.itemData.category.inventory.FirstOrDefault();	

			if(this.activeMode == InventoryMode.Character && response == 0) // stand alone
			{
				// send this to the player account
				Debug.Log("Moved "+ item.DisplayName +" to Player");

				PF_PlayerData.TransferItemToPlayer(PF_PlayerData.activeCharacter.characterDetails.CharacterId, item.ItemInstanceId, RefreshInventory);
			}
			else
			{
				string sourceType = "Player";
				if(this.activeMode == InventoryMode.Character) // stand alone
				{
					// remove the player option since 0 was not selected
					transOptions.RemoveAt(0);
					response--;
					sourceType = "Character";
				}

				// send this to another character
				if(item != null)
				{
					Debug.Log("Moved "+ item.DisplayName +" to Character: " + transOptions[response] + " -- " + PF_PlayerData.playerCharacters[response].CharacterId);
					PF_PlayerData.TransferItemToCharacter(PF_PlayerData.activeCharacter.characterDetails.CharacterId, sourceType, item.ItemInstanceId, PF_PlayerData.playerCharacters[response].CharacterId, RefreshInventory);
				}
			}
		};

		DialogCanvasController.RequestSelectorPrompt("Choose a recipient", transOptions, afterSelect);
	}

	public void TransferVC()
	{
		// specify Currency Code
		// enter currency amount
		// select recipient

		string cCode = "";
		int amount = 0;

		//if player, show picker for valid characters
		//if character, show picker for valid characters + Player
		List<string> transOptions = new List<string>();

		UnityAction<int> afterSelect = (int response) =>
		{
			if(this.activeMode == InventoryMode.Character && response == 0) // stand alone
			{
				// send this to the player account
				Debug.Log("Moved " + amount + cCode + " to Player");
				PF_PlayerData.TransferVcToPlayer(PF_PlayerData.activeCharacter.characterDetails.CharacterId, cCode, amount, RefreshInventory);
			}
			else
			{
				string sourceType = "Player";
				if(this.activeMode == InventoryMode.Character) // stand alone
				{
					// remove the player option since 0 was not selected
					transOptions.RemoveAt(0);
					response--;
					sourceType = "Character";
				}

				Debug.Log("Moved "+ amount + cCode + " to Character: " + transOptions[response] + " -- " + PF_PlayerData.playerCharacters[response].CharacterId);
				PF_PlayerData.TransferVCToCharacter(PF_PlayerData.activeCharacter.characterDetails.CharacterId, sourceType, cCode, amount, PF_PlayerData.playerCharacters[response].CharacterId, RefreshInventory);

			}
		};

		Action<string> afterSetAmount= (string response) =>
		{	
			if(string.IsNullOrEmpty(response))
			{
				return; //user canceled.
			}
			if(!Int32.TryParse(response, out amount) || response == "0")
			{
				PF_Bridge.RaiseCallbackError("Please enter an interger > 0 for VC amount", PlayFabAPIMethods.Generic, MessageDisplayStyle.error);
			}
			else
			{
				if(this.activeMode == InventoryMode.Character)
				{
					transOptions.Add("Player");
				}

				foreach(var c in PF_PlayerData.playerCharacters)
				{
					if(this.activeMode == InventoryMode.Character && c.CharacterId == PF_PlayerData.activeCharacter.characterDetails.CharacterId)
					{
						//Probably better logic for this, and I should feel sad (but i dont). 
					}
					else
					{
						transOptions.Add(c.CharacterName);
					}
				}

				DialogCanvasController.RequestSelectorPrompt("Choose a recipient", transOptions, afterSelect);
			}
		};

		UnityAction<int> afterSelectCC = (int response) =>
		{
			cCode = this.currenciesInUse[response];
			DialogCanvasController.RequestTextInputPrompt("Set an amount to transfer:", "Enter the amount that you wish to send.", afterSetAmount , "0");
		};

		DialogCanvasController.RequestSelectorPrompt("Select a VC:", this.currenciesInUse, afterSelectCC);

	}

	void ToggleTransUiOff()
	{
		foreach(var go in this.transUiElements)
		{
			go.SetActive(false);
		}

	}

	void ToggleTransUiOn()
	{
		if(this.activeMode == InventoryMode.Character)
		{
			this.toggleGroup.playerRadio.overrideSprite = null;
			this.toggleGroup.characterRadio.overrideSprite = this.toggleGroup.toggleOn;
		}
		else
		{
			this.toggleGroup.characterRadio.overrideSprite = null;
			this.toggleGroup.playerRadio.overrideSprite = this.toggleGroup.toggleOn;
		}

		foreach(var go in this.transUiElements)
		{
			go.SetActive(true);
		}

	}

	void ResetItemTiles()
	{
		this.currentPage = 1;
		this.pageCount =  Mathf.CeilToInt( (float)this.itemsToDisplay.Count / (float)this.itemsPerPage);
		this.pageDisplay.text = string.Format("{0} / {1}", this.currentPage, this.pageCount);
		HideSelectedItem();
		
		foreach(var item in this.inventory)
		{
			item.ClearButton();
		}
	}
}
