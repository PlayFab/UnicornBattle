using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ArrowUI : MonoBehaviour {
	public Image arrow;
	public Image icon;
	public Text ponyName;
	public Button btn;
	public RectTransform rt;
	public TweenPos animator;
	public Transform frame;
	public ArrowPickerController controller;
	public ArrowPickerController.AnimationStates currentState = ArrowPickerController.AnimationStates.Off;
	public Vector2 initialPos;

	public UB_ClassDetail details;
	
	// Use this for initialization
	void Start () {
		this.initialPos = rt.anchoredPosition;
		
		this.controller = (ArrowPickerController)this.transform.GetComponentInParent<ArrowPickerController>();
		btn.onClick.RemoveAllListeners();
		btn.onClick.AddListener(() => { controller.SelectSlot(this); } );
	}
}
