using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using PlayFab.Json;

public class PlayerUIEffectsController : MonoBehaviour {
	public RectTransform myRT; 
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
	private float animationStart = 0;
	
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
		
		if(type == PF_GamePlay.GameplayEventTypes.IntroQuest)
		{
			ActionBar.UpdateSpellBar();
			Init();
		}
		
		if(type == PF_GamePlay.GameplayEventTypes.IntroEncounter)
		{
			StartCoroutine(PF_GamePlay.Wait(.5f, () => { TransitionEncounterBarIn(); }));
		}

		if(type == PF_GamePlay.GameplayEventTypes.PlayerTurnBegins)
		{
			TransitionActionBarIn();
		}
		
	}
	
	public void Init()
	{
		UpdateQuestStats();
		this.LifeBar.maxValue = PF_PlayerData.activeCharacter.PlayerVitals.Health;
		StartCoroutine(this.LifeBar.UpdateBar(PF_PlayerData.activeCharacter.PlayerVitals.Health, true));
	}
	
	void Start () {
		this.myRT = (RectTransform)GetComponent<RectTransform>();
	}

	public void TakeDamage(int dmg )
	{
		Debug.Log(string.Format("Player Takes {0}", dmg));
		this.pendingValue = dmg ;
		RequestShake(defaultShakeTime, PF_GamePlay.ShakeEffects.DecreaseHealth);
		PF_PlayerData.activeCharacter.PlayerVitals.Health -= dmg;
		
		//else
		//StartCoroutine(bar.RefillHealth());
	}	
	
	
	public void Callout(Sprite sprite, string message, UnityAction callback)
	{
		TransitionActionBarOut();
		playerAction.actionIcon.overrideSprite = sprite;
		playerAction.actionText.text = message;
		playerAction.CastSpell(callback);
	}
	
	
	public void RequestShake(float seconds,PF_GamePlay. ShakeEffects effect)
	{
		if(this.isShaking == false)
		{
			shaker.enabled = true;
			this.animationStart = Time.time;
			StartCoroutine(Shake(seconds, effect));
		}
		else
		{
			this.animationStart -= seconds;
		}
	}
	
	
	public void StartEncounterInput(PF_GamePlay.PlayerEncounterInputs input)
	{
		
		
		switch(input)
		{
			case PF_GamePlay.PlayerEncounterInputs.Attack:
				
				TransitionEncounterBarOut( () => { TransitionActionBarIn(); });
				//GameplayController.RaiseGameplayEvent("", 
				
			break;
			
			case PF_GamePlay.PlayerEncounterInputs.UseItem:
				//TransitionActionBarOut();
				this.UseItem();
				
			break;
			
			case PF_GamePlay.PlayerEncounterInputs.Evade:
				TransitionActionBarOut();
				// this causes a bug on boss types (cant evade) (should remove this option on un-evadable encounters)
				gameplayController.turnController.Evade();
			break;
			
			case PF_GamePlay.PlayerEncounterInputs.ViewStore:
				string storeID = string.Empty;
                gameplayController.turnController.currentEncounter.Data.EncounterActions.TryGetValue(GlobalStrings.ENCOUNTER_STORE, out storeID);
				
				if(!string.IsNullOrEmpty(storeID))
				{
					DialogCanvasController.RequestStore(storeID);
				}
				else
				{
					Debug.LogError("No store found for merchant");
				}
				
			break;
		
			case PF_GamePlay.PlayerEncounterInputs.Rescue:
				gameplayController.turnController.CompleteEncounter();
			break;
		}
	}
	
	public void UpdateQuestStats()
	{
		if(PF_GamePlay.QuestProgress != null)
		{
			this.CreepEncountersText.text = string.Format("{0} / {1}", PF_GamePlay.QuestProgress.CreepEncounters, PF_GamePlay.QuestProgress.CreepEncounters > 0 ? PF_GamePlay.encounters.Count + PF_GamePlay.QuestProgress.CreepEncounters - 1 : PF_GamePlay.encounters.Count);
			this.GoldCollecteText.text = string.Format("x{0:n0}", PF_GamePlay.QuestProgress.GoldCollected);
			this.ItemsCollectedText.text = string.Format("x{0}", PF_GamePlay.QuestProgress.ItemsFound.Count);
			this.HeroEncountersText.text = string.Format("x{0}", PF_GamePlay.QuestProgress.HeroRescues);
		}
		
		if(PF_PlayerData.activeCharacter != null)
		{
			this.PlayerIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(PF_PlayerData.activeCharacter.baseClass.Icon);
			
			//this.LifeBar.maxValue = PF_PlayerData.activeCharacter.PlayerVitals.Health;
			//StartCoroutine(this.LifeBar.UpdateBar(PF_PlayerData.activeCharacter.PlayerVitals.Health));
			
			//this.ManaBar.maxValue = PF_PlayerData.activeCharacter.PlayerVitals.Mana;;
			
			//StartCoroutine(this.ManaBar.UpdateBar(PF_PlayerData.activeCharacter.PlayerVitals.Mana));
			this.LivesCount.text = "" + PF_PlayerData.characterVirtualCurrency["HT"];
			this.PlayerLevel.text = "" + PF_PlayerData.activeCharacter.characterData.CharacterLevel;
			
			this.PlayerName.text = PF_PlayerData.activeCharacter.characterDetails.CharacterName;
		}
	}
	
	public void TransitionEncounterBarIn(UnityAction cb = null)
	{
//		this.EncounterBar.tweener.OnFinished.RemoveAllListeners();
		if(cb != null)
		{
			//this.EncounterBar.tweener.OnFinished.AddListener(cb);
			cb();
		}
//		
//		TweenCGAlpha.Tween(this.EncounterBar.gameObject, .333f, 1, null);
//		this.EncounterBar.tweener.PlayReverse();
	}
	
	public void TransitionEncounterBarOut(UnityAction cb = null)
	{
//		this.EncounterBar.tweener.OnFinished.RemoveAllListeners();
		if(cb != null)
		{
			cb();
		}
//		
//		TweenCGAlpha.Tween(this.EncounterBar.gameObject, .333f, 0, null);
//		this.EncounterBar.tweener.PlayForward();
	}
	
	public void TransitionActionBarIn(UnityAction cb = null)
	{
//		this.EncounterBar.tweener.OnFinished.RemoveAllListeners();
//		if(cb != null)
//		{
//			this.EncounterBar.tweener.OnFinished.AddListener(cb);
//		}
//		
//		TweenCGAlpha.Tween(this.ActionBar.gameObject, .333f, 1, null);
//		this.ActionBar.tweener.PlayReverse();
			if(gameplayController.turnController.currentEncounter.Data.EncounterType == EncounterTypes.BossCreep)
			{
				this.ActionBar.FleeButton.interactable = false;
			}
			else
			{
				this.ActionBar.FleeButton.interactable = true;
			}
			
			Text txt = this.ActionBar.UseItemButton.GetComponentInChildren<Text>();
			Image img = this.ActionBar.UseItemButton.GetComponent<Image>();
			this.ActionBar.UseItemButton.interactable = true;
			if(gameplayController.turnController.currentEncounter.Data.EncounterType == EncounterTypes.Store)
			{
				// open store
				txt.text = "View Store";
				img.color = Color.green;
				
				this.ActionBar.UseItemButton.onClick.RemoveAllListeners();
				this.ActionBar.UseItemButton.onClick.AddListener(() => 
				{
					StartEncounterInput(PF_GamePlay.PlayerEncounterInputs.ViewStore);
				});
				
			}
			else if(gameplayController.turnController.currentEncounter.Data.EncounterType == EncounterTypes.Hero)
			{
				// rescue
				txt.text = "Rescue";
				img.color = Color.magenta;
				
				this.ActionBar.UseItemButton.onClick.RemoveAllListeners();
				this.ActionBar.UseItemButton.onClick.AddListener(() => 
				                                                 {
					StartEncounterInput(PF_GamePlay.PlayerEncounterInputs.Rescue);
				});
			}
			else
			{
				// use item
				txt.text = "Use Item";
				img.color = Color.blue;
				
				this.ActionBar.UseItemButton.onClick.RemoveAllListeners();
				this.ActionBar.UseItemButton.onClick.AddListener(() => 
				                                                 {
					StartEncounterInput(PF_GamePlay.PlayerEncounterInputs.UseItem);
				});
			}
			
			
		if(this.ActionBar.Spell1Button.isLocked == false && this.ActionBar.Spell1Button.isOnCD == false && this.gameplayController.turnController.currentEncounter.Data.EncounterType.ToString().Contains("Creep"))
			{
				this.ActionBar.Spell1Button.SpellButton.interactable = true;
			}
			else
			{
				this.ActionBar.Spell1Button.SpellButton.interactable = false;
			}
			
			if(this.ActionBar.Spell2Button.isLocked == false && this.ActionBar.Spell2Button.isOnCD == false)
			{
				this.ActionBar.Spell2Button.SpellButton.interactable = true;
			}
			else
			{
				this.ActionBar.Spell2Button.SpellButton.interactable = false;
			}
			
			if(this.ActionBar.Spell3Button.isLocked == false && this.ActionBar.Spell3Button.isOnCD == false)
			{
				this.ActionBar.Spell3Button.SpellButton.interactable = true;
			}
			else
			{
				this.ActionBar.Spell3Button.SpellButton.interactable = false;
			}
	}
	
	public void TransitionActionBarOut(UnityAction cb = null)
	{
//		this.EncounterBar.tweener.OnFinished.RemoveAllListeners();
//		if(cb != null)
//		{
//			this.EncounterBar.tweener.OnFinished.AddListener(cb);
//		}
//		
//		TweenCGAlpha.Tween(this.ActionBar.gameObject, .333f, 0, null);
//		this.ActionBar.tweener.PlayForward();
			this.ActionBar.Spell1Button.SpellButton.interactable = false;
			this.ActionBar.Spell2Button.SpellButton.interactable = false;
			this.ActionBar.Spell3Button.SpellButton.interactable = false;
			this.ActionBar.FleeButton.interactable = false;
			this.ActionBar.UseItemButton.interactable = false;
	}
	
	IEnumerator Shake(float seconds, PF_GamePlay.ShakeEffects effect = PF_GamePlay.ShakeEffects.None)
	{
		yield return new WaitForSeconds(seconds);
		
		this.shaker.ResetToBeginning();     //new Vector3 (0,this.transform.position.y, this.transform.position.z);
		this.isShaking = false;
		this.shaker.enabled = false;
		
		if( effect == PF_GamePlay.ShakeEffects.DecreaseHealth)
		{
			int remainingHP = LifeBar.currentValue - this.pendingValue;
			yield return StartCoroutine(LifeBar.UpdateBar(remainingHP));
			
			if(remainingHP > 0)
			{
                StartCoroutine(PF_GamePlay.Wait(1.0f, () => { GameplayController.RaiseGameplayEvent(GlobalStrings.ENEMY_TURN_END_EVENT, PF_GamePlay.GameplayEventTypes.EnemyTurnEnds); }));
			}
			else
			{
                GameplayController.RaiseGameplayEvent(GlobalStrings.OUTRO_PLAYER_DEATH_EVENT, PF_GamePlay.GameplayEventTypes.OutroEncounter);
			}
		}
		else if (effect == PF_GamePlay.ShakeEffects.IncreaseHealth)
		{
			int remainingHP = LifeBar.currentValue + this.pendingValue;
			yield return StartCoroutine(LifeBar.UpdateBar(remainingHP));

            StartCoroutine(PF_GamePlay.Wait(.5f, () => { GameplayController.RaiseGameplayEvent(GlobalStrings.PLAYER_TURN_END_EVENT, PF_GamePlay.GameplayEventTypes.PlayerTurnEnds); }));
			
		}
		else if(effect == PF_GamePlay.ShakeEffects.DecreaseMana)
		{
			StartCoroutine(ManaBar.UpdateBar(LifeBar.currentValue - this.pendingValue));
		}
		else if(effect == PF_GamePlay.ShakeEffects.IncreaseMana)
		{
			StartCoroutine(ManaBar.UpdateBar(LifeBar.currentValue + this.pendingValue));
		}
		
		this.pendingValue = 0;
		
		yield break;
	}
	
	public void UseItem()
	{
		Action<string> afterPickItem = (string item) =>
		{
			Debug.Log("Using " + item);
			
			InventoryCategory obj;
			PF_PlayerData.characterInvByCategory.TryGetValue(item, out obj);
			
			if(obj != null)
			{
				var first = obj.inventory.FirstOrDefault();
				if(first != null)
				{
					var attributes = PlayFab.Json.JsonWrapper.DeserializeObject<Dictionary<string,string>>(obj.catalogRef.CustomData);
					if(attributes.ContainsKey("modifies") && attributes.ContainsKey("modifyPercent") && attributes.ContainsKey("target"))
					{
						if( string.Equals(attributes["target"], "self"))
						{
							// item effect applies to the player
							string mod = attributes["modifies"];
							float modPercent = float.Parse(attributes["modifyPercent"]);
							
							switch(mod)
							{
							case "HP":
								this.pendingValue = Mathf.CeilToInt((float)this.LifeBar.maxValue * modPercent);
								Debug.Log(string.Format("Player Heals {0}", this.pendingValue));
								PF_PlayerData.activeCharacter.PlayerVitals.Health += this.pendingValue;
								RequestShake(defaultShakeTime, PF_GamePlay.ShakeEffects.IncreaseHealth);
								break;
							}

							gameplayController.DecrementPlayerCDs();
							PF_GamePlay.ConsumeItem(first.ItemInstanceId);
							PF_GamePlay.QuestProgress.ItemsUsed++;
						}
					}
				}
			}
		};
		
		
		DialogCanvasController.RequestInventoryPrompt(afterPickItem, DialogCanvasController.InventoryFilters.UsableInCombat, false, FloatingInventoryController.InventoryMode.Character);
	}
	
}
