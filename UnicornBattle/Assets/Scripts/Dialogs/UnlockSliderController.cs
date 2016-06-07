using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class UnlockSliderController : MonoBehaviour, IPointerUpHandler {
	public Slider uiSlider;
	public Text sliderMessage;
	public Button storeButton;
	
	//public string keyRequired = "Unlocked";
	
	private float dragStart; 
	private float slideDelay = 0.333f;
	private float resistance = 0.05f;

	private Coroutine slideBack;
	private UnityAction<UnlockContainerItemResult> afterUnlock;
	
	public Image endIcon;
	public Image handle;
	
	public ItemViewerController controller;
	public Transform UnpackedItemPrefab;
	public Transform ItemGroup;
	public Text ItemDescription;
	//public Transform ItemsTransform;
	
	void OnEnable()
	{
        GameplayController.OnGameplayEvent += OnGameplayEventReceived;
		uiSlider.onValueChanged.AddListener((v) => { OnSliderChanged(v); });
		uiSlider.value = uiSlider.minValue;
		
	}
	
	void OnDisable()
	{
        GameplayController.OnGameplayEvent -= OnGameplayEventReceived;
		
	}
	
    void OnGameplayEventReceived(string message,  PF_GamePlay.GameplayEventTypes type )
	{
			
	}
	
	
	public void OnPointerUp (PointerEventData eventData) 
	{
		Debug.Log ("The mouse click was released");
		if(!Mathf.Approximately(this.uiSlider.value, this.uiSlider.maxValue))
		{
			// start sliding backwards
			StartCoroutine(SlideBack(this.slideDelay));
		}
		else if(Mathf.Approximately(this.uiSlider.value, this.uiSlider.maxValue))
		{
			CheckUnlock();
		}
	}
	
	
	public IEnumerator SlideBack(float delay)
	{
		if(delay > 0)
		{
			yield return new WaitForSeconds(delay);
		}
		while(!Mathf.Approximately(this.uiSlider.value, this.uiSlider.minValue))
		{
			this.uiSlider.value -= resistance;
			yield return new WaitForEndOfFrame();
		}
		this.handle.color = Color.white;
		yield break;
	}
	
	public void SetupSlider(UnityAction<UnlockContainerItemResult> callback = null)
	{
		this.afterUnlock = callback;
		this.ItemDescription.text = controller.selectedItem.Description;
		
		if(controller.selectedItem.Container != null)
		{
			string id = controller.selectedItem.Container.KeyItemId;
			CatalogItem keyReference = PF_GameData.catalogItems.Find((i) => { return i.ItemId == id; } );
			
			
			if(keyReference != null)
			{
				this.sliderMessage.text = string.Format("{0} Required", keyReference.DisplayName);
				
				string iconName = "Default";
				if( !string.Equals(keyReference.CustomData, null)) //should be !string.IsNullOrEmpty(CI.CustomData)
				{
					Dictionary<string, string> kvps = PlayFab.SimpleJson.DeserializeObject<Dictionary<string, string>>(keyReference.CustomData);
					kvps.TryGetValue("icon", out iconName);	
				}
				
                iconName = iconName == GlobalStrings.DEFAULT_ICON || string.IsNullOrEmpty(iconName) ? GlobalStrings.BRONZE_KEY_ICON : iconName;
				Sprite icon = GameController.Instance.iconManager.GetIconById(iconName);	
				this.handle.overrideSprite = icon;
				
				//make sure player has key
				if(PF_PlayerData.characterInvByCategory.ContainsKey(keyReference.ItemId))
				{
					this.endIcon.color = Color.green;
				}
				else
				{
					this.endIcon.color = Color.red;
				}
			}
			else
			{
                this.handle.overrideSprite = GameController.Instance.iconManager.GetIconById(GlobalStrings.DARKSTONE_LOCK_ICON);
                this.sliderMessage.text = GlobalStrings.UNLOCKED_MSG;
			}
		}
		else
		{
            this.sliderMessage.text = GlobalStrings.UNLOCKED_MSG;
			// set default key icon or lock or something...
		}
	}
	
//	void EnableUnlockedItemsView(List<ItemInstance> unlockedItems = null)
//	{
//		this.ItemsTransform.gameObject.SetActive(true);
//		this.ItemDescription.gameObject.SetActive(false);
//		
//	}
//	
//	void DisableUnlockedItemsView()
//	{
//		this.ItemsTransform.gameObject.SetActive(false);
//		this.ItemDescription.gameObject.SetActive(true);
//	}
	
	
	public void CheckUnlock()
	{
		string item = this.controller.selectedItem.ItemId;
		
		if(controller.UnpackToPlayer)
		{
			PF_GameData.TryOpenContainer(item, null, this.afterUnlock);
		}
		else
		{
			string character = PF_PlayerData.activeCharacter.characterDetails.CharacterId;
			PF_GameData.TryOpenContainer(item, character, this.afterUnlock);
		}
	}
	
	
	
	
	public void OnSliderChanged(float value)
	{
		//Debug.Log("Check!");
//		if(this.slideBack != null)
//		{
//			StopCoroutine(slideBack);
//		}
	}
}
