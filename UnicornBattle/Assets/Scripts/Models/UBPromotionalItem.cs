using System;
using System.Collections.Generic;

namespace UnicornBattle.Models
{
    [Serializable]
    public class UBPromotionalItem
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

    public enum PromotionalItemTypes { All, News, Image, Sale, Event, Tip }
}