using UnicornBattle.Managers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{

    public class PlayerActionBarController : MonoBehaviour
    {
        public TweenPos tweener;

        public SpellSlot Spell1Button;
        public SpellSlot Spell2Button;
        public SpellSlot Spell3Button;
        public Button FleeButton;
        public Button UseItemButton;

        public PlayerUIEffectsController pUIController;

        void Awake()
        {
            Spell1Button.SpellButton.onClick.AddListener(() => { SpellClicked(UBGamePlay.PlayerSpellInputs.Spell1); });
            Spell2Button.SpellButton.onClick.AddListener(() => { SpellClicked(UBGamePlay.PlayerSpellInputs.Spell2); });
            Spell3Button.SpellButton.onClick.AddListener(() => { SpellClicked(UBGamePlay.PlayerSpellInputs.Spell3); });
            FleeButton.onClick.AddListener(() => { SpellClicked(UBGamePlay.PlayerSpellInputs.Flee); });
            UseItemButton.onClick.AddListener(() => { SpellClicked(UBGamePlay.PlayerSpellInputs.UseItem); });

        }

        public void UpdateSpellBar()
        {
            var l_activeChar = GameController.Instance.ActiveCharacter;
            if (null == l_activeChar) return;

            var l_gameDataManager = MainManager.Instance.getGameDataManager();
            if (null == l_gameDataManager) return;

            UBSpellDetail l_spellDetail1 = l_gameDataManager.GetSpellDetail(l_activeChar.baseClass.Spell1);
            Spell1Button.AddSpellData(l_activeChar.baseClass.Spell1, l_spellDetail1, l_activeChar.characterData.Spell1_Level);
            UBSpellDetail l_spellDetail2 = l_gameDataManager.GetSpellDetail(l_activeChar.baseClass.Spell2);
            Spell1Button.AddSpellData(l_activeChar.baseClass.Spell2, l_spellDetail2, l_activeChar.characterData.Spell2_Level);
            UBSpellDetail l_spellDetail3 = l_gameDataManager.GetSpellDetail(l_activeChar.baseClass.Spell3);
            Spell1Button.AddSpellData(l_activeChar.baseClass.Spell3, l_spellDetail3, l_activeChar.characterData.Spell3_Level);
        }

        private void ClearInput() { }

        public void SpellClicked(UBGamePlay.PlayerSpellInputs input)
        {
            switch (input)
            {
                case UBGamePlay.PlayerSpellInputs.Spell1:
                    if (!Spell1Button.isOnCD && !Spell1Button.isLocked)
                        pUIController.gameplayController.PlayerAttacks(Spell1Button);
                    break;

                case UBGamePlay.PlayerSpellInputs.Spell2:
                    if (!Spell2Button.isOnCD && !Spell2Button.isLocked)
                        pUIController.gameplayController.PlayerAttacks(Spell2Button);
                    break;

                case UBGamePlay.PlayerSpellInputs.Spell3:
                    if (!Spell3Button.isOnCD && !Spell3Button.isLocked)
                        pUIController.gameplayController.PlayerAttacks(Spell3Button);
                    break;

                case UBGamePlay.PlayerSpellInputs.Flee:
                    pUIController.StartEncounterInput(UBGamePlay.PlayerEncounterInputs.Evade);
                    break;

                case UBGamePlay.PlayerSpellInputs.UseItem:
                    pUIController.UseItem();
                    break;
            }
        }
    }
}