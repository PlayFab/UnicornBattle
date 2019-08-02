using UnicornBattle.Managers;
using UnicornBattle.Models;
using UnityEngine;

namespace UnicornBattle.Controllers
{

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
            var l_gameDataManager = MainManager.Instance.getGameDataManager();
            if (null == l_gameDataManager) return;

            var l_activeCharacter = GameController.Instance.ActiveCharacter;
            if (l_activeCharacter != null)
            {
                HP_Stat.statValue.text = "" + l_activeCharacter.characterData.Health;
                HP_Stat.levelBonus.text = "+" + l_activeCharacter.baseClass.HPLevelBonus;

                DP_Stat.statValue.text = "" + l_activeCharacter.characterData.Defense;
                DP_Stat.levelBonus.text = "+" + l_activeCharacter.baseClass.DPLevelBonus;

                SP_Stat.statValue.text = "" + l_activeCharacter.characterData.Speed;
                SP_Stat.levelBonus.text = "+" + l_activeCharacter.baseClass.SPLevelBonus;

                // test spell 1
                UBSpellDetail l_spelldetails1 = l_gameDataManager.GetSpellDetail(l_activeCharacter.baseClass.Spell1);
                if (null != l_spelldetails1)
                {
                    this.Spell1.LoadSpell(l_activeCharacter.baseClass.Spell1, l_spelldetails1, l_activeCharacter.characterData.Spell1_Level);
                }
                else
                {
                    // something went wrong, could not find the spell
                    Debug.LogError("something went wrong, could not find spell 1");
                }

                // test spell 2
                UBSpellDetail l_spelldetails2 = l_gameDataManager.GetSpellDetail(l_activeCharacter.baseClass.Spell2);
                if (null != l_spelldetails2)
                {
                    this.Spell2.LoadSpell(l_activeCharacter.baseClass.Spell2, l_spelldetails2, l_activeCharacter.characterData.Spell2_Level);
                }
                else
                {
                    // something went wrong, could not find the spell
                    Debug.LogError("something went wrong, could not find spell 2");
                }

                // test spell 3
                UBSpellDetail l_spelldetails3 = l_gameDataManager.GetSpellDetail(l_activeCharacter.baseClass.Spell3);
                if (null != l_spelldetails3)
                {
                    this.Spell3.LoadSpell(l_activeCharacter.baseClass.Spell3, l_spelldetails3, l_activeCharacter.characterData.Spell3_Level);
                }
                else
                {
                    // something went wrong, could not find the spell
                    Debug.LogError("something went wrong, could not find spell 3");
                }
            }
        }

    }
}