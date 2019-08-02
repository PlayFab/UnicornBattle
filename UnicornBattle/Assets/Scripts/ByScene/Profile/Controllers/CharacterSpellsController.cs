using UnicornBattle.Managers;
using UnicornBattle.Models;
using UnityEngine;

namespace UnicornBattle.Controllers
{

    public class CharacterSpellsController : MonoBehaviour
    {
        public SpellItem Spell1;
        public SpellItem Spell2;
        public SpellItem Spell3;

        public void Init()
        {
            var l_activeChar = GameController.Instance.ActiveCharacter;
            if (l_activeChar == null)
                return;

            DisplaySpell(l_activeChar.baseClass.Spell1, l_activeChar.characterData.Spell1_Level, Spell1, 1);
            DisplaySpell(l_activeChar.baseClass.Spell2, l_activeChar.characterData.Spell2_Level, Spell2, 2);
            DisplaySpell(l_activeChar.baseClass.Spell3, l_activeChar.characterData.Spell3_Level, Spell3, 3);
        }

        private static void DisplaySpell(string spellName, int spellLevel, SpellItem spellItem, int index)
        {
            var l_gameDataManager = MainManager.Instance.getGameDataManager();
            if (null == l_gameDataManager)
                return;

            UBSpellDetail spellDetail = l_gameDataManager.GetSpellDetail(spellName);
            if (null == spellDetail)
            {
                Debug.LogError("something went wrong, could not find spell " + index);
                return;
            }
            spellItem.LoadSpell(spellName, spellDetail, spellLevel);
        }
    }
}