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

        var icon = PF_PlayerData.activeCharacter.characterData.CustomAvatar == null ? GameController.Instance.iconManager.GetIconById(PF_PlayerData.activeCharacter.baseClass.Icon, IconManager.IconTypes.Class) : GameController.Instance.iconManager.GetIconById(PF_PlayerData.activeCharacter.characterData.CustomAvatar, IconManager.IconTypes.Class);
        displayImage.overrideSprite = icon;

        if (PF_GameData.CharacterLevelRamp.Count > 0)
        {
            var key = (PF_PlayerData.activeCharacter.characterData.CharacterLevel + 1).ToString();
            if (PF_GameData.CharacterLevelRamp.ContainsKey(key))
                expBar.maxValue = PF_GameData.CharacterLevelRamp[key];
            expBar.currentValue = PF_PlayerData.activeCharacter.characterData.ExpThisLevel;
        }

        SetSpellDetails(PF_PlayerData.activeCharacter.baseClass.Spell1, spell1, PF_PlayerData.activeCharacter.characterData.Spell1_Level, 1);
        SetSpellDetails(PF_PlayerData.activeCharacter.baseClass.Spell2, spell2, PF_PlayerData.activeCharacter.characterData.Spell2_Level, 2);
        SetSpellDetails(PF_PlayerData.activeCharacter.baseClass.Spell3, spell3, PF_PlayerData.activeCharacter.characterData.Spell3_Level, 3);
    }

    private static void SetSpellDetails(string spellName, SpellItem spellItem, int spellLevel, int index)
    {
        if (PF_GameData.Spells.ContainsKey(spellName))
        {
            var eachSpellDetail = PF_GameData.Spells[spellName];
            spellItem.LoadSpell(spellName, eachSpellDetail, spellLevel);
        }
        else
        {
            Debug.Log("something went wrong, could not find spell " + index);
        }
    }
}
