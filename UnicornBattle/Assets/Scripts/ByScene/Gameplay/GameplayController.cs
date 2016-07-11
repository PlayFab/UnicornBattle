using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameplayController : MonoBehaviour {
	public GameplayEnemyController enemyController;
	public PlayerUIEffectsController playerController;
	public DirectionCanvasController directionController;
	public Transform enemyAction;
	
	public delegate void GameplayEventHandler(string message, PF_GamePlay.GameplayEventTypes type );
	public static event GameplayEventHandler OnGameplayEvent;
	
	public TurnController turnController;
	
	
	
	
	public FX_Placement fxController;
	public float dmgMax = 1.0f;
	public float dmgMin = .33f; 
	
	
	#region Setup & UI Actions
	void OnEnable()
	{
        OnGameplayEvent += OnGameplayEventReceived;
		Init();
		//Debug.Log(PF_PlayerData.activeCharacter.PlayerVitals.Health);
	}
	
	void OnDisable()
	{
        OnGameplayEvent -= OnGameplayEventReceived;
	}
	
    void OnGameplayEventReceived(string message,  PF_GamePlay.GameplayEventTypes type )
	{
		//Debug.Log(string.Format("{0} -- {1}",type.ToString(), message));
		
		if(type == PF_GamePlay.GameplayEventTypes.IntroQuest)
		{
            RaiseGameplayEvent(GlobalStrings.QUEST_START_EVENT, PF_GamePlay.GameplayEventTypes.IntroAct);
		}
		
		if(type == PF_GamePlay.GameplayEventTypes.IntroEncounter)
		{
			
		}
		
		if(type == PF_GamePlay.GameplayEventTypes.IntroEncounter)
		{
			
		}
		
		if(type == PF_GamePlay.GameplayEventTypes.PlayerTurnBegins)
		{
			
		}
		
		if(type == PF_GamePlay.GameplayEventTypes.EnemyTurnBegins)
		{
			EnemyAttackPlayer();
		}
	}
	
//	void OnGUI()
//	{
//		if(GUI.Button (new Rect(Screen.width/2 - 50, 5, 100, 50), "Shaker"))
//		{
//			
//			EnemyAttackPlayer();
//		}
//	}
	
	void Init()
	{
		//TODO this needs to set up all the dynamic data for the many components
		if( (PF_GamePlay.encounters != null && PF_GamePlay.encounters.Count > 0) && (PF_GamePlay.ActiveQuest != null))
		{ 
			if(PF_GamePlay.UseRaidMode == true)
			{
				QuestTracker qt = SpoofQuestResults();
				PF_GamePlay.QuestProgress = qt;
			}

            RaiseGameplayEvent(GlobalStrings.QUEST_START_EVENT, PF_GamePlay.GameplayEventTypes.IntroQuest);
			
		}
		else
		{
			
		}
		
	}

	private QuestTracker SpoofQuestResults()
	{
		QuestTracker tracker = new QuestTracker();

		tracker.CompletedEncounters = PF_GamePlay.encounters;

		float rng = Random.value;
		if (rng > .9)
		{
			tracker.DamageTaken = Random.Range(235,  976);
			tracker.Deaths = 2;
		}
		else if(rng > .1f && rng < .4f)
		{
			tracker.DamageTaken = Random.Range(235,  750);
			tracker.Deaths = 1;
		}
		else
		{
			tracker.DamageTaken = Random.Range(235,  400);
			tracker.Deaths = 0;
		}

		if(PF_GamePlay.isHardMode == true) 
		{
			tracker.DamageTaken = Random.Range(880,  2450);
			tracker.Deaths += Random.value > .6 ? 1 : 0;
		}

		tracker.ItemsUsed = Random.Range(0,5);
	


		foreach (var encounter in PF_GamePlay.encounters) 
		{
			tracker.ItemsFound.AddRange(encounter.Data.Rewards.ItemsDropped);
			tracker.XpCollected += encounter.Data.Rewards.XpMin;
			tracker.GoldCollected += encounter.Data.Rewards.GoldMin;

			if(encounter.Data.EncounterType == EncounterTypes.Hero)
			{
				tracker.HeroRescues++;
			}
			
            if(encounter.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP))
			{
				tracker.CreepEncounters++;
			}
		}
		tracker.isQuestWon = true;

		return tracker;
	}



	public static void RaiseGameplayEvent(string message, PF_GamePlay.GameplayEventTypes type)
	{
		if(OnGameplayEvent != null)
		{
			OnGameplayEvent(message, type);
		}
	}

//	public void StartAct()
//	{
//		RaiseGameplayEvent("Intro Act", PF_GamePlay.GameplayEventTypes.IntroAct);
//	}
//	
//	public void StartQuest()
//	{
//        RaiseGameplayEvent(GlobalStrings.QUEST_START_EVENT, PF_GamePlay.GameplayEventTypes.StartQuest);
//	}
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
		int rng = 0;
		EnemyVitals vitals = turnController.currentEncounter.Data.Vitals;
		if( vitals.Spells.Count > 1)
		{
			List<int> tries = new List<int>();
			do
			{
				if(tries.Count < vitals.Spells.Count)
				{
					rng = Random.Range(0, vitals.Spells.Count);
					if(tries.IndexOf(rng) == -1)
					{
						tries.Add(rng);
					}
				}
				else
				{
					break;
				}
			}
			while(vitals.Spells[rng].IsOnCooldown == false && tries.Count < vitals.Spells.Count);
		}
		
		Debug.Log(string.Format("{0} / {1}",  rng, vitals.Spells.Count));
		EnemySpellDetail spellRecord = vitals.Spells[rng];
		

		
		// we will run this after the Callout animation
		UnityAction applyDamage = () => 
		{	
			
			fxController.PlayerTakesDamage(this.playerController, spellRecord.Detail.FX);
			this.playerController.TakeDamage(spellRecord.Detail.BaseDmg);
			
			// NEED TO make this decrement each turn
			
			// No penalty for the first attack on an ambush
			if(isAmbush != true && spellRecord.Detail.Cooldown > 0)
			{
				spellRecord.IsOnCooldown = true;
				spellRecord.CdTurns = spellRecord.Detail.Cooldown;
			}
		};
		
		//Make our callout
		Sprite spellIcon = GameController.Instance.iconManager.GetIconById(spellRecord.Detail.Icon);
		this.enemyController.Callout(spellIcon, string.Format("{0} casts {1}", this.turnController.currentEncounter.DisplayName, spellRecord.SpellName), applyDamage);
	}
	

	public void PlayerAttacks(SpellSlot sp, bool isAmbush = false) //, bool isAmbush = false)
	{
		Sprite spellIcon = sp.SpellIcon.overrideSprite;
		

		
		// we will run this after the Callout animation
		UnityAction applyDamage = () => 
		{	
			fxController.TestFx(this.enemyController, sp.SpellData.FX);
			this.enemyController.TakeDamage(sp.SpellData.Dmg);
			
			DecrementPlayerCDs();
			
			// No penalty for the first attack on an ambush
			if(isAmbush != true)
			{
				if(sp.SpellData.Cooldown > 0)
				{
					sp.isOnCD = true;
					sp.EnableCD(sp.SpellData.Cooldown);
				}
			}
		};
		
		//Make our callout
		this.playerController.Callout(spellIcon, string.Format("You cast {0}", sp.SpellData.SpellName), applyDamage);
	}


	public void DecrementPlayerCDs()
	{
		playerController.ActionBar.Spell1Button.DecrementCD();
		playerController.ActionBar.Spell2Button.DecrementCD();
		playerController.ActionBar.Spell3Button.DecrementCD();
	}
	
}

