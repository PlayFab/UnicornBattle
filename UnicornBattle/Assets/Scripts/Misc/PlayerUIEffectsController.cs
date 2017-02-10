using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using PlayFab.Json;

public class PlayerUIEffectsController : MonoBehaviour
{
    public TweenPos shaker;
    public float defaultShakeTime = .333f;
    public bool isShaking = false;

    public PlayerActionBarController ActionBar;
    public PlayerEncounterInputController EncounterBar;
    public CalloutController playerAction;

    public GameplayController gameplayController;

    // UI ELEMENTS 
    public Image CreepEncountersImage;
    public Text CreepEncountersText;
    public Image HeroEncountersImage;
    public Text HeroEncountersText;
    public Image GoldCollectedImage;
    public Text GoldCollecteText;
    public Image ItemsCollectedImage;
    public Text ItemsCollectedText;
    public FillBarController LifeBar;
    public FillBarController ManaBar;

    public Text PlayerLevel;
    public Text PlayerName;
    public Image PlayerIcon;
    public Text LivesCount;

    private int pendingValue = 0;

    void OnEnable()
    {
        GameplayController.OnGameplayEvent += OnGameplayEventReceived;
    }

    void OnDisable()
    {
        GameplayController.OnGameplayEvent -= OnGameplayEventReceived;
    }

    void OnGameplayEventReceived(string message, PF_GamePlay.GameplayEventTypes type)
    {

        if (type == PF_GamePlay.GameplayEventTypes.IntroQuest)
        {
            ActionBar.UpdateSpellBar();
            Init();
        }

        if (type == PF_GamePlay.GameplayEventTypes.IntroEncounter)
            StartCoroutine(PF_GamePlay.Wait(.5f, () => { TransitionEncounterBarIn(); }));

        if (type == PF_GamePlay.GameplayEventTypes.PlayerTurnBegins)
            TransitionActionBarIn();
    }

    public void Init()
    {
        UpdateQuestStats();
        LifeBar.maxValue = PF_PlayerData.activeCharacter.PlayerVitals.Health;
        StartCoroutine(LifeBar.UpdateBar(PF_PlayerData.activeCharacter.PlayerVitals.Health, true));
    }

    public void TakeDamage(int dmg)
    {
        pendingValue = dmg;
        RequestShake(defaultShakeTime, PF_GamePlay.ShakeEffects.DecreaseHealth);
        PF_PlayerData.activeCharacter.PlayerVitals.Health -= dmg;
    }


    public void Callout(Sprite sprite, string message, UnityAction callback)
    {
        TransitionActionBarOut();
        playerAction.actionIcon.overrideSprite = sprite;
        playerAction.actionText.text = message;
        playerAction.CastSpell(callback);
    }


    public void RequestShake(float seconds, PF_GamePlay.ShakeEffects effect)
    {
        if (!isShaking)
        {
            shaker.enabled = true;
            StartCoroutine(Shake(seconds, effect));
        }
    }

    public void StartEncounterInput(PF_GamePlay.PlayerEncounterInputs input)
    {
        switch (input)
        {
            case PF_GamePlay.PlayerEncounterInputs.Attack:
                TransitionEncounterBarOut(() => { TransitionActionBarIn(); });
                break;

            case PF_GamePlay.PlayerEncounterInputs.UseItem:
                UseItem();
                break;

            case PF_GamePlay.PlayerEncounterInputs.Evade:
                TransitionActionBarOut();
                // this causes a bug on boss types (cant evade) (should remove this option on un-evadable encounters)
                gameplayController.turnController.Evade();
                break;

            case PF_GamePlay.PlayerEncounterInputs.ViewStore:
                string storeId;
                gameplayController.turnController.currentEncounter.Data.EncounterActions.TryGetValue(GlobalStrings.ENCOUNTER_STORE, out storeId);
                if (!string.IsNullOrEmpty(storeId))
                    DialogCanvasController.RequestStore(storeId);
                else
                    Debug.LogError("No store found for merchant");
                break;

            case PF_GamePlay.PlayerEncounterInputs.Rescue:
                gameplayController.turnController.CompleteEncounter();
                break;
        }
    }

