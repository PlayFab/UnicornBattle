using System;
using System.Collections.Generic;


namespace UnicornBattle.Models
{   
    [Serializable]
    public class UBLevelData
    {
        public string Name;
        public string Description;
        public string Icon;
        public string StatsPrefix;
        public int? MinEntryLevel;
        public UBLevelWinConditions WinConditions;
        public Dictionary<string, UBLevelAct> Acts;
        public string RestrictedToEventKey;
    }
}