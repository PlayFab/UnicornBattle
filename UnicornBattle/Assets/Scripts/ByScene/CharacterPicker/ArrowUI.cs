using UnityEngine;
using UnityEngine.UI;

public class ArrowUI : MonoBehaviour
{
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

    void Start()
    {
        initialPos = rt.anchoredPosition;

        controller = (ArrowPickerController)transform.GetComponentInParent<ArrowPickerController>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => { controller.SelectSlot(this); });
    }
}
