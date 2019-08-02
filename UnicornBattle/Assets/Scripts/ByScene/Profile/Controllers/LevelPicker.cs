using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnicornBattle.Managers;
using UnicornBattle.Models;

namespace UnicornBattle.Controllers
{
    public class LevelPicker : MonoBehaviour
    {
        public Transform levelButtonPrefab;
        public Transform overworldSceneObject;
        public Transform gridView;

        public Text selectedQuestName;
        public Text selectedQuestDesc;

        public Sprite checkBox;
        public Sprite checkBoxChecked;
        public Color selectedColor;

        public List<LevelItem> levelItems;

        private LevelItem selectedLevel;
        private bool isEncounterListAvailable = false;

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

        public void HandleCallbackError(string details, PlayFabAPIMethods method, MessageDisplayStyle style) { }

        public void HandleCallbackSuccess(string details, PlayFabAPIMethods method, MessageDisplayStyle style)
        {
            if (details.Contains("Encounters Loaded!") && method == PlayFabAPIMethods.GetTitleData_Specific)
                isEncounterListAvailable = true;

            CheckToContinue();
        }

        void CheckToContinue()
        {
            if (!isEncounterListAvailable)
                return;

            GameController l_gc = GameController.Instance;
            if (null == l_gc) return;

            l_gc.ActiveEncounterList = BuildEncounterList();
            l_gc.ActiveLevel = selectedLevel.GetLevelItemData();
            SceneController.Instance.RequestSceneChange(SceneController.GameScenes.Gameplay, .333f);
            ResetDataChecks();
        }

        public List<UBEncounter> BuildEncounterList()
        {
            GameDataManager l_gameDataManager = MainManager.Instance.getGameDataManager();

            if (l_gameDataManager.HasAnyEncounters())
            {
                // need to change this list to have another type of class, but for now this will do.
                // TODO add markers for end / beginning of acts (will usually be with boss)
                List<UBEncounter> outputEncounters = new List<UBEncounter>();

                foreach (var act in selectedLevel.levelData.Acts)
                {
                    AddEncountersFromPool(act.Value.CreepEncounters, ref outputEncounters);
                    AddEncountersFromPool(act.Value.MegaCreepEncounters, ref outputEncounters);
                    AddEncountersFromPool(act.Value.RareCreepEncounters, ref outputEncounters);
                    AddEncountersFromPool(act.Value.StoreEncounters, ref outputEncounters);
                    AddEncountersFromPool(act.Value.HeroEncounters, ref outputEncounters);
                    // shuffle deck here
                    outputEncounters.Shuffle();

                    AddEncountersFromPool(act.Value.BossCreepEncounters, ref outputEncounters);
                    outputEncounters[outputEncounters.Count - 1].isEndOfAct = true;
                }
                return outputEncounters;
            }
            return null;
        }

        private static void AddEncountersFromPool(UBLevelEncounters encounterData, ref List<UBEncounter> outputEncounters)
        {
            GameDataManager l_gameDataManager = MainManager.Instance.getGameDataManager();

            var poolKey = encounterData.EncounterPool;
            Dictionary<string, UBEncounterData> encounterPool = l_gameDataManager.getEncounterPool(poolKey);

            // used to access these by index
            var creepCount = encounterData.MinQuantity;
            var nameList = new List<string>();
            nameList.AddRange(encounterPool.Keys);
            var encounterList = new List<UBEncounterData>();
            encounterList.AddRange(encounterPool.Values);

            // try to add the specific creeps
            foreach (var id in encounterData.SpawnSpecificEncountersByID)
            {
                // start back here start with creating the new()s
                if (!encounterPool.ContainsKey(id))
                    continue;

                outputEncounters.Add(new UBEncounter { DisplayName = id, Data = new UBEncounterData(encounterPool[id]) });
                creepCount--;
            }

            // Fill up the rest with randoms
            while (creepCount > 0)
            {
                var rng = Random.Range(0, encounterPool.Count);
                outputEncounters.Add(new UBEncounter { DisplayName = nameList[rng], Data = new UBEncounterData(encounterList[rng]) });
                creepCount--;
            }
        }

        void ResetDataChecks()
        {
            isEncounterListAvailable = false;
        }

        private void SetCheckBoxSprite(Image image, bool isChecked)
        {
            image.overrideSprite = isChecked ? checkBoxChecked : checkBox;
        }

        public void LevelItemClicked(string levelname)
        {
            DeselectLevelItems();

            LevelItem item = null;
            foreach (var eachLevel in levelItems)
                if (eachLevel.levelName == levelname)
                    item = eachLevel;
            if (item == null || item == selectedLevel)
                return;

            selectedLevel = item;
            selectedLevel.GetComponent<Image>().color = selectedColor;
            selectedLevel.myButton.interactable = false;

            selectedQuestName.text = selectedLevel.levelName;
            selectedQuestDesc.text = selectedLevel.levelData.Description;

            //ShowSelectedLevel();
        }

