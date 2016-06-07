using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;

public enum PromotionalItemTypes { None, News, Image, Sale, Event, Tip}
public class UB_PromotionalItem
{
	public string PromoTitle;
	public DateTime TimeStamp;
	public string PromoBody;
	public string PromoBanner;
	public string PromoSplash;
	public string PromoId;
	
	public Dictionary<string,string> CustomTags = new Dictionary<string, string>();
	public PromotionalItemTypes PromoType;	

	//ctor
	public UB_PromotionalItem(){}
}

public class UB_LevelData
{
	public string Description { get; set; }
	public string Icon { get; set; }
	public string StatsPrefix { get; set; }
	public int MinEntryLevel { get; set; }
	public bool IsLocked { get; set; }
	public bool IsHidden { get; set; }
	public Dictionary<string, int> MasteryRequired { get; set; }
	public Dictionary<string, int> EntryToken { get; set; }
	public Dictionary<string, int> EntryCurrency { get; set; } 
	public string DisplayGroup { get; set; }
	public int DisplayOrder { get; set; }
	public string EndOfQuestStore { get; set; }
	public UB_LevelWinConditions WinConditions { get; set; }
	public Dictionary <string, UB_LevelAct> Acts { get; set; }	

	//ctor
	public UB_LevelData(){}
}

public class UB_LevelAct
{
	public string Background { get; set; }
	public string UsePregeneratedEncounter { get; set; }
	public UB_LevelEncounters CreepEncounters { get; set; }
	public UB_LevelEncounters MegaCreepEncounters { get; set; }
	public UB_LevelEncounters RareCreepEncounters { get; set; }
	public UB_LevelEncounters BossCreepEncounters { get; set; }
	public UB_LevelEncounters HeroEncounters { get; set; }
	public UB_LevelEncounters StoreEncounters { get; set; }
	
	public string IntroMonolog { get; set; }
	public string IntroBossMonolog { get; set; }
	public string OutroMonolog { get; set; }
	public string FailureMonolog { get; set; }
	public UB_LevelActRewards RewardTable { get; set; }
	public bool IsActCompleted { get; set; }

	//ctor
	public UB_LevelAct(){}
}

public class UB_LevelWinConditions
{
	public int SurvivalTime { get; set; }
	public long TimeLimit { get; set; }
	public int KillCount { get; set; }
	public string KillTarget { get; set; }
	public bool CompleteAllActs { get; set; }
	public int FindCount { get; set; }
	public string FindTarget { get; set; }

	//ctor
	public UB_LevelWinConditions(){}
}

public class UB_LevelEncounters
{
	public string EncounterPool { get; set; }
	public int MinQuantity { get; set; }
	public int MaxQuantity { get; set; }
	public float ChanceForAddedEncounters { get; set; }
	public List<string> SpawnSpecificEncountersByID { get; set; }
	public bool LimitProbabilityToAct { get; set; }

	//ctor
	public UB_LevelEncounters(){}
}

public class UB_GamePlayEncounter
{
	public string DisplayName { get; set; }
	public UB_EncounterData Data { get; set; }
	public bool playerCompleted { get; set; } // not sure about this var
	public bool isEndOfAct { get; set; }
	//TODO add a bool for signals end of act

	//ctor
	public UB_GamePlayEncounter(){}
}

public class UB_GamePlayActions
{
	string ActionName { get; set; }
	string ActionDetails { get; set; }

	//ctor
	public UB_GamePlayActions(){}
}

public enum EncounterTypes { Creep, MegaCreep, RareCreep, BossCreep, Hero, Store }
public class UB_EncounterData
{
	public float SpawnWeight { get; set; }
	public EncounterTypes EncounterType { get; set; }
	public string Description { get; set; }
	public string Icon  { get; set; }
	
	public Dictionary<string, EnemySpellDetail> Spells { get; set; }
	public EnemyVitals Vitals { get; set; }
	public EncounterRewards Rewards { get; set; }
	
	// some list of actions
	public Dictionary<string,string> EncounterActions { get; set; }
	

