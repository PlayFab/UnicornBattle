using System.Collections.Generic;
using System.Linq;
using UnicornBattle.Managers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.Events;

namespace UnicornBattle.Controllers
{
	public class TurnController : MonoBehaviour
	{
		public int currentTurnNumber = 0;
		public UBSavedCharacter currentPlayer;
		public UBEncounter currentEncounter;
		public GameplayController gameplayController;
		public UBGamePlay.TurnStates CurrentTurn = UBGamePlay.TurnStates.Null;

		void OnEnable()
		{
			GameplayController.OnGameplayEvent += OnGameplayEventReceived;
		}

		void OnDisable()
		{
			GameplayController.OnGameplayEvent -= OnGameplayEventReceived;
		}

		public UBEncounter GetNextEncounter(bool isFirstEncounter = false)
		{
			GameController l_gc = GameController.Instance;

			if (l_gc.ActiveEncounterList != null && l_gc.ActiveEncounterList.Count > 0)
			{
				// pop off top and move to PF_Gameplay QuestTracker
				// But not ON THE FIRST ENCOUNTER

				if (l_gc.QuestProgress.CompletedEncounters.Count != 0 || !isFirstEncounter)
				{
					//LogCompletedEncounterStats();
					UBEncounter topOfStack = new UBEncounter();
					topOfStack = l_gc.ActiveEncounterList.First();
					l_gc.QuestProgress.CompletedEncounters.Add(topOfStack);
					l_gc.ActiveEncounterList.Remove(l_gc.ActiveEncounterList.First());

					if (l_gc.ActiveEncounterList.Count == 0)
					{
						return null;
					}
					else
					{
						var next = l_gc.ActiveEncounterList.First();

						next.Data.Vitals.SetMaxVitals();
						next.Data.SetSpellDetails();

						if (next.Data.EncounterType == EncounterTypes.BossCreep)
						{
							GameplayController.RaiseGameplayEvent(GlobalStrings.BOSS_BATTLE_EVENT, UBGamePlay.GameplayEventTypes.StartBossBattle);
						}

						return next;
					}
				}
				else
				{
					var next = l_gc.ActiveEncounterList.First();

					next.Data.Vitals.SetMaxVitals();
					next.Data.SetSpellDetails();

					if (next.Data.EncounterType == EncounterTypes.BossCreep)
					{
						GameplayController.RaiseGameplayEvent(GlobalStrings.BOSS_BATTLE_EVENT, UBGamePlay.GameplayEventTypes.StartBossBattle);
					}
					return next;
				}
			}

			// end of the list or something!?
			return null;

		}

		public void StartTurn()
		{
			if (this.currentEncounter.Data.EncounterType == EncounterTypes.Hero || this.currentEncounter.Data.EncounterType == EncounterTypes.Store)
			{
				this.CurrentTurn = UBGamePlay.TurnStates.Player;
			}
			else
			{
				//compare speed to see who goes first
				// if the enemy's speed is 150% more than the player's the enemy ambushes the player and gets a free attack
				if (this.currentEncounter.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP) && (float) this.currentPlayer.PlayerVitals.Speed * 1.5f < (float) this.currentEncounter.Data.Vitals.Speed)
				{
					this.CurrentTurn = UBGamePlay.TurnStates.Enemy;
					gameplayController.EnemyAttackPlayer(true);
				}
				else
				{
					this.CurrentTurn = UBGamePlay.TurnStates.Player;
				}
			}
		}

		public void ToggleTurn(UBGamePlay.TurnStates forceTurn = UBGamePlay.TurnStates.Null)
		{
			if (forceTurn != UBGamePlay.TurnStates.Null)
			{
				if (forceTurn == UBGamePlay.TurnStates.Player)
				{
					this.CurrentTurn = UBGamePlay.TurnStates.Player;
					GameplayController.RaiseGameplayEvent(GlobalStrings.PLAYER_TURN_BEGIN_EVENT, UBGamePlay.GameplayEventTypes.PlayerTurnBegins);
				}
				else if (forceTurn == UBGamePlay.TurnStates.Enemy)
				{
					this.CurrentTurn = UBGamePlay.TurnStates.Enemy;
					GameplayController.RaiseGameplayEvent(GlobalStrings.ENEMY_TURN_BEGIN_EVENT, UBGamePlay.GameplayEventTypes.EnemyTurnBegins);
				}
				return;
			}

			if (this.CurrentTurn == UBGamePlay.TurnStates.Player)
			{
				this.CurrentTurn = UBGamePlay.TurnStates.Enemy;
				GameplayController.RaiseGameplayEvent(GlobalStrings.ENEMY_TURN_BEGIN_EVENT, UBGamePlay.GameplayEventTypes.EnemyTurnBegins);
			}
			else if (this.CurrentTurn == UBGamePlay.TurnStates.Enemy)
			{
				this.CurrentTurn = UBGamePlay.TurnStates.Player;
				GameplayController.RaiseGameplayEvent(GlobalStrings.PLAYER_TURN_BEGIN_EVENT, UBGamePlay.GameplayEventTypes.PlayerTurnBegins);
			}
		}

