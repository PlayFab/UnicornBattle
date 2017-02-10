using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FloatingInventoryController : MonoBehaviour
{
    public Text StoreName;
    public Text pageDisplay;
    public Button nextPage;
    public Button prevPage;

    public List<string> currenciesInUse;
    public DialogCanvasController.InventoryFilters activeFilter;

    public Transform ItemGrid;
    public List<InventoryDisplayItem> inventory = new List<InventoryDisplayItem>();

    private int _currentPage = 1;
    private int _pageCount = 1;
    private int _itemsPerPage = 12;

    // private StorePicker sPicker;
    private Dictionary<string, InventoryCategory> _itemsToDisplay = new Dictionary<string, InventoryCategory>();
    private Action<string> _callbackAfterUse;

    public FloatingInventory_SelectedItem selectedItem;
    public InventoryCurrencyBarController Currencies;

    //TODO add in filter for things like:
    // only items on display
    // only containers
    // all items
    public void Init(Action<string> callback = null, DialogCanvasController.InventoryFilters filter = DialogCanvasController.InventoryFilters.AllItems)
    {
        activeFilter = filter;

        if (callback != null)
            _callbackAfterUse = callback;

        // only displaying the main currencies (Gold & Gems) for now
        currenciesInUse.Clear();
        currenciesInUse.Add(GlobalStrings.GOLD_CURRENCY);
        currenciesInUse.Add(GlobalStrings.GEM_CURRENCY);

        if (PF_PlayerData.inventoryByCategory != null && PF_PlayerData.virtualCurrency != null)
        {
            _itemsToDisplay = PF_PlayerData.inventoryByCategory;
            Currencies.Init(PF_PlayerData.virtualCurrency);
        }
        else
        {
            ResetItemTiles();
            return;
        }

        //reset
        ResetItemTiles();

        string filterName;
        GlobalStrings.INV_FILTER_DISPLAY_NAMES.TryGetValue(filter, out filterName);
        StoreName.text = string.Format(GlobalStrings.INV_WINDOW_TITLE, filterName);

        nextPage.interactable = (_pageCount > 1);
        prevPage.interactable = false;

        int count = 0;
        foreach (var kvp in _itemsToDisplay)
        {
            if (count >= _itemsPerPage)
                break;

            bool addItem = false;
            if (filter == DialogCanvasController.InventoryFilters.Containers)
            {
                if (string.Equals(kvp.Value.catalogRef.ItemClass, filter.ToString()))
                    addItem = true;
                else
                    continue;
            }
            else if (filter == DialogCanvasController.InventoryFilters.Keys)
            {
                if (string.Equals(kvp.Value.catalogRef.ItemClass, filter.ToString()))
                    addItem = true;
                else
                    continue;
            }
            else if (filter == DialogCanvasController.InventoryFilters.UsableInCombat)
            {
                if (string.Equals(kvp.Value.catalogRef.ItemClass, filter.ToString()))
                    addItem = true;
                else
                    continue;
            }
            else if (filter == DialogCanvasController.InventoryFilters.AllItems)
            {
                addItem = true;
            }

            if (addItem)
            {
                inventory[count].Init();
                inventory[count].SetButton(kvp.Value.icon, kvp.Value);
                count++;
            }
        }
        gameObject.SetActive(true);
    }


    public void ShowSelectedItem()
    {
        selectedItem.gameObject.SetActive(true);
    }

    public void HideSelectedItem()
    {
        DeselectButtons();
        selectedItem.itemData = null;
        selectedItem.gameObject.SetActive(false);
    }

    public void DeselectButtons()
    {
        foreach (var item in inventory)
            item.Deselect();
    }

    public void CloseInventory()
    {
        // get to close down and also close the tint.	
        // get a confirmation here
        gameObject.SetActive(false);
    }

    public void ItemClicked(InventoryDisplayItem item)
    {
        if (selectedItem.itemData != null)
            selectedItem.itemData.Deselect();

        selectedItem.RefreshSelected(item);
        ShowSelectedItem();
    }


    // start here after lunch! -- dict changes
    public void NextPage()
    {
        HideSelectedItem();
        var nextPageIdx = _currentPage + 1;
        var lowerBound = _itemsPerPage * _currentPage;
        var upperBound = lowerBound + _itemsPerPage;

        pageDisplay.text = string.Format(GlobalStrings.PAGE_NUMBER_MSG, nextPage, _pageCount);

        var uiIndex = 0;
        for (var z = lowerBound; z < upperBound; z++, uiIndex++)
        {
            if (z < _itemsToDisplay.Count)
            {
                var kvp = _itemsToDisplay.ElementAt(z);

                inventory[uiIndex].Init();
                inventory[uiIndex].SetButton(kvp.Value.icon, kvp.Value);
            }
            else
            {
                inventory[uiIndex].ClearButton();
            }
        }

        prevPage.interactable = true;
        nextPage.interactable = _pageCount > nextPageIdx;

        _currentPage++;
    }

    public void PrevPage()
    {
        HideSelectedItem();
        var prevPageIdx = _currentPage - 1;
        var lowerBound = (_itemsPerPage * prevPageIdx) - _itemsPerPage;
        var upperBound = lowerBound + _itemsPerPage;

        pageDisplay.text = string.Format(GlobalStrings.PAGE_NUMBER_MSG, prevPage, _pageCount);

        var uiIndex = 0;
        for (var z = lowerBound; z < upperBound; z++, uiIndex++)
        {
            if (z < _itemsToDisplay.Count)
            {
                var kvp = _itemsToDisplay.ElementAt(z);

                inventory[uiIndex].Init();
                inventory[uiIndex].SetButton(kvp.Value.icon, kvp.Value);
            }
            else
            {
                inventory[uiIndex].ClearButton();
            }
        }

        nextPage.interactable = true;
        prevPage.interactable = prevPageIdx > 1;

        _currentPage--;
    }

    public void UseItem() // not possible outside of battle
    {
        if (_callbackAfterUse != null)
        {
            _callbackAfterUse(selectedItem.itemData.category.catalogRef.ItemId);
            CloseInventory();
        }
    }

    public void UnlockContainer()
    {
        DialogCanvasController.RequestItemViewer(new List<string> { selectedItem.itemData.category.catalogRef.ItemId });
    }

    public void RefreshInventory()
    {
        PF_PlayerData.GetUserInventory();
        DialogCanvasController.RequestInventoryPrompt(null, activeFilter);
    }

    void ResetItemTiles()
    {
        _currentPage = 1;
        _pageCount = Mathf.CeilToInt((float)_itemsToDisplay.Count / (float)_itemsPerPage) > 0 ? Mathf.CeilToInt((float)_itemsToDisplay.Count / (float)_itemsPerPage) : 1;
        pageDisplay.text = string.Format("{0} / {1}", _currentPage, _pageCount);
        HideSelectedItem();

        foreach (var item in inventory)
            item.ClearButton();
    }
}