	public void SetSpellDetails()
	{
		if(this.Spells == null)
		{
			this.Spells = new Dictionary<string, EnemySpellDetail>();
		}
		
		foreach(var spell in this.Spells)
		{
			UB_SpellDetail sp = PF_GameData.Spells.ContainsKey(spell.Value.SpellName) ? PF_GameData.Spells[spell.Value.SpellName] : null;
			if(sp != null)
			{
				sp = this.UpgradeSpell(sp, spell.Value.SpellLevel);
				spell.Value.Detail = sp;
				this.Vitals.Spells.Add(spell.Value);	
			}
		}
	}
	
	UB_SpellDetail UpgradeSpell(UB_SpellDetail sp, int level)
	{
		for(int z = 0; z < level; z++)
		{
			sp.BaseDmg *= Mathf.CeilToInt(1.0f + sp.UpgradePower);
			
			if(sp.ApplyStatus != null)
			{
				sp.ApplyStatus.ChanceToApply *= 1.0f + sp.UpgradePower;
				sp.ApplyStatus.ModifyAmount *= 1.0f + sp.UpgradePower;
			}
		}
		return sp;
	}

	//ctor
	public UB_EncounterData(){}

	//copy ctor
	public UB_EncounterData(UB_EncounterData prius)
	{
		if(prius != null)
		{
			this.SpawnWeight = prius.SpawnWeight;
			this.EncounterType = prius.EncounterType;
			this.Description = prius.Description;
			this.Icon = prius.Icon;
			
			this.Vitals = new EnemyVitals(prius.Vitals);
			
			this.Rewards = new EncounterRewards(prius.Rewards);
			
			this.EncounterActions = new Dictionary<string, string>();
			foreach(var kvp in prius.EncounterActions)
			{
				this.EncounterActions.Add(kvp.Key, kvp.Value);
			}	
			
			this.Spells = new Dictionary<string, EnemySpellDetail>();
			foreach(var spell in prius.Spells)
			{
				this.Spells.Add(spell.Key, new EnemySpellDetail(spell.Value));
			}
			
			this.Vitals.ActiveStati = new List<UB_SpellStatus>();
			foreach(var status in prius.Vitals.ActiveStati)
			{
				this.Vitals.ActiveStati.Add(new UB_SpellStatus(status));
			}
		}
	}


}


public class UB_Achievement
{
	public string AchievementName { get; set; }
	public string MatchingStatistic { get; set; }
	public bool SingleStat { get; set; }
	public int Threshold { get; set; }
	public string Icon { get; set; }

	//ctor
	public UB_Achievement(){}
}


public class EnemySpellDetail
{
	public string SpellName { get; set; }
	public int SpellLevel { get; set; }
	public int SpellPriority { get; set; }
	
	public UB_SpellDetail Detail { get; set; }
	public bool IsLocked { get; set; }
	public bool IsOnCooldown { get; set; }
	public int CdTurns { get; set; }
	
	//ctor
	public EnemySpellDetail(){}

	//copy ctor
	public EnemySpellDetail(EnemySpellDetail prius)
	{
		if(prius != null)
		{
			this.SpellName = prius.SpellName;
			this.SpellPriority = prius.SpellPriority;
			this.SpellLevel = prius.SpellLevel;
			this.IsLocked = prius.IsLocked;
			this.IsOnCooldown = prius.IsOnCooldown;
			this.CdTurns = prius.CdTurns;
			this.Detail = new UB_SpellDetail(prius.Detail);
		}
	}
}


public class EnemyVitals
{
	public int Health { get; set; }
	public int Mana { get; set; }
	public int Speed { get; set; }
	public int Defense { get; set; }
	public int CharacterLevel { get; set; }
	
	public List<string> UsableItems { get; set; }
	public List<UB_SpellStatus> ActiveStati { get; set; } 
	public List<EnemySpellDetail> Spells { get; set; }
	
