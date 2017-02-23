using UnityEngine;

public class CharacterSpellsController : MonoBehaviour
{
    public SpellItem Spell1;
    public SpellItem Spell2;
    public SpellItem Spell3;

    public void Init()
    {
        if (PF_PlayerData.activeCharacter == null)
            return;

        DisplaySpell(PF_PlayerData.activeCharacter.baseClass.Spell1, PF_PlayerData.activeCharacter.characterData.Spell1_Level, Spell1, 1);
        DisplaySpell(PF_PlayerData.activeCharacter.baseClass.Spell2, PF_PlayerData.activeCharacter.characterData.Spell2_Level, Spell2, 2);
        DisplaySpell(PF_PlayerData.activeCharacter.baseClass.Spell3, PF_PlayerData.activeCharacter.characterData.Spell3_Level, Spell3, 3);
    }

    private static void DisplaySpell(string spellName, int spellLevel, SpellItem spellItem, int index)
    {
        UB_SpellDetail spellDetail;
        if (PF_GameData.Spells.TryGetValue(spellName, out spellDetail))
            spellItem.LoadSpell(spellName, spellDetail, spellLevel);
        else
            Debug.Log("something went wrong, could not find spell " + index);
    }
}
