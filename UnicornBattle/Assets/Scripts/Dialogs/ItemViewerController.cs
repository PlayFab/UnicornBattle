using System.Collections.Generic;
using UnicornBattle.Managers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{

    public class ItemViewerController : MonoBehaviour
    {
        public Image CurrentIcon;
        public Text CurrentItemName;
        public Text CurrentItemDesc;
        public Text ItemCount;
        public UnlockSliderController slider;
        public Text ContainerItemDesc;
        public Button CloseButton;
        public Button NextItemButton;
        public Button PrevItemButton;

        public List<UBCatalogItem> pfItems = new List<UBCatalogItem>();
        public UBCatalogItem selectedItem;
        private int selectedIndex = 0;
        private string currentIconId = string.Empty;

        // MAKE THIS index / List<ContainerResultItem>
        public Dictionary<int, List<ContainerResultItem>> openedBoxes = new Dictionary<int, List<ContainerResultItem>>();

        public Transform itemList;
        public Transform itemPrefab;
        public Transform containerResults;
        public Transform ItemMode;
        public Transform ContainerMode;

        #region standardControls
        public void NextItem()
        {
            var index = selectedIndex;
            index++;

            if (index + 1 == pfItems.Count)
                NextItemButton.interactable = false;

            PrevItemButton.interactable = true;

            selectedIndex = index;
            SetSelectedItem(pfItems[index]);
        }

        public void PrevItem()
        {
            int index = selectedIndex;
            index--;

            if (index == 0)
            {
                PrevItemButton.interactable = false;
            }

            NextItemButton.interactable = true;

            selectedIndex = index;
            SetSelectedItem(pfItems[index]);
        }

        public void SetSelectedItem(UBCatalogItem item)
        {
            selectedItem = item;

            CurrentItemName.text = selectedItem.DisplayName;
            CurrentItemDesc.text = selectedItem.Description;
            ItemCount.text = string.Format("{0}/{1}", selectedIndex + 1, pfItems.Count);
            // refresh the UI

            var l_inventoryMgr = MainManager.Instance.getInventoryManager();
            if (null == l_inventoryMgr) return;

            currentIconId = l_inventoryMgr.GetIconByItemById(item.ItemId);
            var icon = GameController.Instance.iconManager.GetIconById(currentIconId, IconManager.IconTypes.Item);
            CurrentIcon.overrideSprite = icon;

            if (openedBoxes.ContainsKey(selectedIndex))
            {
                // this container has been opened, show the items...
                if (l_inventoryMgr.IsContainer(selectedItem.ItemId))
                    CurrentIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(currentIconId + "_Open", IconManager.IconTypes.Item);
                EnableContainerMode(true);
                return;
            }

            // test bundles here....
            // if bundle, we need to show the contents, but not remove it from the list, as it will be unpacked and added automatically
            if (selectedItem.Bundle != null && (selectedItem.Bundle.BundledItems != null || selectedItem.Bundle.BundledResultTables != null || selectedItem.Bundle.BundledVirtualCurrencies != null))
            {
                var bundleItems = new List<ContainerResultItem>();

                if (selectedItem.Bundle.BundledItems != null && selectedItem.Bundle.BundledItems.Count > 0)
                {
                    foreach (var award in selectedItem.Bundle.BundledItems)
                    {
                        var catalogItem = l_inventoryMgr.GetCatalogItemById(award);
                        var awardIcon = l_inventoryMgr.GetIconByItemById(award);
                        bundleItems.Add(new ContainerResultItem
                        {
                            displayIcon = GameController.Instance.iconManager.GetIconById(awardIcon, IconManager.IconTypes.Item),
                                displayName = catalogItem.DisplayName
                        });
                    }
                }

                if (selectedItem.Bundle.BundledResultTables != null && selectedItem.Bundle.BundledResultTables.Count > 0)
                {
                    foreach (var award in selectedItem.Bundle.BundledResultTables)
                    {
                        bundleItems.Add(new ContainerResultItem
                        {
                            displayIcon = GameController.Instance.iconManager.GetIconById("DropTable", IconManager.IconTypes.Misc),
                                displayName = string.Format("Drop Table: {0}", award)
                        });
                    }
                }

                if (selectedItem.Bundle.BundledVirtualCurrencies != null && selectedItem.Bundle.BundledVirtualCurrencies.Count > 0)
                {
                    foreach (var award in selectedItem.Bundle.BundledVirtualCurrencies)
                    {
                        bundleItems.Add(new ContainerResultItem
                        {
                            displayIcon = GameController.Instance.iconManager.GetIconById(award.Key, IconManager.IconTypes.Item),
                                displayName = string.Format("{1} Award: {0:n0}", award.Value, award.Key)
                        });
                    }
                }

                if (bundleItems.Count > 0)
                {
                    openedBoxes.Add(selectedIndex, bundleItems);
                    EnableContainerMode(true);
                    // don't fall through the rest of the logic.
                    return;
                }
            }

            if (l_inventoryMgr.IsContainer(selectedItem.ItemId))
                EnableContainerMode();
            else
                DisableContainerMode();
        }

        public void InitiateViewer(List<string> items) // need a new flag to determine if we should unpack to a player or a character
        {
            pfItems.Clear();
            openedBoxes.Clear();

            var l_inventoryMgr = MainManager.Instance.getInventoryManager();
            if (null == l_inventoryMgr) return;

            // if (PF_GameData.catalogItems == null || PF_GameData.catalogItems.Count == 0)
            //     return;

            foreach (var item in items)
            {
                var catalogItem = l_inventoryMgr.GetCatalogItemById(item);
                if (catalogItem != null)
                    pfItems.Add(catalogItem);
            }

            PrevItemButton.interactable = false;
            NextItemButton.interactable = pfItems.Count != 1;

            //select the first in the list
            SetSelectedItem(pfItems[0]);
            selectedIndex = 0;
            gameObject.SetActive(true);
        }

        public void CloseViewer()
        {
            var l_inventoryMgr = MainManager.Instance.getInventoryManager();
            if (null == l_inventoryMgr)
            {
                gameObject.SetActive(false);
                return;
            }

            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetUserInventory);
            l_inventoryMgr.Refresh(false,
                (string success) =>
                {
                    PF_Bridge.RaiseCallbackSuccess(success, PlayFabAPIMethods.GetUserInventory);
                },
                (string error) =>
                {
                    PF_Bridge.RaiseCallbackError(error, PlayFabAPIMethods.Generic);
                }
            );
            gameObject.SetActive(false);
        }
        #endregion

        public void EnableContainerMode(bool isAlreadyOpen = false)
        {
            ContainerMode.gameObject.SetActive(true);
            ItemMode.gameObject.SetActive(false);

            if (isAlreadyOpen)
            {
                ClearContainerItems();
                EnableUnlockedItemsView(openedBoxes[selectedIndex]);
            }
            else
            {
                DisableUnlockedItemsView();

                ContainerItemDesc.text = selectedItem.Description;
                slider.SetupSlider(AfterUnlock);
                slider.gameObject.SetActive(true);
            }
        }

        public void DisableContainerMode()
        {
            ContainerMode.gameObject.SetActive(false);
            ItemMode.gameObject.SetActive(true);

            //CurrentItemDesc.text = selectedItem.Description;
        }

        public void AfterUnlock(PlayFab.ClientModels.UnlockContainerItemResult result)
        {
            Debug.Log("|||" + GetType().ToString() + ".AfterUnlock() was called.");
            if (result == null)
            {
                Debug.LogError("|||" + GetType().ToString() + ".AfterUnlock() : result is null");
                return;
            }

            // unlocking a container will automatically add the returned items, this ensures that items are not added twice.

            if (null != GameController.Instance && null != selectedItem)
                GameController.Instance.QuestProgress.RemoveItemFound(selectedItem.ItemId);

            Debug.Log("|||" + GetType().ToString() + ".RemoveItemFound() was called.");
            var l_inventoryMgr = MainManager.Instance.getInventoryManager();
            if (null == l_inventoryMgr) return;

            var l_iconMgr = MainManager.Instance.getIconManager();
            if (null == l_iconMgr) return;

            l_inventoryMgr.Refresh(false,
                (s) =>
                {
                    Debug.Log("|||" + GetType().ToString() + ".Refresh() was called.");

                    // build our list for displaying the container results
                    List<ContainerResultItem> items = new List<ContainerResultItem>();

                    foreach (var award in result.GrantedItems)
                    {
                        var awardIcon = l_inventoryMgr.GetIconByItemById(award.ItemId);
                        items.Add(new ContainerResultItem
                        {
                            displayIcon = l_iconMgr.GetIconById(awardIcon, IconManager.IconTypes.Item),
                                displayName = award.DisplayName
                        });
                    }

                    if (result.VirtualCurrency != null)
                    {
                        foreach (var award in result.VirtualCurrency)
                        {
                            string friendlyName;
                            switch (award.Key)
                            {
                                case GlobalStrings.GOLD_CURRENCY:
                                    friendlyName = "Gold";
                                    break;
                                case GlobalStrings.HEART_CURRENCY:
                                    friendlyName = "Lives";
                                    break;
                                case GlobalStrings.GEM_CURRENCY:
                                    friendlyName = "Gems";
                                    break;
                                default:
                                    friendlyName = "";
                                    break;
                            }

                            items.Add(new ContainerResultItem
                            {
                                displayIcon = l_iconMgr.GetIconById(award.Key, IconManager.IconTypes.Item),
                                    displayName = award.Value + " " + friendlyName
                            });
                        }
                    }

                    Debug.Log("|||" + GetType().ToString() + " finished loops");
                    CurrentIcon.overrideSprite = l_iconMgr.GetIconById(currentIconId + "_Open", IconManager.IconTypes.Item);
                    openedBoxes.Add(selectedIndex, items);

                    Debug.Log("|||" + GetType().ToString() + " openedBoxes.Add(selectedIndex, items)");
                    EnableUnlockedItemsView(items);

                    Debug.Log("|||" + GetType().ToString() + " EnableUnlockedItemsView(items)");
                    DialogCanvasController.RequestInventoryPrompt();

                    Debug.Log("|||" + GetType().ToString() + " DialogCanvasController.RequestInventoryPrompt()");
                },
                null);
        }

        void EnableUnlockedItemsView(List<ContainerResultItem> unlockedItems = null)
        {
            ContainerItemDesc.gameObject.SetActive(false);
            slider.gameObject.SetActive(false);
            containerResults.gameObject.SetActive(true);

            foreach (var item in unlockedItems)
            {
                var slot = Instantiate(itemPrefab);
                var cir = slot.GetComponent<ContainerItemResult>();
                cir.Icon.overrideSprite = item.displayIcon;
                cir.ItemName.text = item.displayName;
                slot.SetParent(itemList, false);
            }
        }

        void DisableUnlockedItemsView()
        {
            ClearContainerItems();

            ContainerItemDesc.gameObject.SetActive(true);
            containerResults.gameObject.SetActive(false);
        }

        void ClearContainerItems()
        {
            ContainerItemResult[] children = itemList.GetComponentsInChildren<ContainerItemResult>(true);
            for (int z = 0; z < children.Length; z++)
            {
                if (children[z].gameObject != itemList.gameObject)
                {
                    DestroyImmediate(children[z].gameObject);
                }
            }
        }
    }

    public class ContainerResultItem
    {
        public Sprite displayIcon;
        public string displayName;
    }
}