	public int MaxHealth { get; set; }
	public int MaxMana { get; set; }
	public int MaxSpeed { get; set; }
	public int MaxDefense { get; set; }
	
	//public UB_Spell
	
	public void SetMaxVitals()
	{
		this.MaxHealth = this.Health;
		this.MaxMana = this.Mana;
		this.MaxSpeed = this.Speed;
		this.MaxDefense = this.Defense;
		this.Spells = new List<EnemySpellDetail>();
	}

	//ctor
	public EnemyVitals(){}
	
	//Copy ctor
	public EnemyVitals( EnemyVitals prius)
	{
		if(prius != null)
		{
			this.Health = prius.Health;
			this.Mana = prius.Mana;
			this.Speed = prius.Speed;
			this.Defense = prius.Defense;
			this.CharacterLevel = prius.CharacterLevel;
			this.MaxHealth = prius.MaxHealth;
			this.MaxMana  = prius.MaxMana;
			this.MaxSpeed = prius.MaxSpeed;
			this.MaxDefense = prius.MaxDefense;
			
			if(prius.UsableItems != null && prius.UsableItems.Count > 0)
			{
				this.UsableItems = prius.UsableItems.ToList();
			}
			else
			{
				this.UsableItems = new List<string>();
			}
			
			Spells  = new List<EnemySpellDetail>();
			if(prius.Spells != null && prius.Spells.Count > 0)
			{
				foreach(var spell in prius.Spells)
				{
					this.Spells.Add(new EnemySpellDetail(spell));
				}
			}
			
			ActiveStati = new List<UB_SpellStatus>();
			if(prius.ActiveStati !=null && prius.ActiveStati.Count > 0)
			{
				foreach(var status in ActiveStati)
				{
					this.ActiveStati.Add(new UB_SpellStatus(status));
				}
			}
		}
	}
	
}



public enum PromotionType { Other, Active, Promoted, Upcomming }
public class UB_SaleData
{
	public string SaleName { get; set; }
	public string SaleDescription { get; set; }
	public string StoreToUse { get; set; }
	
	//[JsonConverter(typeof(IsoDateTimeConverter))]
	public DateTime StartDate { get; set; }
	
	//[JsonConverter(typeof(IsoDateTimeConverter))]
	public DateTime EndDate { get; set; }
	
	public string BundleId { get; set; }
	public bool PromoteWithInterstitial { get; set; }
	public bool PromoteWithCarousel { get; set; }
	public UB_SalesOccurence Occurence { get; set; }
	
	public UB_UnpackedAssetBundle Assets { get; set; }
	public PromotionType PromoType { get; set; } 
	
	//ctor
	public UB_SaleData(){}
	
}

public enum SalesAvailability { Null, Daily, Weekends}
public class UB_SalesOccurence
{
	//[JsonConverter(typeof(StringEnumConverter))]
	public SalesAvailability Availability { get; set; }
	public string OpensAt { get; set; }
	public string ClosesAt { get; set; }
	


	//ctor
	public UB_SalesOccurence(){}
}


public class UB_EventData
{
	public string EventName { get; set; }
	public string EventDescription { get; set; }
	public string StoreToUse { get; set; }
	
	//[JsonConverter(typeof(IsoDateTimeConverter))]
	public DateTime StartDate { get; set; }
	
	//[JsonConverter(typeof(IsoDateTimeConverter))]
	public DateTime EndDate { get; set; }
	
	public string BundleId { get; set; }
	public List<string> AssociatedLevels { get; set; }
	public UB_EventTrigger EventTrigger { get; set; }
	
	public UB_UnpackedAssetBundle Assets { get; set; }
	public PromotionType PromoType { get; set; } 

	//ctor
	public UB_EventData(){}
}

public class UB_EventTrigger
{
	public string MinimumPlayerLevel { get; set; }
	public List<string> RequiredAchievements { get; set; }
	
	//ctor
	public UB_EventTrigger(){}
}

public enum OfferAppliesTo { Character, Player }
public class UB_OfferData
{
	public string OfferName { get; set; }
	public string OfferDescription { get; set; }
	public string StoreToUse { get; set; }
	public string ItemToGrant { get; set; }
	
