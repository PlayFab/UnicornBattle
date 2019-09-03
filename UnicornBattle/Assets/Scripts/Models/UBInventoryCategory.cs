using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using UnicornBattle.Models;

namespace UnicornBattle.Models
{
    public class UBInventoryCategory
    {
        public string itemId = string.Empty;
        public UBCatalogItem catalogRef;
        public List<UBInventoryItem> inventory;
        public Sprite icon;
        public int count { get { return inventory.Count; } }

        //ctor
        public UBInventoryCategory(string id, UBCatalogItem cat, List<UBInventoryItem> inv, Sprite icn)
        {
            itemId = id;
            catalogRef = cat;
            inventory = inv;
            icon = icn;
        }
    }
}