		public void CompleteEncounter(bool useEvade = false)
		{
			// was evasion / flee used?
			if (useEvade)
			{
				//TODO evasion check to see if evasion passes
				CycleNextEncounter();
				return;
			}

			// Did the player complete this encounter?

			if (this.currentEncounter.Data.EncounterType == EncounterTypes.Store || this.currentEncounter.Data.EncounterType == EncounterTypes.Hero)
			{
				this.currentEncounter.playerCompleted = true;
			}
			else if (this.currentEncounter.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP))
			{
				if (this.currentEncounter.Data.Vitals.Health <= 0)
				{
					// enemy died
					this.currentEncounter.playerCompleted = true;

					if (this.currentEncounter.Data.EncounterType == EncounterTypes.BossCreep)
					{
						Dictionary<string, object> eventData = new Dictionary<string, object>()
						{ { "Boss_Name", this.currentEncounter.DisplayName }, { "Current_Quest", GameController.Instance.ActiveLevel.levelName }, { "Character_ID", this.currentPlayer.CharacterId }
						};
						TelemetryManager.RecordPlayerEvent(TelemetryEvent.Client_BossKill, eventData);
					}

				}
				else if (this.currentPlayer.PlayerVitals.Health <= 0)
				{
					// player died
					this.currentEncounter.playerCompleted = false;
				}
			}

