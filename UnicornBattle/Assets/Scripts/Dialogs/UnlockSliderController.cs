using System.Collections;
using PlayFab.ClientModels;
using UnicornBattle.Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
    public class UnlockSliderController : MonoBehaviour, IPointerUpHandler
    {
        public Slider uiSlider;
        public Text sliderMessage;
        public Button storeButton;
        public Image endIcon;
        public Image handle;
        public ItemViewerController controller;
        public Text ItemDescription;

        private float slideDelay = 0.333f;
        private float resistance = 0.05f;
        private UnityAction<UnlockContainerItemResult> afterUnlock;

        void OnEnable()
        {
            // TODO clear listeners?
            uiSlider.value = uiSlider.minValue;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!Mathf.Approximately(uiSlider.value, uiSlider.maxValue))
                StartCoroutine(SlideBack(slideDelay));
            else if (Mathf.Approximately(uiSlider.value, uiSlider.maxValue))
                CheckUnlock();
        }

        public IEnumerator SlideBack(float delay)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);

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
                var l_inventoryMgr = MainManager.Instance.getInventoryManager();
                if (null == l_inventoryMgr) return;

                var keyId = controller.selectedItem.Container.KeyItemId;
                var keyReference = l_inventoryMgr.GetCatalogItemById(keyId);
                endIcon.gameObject.SetActive(keyReference != null);

                if (keyReference != null)
                {
                    var chestQty = l_inventoryMgr.CountItemsByID(controller.selectedItem.ItemId);
                    var keyQty = l_inventoryMgr.CountItemsByID(keyId);
                    var useColor = (chestQty > 0 && keyQty > 0) ? Color.cyan : Color.red;

                    endIcon.color = useColor;
                    sliderMessage.text = string.Format("{0} Required ({1} available)", keyReference.DisplayName, Mathf.Min(chestQty, keyQty));
                    sliderMessage.color = useColor;

                    var iconName = l_inventoryMgr.GetIconByItemById(keyReference.ItemId, GlobalStrings.BRONZE_KEY_ICON);
                    var icon = GameController.Instance.iconManager.GetIconById(iconName, IconManager.IconTypes.Item);
                    handle.overrideSprite = icon;
                }
                else
                {
                    handle.overrideSprite = GameController.Instance.iconManager.GetIconById(GlobalStrings.DARKSTONE_LOCK_ICON, IconManager.IconTypes.Misc);
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
            var l_inventoryMgr = MainManager.Instance.getInventoryManager();
            if (null == l_inventoryMgr) return;

            var chestItemId = controller.selectedItem.ItemId;
            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.UnlockContainerItem);
            l_inventoryMgr.TryOpenContainer(chestItemId,
                (r) =>
                {

                    if (null != afterUnlock)
                        afterUnlock.Invoke(r);
                    PF_Bridge.RaiseCallbackSuccess("Container Unlocked", PlayFabAPIMethods.UnlockContainerItem);
                },
                (f) =>
                {
                    if (f != null)
                    {
                        StartCoroutine(SlideBack(slideDelay));
                    }
                    PF_Bridge.RaiseCallbackError(f, PlayFabAPIMethods.UnlockContainerItem);
                }
            );
        }
    }
}