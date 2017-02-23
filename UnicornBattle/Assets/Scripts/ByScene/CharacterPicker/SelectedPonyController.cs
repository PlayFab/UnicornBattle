using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class SelectedPonyController : MonoBehaviour
{
    public Text hp;
    public Text dp;
    public Text sp;
    public List<SpellFrame> spells = new List<SpellFrame>();
    public Image[] colorize;

    public void SwitchPonyDetails(ArrowUI character)
    {
        if (character.details == null || character.details.CatalogCode == null)
            return;

        var ponyType = (PF_PlayerData.PlayerClassTypes)Enum.Parse(typeof(PF_PlayerData.PlayerClassTypes), character.details.CatalogCode);

        switch ((int)ponyType)
        {
            case 0:
                foreach (var item in colorize)
                    item.color = PF_GamePlay.ClassColor1;
                break;
            case 1:
                foreach (var item in colorize)
                    item.color = PF_GamePlay.ClassColor2;
                break;
            case 2:
                foreach (var item in colorize)
                    item.color = PF_GamePlay.ClassColor3;
                break;
            default:
                Debug.LogWarning("Unknown Class type detected...");
                break;
        }

        hp.text = character.details.BaseHP.ToString();
        dp.text = character.details.BaseDP.ToString();
        sp.text = character.details.BaseSP.ToString();

        SetSpellInfo(character.details.Spell1, 0);
        SetSpellInfo(character.details.Spell2, 1);
        SetSpellInfo(character.details.Spell3, 2);
    }

    private void SetSpellInfo(string spellName, int index)
    {
        if (PF_GameData.Spells.ContainsKey(spellName))
        {
            spells[index].spellName.text = spellName;
            spells[index].spellImage.overrideSprite = GameController.Instance.iconManager.GetIconById(PF_GameData.Spells[spellName].Icon, IconManager.IconTypes.Class);
            spells[index].attackValue.text = "" + PF_GameData.Spells[spellName].BaseDmg;
            spells[index].upgradeLevels.text = "" + PF_GameData.Spells[spellName].UpgradeLevels;
            spells[index].cooldown.text = "" + PF_GameData.Spells[spellName].Cooldown;
            spells[index].levelReq.text = "" + PF_GameData.Spells[spellName].LevelReq;
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
