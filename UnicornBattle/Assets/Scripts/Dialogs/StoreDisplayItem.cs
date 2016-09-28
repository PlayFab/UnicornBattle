using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class StoreDisplayItem : MonoBehaviour {
	public Image bg;
	public Image image;
	public Button btn;
    public Image Savings;
    public Text SavingsText;
    public Image ItemCurrencyIcon;
    public Text ItemDesc;
    public Text ItemPrice;
    public List<Sprite> CurrencyIcons;

	public FloatingStoreController controller;
	public CatalogItem catalogItem;

	public void Init()
	{
		this.btn.onClick.AddListener( ()=> 
		{ 
			this.controller.ItemClicked(this); 
			this.bg.color = Color.green;
			this.image.color = new Color(255,255,255,255);
		});
		
	}
	
	
	public void SetButton(Sprite icon, CatalogItem cItem)
	{
		ActivateButton ();
		this.image.overrideSprite = icon;
		
		this.ItemDesc.text = cItem.DisplayName;
		string currencyKey = "RM";
		string currencyFormat = "{0:C2}";
		ItemCurrencyIcon.sprite = CurrencyIcons [0];
		//For now disable uses:
		Savings.gameObject.SetActive (false);

		if (cItem.VirtualCurrencyPrices.ContainsKey ("GM")) {
			currencyKey = "GM";
			currencyFormat = "{0}";
			ItemCurrencyIcon.sprite = CurrencyIcons [1];
		} else if (cItem.VirtualCurrencyPrices.ContainsKey ("AU")) {
			currencyKey = "AU";
			currencyFormat = "{0}";
			ItemCurrencyIcon.sprite = CurrencyIcons [2];
		}
	
		// check prices to see if this is a better deal than MSRP
		this.Savings.gameObject.SetActive(false);
		CatalogItem msrp = PF_GameData.GetCatalogItemById(cItem.ItemId);
		uint msrpPrice;
		if(msrp != null && msrp.VirtualCurrencyPrices != null && msrp.VirtualCurrencyPrices.TryGetValue(currencyKey, out msrpPrice))
		{
			// check prices to see if this is a better deal than MSRP
			uint salePrice;
			cItem.VirtualCurrencyPrices.TryGetValue(currencyKey, out salePrice);
			
			if(salePrice < msrpPrice)
			{
				// VC only, not adjusting RM prices yet.
				float percent = (((float)msrpPrice - (float)salePrice)/(float)msrpPrice);
				this.SavingsText.text = string.Format("Save\n{0}%", Mathf.RoundToInt(percent*100f));
				this.Savings.gameObject.SetActive(true);
			}
		}
		
		string price = string.Empty;
		if (currencyKey == "RM" && cItem.VirtualCurrencyPrices.ContainsKey (currencyKey)) 
		{
			uint pennies = cItem.VirtualCurrencyPrices[currencyKey];
			price = string.Format(currencyFormat, (float)pennies / 100f);	
		}
		else if(cItem.VirtualCurrencyPrices.ContainsKey (currencyKey))
		{
			price = string.Format(currencyFormat, cItem.VirtualCurrencyPrices[currencyKey]);
		}
		this.ItemPrice.text = price;

		btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            controller.selectedItem = this;
            controller.InitiatePurchase();
        });

	    this.catalogItem = cItem;
	}
	
	public void Deselect()
	{
		this.bg.color = Color.white;
		this.image.color = new Color(255,255,255,190);
		this.SavingsText.text = string.Empty;
		this.ItemDesc.text = string.Empty;
		this.ItemPrice.text = string.Empty;
	}
	
	
	public void ClearButton()
	{
		this.image.overrideSprite = null;
		this.image.color = Color.clear;
		this.catalogItem = null;
		this.btn.interactable = false;
		this.bg.color = Color.white;
		this.gameObject.SetActive(false);
	}
	
	public void ActivateButton()
	{
		this.gameObject.SetActive(true);
		this.image.color = new Color(255,255,255,190);
		this.image.overrideSprite = null;
		this.image.color = Color.white;
		this.catalogItem = null;
		this.btn.interactable = true;
		
	}
	
}
