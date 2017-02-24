using UnityEngine;
using UnityEngine.UI;

public class FloatingInventory_SelectedItem : MonoBehaviour
{
    public InventoryDisplayItem itemData;
    public Image icon;
    public Text itemName;
    public Text itemDescription;
    public Text annotation;

    public Text totalUses;
    public Image usesIcon;

    public Button UseAction;
    public Button UnlockAction;
    public FloatingInventoryController controller;

    // Use this for initialization
    void Start()
    {
        UseAction.onClick.AddListener(controller.UseItem);
        UnlockAction.onClick.AddListener(controller.UnlockContainer);
    }

    public void RefreshSelected(InventoryDisplayItem item)
    {
        if (item == null)
            return;

        var qty = PF_PlayerData.GetItemQty(item.category.itemId);
        var isContainer = item.category.catalogRef.Container != null && item.category.catalogRef.Container.ResultTableContents != null;

        itemData = item;
        icon.overrideSprite = item.category.icon;
        itemName.text = item.category.catalogRef.DisplayName;
        itemDescription.text = item.category.catalogRef.Description;
        annotation.text = item.category.inventory[0].Annotation;
        totalUses.text = " x" + qty;

        UnlockAction.gameObject.SetActive(isContainer);
        UseAction.gameObject.SetActive(!isContainer && controller.activeFilter == DialogCanvasController.InventoryFilters.UsableInCombat);
        totalUses.gameObject.SetActive(qty != 1);
    }
}
