using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StorePicker : MonoBehaviour
{
    public List<UnicornStore> stores;
    public StoreDisplayController storeDisplay;
    public StorePickerItem selectedStore;

    public Transform tabMenu;

    void Start()
    {
        storeDisplay.pageController.LoadStore(stores[selectedStore.storeIndex].items);
    }

    public void StorePickerItemClicked(StorePickerItem item)
    {
        if (selectedStore == item)
            return;

        ClearSelected();
        storeDisplay.HideSelected();
        selectedStore = item;
        item.GetComponent<Image>().color = Color.green;
        storeDisplay.pageController.LoadStore(stores[item.storeIndex].items);
    }

    public void ClearSelected()
    {
        selectedStore = null;
        foreach (Transform each in tabMenu)
            each.GetComponent<Image>().color = Color.white;
    }
}


#region helper_classes
[Serializable]
public class UnicornStore
{
    public string storeName;
    //public List<CatalogItem> items; //use this one for actual PF items
    public List<TEST_StoreItem> items; // use this one for UI testing
}

[Serializable]
public class TEST_StoreItem
{
    public string displayName;
    public string displayText;
    public Color iconColor = Color.gray;
    public Color textColor = Color.black;
}
#endregion
