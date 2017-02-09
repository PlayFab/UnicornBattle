using UnityEngine;
using UnityEngine.UI;

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
        // may want these things in an init function that can be called after certain calls complete
        if (PF_PlayerData.activeCharacter == null)
            return;

        characterName.text = PF_PlayerData.activeCharacter.characterDetails.CharacterName;
        characterLevel.text = "" + PF_PlayerData.activeCharacter.characterData.CharacterLevel;
        hp.text = "" + PF_PlayerData.activeCharacter.PlayerVitals.MaxHealth;
        dp.text = "" + PF_PlayerData.activeCharacter.PlayerVitals.MaxDefense;
        sp.text = "" + PF_PlayerData.activeCharacter.PlayerVitals.MaxSpeed;

        var icon = PF_PlayerData.activeCharacter.characterData.CustomAvatar == null ? GameController.Instance.iconManager.GetIconById(PF_PlayerData.activeCharacter.baseClass.Icon) : GameController.Instance.iconManager.GetIconById(PF_PlayerData.activeCharacter.characterData.CustomAvatar);
        displayImage.overrideSprite = icon;

        if (PF_GameData.CharacterLevelRamp.Count > 0)
        {
            var key = string.Format("{0}", PF_PlayerData.activeCharacter.characterData.CharacterLevel + 1);
            if (PF_GameData.CharacterLevelRamp.ContainsKey(key))
                expBar.maxValue = PF_GameData.CharacterLevelRamp[key];
            expBar.currentValue = PF_PlayerData.activeCharacter.characterData.ExpThisLevel;
        }

        if (PF_GameData.Spells.ContainsKey(PF_PlayerData.activeCharacter.baseClass.Spell1))
        {
            var spellDetail1 = PF_GameData.Spells[PF_PlayerData.activeCharacter.baseClass.Spell1];
            spell1.LoadSpell(PF_PlayerData.activeCharacter.baseClass.Spell1, spellDetail1, PF_PlayerData.activeCharacter.characterData.Spell1_Level);
        }
        else
        {
            // something went wrong, could not find the spell
            Debug.Log("something went wrong, could not find spell 1");
        }

        // test spell 2
        if (PF_GameData.Spells.ContainsKey(PF_PlayerData.activeCharacter.baseClass.Spell2))
        {
            var spellDetail2 = PF_GameData.Spells[PF_PlayerData.activeCharacter.baseClass.Spell2];
            spell2.LoadSpell(PF_PlayerData.activeCharacter.baseClass.Spell2, spellDetail2, PF_PlayerData.activeCharacter.characterData.Spell2_Level);
        }
        else
        {
            // something went wrong, could not find the spell
            Debug.Log("something went wrong, could not find spell 2");
        }

        // test spell 3
        if (PF_GameData.Spells.ContainsKey(PF_PlayerData.activeCharacter.baseClass.Spell3))
        {
            var spellDetail3 = PF_GameData.Spells[PF_PlayerData.activeCharacter.baseClass.Spell3];
            spell3.LoadSpell(PF_PlayerData.activeCharacter.baseClass.Spell3, spellDetail3, PF_PlayerData.activeCharacter.characterData.Spell3_Level);
        }
        else
        {
            // something went wrong, could not find the spell
            Debug.Log("something went wrong, could not find spell 3");
        }

    }
}
