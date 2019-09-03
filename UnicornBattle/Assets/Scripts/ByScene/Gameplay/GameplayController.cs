using System.Collections.Generic;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.Events;

namespace UnicornBattle.Controllers
{
    public class GameplayController : MonoBehaviour
    {
        public GameplayEnemyController enemyController;
        public PlayerUIEffectsController playerController;
        public DirectionCanvasController directionController;

        public delegate void GameplayEventHandler(string message, UBGamePlay.GameplayEventTypes type);
        public static event GameplayEventHandler OnGameplayEvent;

        public TurnController turnController;
        public FX_Placement fxController;

        #region Setup & UI Actions
        void OnEnable()
        {
            OnGameplayEvent += OnGameplayEventReceived;
            Init();
        }

        void OnDisable()
        {
            OnGameplayEvent -= OnGameplayEventReceived;
        }

        void OnGameplayEventReceived(string message, UBGamePlay.GameplayEventTypes type)
        {
            switch (type)
            {
                case UBGamePlay.GameplayEventTypes.IntroQuest:
                    RaiseGameplayEvent(GlobalStrings.QUEST_START_EVENT, UBGamePlay.GameplayEventTypes.IntroAct);
                    break;
                case UBGamePlay.GameplayEventTypes.EnemyTurnBegins:
                    EnemyAttackPlayer();
                    break;
            }
        }

        void Init()
        {
            //TODO this needs to set up all the dynamic data for the many components
            if (GameController.Instance.ActiveEncounterList != null
                && GameController.Instance.ActiveEncounterList.Count > 0
                && GameController.Instance.ActiveLevel != null)
            {
                if (UBGamePlay.UseRaidMode)
                    GameController.Instance.QuestProgress = SpoofQuestResults();
                RaiseGameplayEvent(GlobalStrings.QUEST_START_EVENT, UBGamePlay.GameplayEventTypes.IntroQuest);
            }
        }

        private UBQuest SpoofQuestResults()
        {
            var tracker = new UBQuest();
            tracker.CompletedEncounters = GameController.Instance.ActiveEncounterList;

            var rng = Random.value;
            if (rng >.9)
            {
                tracker.DamageTaken = Random.Range(235, 976);
                tracker.Deaths = 2;
            }
            else if (rng >.1f && rng < .4f)
            {
                tracker.DamageTaken = Random.Range(235, 750);
                tracker.Deaths = 1;
            }
            else
            {
                tracker.DamageTaken = Random.Range(235, 400);
                tracker.Deaths = 0;
            }

            if (UBGamePlay.isHardMode)
            {
                tracker.DamageTaken = Random.Range(880, 2450);
                tracker.Deaths += Random.value >.6 ? 1 : 0;
            }

            tracker.ItemsUsed = Random.Range(0, 5);

            foreach (var encounter in tracker.CompletedEncounters)
            {
                tracker.ItemsFound.AddRange(encounter.Data.Rewards.ItemsDropped);
                tracker.XpCollected += Random.Range(encounter.Data.Rewards.XpMin, encounter.Data.Rewards.XpMax);
                tracker.GoldCollected += Random.Range(encounter.Data.Rewards.GoldMin, encounter.Data.Rewards.GoldMax);

                if (encounter.Data.EncounterType == EncounterTypes.Hero)
                    tracker.HeroRescues++;

                if (encounter.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP))
                    tracker.CreepEncounters++;
            }
            tracker.isQuestWon = true;

            return tracker;
        }

        public static void RaiseGameplayEvent(string message, UBGamePlay.GameplayEventTypes type)
        {
            if (OnGameplayEvent != null)
                OnGameplayEvent(message, type);
        }
        #endregion

        public bool AreQuestGoalsComplete()
        {
            //TODO add the logic to check current progress vs the goal 
            return false;
        }

        public void UseItem() { }

        public void OpenStore() { }

        public void OpenInventory() { }

        public void EnemyAttackPlayer(bool isAmbush = false)
        {
            //randomly select a spell from the enemies spells and cast it on the player.
            var rng = 0;
            var vitals = turnController.currentEncounter.Data.Vitals;
            if (vitals.Spells.Count > 1)
            {
                var tries = new List<int>();
                do
                {
                    if (tries.Count < vitals.Spells.Count)
                    {
                        rng = Random.Range(0, vitals.Spells.Count);
                        if (tries.IndexOf(rng) == -1)
                            tries.Add(rng);
                    }
                    else
                    {
                        break;
                    }
                } while (vitals.Spells[rng].IsOnCooldown == false && tries.Count < vitals.Spells.Count);
            }

            var spellRecord = vitals.Spells[rng];

            // we will run this after the Callout animation
            UnityAction applyDamage = () =>
            {
                fxController.PlayerTakesDamage(playerController, spellRecord.FX);
                playerController.TakeDamage(spellRecord.BaseDmg);

                // NEED TO make this decrement each turn

                // No penalty for the first attack on an ambush
                if (isAmbush != true && spellRecord.Cooldown > 0)
                {
                    spellRecord.IsOnCooldown = true;
                    spellRecord.CdTurns = spellRecord.Cooldown;
                }
            };

            //Make our callout
            var spellIcon = GameController.Instance.iconManager.GetIconById(spellRecord.Icon, IconManager.IconTypes.Spell);
            enemyController.Callout(spellIcon, string.Format("{0} casts {1}", turnController.currentEncounter.DisplayName, spellRecord.SpellName), applyDamage);
        }

        public void PlayerAttacks(SpellSlot sp, bool isAmbush = false) //, bool isAmbush = false)
        {
            var spellIcon = sp.SpellIcon.overrideSprite;

            // we will run this after the Callout animation
            UnityAction applyDamage = () =>
            {
                fxController.TestFx(enemyController, sp.SpellData.FX);
                enemyController.TakeDamage(sp.SpellData.Dmg);

                DecrementPlayerCDs();

                // No penalty for the first attack on an ambush
                if (isAmbush != true)
                {
                    if (sp.SpellData.Cooldown > 0)
                    {
                        sp.isOnCD = true;
                        sp.EnableCD(sp.SpellData.Cooldown);
                    }
                }
            };

            //Make our callout
            playerController.Callout(spellIcon, string.Format("You cast {0}", sp.SpellData.SpellName), applyDamage);
        }

        public void DecrementPlayerCDs()
        {
            playerController.ActionBar.Spell1Button.DecrementCD();
            playerController.ActionBar.Spell2Button.DecrementCD();
            playerController.ActionBar.Spell3Button.DecrementCD();
        }
    }
}