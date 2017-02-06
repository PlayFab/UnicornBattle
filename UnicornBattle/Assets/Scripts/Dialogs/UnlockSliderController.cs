using PlayFab.ClientModels;
using PlayFab.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnlockSliderController : MonoBehaviour, IPointerUpHandler
{
    public Slider uiSlider;
    public Text sliderMessage;
    public Button storeButton;

    private float dragStart;
    private float slideDelay = 0.333f;
    private float resistance = 0.05f;

    private Coroutine slideBack;
    private UnityAction<UnlockContainerItemResult> afterUnlock;

    public Image endIcon;
    public Image handle;

    public ItemViewerController controller;
    public Text ItemDescription;

    void OnEnable()
    {
        // TODO clear listeners?
        uiSlider.value = uiSlider.minValue;

    }


    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("The mouse click was released");
        if (!Mathf.Approximately(this.uiSlider.value, this.uiSlider.maxValue))
        {
            // start sliding backwards
            StartCoroutine(SlideBack(this.slideDelay));
        }
        else if (Mathf.Approximately(this.uiSlider.value, this.uiSlider.maxValue))
        {
            CheckUnlock();
        }
    }


    public IEnumerator SlideBack(float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        while (!Mathf.Approximately(this.uiSlider.value, this.uiSlider.minValue))
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

        if (controller.selectedItem.Container != null)
        {
            string id = controller.selectedItem.Container.KeyItemId;
            CatalogItem keyReference = PF_GameData.catalogItems.Find((i) => { return i.ItemId == id; });


            if (keyReference != null)
            {
                this.sliderMessage.text = string.Format("{0} Required", keyReference.DisplayName);

                string iconName = "Default";
                if (!string.Equals(keyReference.CustomData, null)) //should be !string.IsNullOrEmpty(CI.CustomData)
                {
                    Dictionary<string, string> kvps = JsonWrapper.DeserializeObject<Dictionary<string, string>>(keyReference.CustomData);
                    kvps.TryGetValue("icon", out iconName);
                }

                iconName = iconName == GlobalStrings.DEFAULT_ICON || string.IsNullOrEmpty(iconName) ? GlobalStrings.BRONZE_KEY_ICON : iconName;
                Sprite icon = GameController.Instance.iconManager.GetIconById(iconName);
                this.handle.overrideSprite = icon;

                //make sure player has key
                if (PF_PlayerData.characterInvByCategory.ContainsKey(keyReference.ItemId))
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

    public void CheckUnlock()
    {
        string item = this.controller.selectedItem.ItemId;

        if (controller.UnpackToPlayer)
        {
            PF_GameData.TryOpenContainer(item, null, this.afterUnlock);
        }
        else
        {
            string character = PF_PlayerData.activeCharacter.characterDetails.CharacterId;
            PF_GameData.TryOpenContainer(item, character, this.afterUnlock);
        }
    }

}
