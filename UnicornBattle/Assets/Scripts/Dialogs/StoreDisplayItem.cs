using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StoreDisplayItem : MonoBehaviour
{
    public Image bg;
    public Image image;
    public Button btn;
    public Image Savings;
    public Text SavingsText;
    public Image ItemCurrencyIcon;
    public Text ItemDesc;
    public Text ItemPrice;
    public Text OldPrice;
    public Text NewPrice;
    public Text Slash;
    public List<Sprite> CurrencyIcons;
    public FloatingStoreController controller;

    public CatalogItem catalogItem;
    public string currencyKey;
    public uint finalPrice;
    private StoreItem storeItem;
    private string storeId;

    public void Init(FloatingStoreController setController)
    {
        controller = setController;
        btn.onClick.AddListener(() =>
        {
            controller.ItemClicked(this);
            bg.color = Color.green;
            image.color = new Color(255, 255, 255, 255);
        });
    }

    public void SetButton(Sprite icon, string sId, StoreItem sItem)
    {
        storeId = sId;
        storeItem = sItem;
        catalogItem = PF_GameData.GetCatalogItemById(sItem.ItemId);

        var displayName = catalogItem != null ? catalogItem.DisplayName : "Unknown item";

        ActivateButton();
        image.overrideSprite = icon;

        gameObject.name = displayName;
        ItemDesc.text = displayName;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            controller.selectedItem = this;
            InitiatePurchase();
        });

        uint salePrice, basePrice;
        finalPrice = GetPrice(out salePrice, out basePrice, out currencyKey);
        SetIcon();
        SetPrice(salePrice, basePrice, currencyKey);
    }

    public void InitiatePurchase()
    {
        PF_GamePlay.StartBuyStoreItem(catalogItem, storeId, currencyKey, finalPrice);
    }

    /// <returns>Final price that will be passed into the Purchase API</returns>
    private uint GetPrice(out uint salePrice, out uint basePrice, out string getCurrencyKey)
    {
        getCurrencyKey = GlobalStrings.REAL_MONEY_CURRENCY;
        if (storeItem.VirtualCurrencyPrices.ContainsKey(GlobalStrings.GEM_CURRENCY))
            getCurrencyKey = GlobalStrings.GEM_CURRENCY;
        else if (storeItem.VirtualCurrencyPrices.ContainsKey(GlobalStrings.GOLD_CURRENCY))
            getCurrencyKey = GlobalStrings.GOLD_CURRENCY;

        uint temp, tempFinalPrice = 0; salePrice = basePrice = 0;
        if (catalogItem != null && catalogItem.VirtualCurrencyPrices != null
          && catalogItem.VirtualCurrencyPrices.TryGetValue(getCurrencyKey, out temp))
            tempFinalPrice = basePrice = temp;
        if (storeItem.VirtualCurrencyPrices.TryGetValue(getCurrencyKey, out temp))
            tempFinalPrice = salePrice = temp;

        // Fix for if we change currency type
        if (basePrice == 0 && salePrice != 0)
            tempFinalPrice = basePrice = salePrice;

        return tempFinalPrice;
    }

    private void SetIcon()
    {
        switch (currencyKey)
        {
            case GlobalStrings.REAL_MONEY_CURRENCY:
                ItemCurrencyIcon.sprite = CurrencyIcons[0]; break;
            case GlobalStrings.GEM_CURRENCY:
                ItemCurrencyIcon.sprite = CurrencyIcons[1]; break;
            case GlobalStrings.GOLD_CURRENCY:
                ItemCurrencyIcon.sprite = CurrencyIcons[2]; break;
        }
    }

    private void SetPrice(uint salePrice, uint basePrice, string currencyKey)
    {
        var onSale = salePrice < basePrice;
        var percent = basePrice != 0 ? (basePrice - salePrice) / (float)basePrice : 0.0f;
        SavingsText.text = !onSale ? "" : string.Format("Save\n{0}%", Mathf.RoundToInt(percent * 100f));

        Savings.gameObject.SetActive(onSale);
        ItemPrice.gameObject.SetActive(!onSale);

        if (onSale)
        {
            OldPrice.text = currencyKey == GlobalStrings.REAL_MONEY_CURRENCY
                ? string.Format("{0:C2}", basePrice / 100.0f) // Price in cents
                : basePrice.ToString("N0").Trim('.');
            NewPrice.text = currencyKey == GlobalStrings.REAL_MONEY_CURRENCY
                ? string.Format("{0:C2}", salePrice / 100.0f) // Price in cents
                : salePrice.ToString("N0").Trim('.');
            Slash.text = Slashes((int)Math.Ceiling((OldPrice.text.Length + 1) * 1.1));
        }
        else
        {
            ItemPrice.text = currencyKey == GlobalStrings.REAL_MONEY_CURRENCY
                ? string.Format("{0:C2}", basePrice / 100.0f) // Price in cents
                : basePrice.ToString("N0").Trim('.');
        }
    }

    [ThreadStatic]
    private static StringBuilder _sb;
    private string Slashes(int num)
    {
        if (_sb == null)
            _sb = new StringBuilder();
        _sb.Length = 0;
        for (var i = 0; i < num; i++)
            _sb.Append("-");
        return _sb.ToString();
    }

    public void Deselect()
    {
        bg.color = Color.white;
        image.color = new Color(255, 255, 255, 190);
        SavingsText.text = string.Empty;
        ItemDesc.text = string.Empty;
        ItemPrice.text = string.Empty;
    }

    public void ClearButton()
    {
        image.overrideSprite = null;
        image.color = Color.clear;
        btn.interactable = false;
        bg.color = Color.white;
        gameObject.SetActive(false);
    }

    public void ActivateButton()
    {
        gameObject.SetActive(true);
        image.color = new Color(255, 255, 255, 190);
        image.overrideSprite = null;
        image.color = Color.white;
        btn.interactable = true;
    }
}
