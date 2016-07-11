using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
//IPointerClickedHandler
public class FriendItem : MonoBehaviour {
	public FriendPicker fPicker;
	
	// Use this for initialization
	void Start () 
	{
        this.fPicker = (FriendPicker)this.transform.GetComponentInParent<FriendPicker>();
        //slotButton.onClick.AddListener((p) => {this.OnPointerClick(p);});
    }

//	public void OnPointerClick(PointerEventData pData)
//	{
//		Debug.Log("!!!!");
//		fPicker.FriendItemClicked(this);
//	}
}
