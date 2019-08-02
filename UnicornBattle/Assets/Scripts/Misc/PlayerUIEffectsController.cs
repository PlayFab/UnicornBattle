using System.Collections;
using System.Collections.Generic;
using PlayFab;
using UnicornBattle.Managers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{

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

        void OnGameplayEventReceived(string message, UBGamePlay.GameplayEventTypes type)
        {

            if (type == UBGamePlay.GameplayEventTypes.IntroQuest)
            {
                ActionBar.UpdateSpellBar();
                Init();
            }

            if (type == UBGamePlay.GameplayEventTypes.IntroEncounter)
                StartCoroutine(UBAnimator.Wait(.5f, () => { TransitionEncounterBarIn(); }));

            if (type == UBGamePlay.GameplayEventTypes.PlayerTurnBegins)
                TransitionActionBarIn();
        }

        public void Init()
        {
            UpdateQuestStats();
            LifeBar.maxValue = GameController.Instance.ActiveCharacter.PlayerVitals.Health;
            StartCoroutine(LifeBar.UpdateBar(GameController.Instance.ActiveCharacter.PlayerVitals.Health, true));
        }

        public void TakeDamage(int dmg)
        {
            pendingValue = dmg;
            RequestShake(defaultShakeTime, UBGamePlay.ShakeEffects.DecreaseHealth);
            GameController.Instance.ActiveCharacter.PlayerVitals.Health -= dmg;
        }

        public void Callout(Sprite sprite, string message, UnityAction callback)
        {
            TransitionActionBarOut();
            playerAction.actionIcon.overrideSprite = sprite;
            playerAction.actionText.text = message;
            playerAction.CastSpell(callback);
        }

        public void RequestShake(float seconds, UBGamePlay.ShakeEffects effect)
        {
            if (!isShaking)
            {
                shaker.enabled = true;
                StartCoroutine(Shake(seconds, effect));
            }
        }

        public void StartEncounterInput(UBGamePlay.PlayerEncounterInputs input)
        {
            switch (input)
            {
                case UBGamePlay.PlayerEncounterInputs.Attack:
                    TransitionEncounterBarOut(() => { TransitionActionBarIn(); });
                    break;

                case UBGamePlay.PlayerEncounterInputs.UseItem:
                    UseItem();
                    break;

                case UBGamePlay.PlayerEncounterInputs.Evade:
                    TransitionActionBarOut();
                    // this causes a bug on boss types (cant evade) (should remove this option on un-evadable encounters)
                    gameplayController.turnController.Evade();
                    break;

                case UBGamePlay.PlayerEncounterInputs.ViewStore:
                    string storeId;
                    gameplayController.turnController.currentEncounter.Data.EncounterActions.TryGetValue(GlobalStrings.ENCOUNTER_STORE, out storeId);
                    if (!string.IsNullOrEmpty(storeId))
                        DialogCanvasController.RequestStore(storeId);
                    else
                        Debug.LogError("No store found for merchant");
                    break;

                case UBGamePlay.PlayerEncounterInputs.Rescue:
                    gameplayController.turnController.CompleteEncounter();
                    break;
            }
        }

        public void UpdateQuestStats()
        {
            var l_Encounters = GameController.Instance.ActiveEncounterList;
            var l_QuestProgress = GameController.Instance.QuestProgress;
            var l_activeCharacter = GameController.Instance.ActiveCharacter;

            if (l_QuestProgress != null)
            {
                CreepEncountersText.text = string.Format("{0} / {1}", l_QuestProgress.CreepEncounters, l_QuestProgress.CreepEncounters > 0 ? l_Encounters.Count + l_QuestProgress.CreepEncounters - 1 : l_Encounters.Count);
                GoldCollecteText.text = string.Format("x{0:n0}", l_QuestProgress.GoldCollected);
                ItemsCollectedText.text = "x" + l_QuestProgress.ItemsFound.Count;
                HeroEncountersText.text = "x" + l_QuestProgress.HeroRescues;
            }

            var l_inventoryMgr = MainManager.Instance.getInventoryManager();
            if (null == l_inventoryMgr) return;

            IconManager l_iconMgr = MainManager.Instance.getIconManager();
            if (null == l_iconMgr) return;

            if (l_activeCharacter != null)
            {
                PlayerIcon.overrideSprite = l_iconMgr.GetIconById(l_activeCharacter.baseClass.Icon, IconManager.IconTypes.Class);
                LivesCount.text = "" + l_inventoryMgr.GetCurrencyAmount(GlobalStrings.HEART_CURRENCY);
                PlayerLevel.text = "" + l_activeCharacter.characterData.CharacterLevel;
                PlayerName.text = l_activeCharacter.CharacterName;
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

            var txt = ActionBar.UseItemButton.GetComponentInChildren<Text>();
            var img = ActionBar.UseItemButton.GetComponent<Image>();
            ActionBar.UseItemButton.interactable = true;
            if (gameplayController.turnController.currentEncounter.Data.EncounterType == EncounterTypes.Store)
            {
                // open store
                txt.text = "View Store";
                img.color = Color.green;

                ActionBar.UseItemButton.onClick.RemoveAllListeners();
                ActionBar.UseItemButton.onClick.AddListener(() =>
                {
                    StartEncounterInput(UBGamePlay.PlayerEncounterInputs.ViewStore);
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
                    StartEncounterInput(UBGamePlay.PlayerEncounterInputs.Rescue);
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
                    StartEncounterInput(UBGamePlay.PlayerEncounterInputs.UseItem);
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

        IEnumerator Shake(float seconds, UBGamePlay.ShakeEffects effect = UBGamePlay.ShakeEffects.None)
        {
            yield return new WaitForSeconds(seconds);

            shaker.ResetToBeginning();
            isShaking = false;
            shaker.enabled = false;

            if (effect == UBGamePlay.ShakeEffects.DecreaseHealth)
            {
                var remainingHp = LifeBar.currentValue - pendingValue;
                yield return StartCoroutine(LifeBar.UpdateBar(remainingHp));

                if (remainingHp > 0)
                    StartCoroutine(UBAnimator.Wait(1.0f, () => { GameplayController.RaiseGameplayEvent(GlobalStrings.ENEMY_TURN_END_EVENT, UBGamePlay.GameplayEventTypes.EnemyTurnEnds); }));
                else
                    GameplayController.RaiseGameplayEvent(GlobalStrings.OUTRO_PLAYER_DEATH_EVENT, UBGamePlay.GameplayEventTypes.OutroEncounter);
            }
            else if (effect == UBGamePlay.ShakeEffects.IncreaseHealth)
            {
                var remainingHp = LifeBar.currentValue + pendingValue;
                yield return StartCoroutine(LifeBar.UpdateBar(remainingHp));

                StartCoroutine(UBAnimator.Wait(.5f, () => { GameplayController.RaiseGameplayEvent(GlobalStrings.PLAYER_TURN_END_EVENT, UBGamePlay.GameplayEventTypes.PlayerTurnEnds); }));
            }
            else if (effect == UBGamePlay.ShakeEffects.DecreaseMana)
            {
                StartCoroutine(ManaBar.UpdateBar(ManaBar.currentValue - pendingValue));
            }
            else if (effect == UBGamePlay.ShakeEffects.IncreaseMana)
            {
                StartCoroutine(ManaBar.UpdateBar(ManaBar.currentValue + pendingValue));
            }

            pendingValue = 0;
        }

        private void UseCombatItem(string p_item)
        {
            var JsonUtil = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);

            //Debug.Log("Using " + item);
            var l_inventoryMgr = MainManager.Instance.getInventoryManager();
            if (null == l_inventoryMgr) return;

            UBInventoryCategory l_inventoryCategory = l_inventoryMgr.GetItemCategory(p_item);
            // if (!PF_PlayerData.inventoryByCategory.TryGetValue(item, out l_inventoryCategory) || l_inventoryCategory.count == 0)
            //     return;

            var attributes = JsonUtil.DeserializeObject<Dictionary<string, string>>(l_inventoryCategory.catalogRef.CustomData);
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
                    pendingValue = Mathf.CeilToInt(LifeBar.maxValue * modPercent);
                    GameController.Instance.ActiveCharacter.PlayerVitals.Health += pendingValue;
                    RequestShake(defaultShakeTime, UBGamePlay.ShakeEffects.IncreaseHealth);
                    break;
            }

            gameplayController.DecrementPlayerCDs();
            l_inventoryMgr.ConsumeItem(l_inventoryCategory.inventory[0].ItemInstanceId,
                (s) => { PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.ConsumeItemUse); },
                (f) => { PF_Bridge.RaiseCallbackError(f, PlayFabAPIMethods.ConsumeItemUse); }
            );
            GameController.Instance.QuestProgress.ItemsUsed++;
        }

        public void UseItem()
        {
            DialogCanvasController.RequestInventoryPrompt(UseCombatItem, DialogCanvasController.InventoryFilters.UsableInCombat);
        }
    }
}