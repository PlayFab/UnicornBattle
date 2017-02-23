using UnityEngine;
using UnityEngine.UI;

public class FloatingStore_SelectedItem : MonoBehaviour
{
    public StoreDisplayItem itemData;
    public Image icon;
    public Text itemName;
    public Text itemDescription;
    public Text itemCost;
    public Image currencyIcon;
    public Text totalUses;
    public Image usesIcon;
    public Button BuyAction;
    public FloatingStoreController controller;

    // Use this for initialization
    void Start()
    {
        BuyAction.onClick.AddListener(controller.selectedItem.InitiatePurchase);
    }

    public void RefreshSelected(StoreDisplayItem item)
    {
        if (item == null)
            return;

        itemData = item;
        icon.overrideSprite = item.image.overrideSprite;
        itemName.text = item.catalogItem.DisplayName;
        itemDescription.text = item.catalogItem.Description;

        itemCost.text = string.Format(" x{0}", item.finalPrice);
        currencyIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(item.currencyKey, IconManager.IconTypes.Misc);

        if (item.catalogItem.Consumable.UsageCount != null)
            totalUses.text = string.Format(" x{0}", item.catalogItem.Consumable.UsageCount);
    }
}
