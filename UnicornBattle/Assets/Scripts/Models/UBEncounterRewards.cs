using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnicornBattle.Models
{
    [Serializable]
    public class UBEncounterRewards
    {
        public int XpMin;
        public int XpMax;
        public int GoldMin;
        public int GoldMax;
        public List<string> ItemsDropped;

        //ctor
        public UBEncounterRewards() { }
        //copy ctor
        public UBEncounterRewards(UBEncounterRewards prius)
        {
            if (prius == null)
                return;

            XpMin = prius.XpMin;
            XpMax = prius.XpMax;
            GoldMin = prius.GoldMin;
            GoldMax = prius.GoldMax;
            ItemsDropped = prius.ItemsDropped.ToList();
        }
    }
}