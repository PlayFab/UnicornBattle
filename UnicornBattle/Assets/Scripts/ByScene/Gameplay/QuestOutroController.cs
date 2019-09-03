using System;
using System.Collections.Generic;
using UnicornBattle.Managers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
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

        [System.NonSerialized] private CharacterManager m_characterMgr;
        [System.NonSerialized] private GameDataManager m_gameDataMgr;
        [System.NonSerialized] private PlayerManager m_playerMgr;
        [System.NonSerialized] private InventoryManager m_inventoryMgr;

        protected CharacterManager characterManager
        {
            get
            {
                if (null == m_characterMgr)
                {
                    m_characterMgr = MainManager.Instance.getCharacterManager();
                }
                return m_characterMgr;
            }
        }

        protected PlayerManager playerManager
        {
            get
            {
                if (null == m_playerMgr)
                {
                    m_playerMgr = MainManager.Instance.getPlayerManager();
                }
                return m_playerMgr;
            }
        }

        protected GameDataManager gameDataManager
        {
            get
            {
                if (null == m_gameDataMgr)
                {
                    m_gameDataMgr = MainManager.Instance.getGameDataManager();
                }
                return m_gameDataMgr;
            }
        }

        protected InventoryManager inventoryManager
        {
            get
            {
                if (null == m_inventoryMgr)
                    m_inventoryMgr = MainManager.Instance.getInventoryManager();
                return m_inventoryMgr;
            }
        }

        private void OnEnable()
        {
            TelemetryManager.RecordScreenViewed(TelemetryScreenId.BattleResults);
        }

        public void OnReturnToHubClick()
        {
            var l_gc = GameController.Instance;

            var l_activeCharacter = l_gc.ActiveCharacter;
            if (null == l_activeCharacter) return;

            if (l_gc.QuestProgress.isQuestWon)
            {
                Dictionary<string, object> eventData = new Dictionary<string, object>
                    { { "Current_Quest", l_gc.ActiveLevel.levelName },
                        { "Character_ID", l_activeCharacter.CharacterId }
                    };
                TelemetryManager.RecordPlayerEvent(TelemetryEvent.Client_LevelComplete, eventData);
            }

            if (l_activeCharacter.PlayerVitals.didLevelUp)
            {
                Dictionary<string, object> eventData = new Dictionary<string, object>
                    { { "New_Level", l_activeCharacter.characterData.CharacterLevel + 1 },
                        { "Character_ID", l_activeCharacter.CharacterId },
                        { "Current_Quest", l_gc.ActiveLevel.levelName }
                    };
                TelemetryManager.RecordPlayerEvent(TelemetryEvent.Client_LevelUp, eventData);
            }

            // Only save if the game has been won
            // may want to add in some stats for missions failed / deaths
            if (l_gc.QuestProgress.isQuestWon)
            {
                DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.SavePlayerInfo);
                playerManager.SavePlayerData(
                    l_activeCharacter,
                    l_gc.QuestProgress,
                    characterManager.GetAllLevelRamps(),
                    s => PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.SavePlayerInfo),
                    f => PF_Bridge.RaiseCallbackError(f, PlayFabAPIMethods.SavePlayerInfo)
                );
                SaveStatistics();

                InventoryManager l_inventoryMgr = MainManager.Instance.getInventoryManager();

                if (l_gc.QuestProgress.areItemsAwarded == false)
                {
                    DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.RetrieveQuestItems);

                    //Debug.Log( "RetrieveQuestItems() called. ItemsFound:\n" + l_gc.QuestProgress.ItemsFound.WriteToString()) ;
                    l_inventoryMgr.RetrieveQuestItems(
                        l_gc.QuestProgress.ItemsFound,
                        (itemsAwarded) =>
                        {
                            l_gc.QuestProgress.ItemsGranted = new List<UBQuestRewardItem>(itemsAwarded);
                            l_gc.QuestProgress.areItemsAwarded = true;

                            // after items retrieved, refresh user inventory
                            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetUserInventory);
                            l_inventoryMgr.Refresh(true,
                                s2 => { PF_Bridge.RaiseCallbackSuccess(s2, PlayFabAPIMethods.GetUserInventory); },
                                f2 => { PF_Bridge.RaiseCallbackError(f2, PlayFabAPIMethods.GetUserInventory); }
                            );

                            PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.RetrieveQuestItems);
                        },
                        f => { PF_Bridge.RaiseCallbackError(f, PlayFabAPIMethods.RetrieveQuestItems); }
                    );
                }
            }

            var loadingDelay = 2f;
            // USE_RAID_MODE
            if (UBGamePlay.UseRaidMode)
            {
                var eventData = new Dictionary<string, object>
                    {
                        // 
                        { "Killed_By", "Raid Mode" },
                        // 
                        { "Enemy_Health", "Raid Mode" },
                        //
                        { "Current_Quest", l_gc.ActiveLevel.levelName },
                        // 
                        { "Character_ID", l_activeCharacter.CharacterId }
                    };

                for (var z = 0; z < l_gc.QuestProgress.Deaths; z++)
                {
                    DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.ExecuteCloudScript);

                    m_playerMgr.SubtractLifeFromPlayer(
                        l_activeCharacter.CharacterId,
                        s => { PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.ExecuteCloudScript); },
                        f => { PF_Bridge.RaiseCallbackError(f, PlayFabAPIMethods.ExecuteCloudScript); }
                    );

                    TelemetryManager.RecordPlayerEvent(TelemetryEvent.Client_PlayerDied, eventData);
                }
            }

            GameController.Instance.sceneController.RequestSceneChange(SceneController.GameScenes.Profile, loadingDelay);
        }

        public void SaveStatistics()
        {
            var l_activeCharacter = GameController.Instance.ActiveCharacter;
            var l_questProgress = GameController.Instance.QuestProgress;
            var l_activeQuest = GameController.Instance.ActiveLevel;
            var l_prefix = l_activeQuest.levelData.StatsPrefix;
            var l_charUpdates = new Dictionary<string, int>();
            var l_damageDone = 0;
            var l_bossesKilled = 0;

            foreach (var item in l_questProgress.CompletedEncounters)
            {
                if (item.Data.EncounterType == EncounterTypes.BossCreep)
                {
                    l_bossesKilled++;
                }

                if (item.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP))
                {
                    l_damageDone += item.Data.Vitals.MaxHealth;
                }
            }

            // Character Statistics Section
            l_charUpdates.Add($"{l_prefix}Complete", l_activeQuest.difficulty);
            l_charUpdates.Add($"{l_prefix}Deaths", l_questProgress.Deaths);
            l_charUpdates.Add($"{l_prefix}DamageDone", l_damageDone);
            l_charUpdates.Add($"{l_prefix}EncountersCompleted", l_questProgress.CompletedEncounters.Count);
            l_charUpdates.Add($"{l_prefix}UnicornsRescued", l_questProgress.HeroRescues);
            l_charUpdates.Add($"{l_prefix}ItemsUsed", l_questProgress.ItemsUsed);
            l_charUpdates.Add($"{l_prefix}XPGained", l_questProgress.XpCollected);
            l_charUpdates.Add($"{l_prefix}ItemsFound", l_questProgress.ItemsFound.Count);
            l_charUpdates.Add($"{l_prefix}GoldFound", l_questProgress.GoldCollected);
            l_charUpdates.Add("QuestsCompleted", 1);
            l_charUpdates.Add("BossesKilled", l_bossesKilled);

            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.UpdateCharacterStatistics);
            characterManager.UpdateCharacterStatistics(
                l_activeCharacter.CharacterId,
                l_charUpdates,
                () => PF_Bridge.RaiseCallbackSuccess("User Statistics Uploaded", PlayFabAPIMethods.UpdateCharacterStatistics),
                s => PF_Bridge.RaiseCallbackError(s, PlayFabAPIMethods.UpdateCharacterStatistics));

            // User Statistics Section
            Dictionary<string, int> userUpdates = new Dictionary<string, int>();

            // Special calculation for the HighestCharacterLevel (we're pushing a delta, so we have to determine it)
            var curLevel = l_activeCharacter.characterData.CharacterLevel;
            var savedLevel = playerManager.GetPlayerStatistic("HighestCharacterLevel");
            var levelUpdate = (Math.Max(curLevel, savedLevel) - savedLevel);

            userUpdates.Add("Total_DamageDone", l_damageDone);
            userUpdates.Add("Total_EncountersCompleted", l_questProgress.CompletedEncounters.Count);
            userUpdates.Add("Total_UnicornsRescued", l_questProgress.HeroRescues);
            userUpdates.Add("Total_ItemsUsed", l_questProgress.ItemsUsed);
            userUpdates.Add("Total_XPGained", l_questProgress.XpCollected);
            userUpdates.Add($"{l_prefix}XPGained", l_questProgress.XpCollected);
            userUpdates.Add("Total_ItemsFound", l_questProgress.ItemsFound.Count);
            userUpdates.Add("Total_GoldFound", l_questProgress.GoldCollected);
            userUpdates.Add("Total_QuestsCompleted", 1);
            userUpdates.Add("Total_BossesKilled", l_bossesKilled);
            userUpdates.Add("HighestCharacterLevel", levelUpdate);

            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.UpdateUserStatistics);
            playerManager.UpdatePlayerStatistics(
                userUpdates,
                (s) => PF_Bridge.RaiseCallbackSuccess("User Statistics Uploaded", PlayFabAPIMethods.UpdateUserStatistics),
                (f) => PF_Bridge.RaiseCallbackError(f, PlayFabAPIMethods.UpdateUserStatistics));
        }

        public void UpdateQuestStats()
        {
            gameDataManager.Refresh(false, (s) =>
            {
                //TODO update mastery stars to reflect difficulty.
                var l_questProgress = GameController.Instance.QuestProgress;

                if (l_questProgress == null) return;

                CreepEncountersText.text = $"x{l_questProgress.CreepEncounters}";
                HeroEncountersText.text = $"x{l_questProgress.HeroRescues}";
                GoldCollectedText.text = $"+{l_questProgress.GoldCollected:n0}";
                ItemsCollectedText.text = $"+{l_questProgress.ItemsFound.Count}";
                LivesLostText.text = $"- {l_questProgress.Deaths}";

                if (l_questProgress.isQuestWon)
                {
                    UBAnimator.IntroPane(WinGraphics.gameObject, .333f);
                    UBAnimator.OutroPane(LoseGraphics.gameObject, .01f);
                    colorTweener.@from = Color.blue;
                    colorTweener.to = Color.magenta;
                    BG.overrideSprite = winBG;
                }
                else
                {
                    UBAnimator.IntroPane(LoseGraphics.gameObject, .333f);
                    UBAnimator.OutroPane(WinGraphics.gameObject, .01f);
                    colorTweener.@from = Color.red;
                    colorTweener.to = Color.yellow;
                    BG.overrideSprite = loseBG;
                }

                var activeQuest = GameController.Instance.ActiveLevel;

                var activeCharacter = GameController.Instance.ActiveCharacter;
                if (activeCharacter == null || activeQuest.levelIcon == null) return;

                //PlayerIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(PF_PlayerData.activeCharacter.baseClass.Icon);
                QuestIcon.overrideSprite = activeQuest.levelIcon;
                QuestName.text = activeQuest.levelName;

                if (null != inventoryManager)
                    LivesRemaining.text = $"{inventoryManager.GetCurrencyAmount(GlobalStrings.HEART_CURRENCY)}";
                else
                    LivesRemaining.text = "-1";

                var characterData = activeCharacter.characterData;
                var nextLevelStr = $"{characterData.CharacterLevel + 1}";

                if (!l_questProgress.isQuestWon) return;

                XpBar.maxValue = characterManager.GetLevelRamp(nextLevelStr);
                XpBar.currentValue = characterData.ExpThisLevel;

                StartCoroutine(XpBar.UpdateBarWithCallback(XpBar.currentValue + l_questProgress.XpCollected, false, EvaluateLevelUp));

                ViewItems.interactable = true;
                // PlayerLevel.text = "" + PF_PlayerData.activeCharacter.characterData.CharacterLevel;
                // PlayerName.text = PF_PlayerData.activeCharacter.characterDetails.CharacterName;
            });
        }

        void EvaluateLevelUp()
        {
            var l_activeCharacter = GameController.Instance.ActiveCharacter;
            if (null == l_activeCharacter) return;

            if (XpBar.maxValue < l_activeCharacter.characterData.ExpThisLevel
                + GameController.Instance.QuestProgress.XpCollected)
            {
                // Level Up!!!
                l_activeCharacter.PlayerVitals.didLevelUp = true;
                UBAnimator.IntroPane(LevelUp.gameObject, .333f);

                LevelUpPane.Init();
                StartCoroutine(
                    UBAnimator.Wait(1.5f, () => { UBAnimator.IntroPane(LevelUpPane.gameObject, .333f); }));
            }
        }

        public void AcceptLevelupInput(int spellNumber)
        {
            var l_activeCharacter = GameController.Instance.ActiveCharacter;
            if (null == l_activeCharacter) return;

            l_activeCharacter.PlayerVitals.skillSelected = spellNumber;
            UBAnimator.OutroPane(LevelUpPane.gameObject, .333f);
        }

        public void OnTryAgainClick()
        {
            //Debug.Log("Try Again not implemented");
            int hearts = inventoryManager.GetCurrencyAmount(GlobalStrings.HEART_CURRENCY);
            if (hearts > 0)
            {
                // triggers Cloud Script to subtract life on the server side
                DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.ExecuteCloudScript);
                playerManager.SubtractLifeFromPlayer(
                    GameController.Instance.ActiveCharacter.CharacterId,
                    r => PF_Bridge.RaiseCallbackSuccess(r, PlayFabAPIMethods.ExecuteCloudScript),
                    e => PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.ExecuteCloudScript)
                );
                // refill the character's vitals
                GameController.Instance.ActiveCharacter.RefillVitals();

                UBAnimator.OutroPane(gameObject, .333f);
                GameplayController.RaiseGameplayEvent(GlobalStrings.PLAYER_RESPAWN_EVENT,
                    UBGamePlay.GameplayEventTypes.EnemyTurnEnds);
            }
        }

        public void ShowItemsFound()
        {
            GameController.Instance.QuestProgress.areItemsAwarded = true;
            DialogCanvasController.RequestItemViewer(GameController.Instance.QuestProgress.ItemsFound);
        }

        public void OnBuyMoreLivesClick()
        {
            Debug.LogWarning("Buy More Lives not implemented");
            //throw an error?
        }
    }
}