	public OfferAppliesTo AppliesTo { get; set; }
	
	//[JsonConverter(typeof(StringEnumConverter))]

	
	public UB_OfferTrigger OfferTrigger { get; set; }

	//ctor
	public UB_OfferData(){}
}

public enum OfferOccurence { Single, Repeat }
public class UB_OfferTrigger
{
	public int OnLevelGained { get; set; }
	public string OnAchievementGained { get; set; }
	
	
	public OfferOccurence Occurence { get; set; }
	
	//[JsonConverter(typeof(StringEnumConverter))]


	//ctor
	public UB_OfferTrigger(){}
	
}

public class UB_AwardedOffer
{
	public string OfferId { get; set; }
	public string AppliesToCharacter { get; set; }
	
	public UB_OfferTrigger Occurence { get; set; }
	public long AwardedOn { get; set; }
	public long RedeemedOn { get; set; }

	//ctor
	public UB_AwardedOffer(){}
}


public class EncounterRewards
{
	public int XpMin { get; set; } 
	public int XpMax { get; set; } 
	public int GoldMin { get; set; } 
	public int GoldMax { get; set; } 
	public List<string> ItemsDropped { get; set; } 

	//ctor
	public EncounterRewards(){}

	//copy ctor
	public EncounterRewards(EncounterRewards prius)
	{
		if(prius != null)
		{
			this.XpMin = prius.XpMin;
			this.XpMax = prius.XpMax;
			this.GoldMin = prius.GoldMin;
			this.GoldMax = prius.GoldMax;
			this.ItemsDropped = prius.ItemsDropped.ToList();
		}
	}
}





public class UB_LevelActRewards
{
	public List<string> Easy { get; set; }
	public List<string> Medium { get; set; }
	public List<string> Hard { get; set; }	 
	public string VictoryEncounter { get; set; }

	//ctor
	public UB_LevelActRewards(){}
}


public class UB_CharacterData
{
	public UB_ClassDetail ClassDetails  { get; set; }
	public int TotalExp { get; set; }
	public int ExpThisLevel { get; set; }
	public int Health { get; set; }
	public int Mana { get; set; }
	public int Speed { get; set; }
	public int Defense { get; set; }
	public int CharacterLevel { get; set; }
	public int Spell1_Level  { get; set; }
	public int Spell2_Level  { get; set; }
	public int Spell3_Level  { get; set; }
	public string CustomAvatar { get; set; }

	//ctor
	public UB_CharacterData(){}
}



[System.Serializable]
public class UB_ClassDetail
{
	public string Description { get; set; }
	public string CatalogCode { get; set; }
	public string Icon { get; set; }
	public string Spell1 { get; set; }
	public string Spell2 { get; set; }
	public string Spell3 { get; set; }
	public int BaseHP { get; set; }
	public int HPLevelBonus { get; set; }
	public int BaseMP { get; set; }
	public int MPLevelBonus { get; set; }
	public int BaseDP { get; set; }
	public int DPLevelBonus { get; set; }
	public int BaseSP { get; set; }
	public int SPLevelBonus { get; set; }
	public string Prereq { get; set; }
	public string DisplayStatus { get; set; }

	//ctor
	public UB_ClassDetail(){}
}

[System.Serializable]
public class UB_SpellDetail
{
	public string Description { get; set; }
	public string Icon { get; set; }
	public string Target { get; set; }
	public int BaseDmg { get; set; }
	public int ManaCost { get; set; }
	public float UpgradePower { get; set; }
	public int UpgradeLevels { get; set; }
	public string FX { get; set; }
	public int Cooldown { get; set; }
	public int LevelReq { get; set; }
	public UB_SpellStatus ApplyStatus { get; set; }

	//ctor
	public UB_SpellDetail(){}

