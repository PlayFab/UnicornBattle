using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
public class SelectedPonyController : MonoBehaviour {
	public Text hp;
	public Text dp;
	public Text sp;
	
	public List<SpellFrame> spells = new List<SpellFrame>();
	
	public Image[] colorize;

	public void SwitchPonyDetails(ArrowUI pony)
	{
		if (pony.details != null && pony.details.CatalogCode != null) 
		{
			PF_PlayerData.PlayerClassTypes ponyType = (PF_PlayerData.PlayerClassTypes)Enum.Parse (typeof(PF_PlayerData.PlayerClassTypes), pony.details.CatalogCode);

			switch ((int)ponyType) {
			case 0:
				foreach (var item in this.colorize) {
					item.color = PF_GamePlay.ClassColor1;
				}
				break;
			case 1:
				foreach (var item in this.colorize) {
					item.color = PF_GamePlay.ClassColor2;
				}
				break;
			case 2:
				foreach (var item in this.colorize) {
					item.color = PF_GamePlay.ClassColor3;
				}
				break;
			default:
				Debug.LogWarning ("Unknown Class type detected...");
				break;
			}
	
			hp.text = string.Format("{0}", pony.details.BaseHP);
			dp.text = string.Format("{0}", pony.details.BaseDP);
			sp.text = string.Format("{0}", pony.details.BaseSP);
			
			if(PF_GameData.Spells.ContainsKey(pony.details.Spell1))
			{
				spells[0].spellName.text = pony.details.Spell1;
				spells[0].spellImage.overrideSprite = GameController.Instance.iconManager.GetIconById(PF_GameData.Spells[pony.details.Spell1].Icon);
				spells[0].attackValue.text = "" + PF_GameData.Spells[pony.details.Spell1].BaseDmg;
				spells[0].upgradeLevels.text = "" + PF_GameData.Spells[pony.details.Spell1].UpgradeLevels;
				spells[0].cooldown.text = "" + PF_GameData.Spells[pony.details.Spell1].Cooldown;
				spells[0].levelReq.text = ""+ PF_GameData.Spells[pony.details.Spell1].LevelReq;
			}
			if(PF_GameData.Spells.ContainsKey(pony.details.Spell2))
			{
				spells[1].spellName.text = pony.details.Spell2;
				spells[1].spellImage.overrideSprite = GameController.Instance.iconManager.GetIconById(PF_GameData.Spells[pony.details.Spell2].Icon);
				spells[1].attackValue.text = "" + PF_GameData.Spells[pony.details.Spell2].BaseDmg;
				spells[1].upgradeLevels.text = "" + PF_GameData.Spells[pony.details.Spell2].UpgradeLevels;
				spells[1].cooldown.text = "" + PF_GameData.Spells[pony.details.Spell2].Cooldown;
				spells[1].levelReq.text = ""+ PF_GameData.Spells[pony.details.Spell2].LevelReq;
			}
			if(PF_GameData.Spells.ContainsKey(pony.details.Spell3))
			{
				spells[2].spellName.text = pony.details.Spell3;
				spells[2].spellImage.overrideSprite = GameController.Instance.iconManager.GetIconById(PF_GameData.Spells[pony.details.Spell3].Icon);
				spells[2].attackValue.text = "" + PF_GameData.Spells[pony.details.Spell3].BaseDmg;
				spells[2].upgradeLevels.text = "" + PF_GameData.Spells[pony.details.Spell3].UpgradeLevels;
				spells[2].cooldown.text = "" + PF_GameData.Spells[pony.details.Spell3].Cooldown;
				spells[2].levelReq.text = ""+ PF_GameData.Spells[pony.details.Spell3].LevelReq;
			}
		}
	}
}

[System.Serializable]
public class SpellFrame
{
	public Text spellName;
	public Image spellImage;
	public Text attackValue;
	public Text upgradeLevels;
	public Text cooldown;
	public Text levelReq;
}