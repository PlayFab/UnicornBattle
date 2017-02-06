using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Promotional item types.
/// </summary>
public enum PromotionalItemTypes { None, News, Image, Sale, Event, Tip }
public class UB_PromotionalItem
{
    public string PromoTitle;
    public DateTime TimeStamp;
    public string PromoBody;
    public string PromoBanner;
    public string PromoSplash;
    public string PromoId;

    public Dictionary<string, string> CustomTags = new Dictionary<string, string>();
    public PromotionalItemTypes PromoType;
}

public class UB_LevelData
{
    public string Description;
    public string Icon;
    public string StatsPrefix;
    public int MinEntryLevel;
    public bool IsLocked;
    public bool IsHidden;
    public Dictionary<string, int> MasteryRequired;
    public Dictionary<string, int> EntryToken;
    public Dictionary<string, int> EntryCurrency;
    public string DisplayGroup;
    public int DisplayOrder;
    public string EndOfQuestStore;
    public UB_LevelWinConditions WinConditions;
    public Dictionary<string, UB_LevelAct> Acts;
}

public class UB_LevelAct
{
    public string Background;
    public string UsePregeneratedEncounter;
    public UB_LevelEncounters CreepEncounters;
    public UB_LevelEncounters MegaCreepEncounters;
    public UB_LevelEncounters RareCreepEncounters;
    public UB_LevelEncounters BossCreepEncounters;
    public UB_LevelEncounters HeroEncounters;
    public UB_LevelEncounters StoreEncounters;

    public string IntroMonolog;
    public string IntroBossMonolog;
    public string OutroMonolog;
    public string FailureMonolog;
    public UB_LevelActRewards RewardTable;
    public bool IsActCompleted;
}

public class UB_LevelWinConditions
{
    public int SurvivalTime;
    public long TimeLimit;
    public int KillCount;
    public string KillTarget;
    public bool CompleteAllActs;
    public int FindCount;
    public string FindTarget;
}

public class UB_LevelEncounters
{
    public string EncounterPool;
    public int MinQuantity;
    public int MaxQuantity;
    public float ChanceForAddedEncounters;
    public List<string> SpawnSpecificEncountersByID;
    public bool LimitProbabilityToAct;
}

public class UB_GamePlayEncounter
{
    public string DisplayName;
    public UB_EncounterData Data;
    public bool playerCompleted; // not sure about this var
    public bool isEndOfAct;
    //TODO add a bool for signals end of act
}

public class UB_GamePlayActions
{
    string ActionName;
    string ActionDetails;
}

public enum EncounterTypes { Creep, MegaCreep, RareCreep, BossCreep, Hero, Store }
public class UB_EncounterData
{
    public float SpawnWeight;
    public EncounterTypes EncounterType;
    public string Description;
    public string Icon;

    public Dictionary<string, EnemySpellDetail> Spells;
    public EnemyVitals Vitals;
    public EncounterRewards Rewards;

    // some list of actions
    public Dictionary<string, string> EncounterActions;

    public void SetSpellDetails()
    {
        if (Spells == null)
        {
            Spells = new Dictionary<string, EnemySpellDetail>();
        }

        foreach (var spell in Spells)
        {
            UB_SpellDetail sp = PF_GameData.Spells.ContainsKey(spell.Value.SpellName) ? PF_GameData.Spells[spell.Value.SpellName] : null;
            if (sp != null)
            {
                sp = UpgradeSpell(sp, spell.Value.SpellLevel);
                spell.Value.Detail = sp;
                Vitals.Spells.Add(spell.Value);
            }
        }
    }

    UB_SpellDetail UpgradeSpell(UB_SpellDetail sp, int level)
    {
        for (int z = 0; z < level; z++)
        {
            sp.BaseDmg *= Mathf.CeilToInt(1.0f + sp.UpgradePower);

            if (sp.ApplyStatus != null)
            {
                sp.ApplyStatus.ChanceToApply *= 1.0f + sp.UpgradePower;
                sp.ApplyStatus.ModifyAmount *= 1.0f + sp.UpgradePower;
            }
        }
        return sp;
    }

    //ctor
    public UB_EncounterData() { }
    //copy ctor
    public UB_EncounterData(UB_EncounterData prius)
    {
        if (prius != null)
        {
            SpawnWeight = prius.SpawnWeight;
            EncounterType = prius.EncounterType;
            Description = prius.Description;
            Icon = prius.Icon;

            Vitals = new EnemyVitals(prius.Vitals);

            Rewards = new EncounterRewards(prius.Rewards);

            EncounterActions = new Dictionary<string, string>();
            foreach (var kvp in prius.EncounterActions)
            {
                EncounterActions.Add(kvp.Key, kvp.Value);
            }

            Spells = new Dictionary<string, EnemySpellDetail>();
            foreach (var spell in prius.Spells)
            {
                Spells.Add(spell.Key, new EnemySpellDetail(spell.Value));
            }

            Vitals.ActiveStati = new List<UB_SpellStatus>();
            foreach (var status in prius.Vitals.ActiveStati)
            {
                Vitals.ActiveStati.Add(new UB_SpellStatus(status));
            }
        }
    }
}

