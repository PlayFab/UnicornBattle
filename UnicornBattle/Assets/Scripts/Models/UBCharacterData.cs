using System;
namespace UnicornBattle.Models
{
    [Serializable]
    public class UBCharacterData 
    {
         public int TotalExp;
        public int ExpThisLevel;
        public int Health;
        public int Mana;
        public int Speed;
        public int Defense;
        public int CharacterLevel;
        public int Spell1_Level;
        public int Spell2_Level;
        public int Spell3_Level;
        public string CustomAvatar;

        public UBCharacterData() { }
        public UBCharacterData(UBCharacterData p_copy)
        {
            TotalExp = p_copy.TotalExp;
            ExpThisLevel = p_copy.ExpThisLevel;
            Health = p_copy.Health;
            Mana = p_copy.Mana;
            Speed = p_copy.Speed;
            Defense = p_copy.Defense;
            CharacterLevel = p_copy.CharacterLevel;
            Spell1_Level = p_copy.Spell1_Level;
            Spell2_Level = p_copy.Spell2_Level;
            Spell3_Level = p_copy.Spell3_Level;
            CustomAvatar = p_copy.CustomAvatar;
        }
    }
}