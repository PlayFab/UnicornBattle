using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestOutroController : MonoBehaviour
{
    public Image CreepEncountersImage;
    public Text CreepEncountersText;

    public Image HeroEncountersImage;
    public Text HeroEncountersText;

    public Image GoldCollectedImage;
    public Text GoldCollectedText;

    public Image LivesLostImage;
    public Text LivesLostText;

    public Image ItemsCollectedImage;
    public Text ItemsCollectedText;
    public Button ViewItems;

    public FillBarController XpBar;

    public Image QuestIcon;
    public Text QuestName;

    public Image BG;
    public Sprite winBG;
    public Sprite loseBG;

    public Button ReturnToHub;
    public Image Mastery;
    public Image LevelUp;

    public Text LivesRemaining;
    public Button TryAgain;
    public Button BuyMoreLives;

    public LevelUpOverlayController LevelUpPane;
    public Transform WinGraphics;
    public Transform LoseGraphics;
    public TweenColor colorTweener;

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
            case PlayFabAPIMethods.GetCharacterInventory:
                //var items = PF_GamePlay.QuestProgress.ItemsGranted;
                //PF_GamePlay.ActiveQuest.levelData.
                Debug.Log("Enable ViewItems Button.");
                break;
        }
    }

    public void OnReturnToHubClick()
    {
        if (PF_GamePlay.QuestProgress.isQuestWon)
        {
            Dictionary<string, object> eventData = new Dictionary<string, object>()
            {
                { "Current_Quest", PF_GamePlay.ActiveQuest.levelName },
                { "Character_ID", PF_PlayerData.activeCharacter.characterDetails.CharacterId }
            };
            PF_Bridge.LogCustomEvent(PF_Bridge.CustomEventTypes.Client_LevelComplete, eventData);
        }

        if (PF_PlayerData.activeCharacter.PlayerVitals.didLevelUp)
        {
            Dictionary<string, object> eventData = new Dictionary<string, object>()
            {
                { "New_Level", PF_PlayerData.activeCharacter.characterData.CharacterLevel + 1 },
                { "Character_ID", PF_PlayerData.activeCharacter.characterDetails.CharacterId },
                { "Current_Quest", PF_GamePlay.ActiveQuest.levelName }
            };
            PF_Bridge.LogCustomEvent(PF_Bridge.CustomEventTypes.Client_LevelUp, eventData);
        }

        // Only save if the game has been won
        // may want to add in some stats for missions failed / deaths
        if (PF_GamePlay.QuestProgress.isQuestWon)
        {
            PF_GamePlay.SavePlayerData();
            SaveStatistics();

            if (PF_GamePlay.QuestProgress.areItemsAwarded == false)
            {
                PF_GamePlay.RetriveQuestItems();
            }
        }

        var loadingDelay = .5f;
        if (PF_GamePlay.UseRaidMode)
        {
            var eventData = new Dictionary<string, object>
            {
                { "Killed_By", "Raid Mode" },
                { "Enemy_Health", "Raid Mode" },
                { "Current_Quest", PF_GamePlay.ActiveQuest.levelName },
                { "Character_ID", PF_PlayerData.activeCharacter.characterDetails.CharacterId }
            };

            for (var z = 0; z < PF_GamePlay.QuestProgress.Deaths; z++)
            {
                PF_PlayerData.SubtractLifeFromPlayer();
                PF_Bridge.LogCustomEvent(PF_Bridge.CustomEventTypes.Client_PlayerDied, eventData);
            }
        }

        GameController.Instance.sceneController.RequestSceneChange(SceneController.GameScenes.Profile, loadingDelay);
    }


    public void SaveStatistics()
    {
        var prefix = PF_GamePlay.ActiveQuest.levelData.StatsPrefix;
        var charUpdates = new Dictionary<string, int>();

        var damageDone = 0;
        var bossesKilled = 0;

        foreach (var item in PF_GamePlay.QuestProgress.CompletedEncounters)
        {
            if (item.Data.EncounterType == EncounterTypes.BossCreep)
            {
                bossesKilled++;
            }

            if (item.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP))
            {
                damageDone += item.Data.Vitals.MaxHealth;
            }
        }

        // Character Statistics Section
        charUpdates.Add(prefix + "Complete", PF_GamePlay.ActiveQuest.difficulty);
        charUpdates.Add(prefix + "Deaths", PF_GamePlay.QuestProgress.Deaths);
        charUpdates.Add(prefix + "DamageDone", damageDone);
        charUpdates.Add(prefix + "EncountersCompleted", PF_GamePlay.QuestProgress.CompletedEncounters.Count);
        charUpdates.Add(prefix + "UnicornsRescued", PF_GamePlay.QuestProgress.HeroRescues);
        charUpdates.Add(prefix + "ItemsUsed", PF_GamePlay.QuestProgress.ItemsUsed);
        charUpdates.Add(prefix + "XPGained", PF_GamePlay.QuestProgress.XpCollected);
        charUpdates.Add(prefix + "ItemsFound", PF_GamePlay.QuestProgress.ItemsFound.Count);
        charUpdates.Add(prefix + "GoldFound", PF_GamePlay.QuestProgress.GoldCollected);
        charUpdates.Add("QuestsCompleted", 1);
        charUpdates.Add("BossesKilled", bossesKilled);

        PF_PlayerData.UpdateCharacterStatistics(PF_PlayerData.activeCharacter.characterDetails.CharacterId, charUpdates);


        // User Statistics Section
        Dictionary<string, int> userUpdates = new Dictionary<string, int>();

        // Special calculation for the HighestCharacterLevel (we're pushing a delta, so we have to determine it)
        var curLevel = PF_PlayerData.activeCharacter.characterData.CharacterLevel;
        var savedLevel = 0;
        PF_PlayerData.userStatistics.TryGetValue("HighestCharacterLevel", out savedLevel);
        var levelUpdate = (Math.Max(curLevel, savedLevel) - savedLevel);

        userUpdates.Add("Total_DamageDone", damageDone);
        userUpdates.Add("Total_EncountersCompleted", PF_GamePlay.QuestProgress.CompletedEncounters.Count);
        userUpdates.Add("Total_UnicornsRescued", PF_GamePlay.QuestProgress.HeroRescues);
        userUpdates.Add("Total_ItemsUsed", PF_GamePlay.QuestProgress.ItemsUsed);
        userUpdates.Add("Total_XPGained", PF_GamePlay.QuestProgress.XpCollected);
        userUpdates.Add("Total_ItemsFound", PF_GamePlay.QuestProgress.ItemsFound.Count);
        userUpdates.Add("Total_GoldFound", PF_GamePlay.QuestProgress.GoldCollected);
        userUpdates.Add("Total_QuestsCompleted", 1);
        userUpdates.Add("Total_BossesKilled", bossesKilled);
        userUpdates.Add("HighestCharacterLevel", levelUpdate);

        PF_PlayerData.UpdateUserStatistics(userUpdates);
    }

    public void UpdateQuestStats()
    {
        //TODO update mastery stars to reflect difficulty.
        if (PF_GamePlay.QuestProgress != null)
        {
            CreepEncountersText.text = string.Format("x{0}", PF_GamePlay.QuestProgress.CreepEncounters);
            GoldCollectedText.text = string.Format("+{0:n0}", PF_GamePlay.QuestProgress.GoldCollected);
            ItemsCollectedText.text = string.Format("+{0}", PF_GamePlay.QuestProgress.ItemsFound.Count);
            HeroEncountersText.text = string.Format("x{0}", PF_GamePlay.QuestProgress.HeroRescues);
            LivesLostText.text = string.Format("- {0}", PF_GamePlay.QuestProgress.Deaths);

            if (PF_GamePlay.QuestProgress.isQuestWon)
            {
                PF_GamePlay.IntroPane(WinGraphics.gameObject, .333f, null);
                PF_GamePlay.OutroPane(LoseGraphics.gameObject, .01f, null);
                colorTweener.from = Color.blue;
                colorTweener.to = Color.magenta;
                BG.overrideSprite = winBG;

            }
            else
            {
                PF_GamePlay.IntroPane(LoseGraphics.gameObject, .333f, null);
                PF_GamePlay.OutroPane(WinGraphics.gameObject, .01f, null);
                colorTweener.from = Color.red;
                colorTweener.to = Color.yellow;
                BG.overrideSprite = loseBG;
            }
        }

        if (PF_PlayerData.activeCharacter != null && PF_GamePlay.ActiveQuest.levelIcon != null)
        {
            //PlayerIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(PF_PlayerData.activeCharacter.baseClass.Icon);
            QuestIcon.overrideSprite = PF_GamePlay.ActiveQuest.levelIcon;
            QuestName.text = PF_GamePlay.ActiveQuest.levelName;

            var balance = 0;
            LivesRemaining.text = string.Format("{0}", PF_PlayerData.virtualCurrency.TryGetValue(GlobalStrings.HEART_CURRENCY, out balance) ? balance : -1);

            var nextLevelStr = string.Format("{0}", PF_PlayerData.activeCharacter.characterData.CharacterLevel + 1);
            if (PF_GameData.CharacterLevelRamp.ContainsKey(nextLevelStr) && PF_GamePlay.QuestProgress.isQuestWon)
            {
                XpBar.maxValue = PF_GameData.CharacterLevelRamp[nextLevelStr];
                StartCoroutine(XpBar.UpdateBarWithCallback(PF_PlayerData.activeCharacter.characterData.ExpThisLevel + PF_GamePlay.QuestProgress.XpCollected, false, EvaluateLevelUp));

                ViewItems.interactable = true;
                // PlayerLevel.text = "" + PF_PlayerData.activeCharacter.characterData.CharacterLevel;
                // PlayerName.text = PF_PlayerData.activeCharacter.characterDetails.CharacterName;
            }
        }
    }

    void EvaluateLevelUp()
    {
        if (XpBar.maxValue < PF_PlayerData.activeCharacter.characterData.ExpThisLevel + PF_GamePlay.QuestProgress.XpCollected)
        {
            // Level Up!!!
            PF_PlayerData.activeCharacter.PlayerVitals.didLevelUp = true;
            PF_GamePlay.IntroPane(LevelUp.gameObject, .333f, null);

            LevelUpPane.Init();
            StartCoroutine(PF_GamePlay.Wait(1.5f, () => { PF_GamePlay.IntroPane(LevelUpPane.gameObject, .333f, null); }));
        }
    }

    public void AcceptLevelupInput(int spellNumber)
    {
        Debug.Log("Level-UP: Spell Number: " + spellNumber);
        PF_PlayerData.activeCharacter.PlayerVitals.skillSelected = spellNumber;
        PF_GamePlay.OutroPane(LevelUpPane.gameObject, .333f, null);
    }

    public void OnTryAgainClick()
    {
        //Debug.Log("Try Again not implemented");
        int hearts;
        PF_PlayerData.virtualCurrency.TryGetValue(GlobalStrings.HEART_CURRENCY, out hearts);
        if (hearts > 0)
        {
            // decrement HT currency
            hearts -= 1;
            PF_PlayerData.virtualCurrency[GlobalStrings.HEART_CURRENCY] = hearts;
            PF_PlayerData.SubtractLifeFromPlayer();
            // will need to trigger Cloud Script to tick this on the server side

            PF_PlayerData.activeCharacter.RefillVitals();

            PF_GamePlay.OutroPane(gameObject, .333f, null);
            GameplayController.RaiseGameplayEvent(GlobalStrings.PLAYER_RESPAWN_EVENT, PF_GamePlay.GameplayEventTypes.EnemyTurnEnds);
        }
    }

    public void ShowItemsFound()
    {
        DialogCanvasController.RequestItemViewer(PF_GamePlay.QuestProgress.ItemsFound);
    }

    public void OnBuyMoreLivesClick()
    {
        Debug.Log("Buy More Lives not implemented");
        //throw an error?
    }
}
