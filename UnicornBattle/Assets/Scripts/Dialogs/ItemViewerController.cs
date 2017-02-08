using PlayFab.ClientModels;
using PlayFab.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemViewerController : MonoBehaviour
{
    public Image CurrentIcon;
    public Text CurrentItemName;
    public Text CurrentItemDesc;
    public Text ItemCount;

    //container items
    public UnlockSliderController slider;
    public Text ContainerItemDesc;

    // WHAT ITEM OBJECT TO USE?
    //public List<
    public Button CloseButton;
    public Button NextItemButton;
    public Button PrevItemButton;

    public List<CatalogItem> pfItems = new List<CatalogItem>();
    public CatalogItem selectedItem;
    private int selectedIndex = 0;
    private UnityAction<UnlockContainerItemResult> afterUnlockCallback;
    private string currentIconId = string.Empty;

    // MAKE THIS index / List<ContainerResultItem>
    public Dictionary<int, List<ContainerResultItem>> openedBoxes = new Dictionary<int, List<ContainerResultItem>>();

    public Transform itemList;
    public Transform itemPrefab;
    public Transform containerResults;

    public Transform ItemMode;
    public Transform ContainerMode;
    public bool UnpackToPlayer = false;

    #region standardControls
    public void NextItem()
    {
        int index = selectedIndex;
        index++;

        if (index + 1 == pfItems.Count)
        {
            NextItemButton.interactable = false;
        }

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

    public void SetSelectedItem(CatalogItem item)
    {
        selectedItem = item;

        CurrentItemName.text = selectedItem.DisplayName;
        CurrentItemDesc.text = selectedItem.Description;
        ItemCount.text = string.Format("{0}/{1}", selectedIndex + 1, pfItems.Count);
        // refresh the UI

        currentIconId = "Default";
        if (!string.Equals(item.CustomData, null)) //should be !string.IsNullOrEmpty(CI.CustomData)
        {
            Dictionary<string, string> kvps = JsonWrapper.DeserializeObject<Dictionary<string, string>>(item.CustomData);
            kvps.TryGetValue("icon", out currentIconId);
        }
        var icon = GameController.Instance.iconManager.GetIconById(currentIconId);
        CurrentIcon.overrideSprite = icon;

        if (openedBoxes.ContainsKey(selectedIndex))
        {
            // this container has been opened, show the items...
            if (selectedItem.Container != null && selectedItem.Container.ItemContents == null && selectedItem.Container.ResultTableContents == null && selectedItem.Container.VirtualCurrencyContents == null)
            {
                //is a bundle, dont change the icon.

            }
            else
            {
                CurrentIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(currentIconId + "_Open");
            }
            EnableContainerMode(true);
        }
        else
        {
            // test bundles here....
            // if bundle, we need to show the contents, but not remove it from the list, as it will be unpacked and added automatically
            if (selectedItem.Bundle != null && (selectedItem.Bundle.BundledItems != null || selectedItem.Bundle.BundledResultTables != null || selectedItem.Bundle.BundledVirtualCurrencies != null))
            {
                List<ContainerResultItem> items = new List<ContainerResultItem>();

                if (selectedItem.Bundle.BundledItems != null && selectedItem.Bundle.BundledItems.Count > 0)
                {
                    foreach (var award in selectedItem.Bundle.BundledItems)
                    {
                        string awardIcon;
                        var catalogItem = PF_GameData.GetCatalogItemById(award);
                        var kvps = JsonWrapper.DeserializeObject<Dictionary<string, string>>(catalogItem.CustomData);
                        kvps.TryGetValue("icon", out awardIcon);

                        items.Add(new ContainerResultItem()
                        {
                            displayIcon = GameController.Instance.iconManager.GetIconById(awardIcon),
                            displayName = catalogItem.DisplayName
                        });
                    }
                }

                if (selectedItem.Bundle.BundledResultTables != null && selectedItem.Bundle.BundledResultTables.Count > 0)
                {
                    foreach (var award in selectedItem.Bundle.BundledResultTables)
                    {
                        items.Add(new ContainerResultItem()
                        {
                            displayIcon = GameController.Instance.iconManager.GetIconById("DropTable"),
                            displayName = string.Format("Drop Table: {0}", award)
                        });
                    }
                }

                if (selectedItem.Bundle.BundledVirtualCurrencies != null && selectedItem.Bundle.BundledVirtualCurrencies.Count > 0)
                {
                    foreach (var award in selectedItem.Bundle.BundledVirtualCurrencies)
                    {
                        items.Add(new ContainerResultItem()
                        {
                            displayIcon = GameController.Instance.iconManager.GetIconById(award.Key),
                            displayName = string.Format("{1} Award: {0:n0}", award.Value, award.Key)
                        });
                    }
                }

                if (items.Count > 0)
                {
                    openedBoxes.Add(selectedIndex, items);

                    EnableContainerMode(true);

                    // dont fall through the rest of the logic.
                    return;
                }
            }

            if (selectedItem.Container != null && selectedItem.Container.ItemContents == null && selectedItem.Container.ResultTableContents == null && selectedItem.Container.VirtualCurrencyContents == null)
            {
                DisableContainerMode();
            }
            else
            {
                Debug.Log("This is a container");
                EnableContainerMode();
            }
        }
    }

    public void InitiateViewer(List<string> items, bool unpackToPlayer) // need a new flag to determine if we should unpack to a player or a character
    {
        UnpackToPlayer = unpackToPlayer;

        pfItems.Clear();
        openedBoxes.Clear();

        if (PF_GameData.catalogItems == null || PF_GameData.catalogItems.Count == 0)
            return;

        foreach (var item in items)
        {
            var catalogItem = PF_GameData.GetCatalogItemById(item);
            if (catalogItem != null)
                pfItems.Add(catalogItem);
        }

        PrevItemButton.interactable = pfItems.Count != 1;

        //select the first in the list
        SetSelectedItem(pfItems[0]);
        selectedIndex = 0;
        gameObject.SetActive(true);
    }

    public void CloseViewer()
    {
        PF_PlayerData.GetUserInventory();
        PF_PlayerData.GetCharacterInventory(PF_PlayerData.activeCharacter.characterDetails.CharacterId);

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

    public void AfterUnlock(UnlockContainerItemResult result)
    {
        // unlocking a container will automatically add the returned items, this ensures that items are not added twice.
        PF_GamePlay.QuestProgress.ItemsFound.Remove(selectedItem.ItemId);

        // build our list for displaying the container results
        List<ContainerResultItem> items = new List<ContainerResultItem>();

        foreach (var award in result.GrantedItems)
        {
            string awardIcon;
            var catalogItem = PF_GameData.GetCatalogItemById(award.ItemId);
            if (catalogItem != null)
            {
                Dictionary<string, string> kvps = JsonWrapper.DeserializeObject<Dictionary<string, string>>(catalogItem.CustomData);
                kvps.TryGetValue("icon", out awardIcon);

                items.Add(new ContainerResultItem()
                {
                    displayIcon = GameController.Instance.iconManager.GetIconById(awardIcon),
                    displayName = award.DisplayName
                });
            }
        }

        if (result.VirtualCurrency != null)
        {
            foreach (var award in result.VirtualCurrency)
            {
                var friendlyName = string.Empty;
                if (award.Key == "AU")
                {
                    friendlyName = "Gold";
                }
                else if (award.Key == "HT")
                {
                    friendlyName = "Lives";
                }
                else if (award.Key == "GM")
                {
                    friendlyName = "Gems";
                }

                items.Add(new ContainerResultItem()
                {
                    displayIcon = GameController.Instance.iconManager.GetIconById(award.Key),
                    displayName = string.Format("   {0} {1}", award.Value, friendlyName)
                });
            }
        }
        else
        {
            //TODO find out if this is OK to remove:
            //            CatalogItem catRef = PF_GameData.catalogItems.Find( (i) => {return i.ItemId == result. selectedItem.ItemId; });
            //            if(catRef != null && catRef.Container.VirtualCurrencyContents.Count > 0)
            //            {
            //                
            //                foreach(var vc in catRef.Container.VirtualCurrencyContents)
            //                {
            //                    string friendlyName = string.Empty;
            //                    if(vc.Key == "AU")
            //                    {
            //                        friendlyName = "Gold";
            //
            //                    } else if(vc.Key == "HT")
            //                    {
            //                        friendlyName = "Lives";
            //                    }
            //                    else if(vc.Key == "GM")
            //                    {
            //                        friendlyName = "Gems";
            //                    }
            //
            //                    items.Add(new ContainerResultItem(){ 
            //                        displayIcon = GameController.Instance.iconManager.GetIconById(vc.Key),
            //                        displayName = string.Format("   {1} Award: {0}", vc.Value, friendlyName ) 
            //                    });
            //                }
            //            }
        }

        CurrentIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(currentIconId + "_Open");
        openedBoxes.Add(selectedIndex, items);
        EnableUnlockedItemsView(items);

        DialogCanvasController.RequestInventoryPrompt();
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
