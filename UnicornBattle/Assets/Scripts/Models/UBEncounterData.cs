using System;
using System.Collections.Generic;
using UnicornBattle.Managers; // ???
using UnityEngine;

namespace UnicornBattle.Models
{
    [Serializable]
    public class UBEncounterData 
    {
        public float SpawnWeight;
        public EncounterTypes EncounterType;
        public string Description;
        public string Icon;

        public Dictionary<string, UBEnemySpellDetail> Spells;
        public UBEnemyVitals Vitals;
        public UBEncounterRewards Rewards;

        // some list of actions
        public Dictionary<string, string> EncounterActions;

        public void SetSpellDetails()
        {
            if (Spells == null)
                Spells = new Dictionary<string, UBEnemySpellDetail>();

            foreach (var spell in Spells)
            {
                //Debug.Log("Does this ever get called in the game? YES apparently");
                var l_gameDataManager = MainManager.Instance.getGameDataManager();
                if( null == l_gameDataManager) 
                    return; 

                UBSpellDetail l_spellDetail = l_gameDataManager.GetSpellDetail(spell.Value.SpellName);
                if (l_spellDetail == null)
                    continue;

                l_spellDetail.UpgradeSpell(spell.Value.SpellLevel);
                spell.Value.Apply(l_spellDetail);
                Vitals.Spells.Add(spell.Value);
            }
        }

        //ctor
        public UBEncounterData() { }
        //copy ctor
        public UBEncounterData(UBEncounterData prius)
        {
            if (prius == null)
                return;

            SpawnWeight = prius.SpawnWeight;
            EncounterType = prius.EncounterType;
            Description = prius.Description;
            Icon = prius.Icon;

            Vitals = new UBEnemyVitals(prius.Vitals);
            Rewards = new UBEncounterRewards(prius.Rewards);

            EncounterActions = new Dictionary<string, string>();
            foreach (var kvp in prius.EncounterActions)
                EncounterActions.Add(kvp.Key, kvp.Value);

            Spells = new Dictionary<string, UBEnemySpellDetail>();
            foreach (var spell in prius.Spells)
                Spells.Add(spell.Key, new UBEnemySpellDetail(spell.Value));

            Vitals.ActiveStati = new List<UBSpellStatus>();
            foreach (var status in prius.Vitals.ActiveStati)
                Vitals.ActiveStati.Add(new UBSpellStatus(status));
        }
    }

    public enum EncounterTypes { Creep, MegaCreep, RareCreep, BossCreep, Hero, Store }
}