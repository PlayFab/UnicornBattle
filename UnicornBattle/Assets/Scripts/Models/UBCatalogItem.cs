using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;

namespace UnicornBattle.Models {
    [Serializable]
    public class UBCatalogItem {
        /// <summary>
        /// defines the bundle properties for the item - bundles are items which contain other items, including random drop tables
        /// and virtual currencies
        /// </summary>
        public CatalogItemBundleInfo Bundle;
        /// <summary>
        /// defines the container properties for the item - what items it contains, including random drop tables and virtual
        /// currencies, and what item (if any) is required to open it via the UnlockContainerItem API
        /// </summary>
        public CatalogItemContainerInfo Container;
        /// <summary>
        /// game specific custom data
        /// </summary>
        public string CustomData;
        /// <summary>
        /// text description of item, to show in-game
        /// </summary>
        public string Description;
        /// <summary>
        /// text name for the item, to show in-game
        /// </summary>
        public string DisplayName;
        /// <summary>
        /// class to which the item belongs
        /// </summary>
        public string ItemClass;
        /// <summary>
        /// unique identifier for this item
        /// </summary>
        public string ItemId;
        /// <summary>
        /// URL to the item image. For Facebook purchase to display the image on the item purchase page, this must be set to an HTTP
        /// URL.
        /// </summary>
        public string ItemImageUrl;
        /// <summary>
        /// list of item tags
        /// </summary>
        public List<string> Tags;
        /// <summary>
        /// price of this item in virtual currencies and "RM" (the base Real Money purchase price, in USD pennies)
        /// </summary>
        public Dictionary<string, uint> VirtualCurrencyPrices;

        public UBCatalogItem() {
            Bundle = new CatalogItemBundleInfo();
            Container = new CatalogItemContainerInfo();
            Tags = new List<string>();
            VirtualCurrencyPrices = new Dictionary<string, uint>();
        }

        public UBCatalogItem(CatalogItem p_copy) {
            Bundle = p_copy.Bundle;
            Container = p_copy.Container;
            CustomData = p_copy.CustomData;
            Description = p_copy.Description;
            DisplayName = p_copy.DisplayName;
            ItemClass = p_copy.ItemClass;
            ItemId = p_copy.ItemId;
            ItemImageUrl = p_copy.ItemImageUrl;
            Tags = new List<string>(p_copy.Tags);
            if (null != p_copy.VirtualCurrencyPrices)
                VirtualCurrencyPrices = new Dictionary<string, uint>(p_copy.VirtualCurrencyPrices);
        }

        public override string ToString() {
            return "UBCatalogItem: " + DisplayName;
        }
    }
}