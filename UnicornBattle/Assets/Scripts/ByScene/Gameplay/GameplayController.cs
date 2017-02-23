using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayController : MonoBehaviour
{
    public GameplayEnemyController enemyController;
    public PlayerUIEffectsController playerController;
    public DirectionCanvasController directionController;

    public delegate void GameplayEventHandler(string message, PF_GamePlay.GameplayEventTypes type);
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

    void OnGameplayEventReceived(string message, PF_GamePlay.GameplayEventTypes type)
    {
        switch (type)
        {
            case PF_GamePlay.GameplayEventTypes.IntroQuest:
                RaiseGameplayEvent(GlobalStrings.QUEST_START_EVENT, PF_GamePlay.GameplayEventTypes.IntroAct);
                break;
            case PF_GamePlay.GameplayEventTypes.EnemyTurnBegins:
                EnemyAttackPlayer();
                break;
        }
    }

    void Init()
    {
        //TODO this needs to set up all the dynamic data for the many components
        if (PF_GamePlay.encounters != null && PF_GamePlay.encounters.Count > 0 && PF_GamePlay.ActiveQuest != null)
        {
            if (PF_GamePlay.UseRaidMode)
                PF_GamePlay.QuestProgress = SpoofQuestResults();
            RaiseGameplayEvent(GlobalStrings.QUEST_START_EVENT, PF_GamePlay.GameplayEventTypes.IntroQuest);
        }
    }

    private QuestTracker SpoofQuestResults()
    {
        var tracker = new QuestTracker();
        tracker.CompletedEncounters = PF_GamePlay.encounters;

        var rng = Random.value;
        if (rng > .9)
        {
            tracker.DamageTaken = Random.Range(235, 976);
            tracker.Deaths = 2;
        }
        else if (rng > .1f && rng < .4f)
        {
            tracker.DamageTaken = Random.Range(235, 750);
            tracker.Deaths = 1;
        }
        else
        {
            tracker.DamageTaken = Random.Range(235, 400);
            tracker.Deaths = 0;
        }

        if (PF_GamePlay.isHardMode)
        {
            tracker.DamageTaken = Random.Range(880, 2450);
            tracker.Deaths += Random.value > .6 ? 1 : 0;
        }

        tracker.ItemsUsed = Random.Range(0, 5);

        foreach (var encounter in PF_GamePlay.encounters)
        {
            tracker.ItemsFound.AddRange(encounter.Data.Rewards.ItemsDropped);
            tracker.XpCollected += encounter.Data.Rewards.XpMin;
            tracker.GoldCollected += encounter.Data.Rewards.GoldMin;

            if (encounter.Data.EncounterType == EncounterTypes.Hero)
                tracker.HeroRescues++;

            if (encounter.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP))
                tracker.CreepEncounters++;
        }
        tracker.isQuestWon = true;

        return tracker;
    }

    public static void RaiseGameplayEvent(string message, PF_GamePlay.GameplayEventTypes type)
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

    public void UseItem()
    {
    }

    public void OpenStore()
    {
    }

    public void OpenInventory()
    {
    }

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
            fxController.PlayerTakesDamage(playerController, spellRecord.Detail.FX);
            playerController.TakeDamage(spellRecord.Detail.BaseDmg);

            // NEED TO make this decrement each turn

            // No penalty for the first attack on an ambush
            if (isAmbush != true && spellRecord.Detail.Cooldown > 0)
            {
                spellRecord.IsOnCooldown = true;
                spellRecord.CdTurns = spellRecord.Detail.Cooldown;
            }
        };

        //Make our callout
        var spellIcon = GameController.Instance.iconManager.GetIconById(spellRecord.Detail.Icon, IconManager.IconTypes.Spell);
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
