using System;

namespace UnicornBattle.Models
{
    [Serializable]
    public class UBSpellStatus
    {
        public string StatusName;
        public string Target;
        public string UpgradeReq;
        public string StatusDescription;
        public string StatModifierCode;
        public float ModifyAmount;
        public float ChanceToApply;
        public int Turns;
        public string Icon;
        public string FX;

        // ctor
        public UBSpellStatus() { }
        //copy ctor
        public UBSpellStatus(UBSpellStatus p_copy)
        {
            if (p_copy == null)
                return;

            StatusName = p_copy.StatusName;
            Target = p_copy.Target;
            UpgradeReq = p_copy.UpgradeReq;
            StatusDescription = p_copy.StatusDescription;
            StatModifierCode = p_copy.StatModifierCode;
            ModifyAmount = p_copy.ModifyAmount;
            ChanceToApply = p_copy.ChanceToApply;
            Turns = p_copy.Turns;
            Icon = p_copy.Icon;
            FX = p_copy.FX;
        }
    }
}