        public void DeselectLevelItems()
        {
            foreach (var item in levelItems)
            {
                item.GetComponent<Image>().color = Color.white;
                item.myButton.interactable = true;
            }
        }

        public void ShowSelectedLevel()
        {
            //selectedLevelSceneObject.gameObject.SetActive(true);
            overworldSceneObject.gameObject.SetActive(false);
        }

        public void ShowOverworld()
        {
            //selectedLevelSceneObject.gameObject.SetActive(false);
            overworldSceneObject.gameObject.SetActive(true);
            selectedLevel = null;
        }

        public void PlaySelectedLevel()
        {
            //SceneController.Instance.RequestSceneChange(SceneController.GameScenes.Gameplay, .333f);
            //Init();

            // build an encounters' pools list. from these pools we can randomize an order to the encounters.
            List<string> encounters = new List<string>();

            //TODO sometimes this throws a nullref ?
            foreach (var act in selectedLevel.levelData.Acts)
            {
                if (!string.IsNullOrEmpty(act.Value.CreepEncounters.EncounterPool))
                {
                    int exists = encounters.IndexOf(act.Value.CreepEncounters.EncounterPool);
                    if (exists == -1)
                        encounters.Add(act.Value.CreepEncounters.EncounterPool);
                }

                if (!string.IsNullOrEmpty(act.Value.MegaCreepEncounters.EncounterPool))
                {
                    int exists = encounters.IndexOf(act.Value.MegaCreepEncounters.EncounterPool);
                    if (exists == -1)
                        encounters.Add(act.Value.MegaCreepEncounters.EncounterPool);
                }

                if (!string.IsNullOrEmpty(act.Value.RareCreepEncounters.EncounterPool))
                {
                    int exists = encounters.IndexOf(act.Value.RareCreepEncounters.EncounterPool);
                    if (exists == -1)
                        encounters.Add(act.Value.RareCreepEncounters.EncounterPool);
                }

                if (!string.IsNullOrEmpty(act.Value.BossCreepEncounters.EncounterPool))
                {
                    int exists = encounters.IndexOf(act.Value.BossCreepEncounters.EncounterPool);
                    if (exists == -1)
                        encounters.Add(act.Value.BossCreepEncounters.EncounterPool);
                }

                if (!string.IsNullOrEmpty(act.Value.StoreEncounters.EncounterPool))
                {
                    int exists = encounters.IndexOf(act.Value.StoreEncounters.EncounterPool);
                    if (exists == -1)
                        encounters.Add(act.Value.StoreEncounters.EncounterPool);
                }

                if (!string.IsNullOrEmpty(act.Value.HeroEncounters.EncounterPool))
                {
                    int exists = encounters.IndexOf(act.Value.HeroEncounters.EncounterPool);
                    if (exists == -1)
                        encounters.Add(act.Value.HeroEncounters.EncounterPool);
                }
            }

            GameDataManager l_gameDataManager = MainManager.Instance.getGameDataManager();
            if (null == l_gameDataManager) return;
            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetTitleData_Specific);
            l_gameDataManager.RefreshEncounterLists(
                encounters,
                (s) =>
                {
                    GameController.Instance.ClearQuestProgress();
                    PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetTitleData_Specific);
                },
                (f) => { PF_Bridge.RaiseCallbackError(f, PlayFabAPIMethods.GetTitleData_Specific); }
            );
            //TODO Set difficulty level on the selected level item.
        }

        public void Init()
        {
            PromotionsManager l_promotionsMgr = MainManager.Instance.getPromotionsManager();
            if (null == l_promotionsMgr) return;

            GameDataManager l_gameDataManager = MainManager.Instance.getGameDataManager();
            if (null == l_gameDataManager) return;

            l_gameDataManager.Refresh(true, (s) =>
            {
                UBGamePlay.UseRaidMode = true;

                for (var z = 0; z < levelItems.Count; z++)
                    Destroy(levelItems[z].gameObject);
                levelItems.Clear();

                foreach (var levelData in l_gameDataManager.GetAllLevelData())
                {
                    //TODO show locked levels
                    if (levelData.MinEntryLevel != null && levelData.MinEntryLevel.Value > GameController.Instance.ActiveCharacter.characterData.CharacterLevel)
                        continue; // Hide high level dungeons
                    if (l_promotionsMgr.IsEventActive(levelData.RestrictedToEventKey) != PromotionStatus.Active)
                        continue;

                    var slot = Instantiate(levelButtonPrefab);
                    slot.SetParent(gridView, false);
                    var li = slot.GetComponent<LevelItem>();
                    li.levelData = levelData;
                    li.levelName = levelData.Name;
                    li.levelIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(levelData.Icon, IconManager.IconTypes.Misc);
                    levelItems.Add(li);
                }

                LevelItemClicked(levelItems[0].levelName);
            });
        }
    }
}