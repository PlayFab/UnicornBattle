using System;
namespace UnicornBattle.Models
{
    [Serializable]
    public class UBEnemySpellDetail : UBSpellDetail
    {
        public string SpellName;
        public int SpellLevel;
        public int SpellPriority;
        public bool IsOnCooldown;
        public int CdTurns;

        //ctor
        public UBEnemySpellDetail() : base() { }
        //copy ctor
        public UBEnemySpellDetail(UBEnemySpellDetail p_copy) : base( p_copy )
        {
            if (p_copy == null)
                return;

            SpellName = p_copy.SpellName;
            SpellPriority = p_copy.SpellPriority;
            SpellLevel = p_copy.SpellLevel;
            IsOnCooldown = p_copy.IsOnCooldown;
            CdTurns = p_copy.CdTurns;
        }

        // HACK: handling it this way for now, will refactor later
        public void Apply(UBSpellDetail p_copy)
        {
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
    }

}