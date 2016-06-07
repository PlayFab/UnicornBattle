using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using PlayFab;
using PlayFab.ClientModels;


public class ItemViewerContainerController : MonoBehaviour {
	public UnlockSliderController slider;
	public ItemViewerController controller;
	
	public Transform itemContainer;
	public Transform itemList;
	public Transform itemPrefab;
	
	
		
	public void Init()
	{
		
		slider.SetupSlider();
	}
		
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void EnableUnlockedItemsView(List<ItemInstance> unlockedItems = null)
	{
//		this.ItemsTransform.gameObject.SetActive(true);
//		this.ItemDescription.gameObject.SetActive(false);
		
	}
	
	void DisableUnlockedItemsView()
	{
//		this.ItemsTransform.gameObject.SetActive(false);
//		this.ItemDescription.gameObject.SetActive(true);
	}
}
