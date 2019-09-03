using UnicornBattle.Controllers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.UI;

public class ClassButtonUI : MonoBehaviour
{
    public Image bg;
    public Image icon;
    public Text nameText;
    public Button button;
    public RectTransform rectTransform;
    public TweenPos animator;
    public Transform frame;
    public ClassSelectionController controller;
    public ClassSelectionController.AnimationStates currentState = ClassSelectionController.AnimationStates.Off;
    public Vector2 initialPos;
    public UBCharacterClassDetail details;

    void Start()
    {
        initialPos = rectTransform.anchoredPosition;

        controller = (ClassSelectionController) transform.GetComponentInParent<ClassSelectionController>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            controller.SelectClass(this);
        });

    }

    public void OnSelect()
    {
        TweenScale.Tween(button.gameObject, .5f, new Vector3(1, 1, 1), new Vector3(1.08f, 1.08f, 1.08f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);
    }
    public void OnDeselect()
    {
        TweenScale.Tween(button.gameObject, .001f, new Vector3(1, 1, 1), new Vector3(1, 1, 1), TweenMain.Style.Once, TweenMain.Method.Linear, null);
    }
}