public class UB_Achievement
{
    public string AchievementName;
    public string MatchingStatistic;
    public bool SingleStat;
    public int Threshold;
    public string Icon;
}

public class EnemySpellDetail
{
    public string SpellName;
    public int SpellLevel;
    public int SpellPriority;

    public UB_SpellDetail Detail;
    public bool IsLocked;
    public bool IsOnCooldown;
    public int CdTurns;

    //ctor
    public EnemySpellDetail() { }
    //copy ctor
    public EnemySpellDetail(EnemySpellDetail prius)
    {
        if (prius != null)
        {
            SpellName = prius.SpellName;
            SpellPriority = prius.SpellPriority;
            SpellLevel = prius.SpellLevel;
            IsLocked = prius.IsLocked;
            IsOnCooldown = prius.IsOnCooldown;
            CdTurns = prius.CdTurns;
            Detail = new UB_SpellDetail(prius.Detail);
        }
    }
}

public class EnemyVitals
{
    public int Health;
    public int Mana;
    public int Speed;
    public int Defense;
    public int CharacterLevel;

    public List<string> UsableItems;
    public List<UB_SpellStatus> ActiveStati;
    public List<EnemySpellDetail> Spells;

    public int MaxHealth;
    public int MaxMana;
    public int MaxSpeed;
    public int MaxDefense;

    //public UB_Spell

    public void SetMaxVitals()
    {
        MaxHealth = Health;
        MaxMana = Mana;
        MaxSpeed = Speed;
        MaxDefense = Defense;
        Spells = new List<EnemySpellDetail>();
    }

    //ctor
    public EnemyVitals() { }
    //Copy ctor
    public EnemyVitals(EnemyVitals prius)
    {
        if (prius != null)
        {
            Health = prius.Health;
            Mana = prius.Mana;
            Speed = prius.Speed;
            Defense = prius.Defense;
            CharacterLevel = prius.CharacterLevel;
            MaxHealth = prius.MaxHealth;
            MaxMana = prius.MaxMana;
            MaxSpeed = prius.MaxSpeed;
            MaxDefense = prius.MaxDefense;

            if (prius.UsableItems != null && prius.UsableItems.Count > 0)
            {
                UsableItems = prius.UsableItems.ToList();
            }
            else
            {
                UsableItems = new List<string>();
            }

            Spells = new List<EnemySpellDetail>();
            if (prius.Spells != null && prius.Spells.Count > 0)
            {
                foreach (var spell in prius.Spells)
                {
                    Spells.Add(new EnemySpellDetail(spell));
                }
            }

            ActiveStati = new List<UB_SpellStatus>();
            if (prius.ActiveStati != null && prius.ActiveStati.Count > 0)
            {
                foreach (var status in ActiveStati)
                {
                    ActiveStati.Add(new UB_SpellStatus(status));
                }
            }
        }
    }
}

public class UB_AdData
{
    public AdPlacementDetails Details;
}

public enum PromotionType { Other, Active, Promoted, Upcomming }
public class UB_SaleData
{
    public string SaleName;
    public string SaleDescription;
    public string StoreToUse;

    //[JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime StartDate;

    //[JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime EndDate;

    public string BundleId;
    public bool PromoteWithInterstitial;
    public bool PromoteWithCarousel;
    public UB_SalesOccurence Occurence;

    public UB_UnpackedAssetBundle Assets;
    public PromotionType PromoType;
}

public enum SalesAvailability { Null, Daily, Weekends }
public class UB_SalesOccurence
{
    //[JsonConverter(typeof(StringEnumConverter))]
    public SalesAvailability Availability;
    public string OpensAt;
    public string ClosesAt;
}

public class UB_EventData
{
    public string EventName;
    public string EventDescription;
    public string StoreToUse;
    public DateTime StartDate;
    public DateTime EndDate;
    public string BundleId;
    public List<string> AssociatedLevels;
    public UB_EventTrigger EventTrigger;

    public UB_UnpackedAssetBundle Assets;
    public PromotionType PromoType;
}

public class UB_EventTrigger
{
    public string MinimumPlayerLevel;
    public List<string> RequiredAchievements;
}

