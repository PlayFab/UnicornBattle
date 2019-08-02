using System;

namespace UnicornBattle.Models
{
    [Serializable]
    public class UBLevelAct
    {
        public string Background;
        public string UsePregeneratedEncounter;
        public UBLevelEncounters CreepEncounters;
        public UBLevelEncounters MegaCreepEncounters;
        public UBLevelEncounters RareCreepEncounters;
        public UBLevelEncounters BossCreepEncounters;
        public UBLevelEncounters HeroEncounters;
        public UBLevelEncounters StoreEncounters;

        public string IntroMonolog;
        public string IntroBossMonolog;
        public string OutroMonolog;
        public string FailureMonolog;
        public bool IsActCompleted;
    }
}