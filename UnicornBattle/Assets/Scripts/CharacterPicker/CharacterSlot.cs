using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[System.Serializable]
[RequireComponent (typeof (Button))]
public class CharacterSlot : MonoBehaviour {
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
	public bool isLocked = true;
	
	//Button slotButton;
	
	public UB_SavedCharacter saved = null;
	
	private CharacterPicker cPicker;
//	private PonySlot ponyData;
	
	// Use this for initialization
	void Start () 
	{
		Init();
		
	}

//	public void OnPointerClick(PointerEventData pData)
//	{
//		//Debug.Log("!!!!");
//		cPicker.SelectSlot(this);
//	}
//	
	
	public void Init()
	{
		this.cPicker = (CharacterPicker)this.transform.GetComponentInParent<CharacterPicker>();
		
		myButton.onClick.RemoveAllListeners();
		myButton.onClick.AddListener(() => { cPicker.SelectSlot(this); } );
		//slotButton.onClick.AddListener((p) => {this.OnPointerClick(p);});
		
		LockedUI.onClick.RemoveAllListeners();
		LockedUI.onClick.AddListener(() => { cPicker.BuyAdditionalSlots(); } );
	}
	
	public void FillSlot(UB_SavedCharacter ch)
	{
		this.saved = ch;
		if(ch.characterDetails != null && ch.characterData != null)
		{
			this.ponyName.text = ch.characterDetails.CharacterName;
			this.ponyLevel.text = ""+ ch.characterData.CharacterLevel;
		}
		
		ponyIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(this.saved.baseClass.Icon);
		ShowIcon();
	}

	public void ClearSlot()
	{
		UnlockSlot ();
		this.saved = null;
		this.ponyName.text = "New";
		this.ponyLevel.text = "0";
		this.ponyIcon.overrideSprite = GameController.Instance.iconManager.GetIconById("Photo");
		this.IconFrame.color = Color.white;
	}

	public void HideIcon()
	{
		this.ponyIcon.gameObject.SetActive(false);
	}
	
	public void ShowIcon()
	{
		this.ponyIcon.gameObject.SetActive(true);
	}
	
	public void LockSlot()
	{
		this.LockedUI.gameObject.SetActive(true);
		this.isLocked = true;
		this.saved = null;
		DeselectSlot ();
	}
	
	public void UnlockSlot()
	{
		this.LockedUI.gameObject.SetActive(false);
		this.isLocked = false;


	}

	public void SelectSlot()
	{


		if (this.saved == null) {
			this.deleteCharacter.gameObject.SetActive (false);
		} else {
			this.deleteCharacter.gameObject.SetActive (true);
		}

		this.SelectedFrame.gameObject.SetActive (true);
		this.IconFrame.color = Color.green;
		
	}

	public void DeselectSlot()
	{
		this.SelectedFrame.gameObject.SetActive (false);
		this.IconFrame.color = Color.white;
	}

}
