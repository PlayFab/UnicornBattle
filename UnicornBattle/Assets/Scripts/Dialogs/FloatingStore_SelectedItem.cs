using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class FloatingStore_SelectedItem : MonoBehaviour {
	public StoreDisplayItem itemData;
	public Image icon;
	public Text itemName;
	public Text itemDescription;
	
	public Text itemCost;
	public Image currencyIcon;
	
	public Text totalUses;
	public Image usesIcon;
	
	public Button BuyAction;
	public FloatingStoreController controller;
	
	// Use this for initialization
	void Start () {
		this.BuyAction.onClick.AddListener( ()=> 
		{ 
			this.controller.InitiatePurchase();
		});
	}

	public void RefreshSelected(StoreDisplayItem item)
	{
		if(item == null)
			return;

		this.itemData = item;
		this.icon.overrideSprite = item.image.overrideSprite;
		this.itemName.text = item.catalogItem.DisplayName;
		this.itemDescription.text = item.catalogItem.Description;
		
		
		
		var kvp = item.catalogItem.VirtualCurrencyPrices.First();
		this.itemCost.text = string.Format(" x{0}", kvp.Value);
		this.currencyIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(kvp.Key);
		
		if(item.catalogItem.Consumable.UsageCount != null)
		{
			this.totalUses.text = string.Format(" x{0}", item.catalogItem.Consumable.UsageCount);
		}
		
		
	}
}
