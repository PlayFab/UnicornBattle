using System;

namespace UnicornBattle.Models
{
    [Serializable]
    public class UBEventData
    {
        public string EventKey;
        public string EventName;
        public string EventDescription;
        public string StoreToUse;
        public string BundleId;

        public UBUnpackedAssetBundle Assets;
    }

    public enum PromotionStatus
    {
        Inactive,
        Active,
        ActivePromoted,
    }
}