    public void UpdateQuestStats()
    {
        if (PF_GamePlay.QuestProgress != null)
        {
            CreepEncountersText.text = string.Format("{0} / {1}", PF_GamePlay.QuestProgress.CreepEncounters, PF_GamePlay.QuestProgress.CreepEncounters > 0 ? PF_GamePlay.encounters.Count + PF_GamePlay.QuestProgress.CreepEncounters - 1 : PF_GamePlay.encounters.Count);
            GoldCollecteText.text = string.Format("x{0:n0}", PF_GamePlay.QuestProgress.GoldCollected);
            ItemsCollectedText.text = string.Format("x{0}", PF_GamePlay.QuestProgress.ItemsFound.Count);
            HeroEncountersText.text = string.Format("x{0}", PF_GamePlay.QuestProgress.HeroRescues);
        }

        if (PF_PlayerData.activeCharacter != null)
        {
            PlayerIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(PF_PlayerData.activeCharacter.baseClass.Icon);
            LivesCount.text = "" + PF_PlayerData.virtualCurrency[GlobalStrings.HEART_CURRENCY];
            PlayerLevel.text = "" + PF_PlayerData.activeCharacter.characterData.CharacterLevel;
            PlayerName.text = PF_PlayerData.activeCharacter.characterDetails.CharacterName;
        }
    }

    public void TransitionEncounterBarIn(UnityAction cb = null)
    {
        if (cb != null)
            cb();
    }

    public void TransitionEncounterBarOut(UnityAction cb = null)
    {
        if (cb != null)
            cb();
    }

    public void TransitionActionBarIn(UnityAction cb = null)
    {
        ActionBar.FleeButton.interactable = gameplayController.turnController.currentEncounter.Data.EncounterType != EncounterTypes.BossCreep;

        Text txt = ActionBar.UseItemButton.GetComponentInChildren<Text>();
        Image img = ActionBar.UseItemButton.GetComponent<Image>();
        ActionBar.UseItemButton.interactable = true;
        if (gameplayController.turnController.currentEncounter.Data.EncounterType == EncounterTypes.Store)
        {
            // open store
            txt.text = "View Store";
            img.color = Color.green;

            ActionBar.UseItemButton.onClick.RemoveAllListeners();
            ActionBar.UseItemButton.onClick.AddListener(() =>
            {
                StartEncounterInput(PF_GamePlay.PlayerEncounterInputs.ViewStore);
            });

        }
        else if (gameplayController.turnController.currentEncounter.Data.EncounterType == EncounterTypes.Hero)
        {
            // rescue
            txt.text = "Rescue";
            img.color = Color.magenta;

            ActionBar.UseItemButton.onClick.RemoveAllListeners();
            ActionBar.UseItemButton.onClick.AddListener(() =>
            {
                StartEncounterInput(PF_GamePlay.PlayerEncounterInputs.Rescue);
            });
        }
        else
        {
            // use item
            txt.text = "Use Item";
            img.color = Color.blue;

            ActionBar.UseItemButton.onClick.RemoveAllListeners();
            ActionBar.UseItemButton.onClick.AddListener(() =>
            {
                StartEncounterInput(PF_GamePlay.PlayerEncounterInputs.UseItem);
            });
        }

        var isCreep = gameplayController.turnController.currentEncounter.Data.EncounterType.ToString().Contains("Creep");
        ActionBar.Spell1Button.SpellButton.interactable = !ActionBar.Spell1Button.isLocked && !ActionBar.Spell1Button.isOnCD && isCreep;
        ActionBar.Spell2Button.SpellButton.interactable = !ActionBar.Spell2Button.isLocked && !ActionBar.Spell2Button.isOnCD && isCreep;
        ActionBar.Spell3Button.SpellButton.interactable = !ActionBar.Spell3Button.isLocked && !ActionBar.Spell3Button.isOnCD && isCreep;
    }

