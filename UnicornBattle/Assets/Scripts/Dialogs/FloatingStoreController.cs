using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingStoreController : SoftSingleton<FloatingStoreController>
{
    public Text StoreName;
    public Text pageDisplay;
    public Button nextPage;
    public Button prevPage;
    private StorePicker sPicker;
    private List<StoreItem> itemsToDisplay;
    public StoreDisplayItem selectedItem;
    public StoreCurrencyBarController Currencies;

    public List<string> currenciesInUse;
    private readonly List<StoreDisplayItem> _inventory = new List<StoreDisplayItem>();

    private string activeStoreId;
    private int currentPage = 1;
    private int pageCount = 1;
    private int itemsPerPage = 4;

    //TODO solve the confusion with what VC balances are checked player VS character
    // close also needs to fire a callback to the calling area of the code
    public void InitiateStore(GetStoreItemsResult storeResult)
    {
        if (_inventory.Count == 0)
            foreach (var child in gameObject.GetComponentsInChildren<StoreDisplayItem>())
                _inventory.Add(child);

        activeStoreId = storeResult.StoreId;
        Dictionary<string, object> eventData = new Dictionary<string, object> { { "store_name", activeStoreId } };
        PF_Bridge.LogCustomEvent(PF_Bridge.CustomEventTypes.Client_StoreVisit, eventData);

        //reset
        currenciesInUse.Clear();
        currentPage = 1;
        pageCount = Mathf.CeilToInt((float)storeResult.Store.Count / (float)itemsPerPage);
        pageDisplay.text = string.Format("{0} / {1}", currentPage, pageCount);
        foreach (var item in _inventory)
            item.ClearButton();

        itemsToDisplay = storeResult.Store;
        StoreName.text = storeResult.MarketingData != null && !string.IsNullOrEmpty(storeResult.MarketingData.DisplayName) ? storeResult.MarketingData.DisplayName : storeResult.StoreId + " (ID)";
        nextPage.interactable = pageCount > 1;
        prevPage.interactable = false;

        for (var z = 0; z < itemsToDisplay.Count && z < itemsPerPage; z++)
        {
            Sprite icon;
            GetItemIcon(itemsToDisplay[z], out icon);
            _inventory[z].Init();
            _inventory[z].SetButton(icon, itemsToDisplay[z]);

            // keep track of what currencies are being used in the store.				
            var vcPrices = itemsToDisplay[z].VirtualCurrencyPrices.Keys;
            foreach (var eachVc in vcPrices)
            {
                var index = currenciesInUse.FindIndex((key) => { return string.Equals(key, eachVc); });
                // make sure not already in the list.
                if (index < 0)
                    currenciesInUse.Add(eachVc);
            }
        }

        //hide selected
        Currencies.Init();
        gameObject.SetActive(true);
    }

    private static void GetItemIcon(StoreItem storeItem, out Sprite icon)
    {
        var catalogItem = PF_GameData.GetCatalogItemById(storeItem.ItemId);
        var iconName = "Default";
        if (catalogItem.CustomData != null && !string.Equals(catalogItem.CustomData, "null"))
        {
            try
            {
                var kvps = JsonWrapper.DeserializeObject<Dictionary<string, string>>(catalogItem.CustomData);
                kvps.TryGetValue("icon", out iconName);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        icon = GameController.Instance.iconManager.GetIconById(iconName);
    }

    public void ShowSelectedItem()
    {
    }

    public void HideSelectedItem()
    {
        DeselectButtons();
        selectedItem = null;
    }

    public void DeselectButtons()
    {
        foreach (var item in _inventory)
            if (item.catalogItem != null)
                item.Deselect();
    }

    public void CloseStore()
    {
        // get this to close down and also close the tint.	
        // get a confirmation here
        gameObject.SetActive(false);
    }

    public void ItemClicked(StoreDisplayItem item)
    {
        if (selectedItem != null)
            selectedItem.Deselect();
    }

    public void NextPage()
    {
        HideSelectedItem();
        var nextPageIdx = currentPage + 1;
        var lowerBound = itemsPerPage * currentPage;
        var upperBound = lowerBound + itemsPerPage;

        pageDisplay.text = string.Format(GlobalStrings.PAGE_NUMBER_MSG, nextPage, pageCount);

        var uiIndex = 0;
        for (var z = lowerBound; z < upperBound; z++, uiIndex++)
        {
            if (z < itemsToDisplay.Count)
            {
                Sprite icon;
                GetItemIcon(itemsToDisplay[z], out icon);
                _inventory[uiIndex].Init();
                _inventory[uiIndex].SetButton(icon, itemsToDisplay[z]);
            }
            else
            {
                _inventory[uiIndex].ClearButton();
            }
        }

        prevPage.interactable = true;
        nextPage.interactable = pageCount > nextPageIdx;
        currentPage++;
    }

    public void PrevPage()
    {
        HideSelectedItem();
        var prevPageIdx = currentPage - 1;
        var lowerBound = (itemsPerPage * prevPageIdx) - itemsPerPage;
        var upperBound = lowerBound + itemsPerPage;

        pageDisplay.text = string.Format(GlobalStrings.PAGE_NUMBER_MSG, prevPage, pageCount);

        var uiIndex = 0;
        for (var z = lowerBound; z < upperBound; z++, uiIndex++)
        {
            Sprite icon;
            GetItemIcon(itemsToDisplay[z], out icon);
            _inventory[uiIndex].Init();
            _inventory[uiIndex].SetButton(icon, itemsToDisplay[z]);
        }

        nextPage.interactable = true;
        prevPage.interactable = prevPageIdx > 1;
        currentPage--;
    }

    public void InitiatePurchase()
    {
        //NEED TO KNOW WHICH PURCHASE FLOW TO USE
        //Debug.Log ("Starting purchase of " + selectedItem.catalogItem.ItemId);
        PF_GamePlay.StartBuyStoreItem(selectedItem.catalogItem, activeStoreId);
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
        switch (method)
        {
            case PlayFabAPIMethods.MakePurchase:
                // refresh after purchase.
                if (PF_PlayerData.activeCharacter == null)
                    PF_PlayerData.GetUserAccountInfo();
                else
                    PF_PlayerData.GetCharacterInventory(PF_PlayerData.activeCharacter.characterDetails.CharacterId);
                break;
            case PlayFabAPIMethods.GetCharacterInventory:
            case PlayFabAPIMethods.GetAccountInfo:
                DialogCanvasController.RequestStore(activeStoreId);
                break;
        }
    }
}
