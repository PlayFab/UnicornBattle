using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreDisplayPageController : MonoBehaviour
{
    public Button nextPage;
    public Button prevPage;
    public Text displayPageCount;
    public Transform storeItemPrefab;
    public Transform gridView;
    public StoreDisplayController displayController;

    // private StorePicker sPicker;
    private List<TEST_StoreItem> itemsToDisplay;
    private int _currentPage = 1;
    private int _pageCount = 1;
    private const int ItemsPerPage = 8;

    public void LoadStore(List<TEST_StoreItem> items)
    {
        itemsToDisplay = items;

        foreach (Transform each in gridView)
            Destroy(each.gameObject);
        _pageCount = Mathf.CeilToInt((float)items.Count / (float)ItemsPerPage);
        prevPage.interactable = false;
        nextPage.interactable = _pageCount > 1;

        _currentPage = 1;
        displayPageCount.text = string.Format(GlobalStrings.PAGE_NUMBER_MSG, _currentPage, _pageCount);

        var loopBounds = ItemsPerPage < items.Count ? ItemsPerPage : items.Count;
        for (var z = 0; z < loopBounds; z++)
        {
            var temp = Instantiate(storeItemPrefab);
            temp.GetComponent<StoreItemEx>().itemData = items[z];
            temp.SetParent(gridView, false);
        }
    }

    public void NextPage()
    {
        var nextPageIdx = _currentPage + 1;
        displayPageCount.text = string.Format(GlobalStrings.PAGE_NUMBER_MSG, nextPage, _pageCount);

        foreach (Transform each in gridView)
            Destroy(each.gameObject);

        var loopBounds = nextPageIdx * ItemsPerPage;
        for (var z = _currentPage * ItemsPerPage; z < loopBounds; z++)
        {
            if (z > itemsToDisplay.Count - 1)
                break;

            var temp = Instantiate(storeItemPrefab);
            temp.GetComponent<StoreItemEx>().itemData = itemsToDisplay[z];
            temp.SetParent(gridView, false);
        }

        prevPage.interactable = true;
        nextPage.interactable = _pageCount > nextPageIdx;

        _currentPage++;
        displayController.HideSelected();
    }

    public void PrevPage()
    {
        var nextPageIdx = _currentPage - 1;
        displayPageCount.text = string.Format(GlobalStrings.PAGE_NUMBER_MSG, nextPage, _pageCount);

        foreach (Transform each in gridView)
            Destroy(each.gameObject);

        var loopBounds = nextPageIdx * ItemsPerPage;

        //itemsPerPage < itemsToDisplay.Count ? itemsPerPage : itemsToDisplay.Count;
        for (var z = --nextPageIdx * ItemsPerPage; z < loopBounds; z++)
        {
            if (z > itemsToDisplay.Count - 1)
                break;

            var temp = Instantiate(storeItemPrefab);
            temp.GetComponent<StoreItemEx>().itemData = itemsToDisplay[z];
            temp.SetParent(gridView, false);
        }

        prevPage.interactable = nextPageIdx > 0;
        nextPage.interactable = true;

        _currentPage--;
        displayController.HideSelected();
    }
}
