using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class ItemViewerController : MonoBehaviour {
	public Image CurrentIcon;
	public Text CurrentItemName;
	public Text CurrentItemDesc;
	public Text ItemCount;
	
	//container items
	public UnlockSliderController slider;
	public Text ContainerItemDesc;
	
	
	
	// WHAT ITEM OBJECT TO USE?
	//public List<
	public Button CloseButton;
	public Button NextItemButton;
	public Button PrevItemButton;
	
	
	public List<CatalogItem> items = new List<CatalogItem>();
	public CatalogItem selectedItem;
	private int selectedIndex = 0;
	private UnityAction<UnlockContainerItemResult> afterUnlockCallback;
	private string currentIconId = string.Empty;


	// MAKE THIS index / List<ContainerResultItem>
	public Dictionary<int, List<ContainerResultItem>> openedBoxes = new Dictionary<int, List<ContainerResultItem>>();
	//public Dictionary<CatalogItem, int> aggregatedItems = new Dictionary<CatalogItem, int>(); 
	

	
	public Transform itemList;
	public Transform itemPrefab;
	public Transform containerResults;
	
	public Transform ItemMode;
	public Transform ContainerMode;
	public bool UnpackToPlayer = false;
	
#region standardControls	
	public void NextItem()
	{
		int index = this.selectedIndex;
		index++;
		
		if(index + 1 == this.items.Count)
		{
			this.NextItemButton.interactable = false;
		}
		
		this.PrevItemButton.interactable = true;
		
		this.selectedIndex = index;
		SetSelectedItem(this.items[index]);
		
	}
	
	public void PrevItem()
	{
		int index = this.selectedIndex;
		index--;
		
		if(index == 0)
		{
			this.PrevItemButton.interactable = false;
		}
		
		this.NextItemButton.interactable = true;
		
		this.selectedIndex = index;
		SetSelectedItem(this.items[index]);
	}
	

	
	public void SetSelectedItem(CatalogItem item)
	{
		this.selectedItem = item;
		//this.selectedIndex = items.IndexOf(item);
		
		this.CurrentItemName.text = this.selectedItem.DisplayName;
		this.CurrentItemDesc.text = this.selectedItem.Description;
		this.ItemCount.text = string.Format("{0}/{1}", this.selectedIndex+1, this.items.Count);
		// refresh the UI
		
		
		
		this.currentIconId = "Default";
		if( !string.Equals(item.CustomData, null)) //should be !string.IsNullOrEmpty(CI.CustomData)
		{
			Dictionary<string, string> kvps = PlayFab.SimpleJson.DeserializeObject<Dictionary<string, string>>(item.CustomData);
			kvps.TryGetValue("icon", out currentIconId);	
		}
		Sprite icon = GameController.Instance.iconManager.GetIconById(currentIconId);	
		this.CurrentIcon.overrideSprite = icon;
		
		if(openedBoxes.ContainsKey(this.selectedIndex))
		{
			// this container has been opened, show the items...
			if(this.selectedItem.Container != null && this.selectedItem.Container.ItemContents == null && this.selectedItem.Container.ResultTableContents == null && this.selectedItem.Container.VirtualCurrencyContents == null)
			{
				//is a bundle, dont change the icon.
					
			}
			else
			{
				this.CurrentIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(currentIconId+"_Open");
			}
			EnableContainerMode(true);
		}
		else
		{
			// test bundles here....
			// if bundle, we need to show the contents, but not remove it from the list, as it will be unpacked and added automatically
			if(this.selectedItem.Bundle != null && (this.selectedItem.Bundle.BundledItems != null ||  this.selectedItem.Bundle.BundledResultTables != null || this.selectedItem.Bundle.BundledVirtualCurrencies != null) )
			{
				List<ContainerResultItem> items = new List<ContainerResultItem>();
				
				if(this.selectedItem.Bundle.BundledItems != null && this.selectedItem.Bundle.BundledItems.Count > 0)
				{
					foreach(var award in this.selectedItem.Bundle.BundledItems)
					{
						string awardIcon = "Default";
						CatalogItem catItem = PF_GameData.catalogItems.Find( (i) => {  return i.ItemId == award; } );
						Dictionary<string, string> kvps = PlayFab.SimpleJson.DeserializeObject<Dictionary<string, string>>(catItem.CustomData);
						kvps.TryGetValue("icon", out awardIcon);	
						
						items.Add(new ContainerResultItem(){ 
							displayIcon = GameController.Instance.iconManager.GetIconById(awardIcon),	
							displayName = catItem.DisplayName
						});	
					}
				}
				
				if(this.selectedItem.Bundle.BundledResultTables != null && this.selectedItem.Bundle.BundledResultTables.Count > 0)
				{
					foreach(var award in this.selectedItem.Bundle.BundledResultTables)
					{
						items.Add(new ContainerResultItem(){ 
							displayIcon = GameController.Instance.iconManager.GetIconById("DropTable"),
							displayName = string.Format("Drop Table: {0}", award ) 
						});
					}
				}
				
				if(this.selectedItem.Bundle.BundledVirtualCurrencies != null && this.selectedItem.Bundle.BundledVirtualCurrencies.Count > 0)
				{
					foreach(var award in this.selectedItem.Bundle.BundledVirtualCurrencies)
					{
						items.Add(new ContainerResultItem(){ 
							displayIcon = GameController.Instance.iconManager.GetIconById(award.Key),
							displayName = string.Format("{1} Award: {0:n0}", award.Value, award.Key ) 
						});
					}
				}

				if(items.Count > 0)
				{
					this.openedBoxes.Add(this.selectedIndex, items);
					
					EnableContainerMode(true);
					
					// dont fall through the rest of the logic.
					return;
				}
			}
			
			
			if(this.selectedItem.Container != null && this.selectedItem.Container.ItemContents == null && this.selectedItem.Container.ResultTableContents == null && this.selectedItem.Container.VirtualCurrencyContents == null)
			{
				DisableContainerMode();
			}
			else
			{
				Debug.Log ("This is a container");
				EnableContainerMode();
			}
		}
	}
	
	public void InitiateViewer(List<string> items, bool unpackToPlayer) // need a new flag to determine if we should unpack to a player or a character
	{
		
		this.UnpackToPlayer = unpackToPlayer;
		
		this.items.Clear();
		this.openedBoxes.Clear();
		
		if(PF_GameData.catalogItems != null && PF_GameData.catalogItems.Count > 0)
		{
			foreach(var item in items)
			{
				CatalogItem catalogRef = PF_GameData.catalogItems.Find( (i) => { return i.ItemId == item; });
				if(catalogRef != null)
					this.items.Add(catalogRef);
			}
			
		
			this.PrevItemButton.interactable = false;
			if(this.items.Count == 1)
			{
				this.NextItemButton.interactable = false;
			}
			else
			{
				this.NextItemButton.interactable = true;
			}
			
			//select the first in the list
			SetSelectedItem(this.items[0]);
			this.selectedIndex = 0;
			this.gameObject.SetActive(true);
		}
		else
		{
			return;
		}
	}
	
	public void CloseViewer()
	{
		this.gameObject.SetActive(false);
	}
#endregion
	
	public void EnableContainerMode( bool isAlreadyOpen = false)
	{
		this.ContainerMode.gameObject.SetActive(true);
		this.ItemMode.gameObject.SetActive(false);
		
		if(isAlreadyOpen == true)
		{
			ClearContainerItems();
			EnableUnlockedItemsView(this.openedBoxes[this.selectedIndex]);
		}
		else
		{
			DisableUnlockedItemsView();
			
			this.ContainerItemDesc.text = this.selectedItem.Description;
			this.slider.SetupSlider(AfterUnlock);
			this.slider.gameObject.SetActive(true);
		}
	}
	
	public void DisableContainerMode()
	{
		this.ContainerMode.gameObject.SetActive(false);
		this.ItemMode.gameObject.SetActive(true);
		
		//this.CurrentItemDesc.text = this.selectedItem.Description;
	}

	public void AfterUnlock(UnlockContainerItemResult result)
	{
		//Debug.Log("ITEMs: " + result.GrantedItems.Count);
		//Debug.Log("VCs: " + result.VirtualCurrency.Count);
		
		// unlocking a container will automatically add the returned items, this ensures that items are not added twice.
		PF_GamePlay.QuestProgress.ItemsFound.Remove(this.selectedItem.ItemId);
		
		// build our list for displaying the container results
		List<ContainerResultItem> items = new List<ContainerResultItem>();
		
		foreach(var award in result.GrantedItems)
		{
			string awardIcon = "Default";
			CatalogItem catItem = PF_GameData.catalogItems.Find( (i) => {  return i.ItemId == award.ItemId; } );
			Dictionary<string, string> kvps = PlayFab.SimpleJson.DeserializeObject<Dictionary<string, string>>(catItem.CustomData);
			kvps.TryGetValue("icon", out awardIcon);	
			
			items.Add(new ContainerResultItem()
			{ 
				displayIcon = GameController.Instance.iconManager.GetIconById(awardIcon),	
				displayName = award.DisplayName
			});	
		}
		
		if(result.VirtualCurrency != null)
		{
			foreach(var award in result.VirtualCurrency)
			{
				items.Add(new ContainerResultItem(){ 
					displayIcon = GameController.Instance.iconManager.GetIconById(award.Key),
					displayName = string.Format("{1} Award: {0}", award.Value, award.Key ) 
				});
			}
		}
		else
		{
			CatalogItem catRef = PF_GameData.catalogItems.Find( (i) => {return i.ItemId == this.selectedItem.ItemId; });
			if(catRef.Container.VirtualCurrencyContents.Count > 0)
			{
				foreach(var vc in catRef.Container.VirtualCurrencyContents)
				{
					items.Add(new ContainerResultItem(){ 
						displayIcon = GameController.Instance.iconManager.GetIconById(vc.Key),
						displayName = string.Format("{1} Award: {0}", vc.Value, vc.Key ) 
					});
				}
			}
		}
			
		this.CurrentIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(currentIconId+"_Open");	
		this.openedBoxes.Add(this.selectedIndex, items);
		EnableUnlockedItemsView(items);
		
//		if(this.afterUnlockCallback != null)
//		{
			DialogCanvasController.RequestInventoryPrompt();
//		}
	}
	
	
	void EnableUnlockedItemsView(List<ContainerResultItem> unlockedItems = null)
	{
		this.ContainerItemDesc.gameObject.SetActive(false);
		this.slider.gameObject.SetActive(false);
		this.containerResults.gameObject.SetActive(true);
		
		foreach(var item in unlockedItems)
		{
			var slot = Instantiate (this.itemPrefab);
			ContainerItemResult cir = slot.GetComponent<ContainerItemResult>();
			cir.Icon.overrideSprite = item.displayIcon;
			cir.ItemName.text = item.displayName;
			slot.SetParent (this.itemList, false);
		}
	}
	
	void DisableUnlockedItemsView()
	{
		ClearContainerItems();

		
		this.ContainerItemDesc.gameObject.SetActive(true);
		this.containerResults.gameObject.SetActive(false);
		
		//this.it
	}
	
	
	void ClearContainerItems()
	{
		ContainerItemResult[] children = this.itemList.GetComponentsInChildren<ContainerItemResult> (true);
		for (int z = 0; z < children.Length; z++) {
			if (children [z].gameObject != this.itemList.gameObject) {
				DestroyImmediate(children[z].gameObject);
			}
		}
	}
	
	
	
	
#region unused	
	void OnEnable()
	{
        GameplayController.OnGameplayEvent += OnGameplayEventReceived;
		//uiSlider.onValueChanged.AddListener((v) => { OnSliderChanged(v); });
	}
	
	void OnDisable()
	{
        GameplayController.OnGameplayEvent -= OnGameplayEventReceived;
		
	}
	
    void OnGameplayEventReceived(string message,  PF_GamePlay.GameplayEventTypes type )
	{
		
	}
	
	
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () {
	
	}
#endregion	
}



public class ContainerResultItem
{
	public Sprite displayIcon;
	public string displayName;
}

