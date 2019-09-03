using UnicornBattle.Controllers;
using UnicornBattle.Managers;
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

    public void RefreshSelected(InventoryDisplayItem p_item)
    {
        if (p_item == null)
            return;

        var l_inventoryMgr = MainManager.Instance.getInventoryManager();
        if (null == l_inventoryMgr) return;

        var qty = l_inventoryMgr.CountItemsByID(p_item.category.itemId);
        var isContainer = p_item.category.catalogRef.Container != null && p_item.category.catalogRef.Container.ResultTableContents != null;

        itemData = p_item;
        icon.overrideSprite = p_item.category.icon;
        itemName.text = p_item.category.catalogRef.DisplayName;
        itemDescription.text = p_item.category.catalogRef.Description;
        annotation.text = p_item.category.inventory[0].Annotation;
        totalUses.text = " x" + qty;

        UnlockAction.gameObject.SetActive(isContainer);
        UseAction.gameObject.SetActive(!isContainer && controller.activeFilter == DialogCanvasController.InventoryFilters.UsableInCombat);
        totalUses.gameObject.SetActive(qty != 1);
    }
}