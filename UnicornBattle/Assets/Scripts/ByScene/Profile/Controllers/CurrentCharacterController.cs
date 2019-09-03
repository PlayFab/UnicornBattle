using UnicornBattle.Managers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
    public class CurrentCharacterController : MonoBehaviour
    {
        public Text characterName;
        public Text characterLevel;
        public Image displayImage;
        public FillBarController expBar;

        public Text hp;
        public Text dp;
        public Text sp;

        public SpellItem spell1;
        public SpellItem spell2;
        public SpellItem spell3;

        public void Init()
        {
            var l_characterMgr = MainManager.Instance.getCharacterManager();
            if (null == l_characterMgr)
                return;

            l_characterMgr.Refresh(false, (s) =>
            {
                var l_activeChar = GameController.Instance.ActiveCharacter;
                if (l_activeChar == null)
                    return;

                characterName.text = l_activeChar.CharacterName;
                characterLevel.text = "" + l_activeChar.characterData.CharacterLevel;
                hp.text = "" + l_activeChar.PlayerVitals.MaxHealth;
                dp.text = "" + l_activeChar.PlayerVitals.MaxDefense;
                sp.text = "" + l_activeChar.PlayerVitals.MaxSpeed;

                var icon = l_activeChar.characterData.CustomAvatar == null ? GameController.Instance.iconManager.GetIconById(l_activeChar.baseClass.Icon, IconManager.IconTypes.Class) : GameController.Instance.iconManager.GetIconById(l_activeChar.characterData.CustomAvatar, IconManager.IconTypes.Class);
                displayImage.overrideSprite = icon;

                var key = (l_activeChar.characterData.CharacterLevel + 1).ToString();
                expBar.maxValue = l_characterMgr.GetLevelRamp(key);
                expBar.currentValue = l_activeChar.characterData.ExpThisLevel;

                SetSpellDetails(l_activeChar.baseClass.Spell1, spell1, l_activeChar.characterData.Spell1_Level, 1);
                SetSpellDetails(l_activeChar.baseClass.Spell2, spell2, l_activeChar.characterData.Spell2_Level, 2);
                SetSpellDetails(l_activeChar.baseClass.Spell3, spell3, l_activeChar.characterData.Spell3_Level, 3);
            }, null);
        }

        private static void SetSpellDetails(string p_spellName, SpellItem p_spellItem, int p_spellLevel, int p_index)
        {
            var l_gameDataManager = MainManager.Instance.getGameDataManager();
            if (null == l_gameDataManager)
                return;

            var l_spellDetails = l_gameDataManager.GetSpellDetail(p_spellName);
            if (null != l_spellDetails)
            {
                p_spellItem.LoadSpell(p_spellName, l_spellDetails, p_spellLevel);
            }
            else
            {
                Debug.LogError("something went wrong, could not find spell " + p_index + " : " + p_spellName);
            }
        }
    }
}