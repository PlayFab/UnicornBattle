using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
		BuyAction.onClick.AddListener(controller.InitiatePurchase);
	}

	public void RefreshSelected(StoreDisplayItem item)
	{
		if(item == null)
			return;

		itemData = item;
		icon.overrideSprite = item.image.overrideSprite;
		itemName.text = item.catalogItem.DisplayName;
		itemDescription.text = item.catalogItem.Description;
		
		var kvp = item.catalogItem.VirtualCurrencyPrices.First();
		itemCost.text = string.Format(" x{0}", kvp.Value);
		currencyIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(kvp.Key);
		
		if(item.catalogItem.Consumable.UsageCount != null)
			totalUses.text = string.Format(" x{0}", item.catalogItem.Consumable.UsageCount);
	}
}
