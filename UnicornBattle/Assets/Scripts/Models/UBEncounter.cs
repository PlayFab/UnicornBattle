using System;

namespace UnicornBattle.Models
{
    [Serializable]
    public class UBEncounter 
    {
        public string DisplayName;
        public UBEncounterData Data;
        public bool playerCompleted; // not sure about this var
        public bool isEndOfAct;
        //TODO add a bool for signals end of act
    }
}