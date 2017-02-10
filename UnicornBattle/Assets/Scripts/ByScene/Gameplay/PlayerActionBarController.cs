using UnityEngine;
using UnityEngine.UI;

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
        Spell1Button.SpellButton.onClick.AddListener(() => { SpellClicked(PF_GamePlay.PlayerSpellInputs.Spell1); });
        Spell2Button.SpellButton.onClick.AddListener(() => { SpellClicked(PF_GamePlay.PlayerSpellInputs.Spell2); });
        Spell3Button.SpellButton.onClick.AddListener(() => { SpellClicked(PF_GamePlay.PlayerSpellInputs.Spell3); });
        FleeButton.onClick.AddListener(() => { SpellClicked(PF_GamePlay.PlayerSpellInputs.Flee); });
        UseItemButton.onClick.AddListener(() => { SpellClicked(PF_GamePlay.PlayerSpellInputs.UseItem); });

    }

    public void UpdateSpellBar()
    {
        if (PF_PlayerData.activeCharacter == null)
            return;

        var spell1 = PF_PlayerData.activeCharacter.baseClass.Spell1;
        Spell1Button.AddSpellData(spell1, PF_GameData.Spells.ContainsKey(spell1) ? PF_GameData.Spells[spell1] : null, PF_PlayerData.activeCharacter.characterData.Spell1_Level);

        var spell2 = PF_PlayerData.activeCharacter.baseClass.Spell2;
        Spell2Button.AddSpellData(spell2, PF_GameData.Spells.ContainsKey(spell2) ? PF_GameData.Spells[spell2] : null, PF_PlayerData.activeCharacter.characterData.Spell2_Level);

        var spell3 = PF_PlayerData.activeCharacter.baseClass.Spell3;
        Spell3Button.AddSpellData(spell3, PF_GameData.Spells.ContainsKey(spell3) ? PF_GameData.Spells[spell3] : null, PF_PlayerData.activeCharacter.characterData.Spell3_Level);
    }

    private void ClearInput()
    {
    }

    public void SpellClicked(PF_GamePlay.PlayerSpellInputs input)
    {
        switch (input)
        {
            case PF_GamePlay.PlayerSpellInputs.Spell1:
                if (!Spell1Button.isOnCD && !Spell1Button.isLocked)
                    pUIController.gameplayController.PlayerAttacks(Spell1Button);
                break;

            case PF_GamePlay.PlayerSpellInputs.Spell2:
                if (!Spell2Button.isOnCD && !Spell2Button.isLocked)
                    pUIController.gameplayController.PlayerAttacks(Spell2Button);
                break;

            case PF_GamePlay.PlayerSpellInputs.Spell3:
                if (!Spell3Button.isOnCD && !Spell3Button.isLocked)
                    pUIController.gameplayController.PlayerAttacks(Spell3Button);
                break;

            case PF_GamePlay.PlayerSpellInputs.Flee:
                pUIController.StartEncounterInput(PF_GamePlay.PlayerEncounterInputs.Evade);
                break;

            case PF_GamePlay.PlayerSpellInputs.UseItem:
                pUIController.UseItem();
                break;
        }
    }
}
