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
    public Text SlashPrice;
    public Text Slash;
    public List<Sprite> CurrencyIcons;

    public FloatingStoreController controller;

    public CatalogItem catalogItem;

    public void Init()
    {
        btn.onClick.AddListener(() =>
        {
            controller.ItemClicked(this);
            bg.color = Color.green;
            image.color = new Color(255, 255, 255, 255);
        });
    }

    public void SetButton(Sprite icon, StoreItem sItem)
    {
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
            controller.InitiatePurchase();
        });

        uint salePrice, basePrice; string currencyKey;
        GetPrice(sItem, catalogItem, out salePrice, out basePrice, out currencyKey);
        SetIcon(currencyKey);
        SetPrice(salePrice, basePrice, currencyKey);
    }

    private void GetPrice(StoreItem sItem, CatalogItem cItem, out uint salePrice, out uint basePrice, out string currencyKey)
    {
        currencyKey = "RM";
        if (sItem.VirtualCurrencyPrices.ContainsKey(GlobalStrings.GEM_CURRENCY))
            currencyKey = GlobalStrings.GEM_CURRENCY;
        else if (sItem.VirtualCurrencyPrices.ContainsKey(GlobalStrings.GOLD_CURRENCY))
            currencyKey = GlobalStrings.GOLD_CURRENCY;

        uint temp;
        salePrice = basePrice = 0;
        if (sItem.VirtualCurrencyPrices.TryGetValue(currencyKey, out temp))
            salePrice = temp;
        if (cItem != null && cItem.VirtualCurrencyPrices != null
          && cItem.VirtualCurrencyPrices.TryGetValue(currencyKey, out temp))
            basePrice = temp;
    }

    private void SetIcon(string currencyKey)
    {
        switch (currencyKey)
        {
            case "RM":
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

        ItemPrice.text = currencyKey == "RM"
            ? string.Format("{0:C2}", salePrice / 100.0f) // Price in cents
            : salePrice.ToString("N0").Trim('.');
        SlashPrice.text = currencyKey == "RM"
            ? string.Format("{0:C2}", basePrice / 100.0f) // Price in cents
            : basePrice.ToString("N0").Trim('.');
        Slash.text = Slashes(SlashPrice.text.Length + 1);
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
