using System;

namespace UnicornBattle.Models
{
    [Serializable]
    public class UBQuestRewardItem
    {
        // These are used in Cloud Script
        public string PlayFabId;
        public string ItemId;
        public string ItemInstanceId;
        public bool Result;
    }
}