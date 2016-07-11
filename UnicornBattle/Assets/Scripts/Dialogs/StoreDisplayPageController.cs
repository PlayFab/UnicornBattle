using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class StoreDisplayPageController : MonoBehaviour {
	public Button nextPage;
	public Button prevPage;
	public Text displayPageCount;
	
	public int currentPage = 1;
	public int pageCount = 1;
	public int itemsPerPage = 8;
	
	public Transform storeItemPrefab;
	public Transform gridView;
	
	public StoreDisplayController displayController;
	private StorePicker sPicker;
	private List<TEST_StoreItem> itemsToDisplay;

	public void LoadStore(List<TEST_StoreItem> items)
	{
		this.itemsToDisplay = items;
		//for(int z = 0; z < this.gridView.transform.childCount; z++)
		foreach(Transform each in this.gridView)
		{
			GameObject.Destroy(each.gameObject);
		}
		pageCount = Mathf.CeilToInt((float)items.Count / (float)this.itemsPerPage);
		
		if(pageCount > 1)
		{
			nextPage.interactable = true;
		}
		else
		{
			nextPage.interactable = false;
		}
		
		prevPage.interactable = false;
		this.currentPage = 1;
        displayPageCount.text = string.Format(GlobalStrings.PAGE_NUMBER_MSG, currentPage, pageCount);
		Debug.Log(string.Format("Store loaded, {0} items.", items.Count));
		
		
		int loopBounds = this.itemsPerPage < items.Count ? this.itemsPerPage : items.Count;
		for(int z = 0; z < loopBounds; z++)
		{
			Transform temp = Instantiate(this.storeItemPrefab);
			temp.GetComponent<StoreItemEx>().itemData = items[z];
			temp.SetParent(gridView, false);
			
		}
		
	}
	
	public void NextPage()
	{
		int nextPage = currentPage+1;
		
        displayPageCount.text = string.Format(GlobalStrings.PAGE_NUMBER_MSG, nextPage, pageCount);
		
		Debug.Log("Next Page!");
		foreach(Transform each in this.gridView)
		{
			GameObject.Destroy(each.gameObject);
		}
		
		int loopBounds = nextPage * this.itemsPerPage;
		
		//this.itemsPerPage < this.itemsToDisplay.Count ? this.itemsPerPage : this.itemsToDisplay.Count;
		for(int z = currentPage * this.itemsPerPage; z < loopBounds; z++)
		{
			if(z > this.itemsToDisplay.Count-1)
				break;
				
			Transform temp = Instantiate(this.storeItemPrefab);
			temp.GetComponent<StoreItemEx>().itemData = this.itemsToDisplay[z];
			temp.SetParent(gridView, false);
			
		}
		
		this.prevPage.interactable = true;
		
		if(pageCount > nextPage)
		{
			this.nextPage.interactable = true;
		}
		else
		{
			this.nextPage.interactable = false;
		}
		currentPage++;
		this.displayController.HideSelected();
	}
	
	public void PrevPage()
	{
		
		int nextPage = currentPage-1;
		
        displayPageCount.text = string.Format(GlobalStrings.PAGE_NUMBER_MSG, nextPage, pageCount);
		
		Debug.Log("Prev Page!");
		foreach(Transform each in this.gridView)
		{
			GameObject.Destroy(each.gameObject);
		}
		
		int loopBounds = nextPage * this.itemsPerPage;
		
		//this.itemsPerPage < this.itemsToDisplay.Count ? this.itemsPerPage : this.itemsToDisplay.Count;
		for(int z = --nextPage * this.itemsPerPage; z < loopBounds; z++)
		{
			if(z > this.itemsToDisplay.Count-1)
				break;
			
			Transform temp = Instantiate(this.storeItemPrefab);
			temp.GetComponent<StoreItemEx>().itemData = this.itemsToDisplay[z];
			temp.SetParent(gridView, false);
			
		}
		
		this.nextPage.interactable = true;
		
		if(nextPage > 0)
		{
			this.prevPage.interactable = true;
		}
		else
		{
			this.prevPage.interactable = false;
		}
		currentPage--;
		this.displayController.HideSelected();
	}
	

}
