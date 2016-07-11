using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class InventoryDisplayItem : MonoBehaviour {
	public Image bg;
	public Image image;
	public Button btn;
	public FloatingInventoryController controller;
	public InventoryCategory category;

	public void Init()
	{
		this.btn.onClick.AddListener( ()=> 
		                             { 
			this.controller.ItemClicked(this); 
			this.bg.color = Color.green;
			this.image.color = new Color(255,255,255,255);
		});
	}
	
	
	public void SetButton(Sprite icon, InventoryCategory cItem)
	{
		ActivateButton();
		this.image.overrideSprite = icon;
		this.category = cItem;
	}
	
	public void Deselect()
	{
		this.bg.color = Color.white;
		this.image.color = new Color(255,255,255,190);
	}
	
	
	public void ClearButton()
	{
		this.image.overrideSprite = null;
		this.image.color = Color.clear;
		this.category = null;
		this.btn.interactable = false;
		this.bg.color = Color.white;
	}
	
	public void ActivateButton()
	{
		this.image.color = new Color(255,255,255,190);
		this.image.overrideSprite = null;
		this.image.color = Color.white;
		this.category = null;
		this.btn.interactable = true;
	}
}
