using UnicornBattle.Controllers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
[RequireComponent(typeof(Button))]
public class CharacterSlot : MonoBehaviour
{
    public Sprite defaultSprite;
    public Sprite selectedSprite;
    public Transform SelectedFrame;
    public Image IconFrame;
    public Button deleteButton;
    public Image unicornIcon;
    public Text unicornName;
    public Text unicornLevel;
    public Button slotButton;
    public Text ponyClass;

    public bool isLocked = false;
    public Button LockedUI;
    public UBSavedCharacter saved;
    private CharacterSelectionController characterSelectionController;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        isLocked = false;
        characterSelectionController = transform.GetComponentInParent<CharacterSelectionController>();

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(() => { characterSelectionController.SelectSlot(this); });

        LockedUI.onClick.RemoveAllListeners();
        LockedUI.onClick.AddListener(() => { characterSelectionController.BuyAdditionalSlots(); });

        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(() => { characterSelectionController.DeleteCharacter(); });
    }

    public void FillSlot(UBSavedCharacter ch)
    {
        saved = ch;
        if (ch.characterData != null)
        {
            unicornName.text = ch.CharacterName;
            unicornLevel.text = "" + ch.characterData.CharacterLevel;
            ponyClass.text = ch.baseClass.CatalogCode;
        }

        unicornIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(saved.baseClass.Icon, IconManager.IconTypes.Class);
        ShowIcon();
    }

    public void RefreshSlot()
    {
        if (isLocked)
        {
            LockedUI.gameObject.SetActive(true);
            saved = null;
            DeselectSlot();
        }
        else
        {
            LockedUI.gameObject.SetActive(false);
        }

        // IF NO CHARACTER IN THE SLOT
        if (saved == null)
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        saved = null;
        unicornName.text = "New";
        unicornLevel.text = "";
        unicornIcon.overrideSprite = GameController.Instance.iconManager.GetIconById("Photo", IconManager.IconTypes.Misc);
        IconFrame.color = Color.gray;
        ponyClass.text = "";
    }

    public void HideIcon()
    {
        unicornIcon.gameObject.SetActive(false);
    }

    public void ShowIcon()
    {
        unicornIcon.gameObject.SetActive(true);
    }

    public void SelectSlot()
    {
        deleteButton.gameObject.SetActive(saved != null);
        SelectedFrame.gameObject.SetActive(true);
        IconFrame.color = SelectedFrame.GetComponent<Image>().color;
        slotButton.GetComponent<Image>().color = SelectedFrame.GetComponent<Image>().color;
    }

    public void DeselectSlot()
    {
        SelectedFrame.gameObject.SetActive(false);
        IconFrame.color = Color.gray;
        slotButton.GetComponent<Image>().color = Color.white;
    }
}