using System.Collections.Generic;
using UnicornBattle.Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnicornBattle.Controllers
{
    public class ClassSelectionController : SoftSingleton<ClassSelectionController>
    {
        public enum AnimationStates { Off, On, Selected }

        public RectTransform classGroup;
        public ClassButtonUI classButton1;
        public ClassButtonUI classButton2;
        public ClassButtonUI classButton3;
        public ClassButtonUI selectedButton;
        public List<ClassButtonUI> classButtonList;
        public int maxUniSlots = 3;
        public CharacterSelectionController characterSelect;
        public ClassAbilitiesController classAbilities;
        private Vector3 _initialPos = Vector3.zero;

        void OnEnable()
        {
            //PF_Bridge.OnPlayFabCallbackError += HandleCallbackError;
            PF_Bridge.OnPlayfabCallbackSuccess += HandleCallbackSuccess;
        }

        void OnDisable()
        {
            //PF_Bridge.OnPlayFabCallbackError -= HandleCallbackError;
            PF_Bridge.OnPlayfabCallbackSuccess -= HandleCallbackSuccess;
        }

        public void HandleCallbackError(string details, PlayFabAPIMethods method, MessageDisplayStyle style) { }

        public void HandleCallbackSuccess(string details, PlayFabAPIMethods method, MessageDisplayStyle style)
        {
            switch (method)
            {
                case PlayFabAPIMethods.GetTitleData_General:
                    var l_characterMgr = MainManager.Instance.getCharacterManager();
                    if (null == l_characterMgr)
                        return;
                    l_characterMgr.Refresh(false, (string s) => { Init(l_characterMgr); }, null);
                    break;
            }
        }

        public void Init(CharacterManager p_characterMgr)
        {
            maxUniSlots = p_characterMgr.CountCharacterClasses();
            if (maxUniSlots > 3)
                Debug.LogWarning("Currently configured to only allow 3 playable unicorn classes.");

            var classCounter = 0;
            foreach (var pair in p_characterMgr.GetAllCharacterClassDetails())
            {
                if (classCounter > 2) break;

                if (classCounter == 0)
                {
                    classButton1.details = pair.Value;
                    classButton1.nameText.text = pair.Key;
                    classButton1.icon.overrideSprite = GameController.Instance.iconManager.GetIconById(pair.Value.Icon, IconManager.IconTypes.Class);
                }
                else if (classCounter == 1)
                {
                    classButton2.details = pair.Value;
                    classButton2.nameText.text = pair.Key;
                    classButton2.icon.overrideSprite = GameController.Instance.iconManager.GetIconById(pair.Value.Icon, IconManager.IconTypes.Class);
                }
                else if (classCounter == 2)
                {
                    classButton3.details = pair.Value;
                    classButton3.nameText.text = pair.Key;
                    classButton3.icon.overrideSprite = GameController.Instance.iconManager.GetIconById(pair.Value.Icon, IconManager.IconTypes.Class);
                }
                classCounter++;
            }
        }

        void Start()
        {
            _initialPos = classGroup.position;
        }

        public void SelectClass(ClassButtonUI classButton)
        {
            foreach (ClassButtonUI tempButton in classButtonList)
            {
                if (tempButton == classButton)
                {
                    selectedButton = tempButton;
                    classAbilities.ShowClassDetails(tempButton);
                    selectedButton.OnSelect();
                    selectedButton.button.Select();
                    EventSystem.current.SetSelectedGameObject(selectedButton.gameObject, null);
                }
                else
                {
                    tempButton.OnDeselect();
                }
            }
        }

        public void DeselectClass(UnityEvent callback = null)
        {

            var activeButton = classButton3.currentState == AnimationStates.Selected ? classButton3 : null;
            activeButton = classButton2.currentState == AnimationStates.Selected ? classButton2 : activeButton;
            activeButton = classButton1.currentState == AnimationStates.Selected ? classButton1 : activeButton;

            if (activeButton != null)
            {
                activeButton.animator.OnFinished = callback;
                activeButton.currentState = AnimationStates.On;
            }
            else
            {
                if (callback != null)
                    callback.Invoke();
            }
        }
    }
}