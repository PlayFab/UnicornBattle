using System;
using System.Collections.Generic;


namespace UnicornBattle.Models
{
    /// <summary>
    /// Details the player progress 
    /// </summary>
    [Serializable]
    public class UBQuest
    {
        public KeyValuePair<string, UBLevelAct> CurrentAct;
        public int ActIndex;
        public List<string> WinConditions;

        //TODO hook up these stats (commenter: pgilmorepf)
        public int DamageDone;
        public int DamageTaken;
        public int Deaths;

        public int XpCollected;
        public int GoldCollected;
        public List<UBEncounter> CompletedEncounters;
        public List<string> ItemsFound;
        public List<UBQuestRewardItem> ItemsGranted;
        public int CreepEncounters;
        public int HeroRescues;
        public int ItemsUsed;
        public bool isQuestWon = false;
        public bool areItemsAwarded = false;

        //ctor
        public UBQuest()
        {
            WinConditions = new List<string>();
            CompletedEncounters = new List<UBEncounter>();
            ItemsFound = new List<string>();
            ItemsGranted = new List<UBQuestRewardItem>();
        
        }

        public UBQuest(UBQuest p_copy)
        {
            CurrentAct = new KeyValuePair<string, UBLevelAct>(p_copy.CurrentAct.Key, p_copy.CurrentAct.Value);
            ActIndex = p_copy.ActIndex;
            WinConditions = new List<string>(p_copy.WinConditions);
            DamageDone = p_copy.DamageDone;
            DamageTaken = p_copy.DamageTaken;
            Deaths = p_copy.Deaths;
            XpCollected = p_copy.XpCollected;
            GoldCollected = p_copy.GoldCollected;
            CompletedEncounters = new List<UBEncounter>(p_copy.CompletedEncounters);
            ItemsFound = new List<string>(p_copy.ItemsFound);
            ItemsGranted = new List<UBQuestRewardItem>(p_copy.ItemsGranted);
            CreepEncounters = p_copy.CreepEncounters;
            HeroRescues = p_copy.HeroRescues;
            ItemsUsed = p_copy.ItemsUsed;
            isQuestWon = p_copy.isQuestWon;
            areItemsAwarded = p_copy.areItemsAwarded;
        }

        public void RemoveItemFound(string p_id)
        {
            if(null != ItemsFound && !string.IsNullOrEmpty(p_id)) {
                if( ItemsFound.Contains(p_id))
                    ItemsFound.Remove(p_id);
            }
        }


    }
}