using System.Collections.Generic;
//using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelPicker : MonoBehaviour
{
    public Transform levelButtonPrefab;
    public Transform overworldSceneObject;
    public Transform gridView;

    public Text selectedQuestName;
    public Text selectedQuestDesc;

    public Button HardMode;
    public Button RaidMode;

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

    public void HandleCallbackError(string details, PlayFabAPIMethods method, MessageDisplayStyle style)
    {
    }

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

        PF_GamePlay.encounters = BuildEncounterList();
        PF_GamePlay.ActiveQuest = selectedLevel.GetLevelItemData();
        SceneController.Instance.RequestSceneChange(SceneController.GameScenes.Gameplay, .333f);
        ResetDataChecks();
    }

    public List<UB_GamePlayEncounter> BuildEncounterList()
    {
        if (PF_GameData.Encounters.Count > 0)
        {
            // need to change this list to have another type of class, but for now this will do.
            // TODO add markers for end / beginning of acts (will usually be with boss)
            List<UB_GamePlayEncounter> outputEncounters = new List<UB_GamePlayEncounter>();

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

    private static void AddEncountersFromPool(UB_LevelEncounters encounterData, ref List<UB_GamePlayEncounter> outputEncounters)
    {
        var poolKey = encounterData.EncounterPool;
        Dictionary<string, UB_EncounterData> encounterPool;
        if (!PF_GameData.Encounters.TryGetValue(poolKey, out encounterPool))
            return;

        // used to access these by index
        var creepCount = encounterData.MinQuantity;
        var nameList = new List<string>();
        nameList.AddRange(encounterPool.Keys);
        var encounterList = new List<UB_EncounterData>();
        encounterList.AddRange(encounterPool.Values);

        // try to add the specific creeps
        foreach (var id in encounterData.SpawnSpecificEncountersByID)
        {
            // start back here start with creating the new()s
            if (!encounterPool.ContainsKey(id))
                continue;

            outputEncounters.Add(new UB_GamePlayEncounter { DisplayName = id, Data = new UB_EncounterData(encounterPool[id]) });
            creepCount--;
        }

        // Fill up the rest with randoms
        while (creepCount > 0)
        {
            var rng = Random.Range(0, encounterPool.Count);
            outputEncounters.Add(new UB_GamePlayEncounter { DisplayName = nameList[rng], Data = new UB_EncounterData(encounterList[rng]) });
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

    public void ToggleRaidMode()
    {
        PF_GamePlay.UseRaidMode = !PF_GamePlay.UseRaidMode;
        SetCheckBoxSprite(RaidMode.GetComponent<Image>(), PF_GamePlay.UseRaidMode);
    }

    public void ToggleHardMode()
    {
        PF_GamePlay.isHardMode = !PF_GamePlay.isHardMode;
        SetCheckBoxSprite(HardMode.GetComponent<Image>(), PF_GamePlay.isHardMode);
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

        selectedQuestName.text = selectedLevel.levelName;
        selectedQuestDesc.text = selectedLevel.levelData.Description;

        Dictionary<string, int> charStats = PF_PlayerData.characterStatistics[PF_PlayerData.activeCharacter.characterDetails.CharacterId];
        if (PF_PlayerData.characterStatistics.Count > 0)
        {
            var hardModeAvailable = charStats.ContainsKey(selectedLevel.levelData.StatsPrefix + "Complete");
            HardMode.gameObject.SetActive(hardModeAvailable);
            if (hardModeAvailable)
                SetCheckBoxSprite(HardMode.GetComponent<Image>(), PF_GamePlay.isHardMode);
        }
        else
        {
            HardMode.gameObject.SetActive(false);
        }

        //ShowSelectedLevel();
    }

    public void DeselectLevelItems()
    {
        foreach (var item in levelItems)
            item.GetComponent<Image>().color = Color.white;
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

        PF_GameData.GetEncounterLists(encounters);
        //TODO Set difficulty level on the selected level item.
    }

    public void Init()
    {
        SetCheckBoxSprite(RaidMode.GetComponent<Image>(), PF_GamePlay.UseRaidMode);

        for (var z = 0; z < levelItems.Count; z++)
            Destroy(levelItems[z].gameObject);
        levelItems.Clear();
        if (PF_GameData.Levels.Count == 0)
            return;

        foreach (var levelData in PF_GameData.Levels)
        {
            //TODO show locked levels
            if (levelData.Value.MinEntryLevel != null && levelData.Value.MinEntryLevel.Value > PF_PlayerData.activeCharacter.characterData.CharacterLevel)
                continue; // Hide high level dungeons
            if (PF_GameData.IsEventActive(levelData.Value.RestrictedToEventKey) != PromotionType.Active)
                continue;

            var slot = Instantiate(levelButtonPrefab);
            slot.SetParent(gridView, false);
            var li = slot.GetComponent<LevelItem>();
            li.levelData = levelData.Value;
            li.levelName = levelData.Key;
            li.levelIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(levelData.Value.Icon, IconManager.IconTypes.Misc);
            levelItems.Add(li);
        }

        LevelItemClicked(levelItems[0].levelName);
    }
}

// TODO MOVE THIS TO ITS OWN FILE
// LIST EXTENTION SHUFFLE METHOD
public static class ListExtensionMethods
{
    public static void Shuffle<T>(this IList<T> list)
    {
        var rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
