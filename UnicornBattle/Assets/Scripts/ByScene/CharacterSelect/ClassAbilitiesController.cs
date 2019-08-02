using System;
using System.Collections.Generic;
using UnicornBattle.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{

    public class ClassAbilitiesController : MonoBehaviour
    {
        public Text hp;
        public Text dp;
        public Text sp;
        public List<SpellFrame> spells = new List<SpellFrame>();
        public Image[] colorize;

        public void ShowClassDetails(ClassButtonUI character)
        {
            if (character.details == null || string.IsNullOrEmpty(character.details.CatalogCode))
            {
                Debug.LogWarning("character.details is null or character.details.catalogCode is null or empty");
                return;
            }

            CharacterClassTypes ponyType = (CharacterClassTypes) Enum.Parse(typeof(CharacterClassTypes), character.details.CatalogCode);

            switch ((int) ponyType)
            {
                case 0:
                    foreach (var item in colorize)
                        item.color = UBGamePlay.ClassColor1;
                    break;
                case 1:
                    foreach (var item in colorize)
                        item.color = UBGamePlay.ClassColor2;
                    break;
                case 2:
                    foreach (var item in colorize)
                        item.color = UBGamePlay.ClassColor3;
                    break;
                default:
                    Debug.LogWarning("Unknown Class type detected...");
                    break;
            }

            hp.text = character.details.BaseHP.ToString();
            dp.text = character.details.BaseDP.ToString();
            sp.text = character.details.BaseSP.ToString();

            // Three Spells
            SetSpellInfo(character.details.Spell1, 0);
            SetSpellInfo(character.details.Spell2, 1);
            SetSpellInfo(character.details.Spell3, 2);
        }

        private void SetSpellInfo(string spellName, int index)
        {
            var l_gameDataManager = MainManager.Instance.getGameDataManager();
            if (null == l_gameDataManager) return;

            var l_spellDetail = l_gameDataManager.GetSpellDetail(spellName);
            if (null != l_spellDetail)
            {

                spells[index].spellName.text = spellName;
                spells[index].spellImage.overrideSprite = GameController.Instance.iconManager.GetIconById(l_spellDetail.Icon, IconManager.IconTypes.Class);
                spells[index].attackValue.text = "" + l_spellDetail.BaseDmg;
                spells[index].upgradeLevels.text = "" + l_spellDetail.UpgradeLevels;
                spells[index].cooldown.text = "" + l_spellDetail.Cooldown;
                spells[index].levelReq.text = "" + l_spellDetail.LevelReq;
            }
        }
    }

    [Serializable]
    public class SpellFrame
    {
        public Text spellName;
        public Image spellImage;
        public Text attackValue;
        public Text upgradeLevels;
        public Text cooldown;
        public Text levelReq;
    }
}