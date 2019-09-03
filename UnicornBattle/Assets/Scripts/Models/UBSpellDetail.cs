using System;
// using System.Collections.Generic;
// using UnityEngine;

namespace UnicornBattle.Models
{
    [Serializable]
    public class UBSpellDetail
    {
        public string Description;
        public string Icon;
        public string Target;
        public int BaseDmg;
        public int ManaCost;
        public float UpgradePower;
        public int UpgradeLevels;
        public string FX;
        public int Cooldown;
        public int LevelReq;
        public UBSpellStatus ApplyStatus;

        //ctor
        public UBSpellDetail() { }
        //copy ctor
        public UBSpellDetail(UBSpellDetail p_copy)
        {
            if (p_copy == null)
                return;

            Description = p_copy.Description;
            Icon = p_copy.Icon;
            Target = p_copy.Target;
            BaseDmg = p_copy.BaseDmg;
            ManaCost = p_copy.ManaCost;
            BaseDmg = p_copy.BaseDmg;
            UpgradePower = p_copy.UpgradePower;
            UpgradeLevels = p_copy.UpgradeLevels;
            FX = p_copy.FX;
            Cooldown = p_copy.Cooldown;
            LevelReq = p_copy.LevelReq;
            ApplyStatus = new UBSpellStatus(p_copy.ApplyStatus);
        }

        public void UpgradeSpell(int level)
        {
            for (int z = 0; z < level; z++)
            {
                BaseDmg *= UnityEngine.Mathf.CeilToInt(1.0f + UpgradePower);

                if (ApplyStatus != null)
                {
                    ApplyStatus.ChanceToApply *= 1.0f + UpgradePower;
                    ApplyStatus.ModifyAmount *= 1.0f + UpgradePower;
                }
            }
        }
    }
}