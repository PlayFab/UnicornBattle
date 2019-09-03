using System;

namespace UnicornBattle.Models
{
    [Serializable]
    public class UBSpell
    {
        public string SpellName;
        public string Description;
        public string Icon;
        public int Dmg;
        public int Level;
        public int UpgradeLevels;
        public string FX;
        public int Cooldown;
        public int LevelReq;
        public UBSpellStatus ApplyStatus;

    }

}
