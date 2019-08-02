using UnicornBattle.Controllers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplayItem : MonoBehaviour
{
    public Image bg;
    public Image image;
    public Button btn;
    public FloatingInventoryController controller;
    public UBInventoryCategory category;

    public void Init()
    {
        btn.onClick.AddListener(() =>
        {
            controller.ItemClicked(this);
            bg.color = Color.green;
            image.color = new Color(255, 255, 255, 255);
        });
    }

    public void SetButton(Sprite icon, UBInventoryCategory cItem)
    {
        ActivateButton();
        image.overrideSprite = icon;
        category = cItem;
    }

    public void Deselect()
    {
        bg.color = Color.white;
        image.color = new Color(255, 255, 255, 190);
    }

    public void ClearButton()
    {
        image.overrideSprite = null;
        image.color = Color.clear;
        category = null;
        btn.interactable = false;
        bg.color = Color.white;
    }

    public void ActivateButton()
    {
        image.color = new Color(255, 255, 255, 190);
        image.overrideSprite = null;
        image.color = Color.white;
        category = null;
        btn.interactable = true;
    }
}