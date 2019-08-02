using UnicornBattle.Controllers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpSpellItem : MonoBehaviour
{
    public Image SpellIcon;
    public Text SpellName;
    public Text Damage;
    public Text UpgradeDamage;

    public SpellUpgradeController spellUpgradeBar;

    public Transform lockedIcon;
    public Text unlockedLevel;
    public Transform lockedMessage;
    public Button UpgradeButton;

    //public ChoseButton
    private UBSpellDetail baseSpell = null;
    private UBSpell spell = null;

    public void LoadSpell(string sName, UBSpellDetail sp, int lvl)
    {
        baseSpell = sp;
        spell = new UBSpell()
        {
            SpellName = sName,
            Description = sp.Description,
            Icon = sp.Icon,
            Dmg = sp.BaseDmg,
            Level = 0,
            UpgradeLevels = sp.UpgradeLevels,
            FX = sp.FX,
            Cooldown = sp.Cooldown,
            LevelReq = sp.LevelReq,
            ApplyStatus = sp.ApplyStatus
        };

        SpellIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(sp.Icon, IconManager.IconTypes.Spell);

        if (lvl > 0)
            UpgradeSpell(lvl);
        spellUpgradeBar.LoadBar(lvl, sp.UpgradeLevels);

        if (GameController.Instance.ActiveCharacter.characterData.CharacterLevel < spell.LevelReq)
        {
            unlockedLevel.text = "Unlocked at level: " + spell.LevelReq;
            lockedIcon.gameObject.SetActive(true);
            lockedMessage.gameObject.SetActive(true);
            UpgradeButton.interactable = false;
        }
        else
        {
            lockedIcon.gameObject.SetActive(false);
            lockedMessage.gameObject.SetActive(false);
            UpgradeButton.interactable = true;
        }

        SpellName.text = spell.SpellName;
        Damage.text = "" + spell.Dmg;
        UpgradeDamage.text = "" + Mathf.CeilToInt((1f + baseSpell.UpgradePower) * (float) spell.Dmg);
    }

    void UpgradeSpell(int level)
    {
        for (int z = 0; z < level; z++)
        {
            spell.Dmg = Mathf.CeilToInt((1f + baseSpell.UpgradePower) * (float) spell.Dmg);

            if (spell.ApplyStatus != null)
            {
                spell.ApplyStatus.ChanceToApply *= 1.0f + baseSpell.UpgradePower;
                spell.ApplyStatus.ModifyAmount *= 1.0f + baseSpell.UpgradePower;
            }
        }
    }
}