	//copy ctor
	public UB_SpellDetail(UB_SpellDetail prius)
	{
		if(prius != null)
		{
			this.Description = prius.Description;
			this.Icon = prius.Icon;
			this.Target = prius.Target;
			this.BaseDmg = prius.BaseDmg;
			this.ManaCost = prius.ManaCost;
			this.BaseDmg = prius.BaseDmg;
			this.UpgradePower = prius.UpgradePower;
			this.UpgradeLevels = prius.UpgradeLevels;
			this.FX = prius.FX;
			this.Cooldown = prius.Cooldown;
			this.LevelReq = prius.LevelReq;
			this.ApplyStatus = new UB_SpellStatus(prius.ApplyStatus);
		}
	}
}


// struct?
[System.Serializable]
public class UB_SpellStatus
{
	public string StatusName { get; set; }
	public string Target { get; set; }
	public string UpgradeReq { get; set; }
	public string StatusDescription { get; set; }
	public string StatModifierCode  { get; set; } // prbably need to map to an enum 
	public float ModifyAmount  { get; set; }
	public float ChanceToApply { get; set; }
	public int Turns { get; set; }
	public string Icon  { get; set; }
	public string FX  { get; set; }

	//ctor
	public UB_SpellStatus(){}

	// copy ctor
	public UB_SpellStatus(UB_SpellStatus prius)
	{
		if( prius != null)
		{
			this.StatusName = prius.StatusName;
			this.Target = prius.Target;
			this.UpgradeReq = prius.UpgradeReq;
			this.StatusDescription = prius.StatusDescription;
			this.StatModifierCode = prius.StatModifierCode;
			this.ModifyAmount = prius.ModifyAmount;
			this.ChanceToApply = prius.ChanceToApply;
			this.Turns = prius.Turns;
			this.Icon = prius.Icon;
			this.FX = prius.FX;
		}
	}
}

public class UB_Spell
{
	public string SpellName { get; set; }
	public string Description { get; set; }
	public string Icon { get; set; }
	public int Dmg { get; set; }
	public int Level { get; set; }
	public int UpgradeLevels { get; set; }
	public string FX { get; set; }
	public int Cooldown { get; set; }
	public int LevelReq { get; set; }
	public UB_SpellStatus ApplyStatus { get; set; }

	//ctor
	public UB_Spell(){}
}

public class UB_SavedCharacter
{
	public UB_ClassDetail baseClass { get; set; }
	public PlayFab.ClientModels.CharacterResult characterDetails { get; set; }
	public UB_CharacterData characterData { get; set; }
	public PlayerVitals PlayerVitals { get; set; }
	
	public void SetMaxVitals()
	{	
		this.PlayerVitals.MaxHealth = this.characterData.Health;
		this.PlayerVitals.MaxMana = this.characterData.Mana;
		this.PlayerVitals.MaxSpeed = this.characterData.Speed;
		this.PlayerVitals.MaxDefense = this.characterData.Defense;
		
		this.PlayerVitals.Health = this.characterData.Health;
		this.PlayerVitals.Mana = this.characterData.Mana;
		this.PlayerVitals.Speed = this.characterData.Speed;
		this.PlayerVitals.Defense = this.characterData.Defense;
		
		this.PlayerVitals.ActiveStati.Clear();
		this.PlayerVitals.didLevelUp = false;
		this.PlayerVitals.skillSelected = 0;
	}
	
	public void RefillVitals()
	{
		this.PlayerVitals.ActiveStati.Clear();
		this.PlayerVitals.didLevelUp = false;
		this.PlayerVitals.skillSelected = 0;
		
		this.PlayerVitals.Health = this.PlayerVitals.MaxHealth;
		this.PlayerVitals.Mana = this.PlayerVitals.MaxMana;
		this.PlayerVitals.Speed = this.PlayerVitals.MaxSpeed;
		this.PlayerVitals.Defense = this.PlayerVitals.MaxDefense;
		
		
	}
	
	public void LevelUpCharacterStats()
	{
		//TODO add in this -- needs to have a level up table from title data
	}
	