			if (this.currentEncounter.playerCompleted)
			{
				LogCompletedEncounterStats();

				if (this.currentEncounter.isEndOfAct)
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
				GameplayController.RaiseGameplayEvent(GlobalStrings.PLAYER_DIED_EVENT, UBGamePlay.GameplayEventTypes.PlayerDied);
				//Debug.Log("Player Died...");
				GameController.Instance.QuestProgress.Deaths++;

				Dictionary<string, object> eventData = new Dictionary<string, object>()
				{ { "Killed_By", this.currentEncounter.DisplayName }, { "Enemy_Health", this.currentEncounter.Data.Vitals.Health }, { "Current_Quest", GameController.Instance.ActiveLevel.levelName }, { "Character_ID", this.currentPlayer.CharacterId }
				};
				TelemetryManager.RecordPlayerEvent(TelemetryEvent.Client_PlayerDied, eventData);
				return;
			}
		}

		void LogCompletedEncounterStats()
		{
			var progress = GameController.Instance.QuestProgress;
			var encounter = this.currentEncounter;

			if (encounter.playerCompleted)
			{
				if (this.currentEncounter.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP))
				{
					progress.CreepEncounters++;
				}
				else if (this.currentEncounter.Data.EncounterType == EncounterTypes.Hero)
				{
					progress.HeroRescues++;
				}

				progress.GoldCollected += Random.Range(encounter.Data.Rewards.GoldMin, encounter.Data.Rewards.GoldMax);
				progress.XpCollected += Random.Range(encounter.Data.Rewards.XpMin, encounter.Data.Rewards.XpMax);

				if (encounter.Data.Rewards.ItemsDropped.Count > 0)
				{
					progress.ItemsFound.AddRange(encounter.Data.Rewards.ItemsDropped);
				}
			}
			this.gameplayController.playerController.UpdateQuestStats();
		}

		public void CycleNextEncounter()
		{
			this.currentEncounter = GetNextEncounter();
			if (this.currentEncounter != null)
			{
				UnityAction callback = () =>
				{
					UnityAction afterTransition = () =>
					{
						if (this.currentEncounter.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP) && this.currentPlayer.PlayerVitals.Speed * 1.5f < this.currentEncounter.Data.Vitals.Speed)
						{
							this.CurrentTurn = UBGamePlay.TurnStates.Enemy;
							gameplayController.EnemyAttackPlayer(true);
							this.gameplayController.playerController.EncounterBar.EnableEncounterOptions(this.currentEncounter.Data.EncounterType, this.currentEncounter.DisplayName, true);
						}
						else
						{
							this.CurrentTurn = UBGamePlay.TurnStates.Player;
							//this.gameplayController.playerController.EncounterBar.EnableEncounterOptions(this.currentEncounter.Data.EncounterType, this.currentEncounter.DisplayName);
							this.gameplayController.playerController.TransitionActionBarIn();
						}

						GameplayController.RaiseGameplayEvent(GlobalStrings.NEXT_ENCOUNTER_EVENT, UBGamePlay.GameplayEventTypes.IntroEncounter);
					};

					this.gameplayController.enemyController.currentEncounter.UpdateCurrentEncounter(this.currentEncounter);
					this.gameplayController.enemyController.nextController.UpdateNextEncounters();
					this.gameplayController.enemyController.TransitionCurrentEncounterIn(afterTransition);
				};

				this.gameplayController.enemyController.TransitionCurrentEncounterOut(callback);
			}
		}

		void OnGameplayEventReceived(string message, UBGamePlay.GameplayEventTypes type)
		{
			if (type == UBGamePlay.GameplayEventTypes.IntroQuest)
			{
				AdvanceAct();
				this.currentEncounter = GetNextEncounter(true);
				this.gameplayController.enemyController.currentEncounter.UpdateCurrentEncounter(this.currentEncounter);
				this.gameplayController.enemyController.nextController.UpdateNextEncounters();

				this.currentTurnNumber = 1;
				//this.currentEncounter =  PF_GamePlay.encounters.First();

				this.currentPlayer = GameController.Instance.ActiveCharacter;

			}

			if (type == UBGamePlay.GameplayEventTypes.StartQuest)
			{
				StartTurn();

			}

			if (type == UBGamePlay.GameplayEventTypes.OutroEncounter)
			{
				CompleteEncounter();
				return;
			}

			if (type == UBGamePlay.GameplayEventTypes.EnemyTurnEnds || type == UBGamePlay.GameplayEventTypes.PlayerTurnEnds)
			{
				if (message.Contains(GlobalStrings.PLAYER_RESPAWN_EVENT))
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
			if (this.currentEncounter.isEndOfAct)
			{
				//Debug.Log("End of the Road M8..."); // cant skip the last encounter
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
			{ { "Current_Quest", GameController.Instance.ActiveLevel.levelName }, { "Rescued", this.currentEncounter.DisplayName }, { "Character_ID", this.currentPlayer.CharacterId }
			};
			TelemetryManager.RecordPlayerEvent(TelemetryEvent.Client_UnicornFreed, eventData);

			CompleteEncounter(false);
		}

		public void CompleteAct()
		{
			GameController l_gc = GameController.Instance;
			if (null == l_gc) return;

			if (l_gc.QuestProgress != null)
			{
				//this.gameplayController.directionController;
				l_gc.QuestProgress.CurrentAct.Value.IsActCompleted = true;

				if (l_gc.ActiveEncounterList.Count > 1)
				{
					// more acts to go
					GameplayController.RaiseGameplayEvent(GlobalStrings.ACT_COMPLETE_EVENT, UBGamePlay.GameplayEventTypes.OutroAct);

				}
				else
				{
					// no more acts, complete quest
					l_gc.QuestProgress.isQuestWon = true;

					UBEncounter topOfStack = new UBEncounter();
					topOfStack = l_gc.ActiveEncounterList.First();
					l_gc.QuestProgress.CompletedEncounters.Add(topOfStack);
					l_gc.ActiveEncounterList.Remove(l_gc.ActiveEncounterList.First());

					//CycleNextEncounter();
					GameplayController.RaiseGameplayEvent(GlobalStrings.QUEST_COMPLETE_EVENT, UBGamePlay.GameplayEventTypes.OutroQuest);

				}
			}
		}

		public void AdvanceAct()
		{
			GameController l_gc = GameController.Instance;
			if (null == l_gc) return;

			if (l_gc.ActiveLevel.levelData.Acts.Count > 0)
			{

				if (l_gc.QuestProgress.CurrentAct.Equals(new KeyValuePair<string, UBLevelAct>()))
				{
					// FIRST ACT
					l_gc.QuestProgress.CurrentAct = l_gc.ActiveLevel.levelData.Acts.First();
					l_gc.QuestProgress.ActIndex = 0;
				}
				else
				{
					//Following Acts
					int indexToCheck = l_gc.QuestProgress.ActIndex + 1;
					if (l_gc.ActiveLevel.levelData.Acts.Count > indexToCheck)
					{
						l_gc.QuestProgress.CurrentAct = l_gc.ActiveLevel.levelData.Acts.ElementAt(indexToCheck);
						l_gc.QuestProgress.ActIndex = indexToCheck;
						CycleNextEncounter();
					}

				}
			}

		}
	}
}