using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnController : MonoBehaviour {
	public int currentTurnNumber = 0;
	public UB_SavedCharacter currentPlayer;
	public UB_GamePlayEncounter currentEncounter;
	public GameplayController gameplayController;
	public PF_GamePlay.TurnStates CurrentTurn = PF_GamePlay.TurnStates.Null;
	
	void OnEnable()
	{
        GameplayController.OnGameplayEvent += OnGameplayEventReceived;
	}
	
	void OnDisable()
	{
        GameplayController.OnGameplayEvent -= OnGameplayEventReceived;
	}
	

	public UB_GamePlayEncounter GetNextEncounter(bool isFirstEncounter = false)
	{
		
		if(PF_GamePlay.encounters != null && PF_GamePlay.encounters.Count > 0)
		{
			// pop off top and move to PF_Gameplay QuestTracker
			// But not ON THE FIRST ENCOUNTER
			
			if(PF_GamePlay.QuestProgress.CompletedEncounters.Count != 0 || !isFirstEncounter)
			{
				//LogCompletedEncounterStats();
				UB_GamePlayEncounter topOfStack = new UB_GamePlayEncounter();
				topOfStack = PF_GamePlay.encounters.First();
				PF_GamePlay.QuestProgress.CompletedEncounters.Add(topOfStack); 
				PF_GamePlay.encounters.Remove(PF_GamePlay.encounters.First());
				
				if(PF_GamePlay.encounters.Count == 0)
				{
					return null;
				}
				else
				{
					var next = PF_GamePlay.encounters.First();				
					
					next.Data.Vitals.SetMaxVitals();
					next.Data.SetSpellDetails();
					
					if(next.Data.EncounterType == EncounterTypes.BossCreep)
					{
                        GameplayController.RaiseGameplayEvent(GlobalStrings.BOSS_BATTLE_EVENT, PF_GamePlay.GameplayEventTypes.StartBossBattle);
					}
	
					return next;
				}
			}
			else
			{
				var next = PF_GamePlay.encounters.First();				
				
				next.Data.Vitals.SetMaxVitals();
				next.Data.SetSpellDetails();
				
				if(next.Data.EncounterType == EncounterTypes.BossCreep)
				{
                    GameplayController.RaiseGameplayEvent(GlobalStrings.BOSS_BATTLE_EVENT, PF_GamePlay.GameplayEventTypes.StartBossBattle);
				}
				return next;
			}
		}
		
		// end of the list or something!?
		return null;
				
	}


	public void StartTurn()
	{
		if(this.currentEncounter.Data.EncounterType == EncounterTypes.Hero || this.currentEncounter.Data.EncounterType == EncounterTypes.Store)
		{
			this.CurrentTurn = PF_GamePlay.TurnStates.Player;
			//this.gameplayController.playerController.EncounterBar.EnableEncounterOptions(this.currentEncounter.Data.EncounterType, this.currentEncounter.DisplayName);
			//this.gameplayController.playerController.TransitionEncounterBarIn();
		}
		else
		{
			//compare speed to see who goes first
			// if the enemy's speed is 150% more than the player's the enemy ambushes the player and gets a free attack
			if(this.currentEncounter.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP) && (float)this.currentPlayer.PlayerVitals.Speed * 1.5f < (float)this.currentEncounter.Data.Vitals.Speed)
			{
				this.CurrentTurn = PF_GamePlay.TurnStates.Enemy;
				//GameplayController.RaiseGameplayEvent("Enemy Turn Begins", PF_GamePlay.GameplayEventTypes.EnemyTurnBegins);
				gameplayController.EnemyAttackPlayer(true);
				//this.gameplayController.playerController.EncounterBar.EnableEncounterOptions(this.currentEncounter.Data.EncounterType, this.currentEncounter.DisplayName, true);
				//this.gameplayController.playerController.TransitionEncounterBarIn();
			}
			else
			{
				this.CurrentTurn = PF_GamePlay.TurnStates.Player;
				//this.gameplayController.playerController.EncounterBar.EnableEncounterOptions(this.currentEncounter.Data.EncounterType, this.currentEncounter.DisplayName);
				//this.gameplayController.playerController.TransitionEncounterBarIn();
			}
		}
	}
	
	public void ToggleTurn(PF_GamePlay.TurnStates forceTurn = PF_GamePlay.TurnStates.Null)
	{
		if(forceTurn != PF_GamePlay.TurnStates.Null)
		{
			if(forceTurn == PF_GamePlay.TurnStates.Player)
			{
				this.CurrentTurn = PF_GamePlay.TurnStates.Player;
                GameplayController.RaiseGameplayEvent(GlobalStrings.PLAYER_TURN_BEGIN_EVENT, PF_GamePlay.GameplayEventTypes.PlayerTurnBegins);
			}
			else if(forceTurn == PF_GamePlay.TurnStates.Enemy)
			{
				this.CurrentTurn = PF_GamePlay.TurnStates.Enemy;
                GameplayController.RaiseGameplayEvent(GlobalStrings.ENEMY_TURN_BEGIN_EVENT, PF_GamePlay.GameplayEventTypes.EnemyTurnBegins);
			}
			return;
		}
		

		
		if(this.CurrentTurn == PF_GamePlay.TurnStates.Player)
		{
			this.CurrentTurn = PF_GamePlay.TurnStates.Enemy;
            GameplayController.RaiseGameplayEvent(GlobalStrings.ENEMY_TURN_BEGIN_EVENT, PF_GamePlay.GameplayEventTypes.EnemyTurnBegins);
		}
		else if(this.CurrentTurn == PF_GamePlay.TurnStates.Enemy)
		{
			this.CurrentTurn = PF_GamePlay.TurnStates.Player;
            GameplayController.RaiseGameplayEvent(GlobalStrings.PLAYER_TURN_BEGIN_EVENT, PF_GamePlay.GameplayEventTypes.PlayerTurnBegins);
		}
	}
	
	
	public void CompleteEncounter(bool useEvade = false)
	{
		// was evasion / flee used?
		if(useEvade)
		{
			//TODO evasion check to see if evasion passes
			CycleNextEncounter();
			return;
		}
		
		// Did the player complete this encounter?
		
		if(this.currentEncounter.Data.EncounterType == EncounterTypes.Store || this.currentEncounter.Data.EncounterType == EncounterTypes.Hero)
		{
			this.currentEncounter.playerCompleted = true;
		}
        else if(this.currentEncounter.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP))
		{
			if(this.currentEncounter.Data.Vitals.Health <= 0)
			{
				// enemy died
				this.currentEncounter.playerCompleted = true;
				
				if(this.currentEncounter.Data.EncounterType == EncounterTypes.BossCreep)
				{
					Dictionary<string, object> eventData = new Dictionary<string, object>()
					{
						{ "Boss_Name", this.currentEncounter.DisplayName },
						{ "Current_Quest", PF_GamePlay.ActiveQuest.levelName },
						{ "Character_ID", PF_PlayerData.activeCharacter.characterDetails.CharacterId }
					};
					PF_Bridge.LogCustomEvent(PF_Bridge.CustomEventTypes.Client_BossKill, eventData);
				}
				
			}
			else if(this.currentPlayer.PlayerVitals.Health <= 0)
			{
				// player died
				this.currentEncounter.playerCompleted = false; 
			}
		}
		
		if(this.currentEncounter.playerCompleted)
		{
			LogCompletedEncounterStats();
			
			if(this.currentEncounter.isEndOfAct)
			{
				CompleteAct();
				return;
			}
			//TODO make this work for complex goals
//			else if(this.gameplayController.AreQuestGoalsComplete())
//			{
//				GameplayController.RaiseGameplayEvent("Quest Complete", PF_GamePlay.GameplayEventTypes.EndQuest);
//			}
			CycleNextEncounter();
		}
		else
		{
			//trigger game over 
            GameplayController.RaiseGameplayEvent(GlobalStrings.PLAYER_DIED_EVENT, PF_GamePlay.GameplayEventTypes.PlayerDied);
			Debug.Log ("Player Died...");
			PF_GamePlay.QuestProgress.Deaths++;
			
			Dictionary<string, object> eventData = new Dictionary<string, object>()
			{
				{ "Killed_By", this.currentEncounter.DisplayName },
				{ "Enemy_Health", this.currentEncounter.Data.Vitals.Health },
				{ "Current_Quest", PF_GamePlay.ActiveQuest.levelName },
				{ "Character_ID", PF_PlayerData.activeCharacter.characterDetails.CharacterId }
			};
			PF_Bridge.LogCustomEvent(PF_Bridge.CustomEventTypes.Client_PlayerDied, eventData);
			return;	
		}
	}

	
	void LogCompletedEncounterStats()
	{
		var progress = PF_GamePlay.QuestProgress;
		var encounter = this.currentEncounter;
		
		if(encounter.playerCompleted)
		{
            if(this.currentEncounter.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP))
			{
				progress.CreepEncounters++;
			}
			else if(this.currentEncounter.Data.EncounterType == EncounterTypes.Hero)
			{
				progress.HeroRescues++;
			}
			
			progress.GoldCollected += Random.Range(encounter.Data.Rewards.GoldMin, encounter.Data.Rewards.GoldMax);
			progress.XpCollected += Random.Range(encounter.Data.Rewards.XpMin, encounter.Data.Rewards.XpMax);
			
			if(encounter.Data.Rewards.ItemsDropped.Count > 0)
			{
				progress.ItemsFound.AddRange(encounter.Data.Rewards.ItemsDropped);
			}
		}
		this.gameplayController.playerController.UpdateQuestStats();
	}


	public void CycleNextEncounter()
	{
		this.currentEncounter = GetNextEncounter();
		if(this.currentEncounter != null)
		{
			UnityAction callback = () => 
			{ 
				UnityAction afterTransition = () =>  
				{
                    if(this.currentEncounter.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP) && this.currentPlayer.PlayerVitals.Speed * 1.5f < this.currentEncounter.Data.Vitals.Speed)
					{
						this.CurrentTurn = PF_GamePlay.TurnStates.Enemy;
						gameplayController.EnemyAttackPlayer(true);
						this.gameplayController.playerController.EncounterBar.EnableEncounterOptions(this.currentEncounter.Data.EncounterType, this.currentEncounter.DisplayName, true);
					}
					else
					{
						this.CurrentTurn = PF_GamePlay.TurnStates.Player;
						//this.gameplayController.playerController.EncounterBar.EnableEncounterOptions(this.currentEncounter.Data.EncounterType, this.currentEncounter.DisplayName);
						this.gameplayController.playerController.TransitionActionBarIn();
					}
					
                    GameplayController.RaiseGameplayEvent(GlobalStrings.NEXT_ENCOUNTER_EVENT, PF_GamePlay.GameplayEventTypes.IntroEncounter);
				}; 
				
				this.gameplayController.enemyController.currentEncounter.UpdateCurrentEncounter(this.currentEncounter);
				this.gameplayController.enemyController.nextController.UpdateNextEncounters();
				this.gameplayController.enemyController.TransitionCurrentEncounterIn(afterTransition); 
			};
			
			this.gameplayController.enemyController.TransitionCurrentEncounterOut(callback);
		}
	}
	
	
    void OnGameplayEventReceived(string message,  PF_GamePlay.GameplayEventTypes type )
	{
		if(type == PF_GamePlay.GameplayEventTypes.IntroQuest)
		{
			AdvanceAct();
			this.currentEncounter = GetNextEncounter(true);
			this.gameplayController.enemyController.currentEncounter.UpdateCurrentEncounter(this.currentEncounter);
			this.gameplayController.enemyController.nextController.UpdateNextEncounters();
			
			this.currentTurnNumber = 1;
			//this.currentEncounter =  PF_GamePlay.encounters.First();
			this.currentPlayer = PF_PlayerData.activeCharacter;
			
			

		}
		
		if(type == PF_GamePlay.GameplayEventTypes.StartQuest)
		{
			StartTurn();
			
		}
		
		if(type == PF_GamePlay.GameplayEventTypes.OutroEncounter)
		{
			CompleteEncounter();
			return;
		}
		
		if(type == PF_GamePlay.GameplayEventTypes.EnemyTurnEnds || type == PF_GamePlay.GameplayEventTypes.PlayerTurnEnds)
		{
            if(message.Contains(GlobalStrings.PLAYER_RESPAWN_EVENT))
			{
				gameplayController.playerController.UpdateQuestStats();
				StartCoroutine(gameplayController.playerController.LifeBar.UpdateBar(gameplayController.playerController.LifeBar.maxValue));
			}
			else
			{
				ToggleTurn();
			}
		}
	}
	
	
	public void Evade()
	{
		//this.currentEncounter.playerCompleted 
		if(this.currentEncounter.isEndOfAct)
		{
			Debug.Log("End of the Road M8..."); // cant skip the last encounter
		}
		else
		{
			CompleteEncounter(true);
			this.gameplayController.DecrementPlayerCDs();
		}
	}
	
	public void Rescue()
	{
		this.currentEncounter.playerCompleted = true;

		Dictionary<string, object> eventData = new Dictionary<string, object>()
		{
			{ "Current_Quest", PF_GamePlay.ActiveQuest.levelName },
			{ "Rescued", this.currentEncounter.DisplayName },
			{ "Character_ID", PF_PlayerData.activeCharacter.characterDetails.CharacterId }
		};
		PF_Bridge.LogCustomEvent(PF_Bridge.CustomEventTypes.Client_UnicornFreed, eventData);
		
		CompleteEncounter(false);
	}

	public void CompleteAct()
	{
		if(PF_GamePlay.QuestProgress != null)
		{
			//this.gameplayController.directionController;
			PF_GamePlay.QuestProgress.CurrentAct.Value.IsActCompleted = true;
			
			
			if(PF_GamePlay.encounters.Count > 1)
			{
				// more acts to go
				GameplayController.RaiseGameplayEvent(GlobalStrings.ACT_COMPLETE_EVENT, PF_GamePlay.GameplayEventTypes.OutroAct);

			}
			else
			{
				// no more acts, complete quest
				PF_GamePlay.QuestProgress.isQuestWon = true;
				
				UB_GamePlayEncounter topOfStack = new UB_GamePlayEncounter();
				topOfStack = PF_GamePlay.encounters.First();
				PF_GamePlay.QuestProgress.CompletedEncounters.Add(topOfStack); 
				PF_GamePlay.encounters.Remove(PF_GamePlay.encounters.First());
				
				//CycleNextEncounter();
                GameplayController.RaiseGameplayEvent(GlobalStrings.QUEST_COMPLETE_EVENT, PF_GamePlay.GameplayEventTypes.OutroQuest);
				
				
			}
		}
	}

	public void AdvanceAct()
	{
		if(PF_GamePlay.ActiveQuest.levelData.Acts.Count > 0)
		{
			
			if(PF_GamePlay.QuestProgress.CurrentAct.Equals(new KeyValuePair<string, UB_LevelAct>()))
			{
				// FIRST ACT
				PF_GamePlay.QuestProgress.CurrentAct = PF_GamePlay.ActiveQuest.levelData.Acts.First();
				PF_GamePlay.QuestProgress.ActIndex = 0;
			}
			else
			{
				//Following Acts
				int indexToCheck = PF_GamePlay.QuestProgress.ActIndex+1;
				if(PF_GamePlay.ActiveQuest.levelData.Acts.Count > indexToCheck)
				{
					PF_GamePlay.QuestProgress.CurrentAct = PF_GamePlay.ActiveQuest.levelData.Acts.ElementAt(indexToCheck);
					PF_GamePlay.QuestProgress.ActIndex = indexToCheck;
					CycleNextEncounter();
				}
				
			}
		}
		
	}
}
