using System;
using System.Collections.Generic;
using System.Linq;
using UnicornBattle.Managers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
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
        private Dictionary<string, UBInventoryCategory> _itemsToDisplay = new Dictionary<string, UBInventoryCategory>();
        private Action<string> _callbackAfterUse;

        public FloatingInventory_SelectedItem m_selectedItem;
        public InventoryCurrencyBarController m_currencyBarController;

        private void OnEnable()
        {
            TelemetryManager.RecordScreenViewed(TelemetryScreenId.Inventory);
        }

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

            m_currencyBarController.Init();
            //reset
            ResetItemTiles();

            var l_inventoryMgr = MainManager.Instance.getInventoryManager();
            if (null == l_inventoryMgr) return;
            _itemsToDisplay = l_inventoryMgr.GetAllCategories();

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
            m_selectedItem.gameObject.SetActive(true);
        }

        public void HideSelectedItem()
        {
            DeselectButtons();
            m_selectedItem.itemData = null;
            m_selectedItem.gameObject.SetActive(false);
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
            if (m_selectedItem.itemData != null)
                m_selectedItem.itemData.Deselect();

            m_selectedItem.RefreshSelected(item);
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
                _callbackAfterUse(m_selectedItem.itemData.category.catalogRef.ItemId);
                CloseInventory();
            }
        }

        public void UnlockContainer()
        {
            DialogCanvasController.RequestItemViewer(new List<string> { m_selectedItem.itemData.category.catalogRef.ItemId });
        }

        public void RefreshInventory()
        {
            var l_inventoryMgr = MainManager.Instance.getInventoryManager();
            if (null == l_inventoryMgr) return;

            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetUserInventory);
            l_inventoryMgr.Refresh(true,
                (string success) => { PF_Bridge.RaiseCallbackSuccess(success, PlayFabAPIMethods.GetUserInventory); },
                (string error) => { PF_Bridge.RaiseCallbackError(error, PlayFabAPIMethods.GetUserInventory); }
            );

            DialogCanvasController.RequestInventoryPrompt(null, activeFilter);
        }

        void ResetItemTiles()
        {
            _currentPage = 1;
            _pageCount = Mathf.CeilToInt((float) _itemsToDisplay.Count / (float) _itemsPerPage) > 0 ? Mathf.CeilToInt((float) _itemsToDisplay.Count / (float) _itemsPerPage) : 1;
            pageDisplay.text = string.Format("{0} / {1}", _currentPage, _pageCount);
            HideSelectedItem();

            foreach (var item in inventory)
                item.ClearButton();
        }
    }

}