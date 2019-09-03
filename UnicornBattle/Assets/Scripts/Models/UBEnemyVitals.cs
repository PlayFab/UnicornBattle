using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnicornBattle.Models
{
    [Serializable]
    public class UBEnemyVitals
    {
        public int Health;
        public int Mana;
        public int Speed;
        public int Defense;
        public int CharacterLevel;

        public List<string> UsableItems;
        public List<UBSpellStatus> ActiveStati;
        public List<UBEnemySpellDetail> Spells;

        public int MaxHealth;
        public int MaxMana;
        public int MaxSpeed;
        public int MaxDefense;

        //public UB_Spell

        public void SetMaxVitals()
        {
            MaxHealth = Health;
            MaxMana = Mana;
            MaxSpeed = Speed;
            MaxDefense = Defense;
            Spells = new List<UBEnemySpellDetail>();
        }

        //ctor
        public UBEnemyVitals() { }
        //Copy ctor
        public UBEnemyVitals(UBEnemyVitals prius)
        {
            if (prius == null)
                return;

            Health = prius.Health;
            Mana = prius.Mana;
            Speed = prius.Speed;
            Defense = prius.Defense;
            CharacterLevel = prius.CharacterLevel;
            MaxHealth = prius.MaxHealth;
            MaxMana = prius.MaxMana;
            MaxSpeed = prius.MaxSpeed;
            MaxDefense = prius.MaxDefense;

            if (prius.UsableItems != null && prius.UsableItems.Count > 0)
                UsableItems = prius.UsableItems.ToList();
            else
                UsableItems = new List<string>();

            Spells = new List<UBEnemySpellDetail>();
            if (prius.Spells != null && prius.Spells.Count > 0)
                foreach (var spell in prius.Spells)
                    Spells.Add(new UBEnemySpellDetail(spell));

            ActiveStati = new List<UBSpellStatus>();
            if (prius.ActiveStati != null && prius.ActiveStati.Count > 0)
                foreach (var status in ActiveStati)
                    ActiveStati.Add(new UBSpellStatus(status));
        }
    }
}
