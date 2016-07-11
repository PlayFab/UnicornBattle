using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;

public class StorePicker : MonoBehaviour {
 	
 	public List<UnicornStore> stores;
 	public StoreDisplayController storeDisplay;
 	public StorePickerItem selectedStore;
 	
 	public Transform tabMenu;
 	/* 
 	private Dictionary<string, int> virtualCurrency;
	
	private List<ItemInstance> mostRecentlyUnpacked;
	
	private List<CatalogItem> catalogItems = new List<CatalogItem>(); 
	private List<StoreItem> storeItems = new List<StoreItem>();
	private List<ItemInstance> allInventory = new List<ItemInstance>();
	private Dictionary<string, InventoryCategory> inventoryByCategory = new Dictionary<string, InventoryCategory>(); 
	
	//private bool keySelected = false;
	int selectedStoreIndex = -1;
	CatalogItem selectedStoreItem;
	*/
	
	// Use this for initialization
	void Start () {
		// for testing only
		storeDisplay.pageController.LoadStore(stores[selectedStore.storeIndex].items);
	}

	public void StorePickerItemClicked(StorePickerItem item)
	{
		if(selectedStore != item)
		{
			ClearSelected();
			storeDisplay.HideSelected();
			this.selectedStore = item;
			item.GetComponent<Image>().color = Color.green;
			storeDisplay.pageController.LoadStore(stores[item.storeIndex].items);
		}
	}
	
	public void ClearSelected()
	{
		this.selectedStore = null;
		foreach (Transform each in tabMenu)
		{
			each.GetComponent<Image>().color = Color.white;
		}
	}
	
}


#region helper_classes
[System.Serializable]
public class UnicornStore
{
	public string storeName;
	//public List<CatalogItem> items; //use this one for actual PF items
	public List<TEST_StoreItem> items; // use this one for UI testing
	
	
}

[System.Serializable]
public class TEST_StoreItem
{
	public string displayName;
	public string displayText;
	public Color iconColor = Color.gray;
	public Color textColor = Color.black;

	
}



#endregion