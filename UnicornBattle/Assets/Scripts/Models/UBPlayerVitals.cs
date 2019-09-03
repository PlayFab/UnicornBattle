using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornBattle.Models
{
    /// <summary>
    /// A class for tracking players through combat
    /// </summary>
    [Serializable]
    public class UBPlayerVitals
    {
        public int Health;
        public int Mana;
        public int Speed;
        public int Defense;
        public List<UBSpellStatus> ActiveStati;
        public int MaxHealth;
        public int MaxMana;
        public int MaxSpeed;
        public int MaxDefense;
        public bool didLevelUp;
        public int skillSelected;

        public UBPlayerVitals() {
            ActiveStati = new List<UBSpellStatus>();
        }

        public UBPlayerVitals(UBPlayerVitals p_copy)
        {
            Health = p_copy.Health;
            Mana = p_copy.Mana;
            Speed = p_copy.Speed;
            Defense = p_copy.Defense;
            ActiveStati = new List<UBSpellStatus>(p_copy.ActiveStati);
            MaxHealth = p_copy.MaxHealth;
            MaxMana = p_copy.MaxMana;
            MaxSpeed = p_copy.MaxSpeed;
            MaxDefense = p_copy.MaxDefense;
            didLevelUp = p_copy.didLevelUp;
            skillSelected = p_copy.skillSelected;
        }
    }
}