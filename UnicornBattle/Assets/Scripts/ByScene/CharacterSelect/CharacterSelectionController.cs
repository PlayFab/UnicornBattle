using System;
using System.Collections.Generic;
using UnicornBattle.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
    public class CharacterSelectionController : MonoBehaviour
    {
        private const string TITLE_ABILITIES = "Abilities";
        private const string TITLE_ACHIEVEMENTS = "Achievements";
        private const int MAX_CHAR_NAME_LENGTH = 16;

        public int maxCharacterSlots = 0;
        public Transform defaultSlotUI;

        public ClassSelectionController classSelection;
        public ClassAbilitiesController classAbilities;
        public CharacterAchievementsController characterAchievement;

        public Text rightPanelUIBannerTitle;

        public Transform characterSlots;

        public Sprite defaultSprite;
        public Sprite selectedSprite;

        public Button startButton;
        public Button createButton;

        public CharacterSlot selectedSlot;
        public List<CharacterSlot> slotList = new List<CharacterSlot>();

        private bool isPlayerCharactersLoaded = false;
        private bool isTitleDataLoaded = false;
        private bool isPlayerInventoryLoaded = false;
        private bool isCharacterDataLoaded = false;
        private bool isUserStatsLoaded = false;
        private bool isPlayerCharactersStatsLoaded = false;

        #region Monobehaviour Methods
        void OnEnable()
        {
            PF_Bridge.OnPlayfabCallbackSuccess += HandleCallbackSuccess;

            GameController.Instance.ClearActiveCharacter();
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
                case PlayFabAPIMethods.GetTitleData_General:
                case PlayFabAPIMethods.GetAccountInfo:
                    isTitleDataLoaded |= method == PlayFabAPIMethods.GetTitleData_General;
                    isPlayerInventoryLoaded |= method == PlayFabAPIMethods.GetAccountInfo;

                    int extraCount = 0;
                    var l_inventoryMgr = MainManager.Instance.getInventoryManager();
                    if (null != l_inventoryMgr)
                        extraCount = l_inventoryMgr.CountItemsInCategory("ExtraCharacterSlot");

                    var l_characterMgr = MainManager.Instance.getCharacterManager();
                    if (l_characterMgr != null)
                        maxCharacterSlots = l_characterMgr.StartingCharacterSlots + extraCount;
                    break;

                case PlayFabAPIMethods.GetAllUsersCharacters:
                    isPlayerCharactersLoaded = true;
                    isPlayerCharactersStatsLoaded = true;
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
            if (isPlayerCharactersStatsLoaded
                && isPlayerCharactersLoaded
                && isTitleDataLoaded
                && isPlayerInventoryLoaded
                && isCharacterDataLoaded
                && isUserStatsLoaded
                && maxCharacterSlots > 0)
                Init();
        }

        void Init()
        {
            var l_characterMgr = MainManager.Instance.getCharacterManager();
            if (null == l_characterMgr) return;

            l_characterMgr.Refresh(false,
                (string s) =>
                {
                    ResetDataChecks();
                    CharacterSlot[] children = characterSlots.GetComponentsInChildren<CharacterSlot>();

                    if (maxCharacterSlots > children.Length)
                    {
                        for (var z = 0; z < maxCharacterSlots - children.Length; z++)
                        {
                            var slotTransform = Instantiate(defaultSlotUI);
                            slotTransform.SetParent(characterSlots, false);
                            var slotComponent = slotTransform.GetComponent<CharacterSlot>();
                            slotComponent.Init();
                            slotList.Add(slotComponent);
                        }
                    }

                    foreach (CharacterSlot slot in slotList)
                    {
                        slot.ClearSlot();
                    }
                    var savedCharacters = l_characterMgr.GetAllSavedCharacters();
                    var unlockedSlotCounter = 0;
                    foreach (CharacterSlot slot in slotList)
                    {
                        if (unlockedSlotCounter < maxCharacterSlots)
                        {
                            if (slot.saved == null && savedCharacters.Length > unlockedSlotCounter)
                            {
                                var character = savedCharacters[unlockedSlotCounter];
                                slot.FillSlot(character);
                            }

                            slot.isLocked = false;
                            unlockedSlotCounter++;
                        }
                        else
                        {
                            slot.isLocked = true;
                        }

                        slot.RefreshSlot();
                    }

                    // GET THE MOST RECENT USED CHARACTER 
                    if (selectedSlot == null)
                    {
                        foreach (var slot in slotList)
                        {
                            string rc = l_characterMgr.GetMostRecentlyUsedCharacterId();

                            if (slot.saved != null && slot.saved.CharacterId == rc)
                            {
                                selectedSlot = slot;
                                break;
                            }
                        }
                    }

                    if (selectedSlot == null)
                    {
                        // DEFAULT SELECTED SLOT
                        selectedSlot = slotList[0];
                        selectedSlot.SelectSlot();
                        SelectSlot(slotList[0]);
                    }
                    else
                    {
                        SelectSlot(selectedSlot);
                        selectedSlot.SelectSlot();
                    }

                    startButton.onClick.RemoveAllListeners();
                    startButton.onClick.AddListener(Continue);

                    if (maxCharacterSlots < slotList.Count || maxCharacterSlots > savedCharacters.Length)
                    {
                        createButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        createButton.gameObject.SetActive(false);
                    }
                },
                (e) =>
                {
                    Debug.LogError("CharacterSelectionController.Init() error: " + e);
                }
            );
        }
        void ResetDataChecks()
        {
            isPlayerCharactersLoaded = false;
            isTitleDataLoaded = false;
            isPlayerInventoryLoaded = false;
            isCharacterDataLoaded = false;
            isUserStatsLoaded = false;
        }

        #region UI Control	
        public void SelectSlot(CharacterSlot cSlot)
        {
            if (selectedSlot == null)
            {
                selectedSlot = cSlot;
                if (cSlot.saved != null)
                    ShowCharacterAchievements();
            }

            if (selectedSlot == cSlot)
            {
                if (cSlot.saved == null) ShowCharacterAbilities();
                else
                {
                    ShowCharacterAchievements();
                    return;
                }

            }

            foreach (Transform child in characterSlots)
            {
                if (child.gameObject == cSlot.gameObject)
                {
                    child.GetComponent<CharacterSlot>().SelectSlot();
                    selectedSlot = cSlot;

                    if (cSlot.saved == null)
                    {
                        ShowCharacterAbilities();

                        int randomInt = UnityEngine.Random.Range(0, 3);

                        ClassButtonUI tClassButton = classSelection.classButtonList[randomInt];
                        classSelection.SelectClass(tClassButton);
                    }
                    else
                    {
                        selectedSlot.saved = cSlot.saved;
                        ShowCharacterAchievements();
                    }
                }
                else
                {
                    child.GetComponent<CharacterSlot>().DeselectSlot();
                }
            }
        }

        public void DeleteCharacter()
        {
            Action<bool> processResponse = (bool response) =>
            {
                if (response && selectedSlot != null)
                {
                    var l_characterMgr = MainManager.Instance.getCharacterManager();
                    if (null == l_characterMgr) return;

                    l_characterMgr.DeleteCharacter(selectedSlot.saved.CharacterId,
                        (r) =>
                        {
                            PF_Bridge.RaiseCallbackSuccess(r, PlayFabAPIMethods.DeleteCharacter);
                        },
                        (e) =>
                        {
                            PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.DeleteCharacter);
                        });
                }
            };

            DialogCanvasController.RequestConfirmationPrompt(GlobalStrings.DEL_CHAR_PROMPT, string.Format(GlobalStrings.DEL_CHAR_MSG, selectedSlot.unicornName.text), processResponse);
        }

        public void CreateOnClick()
        {
            int slotIndex = -1;
            if (selectedSlot.saved != null)
            {
                foreach (CharacterSlot slot in slotList)
                {
                    if (slot.saved == null && !slot.isLocked)
                    {
                        SelectSlot(slot);
                        break;
                    }
                }
                for (int i = 0; i < maxCharacterSlots; i++)
                {
                    CharacterSlot temp = slotList[i];
                    if (temp.saved == null)
                    {
                        slotIndex = i;
                        SelectSlot(slotList[slotIndex]);
                        break;
                    }
                }
                if (slotIndex < 0)
                {
                    InventoryManager l_inventoryMgr = MainManager.Instance.getInventoryManager();

                    int gemCurrencyAmount = l_inventoryMgr.GetCurrencyAmount(GlobalStrings.GEM_CURRENCY);
                    int slotCost = 5;
                    // update slot cost to be dynamic
                    if (gemCurrencyAmount < slotCost)
                    {
                        DialogCanvasController.RequestStatusPrompt(GlobalStrings.LOADING_MSG, GlobalStrings.INSUFFICIENT_GEM_SLOT_MSG, () => { });
                    }
                    else
                    {
                        BuyAdditionalSlots();
                    }
                }
            }
            else
            {
                CreateCharacter(classSelection.selectedButton.details.CatalogCode);
            }

        }

        public void CreateCharacter(string classID)
        {
            Action<string> processResponse = (string response) =>
            {
                if (response != null)
                {
                    var l_characterMgr = MainManager.Instance.getCharacterManager();
                    if (null == l_characterMgr) return;

                    l_characterMgr.CreateNewCharacter(response, classID,
                        (string r) =>
                        {
                            PF_Bridge.RaiseCallbackSuccess(r, PlayFabAPIMethods.GrantCharacterToUser);
                        }, (string e) =>
                        {
                            PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GrantCharacterToUser);
                        }
                    );
                }
            };

            string[] randomNames = {
                "Sparkle",
                "Twinkle",
                "PowerHoof",
                "Nova",
                "Lightning",
                "ReignBow",
                "Charlie",
                "Starry Sky",
                "Glitterbomb",
                "Sparkler",
                "Sliver Lining",
                "Confetti",
                "Rainbow Sprinkle",
                "Trueheart",
                "Starseeker",
                "Raindance",
                "Cloud Hop",
                "Gold Locket",
                "Lovely Dream",
                "Butterfly Kisses",
                "Sugar Cube",
                "Amora",
                "Sunnyside",
                "Pretty Pastel",
                "Cupcake",
                "Starlight",
                "Dandelion Puff",
                "Daydream",
                "Moonlight",
                "Lollipop",
                "Star Shimmer",
                "Amalthea",
                "Starswirl",
                "Galaxi",
                "Sunshine",
                "Cutie Pie",
                "Whimsy",
                "Honey Blossom",
                "Peaches",
                "Diamond Eyes",
                "Tinsel",
                "Lucky Clover",
                "Mystery",
                "Lilac",
                "Prancer",
                "Dainty Mane",
                "Wishing Star",
                "Lilac Breeze",
                "Sweet Harmony",
                "Tulip",
                "Miss Daisy",
                "Lightning Strike",
                "Crystal Prism"
            };

            var rng = UnityEngine.Random.Range(0, randomNames.Length);

            DialogCanvasController.RequestTextInputPrompt(GlobalStrings.CHAR_NAME_PROMPT, GlobalStrings.CHAR_NAME_MSG, processResponse, randomNames[rng], MAX_CHAR_NAME_LENGTH);
        }

        public void BuyAdditionalSlots()
        {
            // User cannot have more slots than the slots available in the list
            if (slotList.Count > maxCharacterSlots)
            {
                ResetDataChecks();
                DialogCanvasController.RequestStore("Character Slot Store", true);
            }
            else
            {
                DialogCanvasController.RequestStatusPrompt(GlobalStrings.LOADING_MSG, "Cannot buy more slot", () => { });
            }
        }

        public void Continue()
        {
            GameController.Instance.SetActiveCharacter(selectedSlot.saved.CharacterId);
            SceneController.Instance.RequestSceneChange(SceneController.GameScenes.Profile, 0f);
        }

        #endregion

        #region MISC
        public void ShowCharacterAchievements()
        {
            rightPanelUIBannerTitle.text = TITLE_ACHIEVEMENTS;
            classSelection.gameObject.SetActive(false);
            classAbilities.gameObject.SetActive(false);
            characterAchievement.gameObject.SetActive(true);
            characterAchievement.Init();

            startButton.interactable = true;
            TweenScale.Tween(startButton.gameObject, .5f, new Vector3(1, 1, 1), new Vector3(1.1f, 1.1f, 1.1f), TweenMain.Style.PingPong, TweenMain.Method.Linear, null);
        }

        public void ShowCharacterAbilities()
        {
            rightPanelUIBannerTitle.text = TITLE_ABILITIES;
            classSelection.gameObject.SetActive(true);
            classAbilities.gameObject.SetActive(true);
            characterAchievement.gameObject.SetActive(false);

            startButton.interactable = false;
            TweenScale.Tween(startButton.gameObject, .5f, new Vector3(1, 1, 1), new Vector3(1f, 1f, 1f), TweenMain.Style.Once, TweenMain.Method.Linear, null);
        }

        private int GetSelectedSlotIndex()
        {
            for (int i = 0; i < slotList.Count; i++)
            {
                if (slotList[i] == selectedSlot) return i;
            }
            return -1;
        }

        #endregion
    }
}