	//ctor
	public UB_SavedCharacter()
	{	
		this.PlayerVitals = new PlayerVitals();
		this.PlayerVitals.ActiveStati = new List<UB_SpellStatus>();
		//TODO can initialize an ingame character tracker.
		//^^^ this will be what will need to get leveled up to match the stats
		//Debug.LogError("UB_SavedCharacter RAN!!!!");
	}
	
}

public class PlayerVitals
{
	public int Health { get; set; }
	public int Mana { get; set; }
	public int Speed { get; set; }
	public int Defense { get; set; }
	public List<UB_SpellStatus> ActiveStati { get; set; } 
	
	public int MaxHealth { get; set; }
	public int MaxMana { get; set; }
	public int MaxSpeed { get; set; }
	public int MaxDefense { get; set; }
	
	public bool didLevelUp { get; set; } 
	public int skillSelected { get; set; } 

	//ctor
	public PlayerVitals(){}
}

public class QuestTracker
{
	public KeyValuePair<string, UB_LevelAct> CurrentAct { get; set; }
	public int ActIndex { get; set; }
	public List<string> WinConditions { get; set; }
	
	//TODO hook up these stats 
	public int DamageDone { get; set; }
	public int DamageTaken { get; set; }
	public int Deaths { get; set; }
	
	public int XpCollected { get; set; }
	public int GoldCollected { get; set; }
	public List<UB_GamePlayEncounter> CompletedEncounters { get; set; }
	public List<string> ItemsFound { get; set; }
	public List<ItemGrantResult> ItemsGranted { get; set; }
	public int CreepEncounters { get; set; }
	public int HeroRescues { get; set; }
	public int ItemsUsed { get; set; }

	//ctor
	public QuestTracker()
	{
		this.WinConditions = new List<string>();
		this.CompletedEncounters = new List<UB_GamePlayEncounter>();
		this.ItemsFound = new List<string>();
	}
	
	public bool isQuestWon = false;
	public bool areItemsAwarded = false;
}

public class ItemGrantResult
{
	public string PlayFabId { get; set; }
	public string ItemId { get; set; }
	public string ItemInstanceId { get; set; }
	public bool Result { get; set; }

	//ctor
	public ItemGrantResult(){}
}

public class CcObj
{
	public int kills { get; set; }
	public int currency { get; set; }
	public List<ItemGrantResult> ItemGrantResults { get; set; }

	//ctor
	public CcObj(){}
}

public class CharacterResult
{
	public string CharacterId { get; set; }
	public string CharacterName { get; set; }
	public string CharacterType { get; set; }

	//ctor
	public CharacterResult(){}

	//ctor
	public CharacterResult(PlayFab.ClientModels.CharacterResult C)
	{
		CharacterId = C.CharacterId;
		CharacterName = C.CharacterName;
		CharacterType = C.CharacterType;
	}
}

public class InventoryCategory
{
	public string itemId = string.Empty;
	public CatalogItem catalogRef;
	public List<ItemInstance> inventory;
	public Sprite icon;
	public bool isConsumable = false;
	public int totalUses = 0;
	public int count { get { return this.inventory.Count; }}


	//ctor
	public InventoryCategory(){}

	//ctor
	public InventoryCategory (string id, CatalogItem cat, List<ItemInstance> inv, Sprite icon)
	{
		this.itemId = id;
		this.catalogRef = cat;
		this.inventory = inv;
		this.icon = icon;
	}

	//ctor
	public InventoryCategory (string id, CatalogItem cat, List<ItemInstance> inv, Sprite icon, bool consumable)
	{
		this.itemId = id;
		this.catalogRef = cat;
		this.inventory = inv;
		this.icon = icon;
		this.isConsumable = consumable;
		
		CalcTotalUses();
	}
	
	public void CalcTotalUses()
	{
		if(this.isConsumable == true)
		{
			this.totalUses = 0;
			foreach(var item in inventory)
			{
				if(item.RemainingUses != null)
				{
					totalUses += (int)item.RemainingUses;
				}
			}
		}
		else
		{
			totalUses = 0;
		}
	}

}