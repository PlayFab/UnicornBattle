using System;

namespace UnicornBattle.Models
{
    [Serializable]
    public class UBLevelWinConditions 
    {
        public int SurvivalTime;
        public long TimeLimit;
        public int KillCount;
        public string KillTarget;
        public bool CompleteAllActs;
        public int FindCount;
        public string FindTarget;
    }
}