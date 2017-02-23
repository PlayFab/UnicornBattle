using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ArrowPickerController : SoftSingleton<ArrowPickerController>
{
    public enum AnimationStates { Off, On, Selected }

    public RectTransform ArrowGroup;
    public ArrowUI Arrow1;
    public ArrowUI Arrow2;
    public ArrowUI Arrow3;
    public ArrowUI selectedSlot;
    public int maxUniSlots = 3;
    public Button confirmButton;
    public CharacterPicker cPicker;
    public SelectedPonyController selectedPonyUI;
    private Vector3 _initialPos = Vector3.zero;
    private bool _buttonsPulsing = false;

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
        switch (method)
        {
            case PlayFabAPIMethods.GetTitleData_General:
                maxUniSlots = PF_GameData.Classes.Count;
                if (maxUniSlots > 3)
                    Debug.LogWarning("Currently configured to only allow 3 playable unicorn classes.");

                Init();
                break;
        }
    }

    public void Init()
    {
        var classCounter = 0;
        foreach (var pair in PF_GameData.Classes)
        {
            if (classCounter > 2) break;

            if (classCounter == 0)
            {
                Arrow1.details = pair.Value;
                Arrow1.ponyName.text = pair.Key;
                Arrow1.icon.overrideSprite = GameController.Instance.iconManager.GetIconById(pair.Value.Icon, IconManager.IconTypes.Class);
            }
            else if (classCounter == 1)
            {
                Arrow2.details = pair.Value;
                Arrow2.ponyName.text = pair.Key;
                Arrow2.icon.overrideSprite = GameController.Instance.iconManager.GetIconById(pair.Value.Icon, IconManager.IconTypes.Class);
            }
            else if (classCounter == 2)
            {
                Arrow3.details = pair.Value;
                Arrow3.ponyName.text = pair.Key;
                Arrow3.icon.overrideSprite = GameController.Instance.iconManager.GetIconById(pair.Value.Icon, IconManager.IconTypes.Class);
            }
            classCounter++;
        }
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => { cPicker.UnicornPicked(selectedSlot); });

        TurnOnArrows();
    }

    void Start()
    {
        _initialPos = ArrowGroup.position;
    }

    public void SelectSlot(ArrowUI go)
    {
        if (selectedSlot == go || cPicker.selectedSlot.saved != null)
            return;

        selectedPonyUI.gameObject.SetActive(false);
        var arrowIndex = 0;
        if (go.tag == "Arrow1")
            arrowIndex = 0;
        else if (go.tag == "Arrow2")
            arrowIndex = 1;
        else
            arrowIndex = 2;

        var afterSelect = new UnityEvent();
        afterSelect.AddListener(() =>
        {
            selectedPonyUI.gameObject.SetActive(true);
            TweenScale.Tween(confirmButton.gameObject, .5f, new Vector3(1, 1, 1), new Vector3(1.1f, 1.1f, 1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);
        });

        var afterDeselect = new UnityEvent();
        afterDeselect.AddListener(() =>
        {
            selectedSlot = go;
            selectedPonyUI.SwitchPonyDetails(go);
            SelectArrow(arrowIndex, afterSelect);
        });

        DeselectArrows(afterDeselect);
    }

    public void TurnOffArrows(UnityEvent callback = null)
    {
        DeselectArrows();
        DisablePulsingButtons();
        TweenPos.Tween(ArrowGroup.gameObject, .333f, _initialPos, () =>
        {
            Arrow1.btn.interactable = false;
            Arrow2.btn.interactable = false;
            Arrow3.btn.interactable = false;

            Arrow1.currentState = AnimationStates.Off;
            Arrow2.currentState = AnimationStates.Off;
            Arrow3.currentState = AnimationStates.Off;

            if (callback != null)
            {
                callback.Invoke();
            }
        }, Space.World);
    }

    public void TurnOnArrows(UnityEvent callback = null)
    {
        //Debug.Log(ArrowGroup.rect.width);
        selectedSlot = null;
        TweenPos.Tween(ArrowGroup.gameObject, .333f, new Vector3(445, 30, 0), () =>
        {
            Arrow1.btn.interactable = true;
            Arrow1.currentState = AnimationStates.On;

            Arrow2.btn.interactable = true;
            Arrow2.currentState = AnimationStates.On;

            Arrow3.btn.interactable = true;
            Arrow3.currentState = AnimationStates.On;

            if (callback != null)
                callback.Invoke();
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
        if (selectedArrow.currentState == AnimationStates.Selected)
            return;

        selectedArrow.animator.OnFinished = callback;
        selectedArrow.animator.OnFinished.AddListener(() =>
        {
            selectedArrow.currentState = AnimationStates.Selected;
            SetSelectedButton(index);
        });
        selectedArrow.animator.PlayForward();
    }

    public void DeselectArrows(UnityEvent callback = null)
    {
        selectedPonyUI.gameObject.SetActive(false);
        var activeArrow = Arrow3.currentState == AnimationStates.Selected ? Arrow3 : null;
        activeArrow = Arrow2.currentState == AnimationStates.Selected ? Arrow2 : activeArrow;
        activeArrow = Arrow1.currentState == AnimationStates.Selected ? Arrow1 : activeArrow;

        if (activeArrow != null)
        {
            activeArrow.animator.OnFinished = callback;
            activeArrow.animator.PlayReverse();
            activeArrow.currentState = AnimationStates.On;
        }
        else
        {
            if (callback != null)
                callback.Invoke();
        }
    }

    public void EnablePulsingButtons()
    {
        _buttonsPulsing = true;
        TweenScale.Tween(Arrow1.gameObject, .5f, new Vector3(1, 1, 1), new Vector3(1.1f, 1.1f, 1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);
        TweenScale.Tween(Arrow2.gameObject, .5f, new Vector3(1, 1, 1), new Vector3(1.1f, 1.1f, 1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);
        TweenScale.Tween(Arrow3.gameObject, .5f, new Vector3(1, 1, 1), new Vector3(1.1f, 1.1f, 1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);
    }

    public void DisablePulsingButtons()
    {
        _buttonsPulsing = false;
        TweenScale.Tween(Arrow1.gameObject, .001f, new Vector3(1, 1, 1), new Vector3(1, 1, 1), TweenMain.Style.Once, TweenMain.Method.Linear, null);
        TweenScale.Tween(Arrow2.gameObject, .001f, new Vector3(1, 1, 1), new Vector3(1, 1, 1), TweenMain.Style.Once, TweenMain.Method.Linear, null);
        TweenScale.Tween(Arrow3.gameObject, .001f, new Vector3(1, 1, 1), new Vector3(1, 1, 1), TweenMain.Style.Once, TweenMain.Method.Linear, null);
    }

    public void SetSelectedButton(int index)
    {
        if (index == 0 || !_buttonsPulsing)
            TweenScale.Tween(Arrow1.gameObject, .001f, new Vector3(1, 1, 1), new Vector3(1, 1, 1), TweenMain.Style.Once, TweenMain.Method.Linear, null);
        else
            TweenScale.Tween(Arrow1.gameObject, .5f, new Vector3(1, 1, 1), new Vector3(1.1f, 1.1f, 1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);

        if (index == 1 || !_buttonsPulsing)
            TweenScale.Tween(Arrow2.gameObject, .001f, new Vector3(1, 1, 1), new Vector3(1, 1, 1), TweenMain.Style.Once, TweenMain.Method.Linear, null);
        else
            TweenScale.Tween(Arrow2.gameObject, .5f, new Vector3(1, 1, 1), new Vector3(1.1f, 1.1f, 1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);

        if (index == 2 || !_buttonsPulsing)
            TweenScale.Tween(Arrow3.gameObject, .001f, new Vector3(1, 1, 1), new Vector3(1, 1, 1), TweenMain.Style.Once, TweenMain.Method.Linear, null);
        else
            TweenScale.Tween(Arrow3.gameObject, .5f, new Vector3(1, 1, 1), new Vector3(1.1f, 1.1f, 1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);
    }
}
