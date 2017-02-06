using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FloatingStoreController : SoftSingleton<FloatingStoreController>
{
	public Text StoreName;
	public Text pageDisplay;
	public Button nextPage;
	public Button prevPage;
	
	public List<string> currenciesInUse;
	
	
	public Transform ItemGrid;
	public List<StoreDisplayItem> inventory = new List<StoreDisplayItem>();
	
	
	private int currentPage = 1;
	private int pageCount = 1;
	private int itemsPerPage = 4;
	
	private StorePicker sPicker;
	private List<StoreItem> itemsToDisplay;
	
	//public StoreDisplayItem selectedItem
	public StoreDisplayItem selectedItem;
	public StoreCurrencyBarController Currencies;

	//TODO solve the confusion with what VC balances are checked player VS character
	// close also needs to fire a callback to the calling area of the code
	public void InitiateStore(string name, List<StoreItem> items)
	{
		Dictionary<string, object> eventData = new Dictionary<string, object>()
		{
			{ "store_name", name }
		};
		PF_Bridge.LogCustomEvent(PF_Bridge.CustomEventTypes.Client_StoreVisit, eventData);
		
		
		//reset
		this.currenciesInUse.Clear ();
		this.currentPage = 1;
		this.pageCount =  Mathf.CeilToInt( (float)items.Count / (float)this.itemsPerPage);
		this.pageDisplay.text = string.Format("{0} / {1}", this.currentPage, this.pageCount);
		//HideSelectedItem();
		
		foreach(var item in this.inventory)
		{
			item.ClearButton();
		}
			

		this.itemsToDisplay = items;
		this.StoreName.text = name;

		if(pageCount > 1)
		{
			nextPage.interactable = true;
		}
		else
		{
			nextPage.interactable = false;
		}
		
		prevPage.interactable = false;
		
		
		for (int z = 0; z < items.Count; z++) 
		{
			if(z >= this.itemsPerPage)
				break;
				
			CatalogItem CI =PF_GameData.ConvertStoreItemToCatalogItem(items[z]);
			
			
			string iconName = "Default";
			if(CI.CustomData != null && !string.Equals(CI.CustomData, "null"))
			{
                try
                {
					Dictionary<string, string> kvps = JsonWrapper.DeserializeObject<Dictionary<string, string>>(CI.CustomData);
                    kvps.TryGetValue("icon", out iconName);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
			}
			Sprite icon = GameController.Instance.iconManager.GetIconById(iconName);	
			
			this.inventory[z].Init();
			this.inventory[z].SetButton(icon, CI);
			
				
					
						
			// keep track of what currencies are being used in the store.				
			List<string> VCs = items[z].VirtualCurrencyPrices.Keys.ToList();
			foreach(var vc in VCs)
			{
				int index = this.currenciesInUse.FindIndex((key) => {return string.Equals(key, vc); });
				// make sure not already in the list.
				if(index < 0)
				{
					this.currenciesInUse.Add(vc);
				}
			}
		}

		//hide selected
		this.Currencies.Init ();
		this.gameObject.SetActive (true);

	}


	public void ShowSelectedItem()
	{
		//this.selectedItem.gameObject.SetActive(true);
	}	
	
	public void HideSelectedItem()
	{
		DeselectButtons();
		
		if(this.selectedItem != null)
		{
			this.selectedItem = null;
		}
		
		//this.selectedItem.gameObject.SetActive(false);
		
	}

	public void DeselectButtons()
	{
		foreach(var item in this.inventory)
		{
			if(item.catalogItem != null)
			{
				item.Deselect();
			}
		}
	}

	public void CloseStore()
	{
		// get this to close down and also close the tint.	
		// get a confirmation here
		this.gameObject.SetActive (false);
	}

	public void ItemClicked(StoreDisplayItem item)
	{
		if(this.selectedItem != null)
		{
			this.selectedItem.Deselect();
		}
		
		//this.selectedItem.RefreshSelected (item);
		//ShowSelectedItem();
		//item.bg.color = Color.green;
	}
	
	
	
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
				CatalogItem CI = PF_GameData.ConvertStoreItemToCatalogItem(itemsToDisplay[z]);
				
				
				string iconName = "Default";
				if( !string.Equals(CI.CustomData, null)) //should be !string.IsNullOrEmpty(CI.CustomData)
				{
					Dictionary<string, string> kvps = JsonWrapper.DeserializeObject<Dictionary<string, string>>(CI.CustomData);
					kvps.TryGetValue("icon", out iconName);	
				}
				Sprite icon = GameController.Instance.iconManager.GetIconById(iconName);	
				
				this.inventory[uiIndex].Init();
				this.inventory[uiIndex].SetButton(icon, CI);
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
			CatalogItem CI = PF_GameData.ConvertStoreItemToCatalogItem(itemsToDisplay[z]);
			
			
			string iconName = "Default";
			if( !string.Equals(CI.CustomData, null)) //should be !string.IsNullOrEmpty(CI.CustomData)
			{
				Dictionary<string, string> kvps = JsonWrapper.DeserializeObject<Dictionary<string, string>>(CI.CustomData);
				kvps.TryGetValue("icon", out iconName);	
			}
			Sprite icon = GameController.Instance.iconManager.GetIconById(iconName);	
			
			this.inventory[uiIndex].Init();
			this.inventory[uiIndex].SetButton(icon, CI);
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
	
	
	
	public void InitiatePurchase()
	{
		//NEED TO KNOW WHICH PURCHASE FLOW TO USE
		//Debug.Log ("Starting purchase of " + selectedItem.catalogItem.ItemId);
		PF_GamePlay.StartBuyStoreItem(this.selectedItem.catalogItem, this.StoreName.text);
		
	}
	
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
			case PlayFabAPIMethods.MakePurchase:
				// refresh after purchase.
				if(PF_PlayerData.activeCharacter == null)
				{
					PF_PlayerData.GetUserAccountInfo();
				}
				else
				{
					PF_PlayerData.GetCharacterInventory(PF_PlayerData.activeCharacter.characterDetails.CharacterId);
				}
			break;
			
			case PlayFabAPIMethods.GetCharacterInventory:
				DialogCanvasController.RequestStore(this.StoreName.text);	
			break;
			
			case PlayFabAPIMethods.GetAccountInfo:
				DialogCanvasController.RequestStore(this.StoreName.text);
			break;
			
			
		}
	}
	
	
}
