using UnityEngine;

public class LevelUpOverlayController : MonoBehaviour
{

    public StatItem HP_Stat;
    public StatItem DP_Stat;
    public StatItem SP_Stat;

    public LevelUpSpellItem Spell1;
    public LevelUpSpellItem Spell2;
    public LevelUpSpellItem Spell3;

    public void Init()
    {
        var active = PF_PlayerData.activeCharacter;
        if (active != null)
        {
            HP_Stat.statValue.text = "" + active.characterData.Health;
            HP_Stat.levelBonus.text = "+" + active.baseClass.HPLevelBonus;

            DP_Stat.statValue.text = "" + active.characterData.Defense;
            DP_Stat.levelBonus.text = "+" + active.baseClass.DPLevelBonus;

            SP_Stat.statValue.text = "" + active.characterData.Speed;
            SP_Stat.levelBonus.text = "+" + active.baseClass.SPLevelBonus;
        }

        if (PF_PlayerData.activeCharacter != null)
        {
            // test spell 1
            if (PF_GameData.Spells.ContainsKey(PF_PlayerData.activeCharacter.baseClass.Spell1))
            {
                UB_SpellDetail spell1 = PF_GameData.Spells[PF_PlayerData.activeCharacter.baseClass.Spell1];
                this.Spell1.LoadSpell(PF_PlayerData.activeCharacter.baseClass.Spell1, spell1, PF_PlayerData.activeCharacter.characterData.Spell1_Level);
            }
            else
            {
                // something went wrong, could not find the spell
                Debug.Log("something went wrong, could not find spell 1");
            }

            // test spell 2
            if (PF_GameData.Spells.ContainsKey(PF_PlayerData.activeCharacter.baseClass.Spell2))
            {
                UB_SpellDetail spell2 = PF_GameData.Spells[PF_PlayerData.activeCharacter.baseClass.Spell2];
                this.Spell2.LoadSpell(PF_PlayerData.activeCharacter.baseClass.Spell2, spell2, PF_PlayerData.activeCharacter.characterData.Spell2_Level);
            }
            else
            {
                // something went wrong, could not find the spell
                Debug.Log("something went wrong, could not find spell 2");
            }

            // test spell 3
            if (PF_GameData.Spells.ContainsKey(PF_PlayerData.activeCharacter.baseClass.Spell3))
            {
                UB_SpellDetail spell3 = PF_GameData.Spells[PF_PlayerData.activeCharacter.baseClass.Spell3];
                this.Spell3.LoadSpell(PF_PlayerData.activeCharacter.baseClass.Spell3, spell3, PF_PlayerData.activeCharacter.characterData.Spell3_Level);
            }
            else
            {
                // something went wrong, could not find the spell
                Debug.Log("something went wrong, could not find spell 3");
            }
        }
    }


}
