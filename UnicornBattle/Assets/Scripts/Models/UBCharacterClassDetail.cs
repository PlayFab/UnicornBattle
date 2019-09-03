using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornBattle.Models
{
    [Serializable]
    public class UBCharacterClassDetail
    {
        public string Description;
        public string CatalogCode;
        public string Icon;
        public string Spell1;
        public string Spell2;
        public string Spell3;
        public int BaseHP;
        public int HPLevelBonus;
        public int BaseMP;
        public int MPLevelBonus;
        public int BaseDP;
        public int DPLevelBonus;
        public int BaseSP;
        public int SPLevelBonus;
        public string Prereq;
        public string DisplayStatus;

        public UBCharacterClassDetail() {}
        public UBCharacterClassDetail(UBCharacterClassDetail p_copy)
        {
            Description = p_copy.Description;
            CatalogCode = p_copy.CatalogCode;
            Icon = p_copy.Icon;
            Spell1 = p_copy.Spell1;
            Spell2 = p_copy.Spell2;
            Spell3 = p_copy.Spell3;
            BaseHP = p_copy.BaseHP;
            HPLevelBonus = p_copy.HPLevelBonus;
            BaseMP = p_copy.BaseMP;
            MPLevelBonus = p_copy.MPLevelBonus;
            BaseDP = p_copy.BaseDP;
            DPLevelBonus = p_copy.DPLevelBonus;
            BaseSP = p_copy.BaseSP;
            SPLevelBonus = p_copy.SPLevelBonus;
            Prereq = p_copy.Prereq;
            DisplayStatus = p_copy.DisplayStatus;
        }
    }
}