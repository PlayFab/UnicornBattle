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
        if (!Mathf.Approximately(uiSlider.value, uiSlider.maxValue))
        {
            // start sliding backwards
            StartCoroutine(SlideBack(slideDelay));
        }
        else if (Mathf.Approximately(uiSlider.value, uiSlider.maxValue))
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
        while (!Mathf.Approximately(uiSlider.value, uiSlider.minValue))
        {
            uiSlider.value -= resistance;
            yield return new WaitForEndOfFrame();
        }
        handle.color = Color.white;
    }

    public void SetupSlider(UnityAction<UnlockContainerItemResult> callback = null)
    {
        afterUnlock = callback;
        ItemDescription.text = controller.selectedItem.Description;

        if (controller.selectedItem.Container != null)
        {
            var id = controller.selectedItem.Container.KeyItemId;
            var keyReference = PF_GameData.GetCatalogItemById(id);

            if (keyReference != null)
            {
                sliderMessage.text = string.Format("{0} Required", keyReference.DisplayName);

                var iconName = "Default";
                if (!string.Equals(keyReference.CustomData, null)) //should be !string.IsNullOrEmpty(CI.CustomData)
                {
                    Dictionary<string, string> kvps = JsonWrapper.DeserializeObject<Dictionary<string, string>>(keyReference.CustomData);
                    kvps.TryGetValue("icon", out iconName);
                }

                iconName = iconName == GlobalStrings.DEFAULT_ICON || string.IsNullOrEmpty(iconName) ? GlobalStrings.BRONZE_KEY_ICON : iconName;
                var icon = GameController.Instance.iconManager.GetIconById(iconName);
                handle.overrideSprite = icon;

                //make sure player has key
                endIcon.color = PF_PlayerData.characterInvByCategory.ContainsKey(keyReference.ItemId) ? Color.green : Color.red;
            }
            else
            {
                handle.overrideSprite = GameController.Instance.iconManager.GetIconById(GlobalStrings.DARKSTONE_LOCK_ICON);
                sliderMessage.text = GlobalStrings.UNLOCKED_MSG;
            }
        }
        else
        {
            sliderMessage.text = GlobalStrings.UNLOCKED_MSG;
            // set default key icon or lock or something...
        }
    }

    public void CheckUnlock()
    {
        var item = controller.selectedItem.ItemId;

        if (controller.UnpackToPlayer)
        {
            PF_GameData.TryOpenContainer(item, null, afterUnlock);
        }
        else
        {
            var character = PF_PlayerData.activeCharacter.characterDetails.CharacterId;
            PF_GameData.TryOpenContainer(item, character, afterUnlock);
        }
    }

}
