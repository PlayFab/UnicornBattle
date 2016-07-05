using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelPicker : MonoBehaviour {
	public int numberOfLevels = 6;
	public Transform levelButtonPrefab;
	public Transform overworldSceneObject;
	public Transform gridView;

	public Text selectedQuestName;
	public Text selectedQuestDesc;

	public Button HardMode;
	public Button RaidMode;


	public Sprite checkBox;
	public Sprite checkBoxChecked;
	public Color selectedColor;

	public List<LevelItem> levelItems;
	
	private LevelItem selectedLevel;
	private bool isEncounterListAvailable = false;


	
	// need to track the previous scene
	// need to track when to show the tween scene / when the last tween scene was loaded
	
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
	
		if(details.Contains("Encounters Loaded!") && method == PlayFabAPIMethods.GetTitleData)
		{
			this.isEncounterListAvailable = true;
		}

		CheckToContinue();
	}
	
	void CheckToContinue()
	{
		if(this.isEncounterListAvailable)
		{
			PF_GamePlay.encounters = BuildEncounterList();
			PF_GamePlay.ActiveQuest = selectedLevel.GetLevelItemData();
			SceneController.Instance.RequestSceneChange(SceneController.GameScenes.Gameplay, .333f);
			ResetDataChecks();
		}

	}
	
	
	public List<UB_GamePlayEncounter> BuildEncounterList()
	{
		if(PF_GameData.Encounters.Count > 0)
		{	
			// need to change this list to have another type of class, but for now this will do.
			// TODO add markers for end / beginning of acts (will usually be with boss)
			List<UB_GamePlayEncounter> EncounterOrder = new List<UB_GamePlayEncounter>();
			
			foreach(var act in this.selectedLevel.levelData.Acts)
			{
				List<UB_GamePlayEncounter> ActEncounters = new List<UB_GamePlayEncounter>();
				
                //int bossCreepCount = act.Value.BossCreepEncounters.MinQuantity;
                //int storeCreepCount = act.Value.StoreEncounters.MinQuantity;
                //int heroCreepCount = act.Value.HeroEncounters.MinQuantity;
				
				//Build Creep Set
				if(PF_GameData.Encounters.ContainsKey(act.Value.CreepEncounters.EncounterPool))
				{
					Dictionary<string, UB_EncounterData> pool = PF_GameData.Encounters[act.Value.CreepEncounters.EncounterPool];
					
					// used to access these by index
					List<string> name_list = pool.Keys.ToList();
					List<UB_EncounterData> encounter_list = pool.Values.ToList();
					int creepCount = act.Value.CreepEncounters.MinQuantity;
					
					// try to add the specific creeps
					foreach(var id in act.Value.CreepEncounters.SpawnSpecificEncountersByID)
					{
					
						// start back here start with creating the new()s
						if(pool.ContainsKey(id))
						{
							ActEncounters.Add(new UB_GamePlayEncounter() { DisplayName = id, Data = new UB_EncounterData(pool[id]) });
							creepCount--;
						}
					}
					
					while(creepCount > 0)
					{
						int rng = Random.Range(0, pool.Count);
						ActEncounters.Add(new UB_GamePlayEncounter() { DisplayName = name_list[rng], Data = new UB_EncounterData(encounter_list[rng]) });
						creepCount--;
					} 
				}
				
				//Build MegaCreep Set
				if(PF_GameData.Encounters.ContainsKey(act.Value.MegaCreepEncounters.EncounterPool))
				{
					Dictionary<string, UB_EncounterData> pool = PF_GameData.Encounters[act.Value.MegaCreepEncounters.EncounterPool];
					
					// used to access these by index
					List<string> name_list = pool.Keys.ToList();
					List<UB_EncounterData> encounter_list = pool.Values.ToList();
					int creepCount = act.Value.MegaCreepEncounters.MinQuantity;
					
					// try to add the specific creeps
					foreach(var id in act.Value.MegaCreepEncounters.SpawnSpecificEncountersByID)
					{
						if(pool.ContainsKey(id))
						{
							ActEncounters.Add(new UB_GamePlayEncounter() { DisplayName = id, Data = new UB_EncounterData(pool[id]) });
							creepCount--;
						}
					}
					
					while(creepCount > 0)
					{
						int rng = Random.Range(0, pool.Count);
						ActEncounters.Add(new UB_GamePlayEncounter() { DisplayName = name_list[rng], Data = new UB_EncounterData(encounter_list[rng]) });
						creepCount--;
					} 
				}
				
				//Build RareCreep Set
				if(PF_GameData.Encounters.ContainsKey(act.Value.RareCreepEncounters.EncounterPool))
				{
					Dictionary<string, UB_EncounterData> pool = PF_GameData.Encounters[act.Value.RareCreepEncounters.EncounterPool];
					
					// used to access these by index
					List<string> name_list = pool.Keys.ToList();
					List<UB_EncounterData> encounter_list = pool.Values.ToList();
					int creepCount = act.Value.RareCreepEncounters.MinQuantity;
					
					// try to add the specific creeps
					foreach(var id in act.Value.RareCreepEncounters.SpawnSpecificEncountersByID)
					{
						if(pool.ContainsKey(id))
						{
							ActEncounters.Add(new UB_GamePlayEncounter() { DisplayName = id, Data = new UB_EncounterData(pool[id]) });
							creepCount--;
						}
					}
					
					while(creepCount > 0)
					{
						int rng = Random.Range(0, pool.Count);
						ActEncounters.Add(new UB_GamePlayEncounter() { DisplayName = name_list[rng], Data = new UB_EncounterData(encounter_list[rng]) });
						creepCount--;
					} 
				}
				
				//Build Store Set
				if(PF_GameData.Encounters.ContainsKey(act.Value.StoreEncounters.EncounterPool))
				{
					Dictionary<string, UB_EncounterData> pool = PF_GameData.Encounters[act.Value.StoreEncounters.EncounterPool];
					
					// used to access these by index
					List<string> name_list = pool.Keys.ToList();
					List<UB_EncounterData> encounter_list = pool.Values.ToList();
					int creepCount = act.Value.StoreEncounters.MinQuantity;
					
					// try to add the specific creeps
					foreach(var id in act.Value.StoreEncounters.SpawnSpecificEncountersByID)
					{
						if(pool.ContainsKey(id))
						{
							ActEncounters.Add(new UB_GamePlayEncounter() { DisplayName = id, Data = new UB_EncounterData(pool[id]) });
							creepCount--;
						}
					}
					
					while(creepCount > 0)
					{
						int rng = Random.Range(0, pool.Count);
						ActEncounters.Add(new UB_GamePlayEncounter() { DisplayName = name_list[rng], Data = new UB_EncounterData(encounter_list[rng]) });
						creepCount--;
					} 
				}
				
				//Build Hero Set
				if(PF_GameData.Encounters.ContainsKey(act.Value.HeroEncounters.EncounterPool))
				{
					Dictionary<string, UB_EncounterData> pool = PF_GameData.Encounters[act.Value.HeroEncounters.EncounterPool];
					
					// used to access these by index
					List<string> name_list = pool.Keys.ToList();
					List<UB_EncounterData> encounter_list = pool.Values.ToList();
					int creepCount = act.Value.HeroEncounters.MinQuantity;
					
					// try to add the specific creeps
					foreach(var id in act.Value.HeroEncounters.SpawnSpecificEncountersByID)
					{
						if(pool.ContainsKey(id))
						{
							ActEncounters.Add(new UB_GamePlayEncounter() { DisplayName = id, Data = new UB_EncounterData(pool[id]) });
							creepCount--;
						}
					}
					
					while(creepCount > 0)
					{
						int rng = Random.Range(0, pool.Count);
						ActEncounters.Add(new UB_GamePlayEncounter() { DisplayName = name_list[rng], Data = new UB_EncounterData(encounter_list[rng]) });
						creepCount--;
					} 
				}
				
				// shuffle deck here?
				//EncounterOrder = EncounterOrder.shuffle();
				ActEncounters.Shuffle();
				
				//Build Boss Set
				if(PF_GameData.Encounters.ContainsKey(act.Value.BossCreepEncounters.EncounterPool))
				{
					Dictionary<string, UB_EncounterData> pool = PF_GameData.Encounters[act.Value.BossCreepEncounters.EncounterPool];
					
					// used to access these by index
					List<string> name_list = pool.Keys.ToList();
					List<UB_EncounterData> encounter_list = pool.Values.ToList();
					int creepCount = act.Value.BossCreepEncounters.MinQuantity;
					
					// try to add the specific creeps
					foreach(var id in act.Value.BossCreepEncounters.SpawnSpecificEncountersByID)
					{
						if(pool.ContainsKey(id))
						{
							ActEncounters.Add(new UB_GamePlayEncounter() { DisplayName = id, Data = new UB_EncounterData(pool[id]) });
							creepCount--;
						}
					}
					
					while(creepCount > 0)
					{
						int rng = Random.Range(0, pool.Count);
						ActEncounters.Add(new UB_GamePlayEncounter() { DisplayName = name_list[rng], Data = new UB_EncounterData(encounter_list[rng]) });
						creepCount--;
					} 
				}
				ActEncounters.Last().isEndOfAct = true;
				EncounterOrder.AddRange(ActEncounters);
			}
			return EncounterOrder;
		}
		return null;
	}
	
	void ResetDataChecks()
	{
		this.isEncounterListAvailable = false;
	}

	public void ToggleRaidMode()
	{
		PF_GamePlay.UseRaidMode = !PF_GamePlay.UseRaidMode;
		if (PF_GamePlay.UseRaidMode == true) {
			this.RaidMode.GetComponent<Image> ().overrideSprite = this.checkBoxChecked;
		} else {
			this.RaidMode.GetComponent<Image> ().overrideSprite = this.checkBox;
		}
	}

	public void ToggleHardMode()
	{
		PF_GamePlay.isHardMode = !PF_GamePlay.isHardMode;

		if (PF_GamePlay.isHardMode == true) {
			this.HardMode.GetComponent<Image> ().overrideSprite = this.checkBoxChecked;
		} else {
			this.HardMode.GetComponent<Image> ().overrideSprite = this.checkBox;
		}
	}

	
	public void LevelItemClicked(LevelItem item)
	{
		if (item == this.selectedLevel)
			return;

		DeselectLevelItems ();

		selectedLevel = item;
		selectedLevel.GetComponent<Image> ().color = selectedColor;

		this.selectedQuestName.text = selectedLevel.levelName;
		this.selectedQuestDesc.text = selectedLevel.levelData.Description;

		Dictionary<string, int> charStats = PF_PlayerData.characterStatistics [PF_PlayerData.activeCharacter.characterDetails.CharacterId];


		if (PF_PlayerData.characterStatistics.Count > 0) {
			if (charStats.ContainsKey(selectedLevel.levelData.StatsPrefix + "Complete")) 
			{
				if (PF_GamePlay.isHardMode == true) 
				{
					this.HardMode.GetComponent<Image> ().overrideSprite = this.checkBoxChecked;
				}
				else
				{
					this.HardMode.GetComponent<Image> ().overrideSprite = this.checkBox;
				}
				this.HardMode.gameObject.SetActive (true);
			} 
			else 
			{
				this.HardMode.gameObject.SetActive (false);
			}
		}
		else 
		{
			this.HardMode.gameObject.SetActive (false);
		}

		//ShowSelectedLevel();
	}
	
	public void DeselectLevelItems()
	{
		foreach (var item in this.levelItems) {
			item.GetComponent<Image>().color = Color.white;
		}
	}


	public void ShowSelectedLevel()
	{
		//this.selectedLevelSceneObject.gameObject.SetActive(true);
		this.overworldSceneObject.gameObject.SetActive(false);
	}
	
	public void ShowOverworld()
	{
		//this.selectedLevelSceneObject.gameObject.SetActive(false);
		this.overworldSceneObject.gameObject.SetActive(true);
		this.selectedLevel = null;
		
	}


	public void PlaySelectedLevel()
	{
		//SceneController.Instance.RequestSceneChange(SceneController.GameScenes.Gameplay, .333f);
		//Init();
		
		// build an encounters' pools list. from these pools we can randomize an order to the encounters.
		List<string> encounters = new List<string>();
		
		//TODO sometimes this throws a nullref ?
		foreach(var act in selectedLevel.levelData.Acts)
		{
			if( !string.IsNullOrEmpty(act.Value.CreepEncounters.EncounterPool))
			{
				int exists = encounters.IndexOf(act.Value.CreepEncounters.EncounterPool);
				
				if(exists == -1)
				{
					encounters.Add(act.Value.CreepEncounters.EncounterPool);
				}
			} 
			
			if( !string.IsNullOrEmpty(act.Value.MegaCreepEncounters.EncounterPool))
			{
				int exists = encounters.IndexOf(act.Value.MegaCreepEncounters.EncounterPool);
				
				if(exists == -1)
				{
					encounters.Add(act.Value.MegaCreepEncounters.EncounterPool);
				}
			} 
			
			if( !string.IsNullOrEmpty(act.Value.RareCreepEncounters.EncounterPool))
			{
				int exists = encounters.IndexOf(act.Value.RareCreepEncounters.EncounterPool);
				
				if(exists == -1)
				{
					encounters.Add(act.Value.RareCreepEncounters.EncounterPool);
				}
			} 
			
			if( !string.IsNullOrEmpty(act.Value.BossCreepEncounters.EncounterPool))
			{
				int exists = encounters.IndexOf(act.Value.BossCreepEncounters.EncounterPool);
				
				if(exists == -1)
				{
					encounters.Add(act.Value.BossCreepEncounters.EncounterPool);
				}
			} 
			
			if( !string.IsNullOrEmpty(act.Value.StoreEncounters.EncounterPool))
			{
				int exists = encounters.IndexOf(act.Value.StoreEncounters.EncounterPool);
				
				if(exists == -1)
				{
					encounters.Add(act.Value.StoreEncounters.EncounterPool);
				}
			} 
			
			if( !string.IsNullOrEmpty(act.Value.HeroEncounters.EncounterPool))
			{
				int exists = encounters.IndexOf(act.Value.HeroEncounters.EncounterPool);
				
				if(exists == -1)
				{
					encounters.Add(act.Value.HeroEncounters.EncounterPool);
				}
			} 
			
		}
		
		PF_GameData.GetEncounterLists(encounters);
		//TODO Set difficulty level on the selected level item.
	}
	
	
	public void Init()
	{
		for(int z = 0; z < this.levelItems.Count; z++)
		{	
			Destroy(this.levelItems[z].gameObject);
		}
		this.levelItems.Clear();
		
		if(PF_GameData.Levels.Count > 0)
		{
			foreach(var item in PF_GameData.Levels)
			{
				if(item.Value.IsHidden == false)
				{
					//TODO add locked levels

					Transform slot = Instantiate(this.levelButtonPrefab);
					slot.SetParent(gridView, false);
					LevelItem li = slot.GetComponent<LevelItem>();
					li.levelData = item.Value;
					li.levelName = item.Key;
					li.levelIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(item.Value.Icon);
					this.levelItems.Add(li);
				}

			}

			LevelItemClicked(this.levelItems[0]);
		}
		
		if (PF_GamePlay.UseRaidMode == true) {
			this.RaidMode.GetComponent<Image> ().overrideSprite = this.checkBoxChecked;
		} else {
			this.RaidMode.GetComponent<Image> ().overrideSprite = this.checkBox;
		}
		//this.confirmButton.onClick.RemoveAllListeners();
		//this.confirmButton.onClick.AddListener(() => { cPicker.PonyPicked(selectedSlot);});
		
//		if(selectedLevel != null)
//		{
//			ShowSelectedLevel();
//		}
//		else
//		{
//			ShowOverworld();
//		}
	}
	
}



// TODO MOVE THIS TO ITS OWN FILE
// LIST EXTENTION SHUFFLE METHOD
public static class ListExtensionMethods
{
	
	public static void Shuffle<T>(this IList<T> list)  
	{  
		System.Random rng = new System.Random();  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}
}

