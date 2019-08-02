using System;
using PlayFab.ClientModels;

namespace UnicornBattle.Models {
    [Serializable]
    public class UBInventoryItem {
        public string Annotation;
        public string DisplayName;
        public string ItemId;
        public string ItemInstanceId;
        public int? RemainingUses;
        //public Sprite icon;

        public UBInventoryItem(ItemInstance p_playFabItem) {
            Annotation = p_playFabItem.Annotation;
            DisplayName = p_playFabItem.DisplayName;
            ItemId = p_playFabItem.ItemId;
            ItemInstanceId = p_playFabItem.ItemInstanceId;
            RemainingUses = p_playFabItem.RemainingUses;
        }

        public UBInventoryItem() {
            Annotation = string.Empty;
            DisplayName = string.Empty;
            ItemId = string.Empty;
            ItemInstanceId = string.Empty;
            RemainingUses = null;
        }

        public UBInventoryItem(UBInventoryItem p_item) {
            Annotation = p_item.Annotation;
            DisplayName = p_item.DisplayName;
            ItemId = p_item.ItemId;
            ItemInstanceId = p_item.ItemInstanceId;
            RemainingUses = p_item.RemainingUses;
        }

        public override string ToString() {
            return "UBInventoryItem: " + DisplayName;
        }
    }
}