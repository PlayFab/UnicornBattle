using System;
using System.Collections.Generic;

namespace UnicornBattle.Models
{
    [Serializable]
    public class UBLevelEncounters 
    {
        public string EncounterPool;
        public int MinQuantity;
        public int MaxQuantity;
        public float ChanceForAddedEncounters;
        public List<string> SpawnSpecificEncountersByID;
        public bool LimitProbabilityToAct;
    }
}