public enum OfferAppliesTo { Character, Player }
public class UB_OfferData
{
    public string OfferName;
    public string OfferDescription;
    public string StoreToUse;
    public string ItemToGrant;

    public OfferAppliesTo AppliesTo;
    public UB_OfferTrigger OfferTrigger;
}

public enum OfferOccurence { Single, Repeat }
public class UB_OfferTrigger
{
    public int OnLevelGained;
    public string OnAchievementGained;
    public OfferOccurence Occurence;
}

public class UB_AwardedOffer
{
    public string OfferId;
    public string AppliesToCharacter;

    public UB_OfferTrigger Occurence;
    public long AwardedOn;
    public long RedeemedOn;
}


public class EncounterRewards
{
    public int XpMin;
    public int XpMax;
    public int GoldMin;
    public int GoldMax;
    public List<string> ItemsDropped;

    //ctor
    public EncounterRewards() { }
    //copy ctor
    public EncounterRewards(EncounterRewards prius)
    {
        if (prius != null)
        {
            XpMin = prius.XpMin;
            XpMax = prius.XpMax;
            GoldMin = prius.GoldMin;
            GoldMax = prius.GoldMax;
            ItemsDropped = prius.ItemsDropped.ToList();
        }
    }
}

public class UB_LevelActRewards
{
    public List<string> Easy;
    public List<string> Medium;
    public List<string> Hard;
    public string VictoryEncounter;
}

public class UB_CharacterData
{
    public UB_ClassDetail ClassDetails;
    public int TotalExp;
    public int ExpThisLevel;
    public int Health;
    public int Mana;
    public int Speed;
    public int Defense;
    public int CharacterLevel;
    public int Spell1_Level;
    public int Spell2_Level;
    public int Spell3_Level;
    public string CustomAvatar;
}

[Serializable]
public class UB_ClassDetail
{
    public string Description;
    public string CatalogCode;
    public string Icon;
    public string Spell1;
    public string Spell2;
    public string Spell3;
    public int BaseHP;
    public int HPLevelBonus;
    public int BaseMP;
    public int MPLevelBonus;
    public int BaseDP;
    public int DPLevelBonus;
    public int BaseSP;
    public int SPLevelBonus;
    public string Prereq;
    public string DisplayStatus;
}

/// <summary>
/// 
/// </summary>
[Serializable]
public class UB_SpellDetail
{
    public string Description;
    public string Icon;
    public string Target;
    public int BaseDmg;
    public int ManaCost;
    public float UpgradePower;
    public int UpgradeLevels;
    public string FX;
    public int Cooldown;
    public int LevelReq;
    public UB_SpellStatus ApplyStatus;

    //ctor
    public UB_SpellDetail() { }
    //copy ctor
    public UB_SpellDetail(UB_SpellDetail prius)
    {
        if (prius != null)
        {
            Description = prius.Description;
            Icon = prius.Icon;
            Target = prius.Target;
            BaseDmg = prius.BaseDmg;
            ManaCost = prius.ManaCost;
            BaseDmg = prius.BaseDmg;
            UpgradePower = prius.UpgradePower;
            UpgradeLevels = prius.UpgradeLevels;
            FX = prius.FX;
            Cooldown = prius.Cooldown;
            LevelReq = prius.LevelReq;
            ApplyStatus = new UB_SpellStatus(prius.ApplyStatus);
        }
    }
}


/// <summary>
/// details the attributes for effects to apply when spells hit their target. 
/// </summary>
[Serializable]
public class UB_SpellStatus
{
    public string StatusName;
    public string Target;
    public string UpgradeReq;
    public string StatusDescription;
    public string StatModifierCode; // prbably need to map to an enum 
    public float ModifyAmount;
    public float ChanceToApply;
    public int Turns;
    public string Icon;
    public string FX;

    // ctor
    public UB_SpellStatus() { }
    //copy ctor
    public UB_SpellStatus(UB_SpellStatus prius)
    {
        if (prius != null)
        {
            StatusName = prius.StatusName;
            Target = prius.Target;
            UpgradeReq = prius.UpgradeReq;
            StatusDescription = prius.StatusDescription;
            StatModifierCode = prius.StatModifierCode;
            ModifyAmount = prius.ModifyAmount;
            ChanceToApply = prius.ChanceToApply;
            Turns = prius.Turns;
            Icon = prius.Icon;
            FX = prius.FX;
        }
    }
}

/// <summary>
/// details spell attributes
/// </summary>
public class UB_Spell
{
    public string SpellName;
    public string Description;
    public string Icon;
    public int Dmg;
    public int Level;
    public int UpgradeLevels;
    public string FX;
    public int Cooldown;
    public int LevelReq;
    public UB_SpellStatus ApplyStatus;
}