    public void TransitionActionBarOut(UnityAction cb = null)
    {
        ActionBar.Spell1Button.SpellButton.interactable = false;
        ActionBar.Spell2Button.SpellButton.interactable = false;
        ActionBar.Spell3Button.SpellButton.interactable = false;
        ActionBar.FleeButton.interactable = false;
        ActionBar.UseItemButton.interactable = false;
    }

    IEnumerator Shake(float seconds, PF_GamePlay.ShakeEffects effect = PF_GamePlay.ShakeEffects.None)
    {
        yield return new WaitForSeconds(seconds);

        shaker.ResetToBeginning();     //new Vector3 (0,transform.position.y, transform.position.z);
        isShaking = false;
        shaker.enabled = false;

        if (effect == PF_GamePlay.ShakeEffects.DecreaseHealth)
        {
            int remainingHP = LifeBar.currentValue - pendingValue;
            yield return StartCoroutine(LifeBar.UpdateBar(remainingHP));

            if (remainingHP > 0)
                StartCoroutine(PF_GamePlay.Wait(1.0f, () => { GameplayController.RaiseGameplayEvent(GlobalStrings.ENEMY_TURN_END_EVENT, PF_GamePlay.GameplayEventTypes.EnemyTurnEnds); }));
            else
                GameplayController.RaiseGameplayEvent(GlobalStrings.OUTRO_PLAYER_DEATH_EVENT, PF_GamePlay.GameplayEventTypes.OutroEncounter);
        }
        else if (effect == PF_GamePlay.ShakeEffects.IncreaseHealth)
        {
            int remainingHP = LifeBar.currentValue + pendingValue;
            yield return StartCoroutine(LifeBar.UpdateBar(remainingHP));

            StartCoroutine(PF_GamePlay.Wait(.5f, () => { GameplayController.RaiseGameplayEvent(GlobalStrings.PLAYER_TURN_END_EVENT, PF_GamePlay.GameplayEventTypes.PlayerTurnEnds); }));
        }
        else if (effect == PF_GamePlay.ShakeEffects.DecreaseMana)
        {
            StartCoroutine(ManaBar.UpdateBar(LifeBar.currentValue - pendingValue));
        }
        else if (effect == PF_GamePlay.ShakeEffects.IncreaseMana)
        {
            StartCoroutine(ManaBar.UpdateBar(LifeBar.currentValue + pendingValue));
        }

        pendingValue = 0;
    }

    public void UseItem()
    {
        Action<string> afterPickItem = (string item) =>
        {
            Debug.Log("Using " + item);

            InventoryCategory obj;
            if (!PF_PlayerData.inventoryByCategory.TryGetValue(item, out obj) || obj.count == 0)
                return;

            var attributes = JsonWrapper.DeserializeObject<Dictionary<string, string>>(obj.catalogRef.CustomData);
            if (!attributes.ContainsKey("modifies")
              || !attributes.ContainsKey("modifyPercent")
              || !attributes.ContainsKey("target")
              || !string.Equals(attributes["target"], "self"))
                return;

            // item effect applies to the player
            var mod = attributes["modifies"];
            var modPercent = float.Parse(attributes["modifyPercent"]);

            switch (mod)
            {
                case "HP":
                    pendingValue = Mathf.CeilToInt((float)LifeBar.maxValue * modPercent);
                    Debug.Log(string.Format("Player Heals {0}", pendingValue));
                    PF_PlayerData.activeCharacter.PlayerVitals.Health += pendingValue;
                    RequestShake(defaultShakeTime, PF_GamePlay.ShakeEffects.IncreaseHealth);
                    break;
            }

            gameplayController.DecrementPlayerCDs();
            PF_GamePlay.ConsumeItem(obj.inventory[0].ItemInstanceId);
            PF_GamePlay.QuestProgress.ItemsUsed++;
        };

        DialogCanvasController.RequestInventoryPrompt(afterPickItem, DialogCanvasController.InventoryFilters.UsableInCombat);
    }
}
