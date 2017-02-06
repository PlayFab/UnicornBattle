using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterPicker : MonoBehaviour
{
    public int maxCharacterSlots = 0;
    public Transform defaultSlotUI;

    public ArrowPickerController pPicker;
    public CharacterAchievementsController pickedPonyDetails;

    public Transform slots;

    public Sprite defaultSprite;
    public Sprite selectedSprite;

    public Button confirmButton;

    public CharacterSlot selectedSlot;
    public List<CharacterSlot> slotObjects = new List<CharacterSlot>();

    private bool isPlayerCharatersLoaded = false;
    private bool isTitleDataLoaded = false;
    private bool isPlayerInventoryLoaded = false;
    private bool isCharacterDataLoaded = false;
    private bool isUserStatsLoaded = false;

    #region Monobehaviour Methods
    void OnEnable()
    {
        PF_Bridge.OnPlayfabCallbackSuccess += HandleCallbackSuccess;
        PF_PlayerData.ClearActiveCharacter();
        maxCharacterSlots = 0;
    }

    void OnDisable()
    {
        PF_Bridge.OnPlayfabCallbackSuccess -= HandleCallbackSuccess;
    }

    public void HandleCallbackSuccess(string details, PlayFabAPIMethods method, MessageDisplayStyle style)
    {
        switch (method)
        {
            case PlayFabAPIMethods.GetTitleData:
            case PlayFabAPIMethods.GetAccountInfo:
                isTitleDataLoaded |= method == PlayFabAPIMethods.GetTitleData;
                isPlayerInventoryLoaded |= method == PlayFabAPIMethods.GetAccountInfo;

                int extraCount = 0;
                InventoryCategory temp;
                if (PF_PlayerData.inventoryByCategory != null && PF_PlayerData.inventoryByCategory.TryGetValue("ExtraCharacterSlot", out temp))
                    extraCount = temp.count;
                maxCharacterSlots = PF_GameData.StartingCharacterSlots + extraCount;
                break;

            case PlayFabAPIMethods.GetAllUsersCharacters:
                isPlayerCharatersLoaded = true;
                PF_PlayerData.GetCharacterData();
                PF_PlayerData.GetCharacterStatistics();
                break;

            case PlayFabAPIMethods.DeleteCharacter:
                ResetDataChecks();
                GameController.CharacterSelectDataRefresh();
                break;

            case PlayFabAPIMethods.GrantCharacterToUser:
                ResetDataChecks();
                GameController.CharacterSelectDataRefresh();
                break;

            case PlayFabAPIMethods.GetCharacterReadOnlyData:
                isCharacterDataLoaded = true;
                break;

            case PlayFabAPIMethods.GetUserStatistics:
                isUserStatsLoaded = true;
                break;
        }
        CheckToInit();

    }
    #endregion

    void CheckToInit()
    {
        if (isPlayerCharatersLoaded && isTitleDataLoaded && isPlayerInventoryLoaded && isCharacterDataLoaded && isUserStatsLoaded && maxCharacterSlots > 0)
        {
            Debug.Log("CharSelect Check Passed!");
            Init();
        }
    }

    void Init()
    {
        ResetDataChecks();

        CharacterSlot[] children = slots.GetComponentsInChildren<CharacterSlot>();

        if (maxCharacterSlots > children.Length)
        {
            for (var z = 0; z < maxCharacterSlots - children.Length; z++)
            {
                var slotTransform = Instantiate(defaultSlotUI);
                slotTransform.SetParent(slots, false);
                var slotCmp = slotTransform.GetComponent<CharacterSlot>();
                slotCmp.Init();
                slotObjects.Add(slotCmp);
            }
        }

        foreach (var slot in slotObjects)
            slot.ClearSlot();

        var slotCount = 0;
        foreach (var character in PF_PlayerData.playerCharacters)
        {
            foreach (var slot in slotObjects)
            {
                if (slot.saved == null)
                {
                    //Fill slot 
                    if (PF_GameData.Classes.ContainsKey(character.CharacterType))
                    {
                        slot.FillSlot(new UB_SavedCharacter()
                        {
                            baseClass = PF_GameData.Classes[character.CharacterType],
                            characterDetails = character,
                            characterData = PF_PlayerData.playerCharacterData.ContainsKey(character.CharacterId) ? PF_PlayerData.playerCharacterData[character.CharacterId] : null
                        });
                        slotCount++;
                        break;
                    }
                }
            }
        }

        // -1 for using index
        if (slotCount < maxCharacterSlots)
            for (var z = slotCount; z < maxCharacterSlots; z++)
                slotObjects[z].ClearSlot();

        for (var z = maxCharacterSlots; z < slotObjects.Count; z++)
            slotObjects[z].LockSlot();

        if (selectedSlot != null && selectedSlot.saved != null)
            ShowPickedPonyDetails();
        else
            HidePickedPonyDetails();

        SelectSlot(slotObjects[0]);

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(Continue);
    }

    void ResetDataChecks()
    {
        isPlayerCharatersLoaded = false;
        isTitleDataLoaded = false;
        isPlayerInventoryLoaded = false;
        isCharacterDataLoaded = false;
        isUserStatsLoaded = false;
    }

    #region UI Control	
    public void SelectSlot(CharacterSlot go)
    {
        if (selectedSlot != null && go.gameObject != selectedSlot.gameObject)
        {
            pPicker.DeselectArrows();
            pickedPonyDetails.gameObject.SetActive(false);
        }
        else if (selectedSlot == null)
        {
            pPicker.DeselectArrows();
        }

        foreach (Transform child in slots)
        {
            if (child.gameObject == go.gameObject)
            {
                child.GetComponent<CharacterSlot>().SelectSlot();
                selectedSlot = go;

                // need to adjust the arrrows here to be in sync with the saved chars
                if (selectedSlot.saved == null)
                {
                    int rng = UnityEngine.Random.Range(0, 3);
                    ArrowUI slot;

                    if (rng == 0)
                    {
                        slot = pPicker.Arrow1;
                    }
                    else if (rng == 1)
                    {
                        slot = pPicker.Arrow2;
                    }
                    else
                    {
                        slot = pPicker.Arrow3;
                    }

                    pPicker.selectedSlot = null;
                    pPicker.SelectSlot(slot);
                    HidePickedPonyDetails();
                }
                else
                {
                    var ponyType = (PF_PlayerData.PlayerClassTypes)Enum.Parse(typeof(PF_PlayerData.PlayerClassTypes), selectedSlot.saved.baseClass.CatalogCode);

                    var afterSelect = new UnityEvent();
                    afterSelect.AddListener(ShowPickedPonyDetails);

                    switch ((int)ponyType)
                    {
                        case 0:
                            pPicker.SelectArrow(0, afterSelect);
                            pPicker.DisablePulsingButtons();
                            break;
                        case 1:
                            pPicker.SelectArrow(1, afterSelect);
                            pPicker.DisablePulsingButtons();
                            break;
                        case 2:
                            pPicker.SelectArrow(2, afterSelect);
                            pPicker.DisablePulsingButtons();
                            break;
                        default:
                            Debug.LogWarning("Unknown Class type detected...");
                            break;
                    }
                }
            }
            else
            {
                child.GetComponent<CharacterSlot>().DeselectSlot();
            }
        }

        if (selectedSlot == null || selectedSlot.saved != null)
            ArrowPickerController.instance.DisablePulsingButtons();
        else
            ArrowPickerController.instance.EnablePulsingButtons();
    }

    public void ShowPonyPicker()
    {
        pPicker.TurnOnArrows();
    }

    public void ShowPickedPonyDetails()
    {
        pickedPonyDetails.gameObject.SetActive(true);
        pickedPonyDetails.Init();

        TweenScale.Tween(confirmButton.gameObject, .5f, new Vector3(1, 1, 1), new Vector3(1.1f, 1.1f, 1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);
    }

    public void HidePickedPonyDetails()
    {
        pickedPonyDetails.gameObject.SetActive(false);
    }

    public void DeleteCharacter()
    {
        Action<bool> processResponse = (bool response) =>
        {
            if (response && selectedSlot != null)
            {
                PF_PlayerData.DeleteCharacter(selectedSlot.saved.characterDetails.CharacterId);
                selectedSlot = null;
            }
        };

        DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.DEL_CHAR_PROMPT, string.Format(GlobalStrings.DEL_CHAR_MSG, selectedSlot.ponyName.text), processResponse);
    }

    public void PonyPicked(ArrowUI pony)
    {
        Action<string> processResponse = (string response) =>
        {
            if (response != null)
            {
                //ShowButtons();
                PF_PlayerData.CreateNewCharacter(response, pony.details);
            }
        };

        string[] randomNames = { "Sparkle", "Twinkle", "PowerHoof", "Nova", "Lightning", "ReignBow", "Charlie", "Balloonicorn", "Roach" };
        var rng = UnityEngine.Random.Range(0, randomNames.Length);

        DialogCanvasController.RequestTextInputPrompt(GlobalStrings.CHAR_NAME_PROMPT, GlobalStrings.CHAR_NAME_MSG, processResponse, randomNames[rng]);
    }


    public void BuyAdditionalSlots()
    {
        ResetDataChecks();
        DialogCanvasController.RequestStore("Character Slot Store");
    }


    public void Continue()
    {
        PF_PlayerData.activeCharacter = selectedSlot.saved;
        PF_PlayerData.activeCharacter.SetMaxVitals();

        SceneController.Instance.RequestSceneChange(SceneController.GameScenes.Profile, 0f);
    }
    #endregion
}
