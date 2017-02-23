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
    public Button deleteCharacter;
    public Image ponyIcon;
    public Text ponyName;
    public Text ponyLevel;
    public Button myButton;

    public Button LockedUI;
    public UB_SavedCharacter saved;
    private CharacterPicker cPicker;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        cPicker = transform.GetComponentInParent<CharacterPicker>();

        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(() => { cPicker.SelectSlot(this); });

        LockedUI.onClick.RemoveAllListeners();
        LockedUI.onClick.AddListener(() => { cPicker.BuyAdditionalSlots(); });
    }

    public void FillSlot(UB_SavedCharacter ch)
    {
        saved = ch;
        if (ch.characterDetails != null && ch.characterData != null)
        {
            ponyName.text = ch.characterDetails.CharacterName;
            ponyLevel.text = "" + ch.characterData.CharacterLevel;
        }

        ponyIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(saved.baseClass.Icon, IconManager.IconTypes.Class);
        ShowIcon();
    }

    public void ClearSlot()
    {
        UnlockSlot();
        saved = null;
        ponyName.text = "New";
        ponyLevel.text = "0";
        ponyIcon.overrideSprite = GameController.Instance.iconManager.GetIconById("Photo", IconManager.IconTypes.Misc);
        IconFrame.color = Color.white;
    }

    public void HideIcon()
    {
        ponyIcon.gameObject.SetActive(false);
    }

    public void ShowIcon()
    {
        ponyIcon.gameObject.SetActive(true);
    }

    public void LockSlot()
    {
        LockedUI.gameObject.SetActive(true);
        saved = null;
        DeselectSlot();
    }

    public void UnlockSlot()
    {
        LockedUI.gameObject.SetActive(false);
    }

    public void SelectSlot()
    {
        deleteCharacter.gameObject.SetActive(saved != null);
        SelectedFrame.gameObject.SetActive(true);
        IconFrame.color = Color.green;
    }

    public void DeselectSlot()
    {
        SelectedFrame.gameObject.SetActive(false);
        IconFrame.color = Color.white;
    }
}
