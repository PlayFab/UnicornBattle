using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ArrowPickerController : SoftSingleton<ArrowPickerController> {
	public RectTransform ArrowGroup;
	public ArrowUI Arrow1;
	public ArrowUI Arrow2;
	public ArrowUI Arrow3;
	
	public enum AnimationStates { Off, On, Selected }
	
	public int maxPonySlots = 3;
//	public Transform defaultSlotUI;
	public Transform[] slots;
	public Button confirmButton;
	
	//public Sprite defaultSprite;
	//public Sprite selectedSprite;
	
	public CharacterPicker cPicker;
	
	public ArrowUI selectedSlot;
//	public List<PonySlot> ponySlots;
	
	public SelectedPonyController selectedPonyUI;
	
	private Vector3 initialPos = Vector3.zero;
    private bool buttonsPulsing = false;
	
	void OnEnable()
	{
		PF_Bridge.OnPlayFabCallbackError += HandleCallbackError;
		PF_Bridge.OnPlayfabCallbackSuccess += HandleCallbackSuccess;
	}
	
	void OnDisable()
	{
		PF_Bridge.OnPlayFabCallbackError -= HandleCallbackError;	
		PF_Bridge.OnPlayfabCallbackSuccess -= HandleCallbackSuccess;
	}
	
	public void HandleCallbackError(string details, PlayFabAPIMethods method, MessageDisplayStyle style)
	{	
		
	}
	
	public void HandleCallbackSuccess(string details, PlayFabAPIMethods method, MessageDisplayStyle style)
	{
		switch(method)
		{
			case PlayFabAPIMethods.GetTitleData:
				this.maxPonySlots = PF_GameData.Classes.Count;
				
				if(this.maxPonySlots > 3)
				{
					Debug.LogWarning("Currently configured to only allow 3 playable pony classes.");
				}
				
				Init();
				break;
		}
	}
	
	public void Init()
	{
		int classCounter = 0;
		foreach(var pair in PF_GameData.Classes)
		{
			if(classCounter > 2)
			{
				break;
			}
			if(classCounter == 0)
			{
				Arrow1.details = pair.Value;
				Arrow1.ponyName.text = pair.Key;
				Arrow1.icon.overrideSprite = GameController.Instance.iconManager.GetIconById(pair.Value.Icon);
			}
			else if(classCounter == 1)
			{
				Arrow2.details = pair.Value;
				Arrow2.ponyName.text = pair.Key;
				Arrow2.icon.overrideSprite = GameController.Instance.iconManager.GetIconById(pair.Value.Icon);
			}
			else if(classCounter == 2)
			{
				Arrow3.details = pair.Value;
				Arrow3.ponyName.text = pair.Key;
				Arrow3.icon.overrideSprite = GameController.Instance.iconManager.GetIconById(pair.Value.Icon);
			}
//			ps.Init();
			classCounter++;
		}
		this.confirmButton.onClick.RemoveAllListeners();
		this.confirmButton.onClick.AddListener(() => { cPicker.PonyPicked(selectedSlot);});
		
		if(selectedSlot != null)
		{
			//this.confirmButton.gameObject.SetActive(true);
		}
		else
		{
			//this.confirmButton.gameObject.SetActive(false);
		}
		TurnOnArrows ();
	}
	
	// Use this for initialization
	void Start () 
	{
		this.initialPos = ArrowGroup.position;
	}

	void OnGUI()
	{
/*		if(GUI.Button(new Rect(5,5,125,50), "Turn Off"))
		{
			TurnOffArrows();
		}
		if(GUI.Button(new Rect(5,60,125,50), "Turn On"))
		{
			TurnOnArrows();
		}
		if(GUI.Button(new Rect(5,120,125,50), "Select"))
		{
			int rng = Random.Range(0,4);
			SelectArrow(rng);
		}
		if(GUI.Button(new Rect(5,180,125,50), "Deselect"))
		{
			DeselectArrows();
		}*/
	}
	
	
	public void SelectSlot(ArrowUI go)
	{
		if (this.selectedSlot == go || this.cPicker.selectedSlot.saved != null) {
			return;
		} 
		else
		{
				this.selectedPonyUI.gameObject.SetActive(false);
				int arrowIndex = 0;
				if(go.tag == "Arrow1")
				{
					arrowIndex = 0;
				}
				else if(go.tag == "Arrow2")
				{
					arrowIndex = 1;
				}
				else
				{
					arrowIndex = 2;
				}

				UnityEvent afterSelect = new UnityEvent();
				afterSelect.AddListener(() =>
				{
					this.selectedPonyUI.gameObject.SetActive(true);
					TweenScale.Tween(this.confirmButton.gameObject, .5f, new Vector3(1,1,1), new Vector3(1.1f,1.1f,1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);
				});

				UnityEvent afterDeselect = new UnityEvent();
				afterDeselect.AddListener(() => {
					this.selectedSlot = go;
					this.selectedPonyUI.SwitchPonyDetails(go);
					SelectArrow(arrowIndex, afterSelect);
				});

				//afterDeselect.Invoke();

				DeselectArrows(afterDeselect);
		}
	}

	public void TurnOffArrows(UnityEvent callback = null)
	{
		DeselectArrows();
		DisablePulsingButtons();
		TweenPos.Tween(this.ArrowGroup.gameObject, .333f, this.initialPos, () => 
		{
			this.Arrow1.btn.interactable = false;
			this.Arrow2.btn.interactable = false;
			this.Arrow3.btn.interactable = false;
			
			this.Arrow1.currentState = AnimationStates.Off;
			this.Arrow2.currentState = AnimationStates.Off;
			this.Arrow3.currentState = AnimationStates.Off;
			
			if(callback != null)
			{
				callback.Invoke();
			}
		}, Space.World);
	}
	
	public void TurnOnArrows(UnityEvent callback = null)
	{
		//Debug.Log(this.ArrowGroup.rect.width);
		this.selectedSlot = null;
		TweenPos.Tween(this.ArrowGroup.gameObject, .333f, new Vector3(445,30,0), () => 
		{
			this.Arrow1.btn.interactable = true;
			this.Arrow1.currentState = AnimationStates.On;
			
			this.Arrow2.btn.interactable = true;
			this.Arrow2.currentState = AnimationStates.On;
			
			this.Arrow3.btn.interactable = true;
			this.Arrow3.currentState = AnimationStates.On;
			
			if(callback != null)
			{
				callback.Invoke();
			}
		}, Space.Self);
	}
	
	public void SelectArrow(int index, UnityEvent callback = null)
	{
        ArrowUI selectedArrow;
        switch (index)
        {
            case 0: selectedArrow = Arrow1; break; // bucephelous
            case 1: selectedArrow = Arrow2; break; // PegaZeus
            case 2: selectedArrow = Arrow3; break; // nightmare
            default: return;
        }
		if(selectedArrow.currentState == AnimationStates.Selected)
            return;

		selectedArrow.animator.OnFinished = callback;
		selectedArrow.animator.OnFinished.AddListener(() => 
		{
	    	selectedArrow.currentState = AnimationStates.Selected;
			SetSelectedButton(index);
		});
		selectedArrow.animator.PlayForward();
	}
	
	public void DeselectArrows( UnityEvent callback = null)
	{
		this.selectedPonyUI.gameObject.SetActive (false);
		//this.selectedSlot = null;

		if (this.Arrow1.currentState == AnimationStates.Selected) {
			if (callback != null) {
				this.Arrow1.animator.OnFinished = callback;
			} else {
				this.Arrow1.animator.OnFinished = null;
			}
			this.Arrow1.animator.PlayReverse ();
			this.Arrow1.currentState = AnimationStates.On;
			//return;
		} else if (this.Arrow2.currentState == AnimationStates.Selected) {
			if (callback != null) {
				this.Arrow2.animator.OnFinished = callback;
			} else {
				this.Arrow2.animator.OnFinished = null;
			}
			this.Arrow2.animator.PlayReverse ();
			this.Arrow2.currentState = AnimationStates.On;
			//return;
		} else if (this.Arrow3.currentState == AnimationStates.Selected) {
			if (callback != null) {
				this.Arrow3.animator.OnFinished = callback;
			} else {
				this.Arrow3.animator.OnFinished = null;
			}
			this.Arrow3.animator.PlayReverse ();
			this.Arrow3.currentState = AnimationStates.On;
			//return;
		} else {
			if (callback != null) {
				callback.Invoke();
			}
		}
	}

	public void EnablePulsingButtons()
	{
        buttonsPulsing = true;
		TweenScale.Tween(this.Arrow1.gameObject, .5f, new Vector3(1,1,1), new Vector3(1.1f,1.1f,1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);
		TweenScale.Tween(this.Arrow2.gameObject, .5f, new Vector3(1,1,1), new Vector3(1.1f,1.1f,1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);
		TweenScale.Tween(this.Arrow3.gameObject, .5f, new Vector3(1,1,1), new Vector3(1.1f,1.1f,1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);
	}

    public void DisablePulsingButtons()
    {
        buttonsPulsing = false;
        TweenScale.Tween(this.Arrow1.gameObject, .001f, new Vector3(1, 1, 1), new Vector3(1, 1, 1), TweenMain.Style.Once, TweenMain.Method.Linear, null);
        TweenScale.Tween(this.Arrow2.gameObject, .001f, new Vector3(1, 1, 1), new Vector3(1, 1, 1), TweenMain.Style.Once, TweenMain.Method.Linear, null);
        TweenScale.Tween(this.Arrow3.gameObject, .001f, new Vector3(1, 1, 1), new Vector3(1, 1, 1), TweenMain.Style.Once, TweenMain.Method.Linear, null);
    }
	
	public void SetSelectedButton(int index)
	{
        if (index == 0 || !buttonsPulsing)
            TweenScale.Tween(this.Arrow1.gameObject, .001f, new Vector3(1, 1, 1), new Vector3(1, 1, 1), TweenMain.Style.Once, TweenMain.Method.Linear, null);
        else
            TweenScale.Tween(this.Arrow1.gameObject, .5f, new Vector3(1, 1, 1), new Vector3(1.1f, 1.1f, 1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);

        if (index == 1 || !buttonsPulsing)
            TweenScale.Tween(this.Arrow2.gameObject, .001f, new Vector3(1, 1, 1), new Vector3(1, 1, 1), TweenMain.Style.Once, TweenMain.Method.Linear, null);
        else
            TweenScale.Tween(this.Arrow2.gameObject, .5f, new Vector3(1, 1, 1), new Vector3(1.1f, 1.1f, 1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);

        if (index == 2 || !buttonsPulsing)
            TweenScale.Tween(this.Arrow3.gameObject, .001f, new Vector3(1, 1, 1), new Vector3(1, 1, 1), TweenMain.Style.Once, TweenMain.Method.Linear, null);
        else
            TweenScale.Tween(this.Arrow3.gameObject, .5f, new Vector3(1, 1, 1), new Vector3(1.1f, 1.1f, 1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);
	}
}