/// <summary>
/// A wrapper class that contains several important classes for tracking player character state.  
/// </summary>
public class UB_SavedCharacter
{
    public UB_ClassDetail baseClass;
    public PlayFab.ClientModels.CharacterResult characterDetails;
    public UB_CharacterData characterData;
    public PlayerVitals PlayerVitals;

    public void SetMaxVitals()
    {
        PlayerVitals.MaxHealth = characterData.Health;
        PlayerVitals.MaxMana = characterData.Mana;
        PlayerVitals.MaxSpeed = characterData.Speed;
        PlayerVitals.MaxDefense = characterData.Defense;

        PlayerVitals.Health = characterData.Health;
        PlayerVitals.Mana = characterData.Mana;
        PlayerVitals.Speed = characterData.Speed;
        PlayerVitals.Defense = characterData.Defense;

        PlayerVitals.ActiveStati.Clear();
        PlayerVitals.didLevelUp = false;
        PlayerVitals.skillSelected = 0;
    }

    public void RefillVitals()
    {
        PlayerVitals.ActiveStati.Clear();
        PlayerVitals.didLevelUp = false;
        PlayerVitals.skillSelected = 0;

        PlayerVitals.Health = PlayerVitals.MaxHealth;
        PlayerVitals.Mana = PlayerVitals.MaxMana;
        PlayerVitals.Speed = PlayerVitals.MaxSpeed;
        PlayerVitals.Defense = PlayerVitals.MaxDefense;
    }

    public void LevelUpCharacterStats()
    {
        //TODO add in this -- needs to have a level up table from title data
    }

    //ctor
    public UB_SavedCharacter()
    {
        PlayerVitals = new PlayerVitals();
        PlayerVitals.ActiveStati = new List<UB_SpellStatus>();
        //TODO can initialize an ingame character tracker.
        //^^^ this will be what will need to get leveled up to match the stats
        //Debug.LogError("UB_SavedCharacter RAN!!!!");
    }
}


/// <summary>
/// A class for tracking players through combat
/// </summary>
public class PlayerVitals
{
    public int Health;
    public int Mana;
    public int Speed;
    public int Defense;
    public List<UB_SpellStatus> ActiveStati;

    public int MaxHealth;
    public int MaxMana;
    public int MaxSpeed;
    public int MaxDefense;

    public bool didLevelUp;
    public int skillSelected;
}

/// <summary>
/// Details the player progress 
/// </summary>
public class QuestTracker
{
    public KeyValuePair<string, UB_LevelAct> CurrentAct;
    public int ActIndex;
    public List<string> WinConditions;

    //TODO hook up these stats 
    public int DamageDone;
    public int DamageTaken;
    public int Deaths;

    public int XpCollected;
    public int GoldCollected;
    public List<UB_GamePlayEncounter> CompletedEncounters;
    public List<string> ItemsFound;
    public List<ItemGrantResult> ItemsGranted;
    public int CreepEncounters;
    public int HeroRescues;
    public int ItemsUsed;

    //ctor
    public QuestTracker()
    {
        WinConditions = new List<string>();
        CompletedEncounters = new List<UB_GamePlayEncounter>();
        ItemsFound = new List<string>();
    }

    public bool isQuestWon = false;
    public bool areItemsAwarded = false;
}

/// <summary>
/// basic details that describe an item that was granted to the player
/// </summary>
public class ItemGrantResult
{
    public string PlayFabId;
    public string ItemId;
    public string ItemInstanceId;
    public bool Result;
}

/// <summary>
/// A custom grouping optomized for use in UB.
/// </summary>
public class InventoryCategory
{
    public string itemId = string.Empty;
    public CatalogItem catalogRef;
    public List<ItemInstance> inventory;
    public Sprite icon;
    public bool isConsumable = false;
    public int totalUses = 0;
    public int count { get { return inventory.Count; } }

    //ctor
    public InventoryCategory(string id, CatalogItem cat, List<ItemInstance> inv, Sprite icn)
    {
        itemId = id;
        catalogRef = cat;
        inventory = inv;
        icon = icn;
    }

    //ctor
    public InventoryCategory(string id, CatalogItem cat, List<ItemInstance> inv, Sprite icn, bool consumable)
    {
        itemId = id;
        catalogRef = cat;
        inventory = inv;
        icon = icn;
        isConsumable = consumable;

        CalcTotalUses();
    }

    public void CalcTotalUses()
    {
        if (isConsumable)
        {
            totalUses = 0;
            foreach (var item in inventory)
            {
                if (item.RemainingUses != null)
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
