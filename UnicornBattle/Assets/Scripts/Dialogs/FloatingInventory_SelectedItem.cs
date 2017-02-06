using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class FloatingInventory_SelectedItem : MonoBehaviour {
	public InventoryDisplayItem itemData;
	public Image icon;
	public Text itemName;
	public Text itemDescription;
	public Text annotation;
	
	public Text totalUses;
	public Image usesIcon;
	
	public Button UseAction;
	public Button UnlockAction;
	public FloatingInventoryController controller;
	
	// Use this for initialization
	void Start () {
		this.UseAction.onClick.AddListener( ()=> 
		                                   { 
			this.controller.UseItem();
		});
		
		this.UnlockAction.onClick.AddListener( ()=> 
		                                   { 
			this.controller.UnlockContainer();
		});
	}

	public void RefreshSelected(InventoryDisplayItem item)
	{
		if(item == null)
			return;
		
		this.itemData = item;
		this.icon.overrideSprite = item.category.icon;
		this.itemName.text = item.category.catalogRef.DisplayName;
		this.itemDescription.text = item.category.catalogRef.Description;
		this.annotation.text = item.category.inventory[0].Annotation;
		
//		var kvp = item.category.catalogRef.VirtualCurrencyPrices.First();
//		this.itemCost.text = string.Format(" x{0}", kvp.Value);
//		this.currencyIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(kvp.Key);
		
		if(item.category.catalogRef.Container != null && item.category.catalogRef.Container.ResultTableContents != null)
		{
			this.totalUses.gameObject.SetActive(true);
			this.totalUses.text = string.Format(" x{0}", item.category.count);
			
			this.UseAction.gameObject.SetActive(false);
			this.UnlockAction.gameObject.SetActive(true);
		}
		else if(item.category.isConsumable || item.category.totalUses > 0 || item.category.count > 1)
		{
			if(controller.activeFilter == DialogCanvasController.InventoryFilters.UsableInCombat)
			{
				this.UseAction.gameObject.SetActive(true);
			}

            this.UnlockAction.gameObject.SetActive(false);
			this.totalUses.text = string.Format(" x{0}", item.category.totalUses > item.category.count ? item.category.totalUses : item.category.count );
			this.totalUses.gameObject.SetActive(true);
		}
		else
		{
			this.totalUses.gameObject.SetActive(false);
			this.UseAction.gameObject.SetActive(false);
            this.UnlockAction.gameObject.SetActive(false);
		}
	}
}
