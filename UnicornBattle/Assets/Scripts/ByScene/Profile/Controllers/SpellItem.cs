using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SpellItem : MonoBehaviour {
	public SpellUpgradeController spellUpgradeBar;
	public Image spellIcon;
	
	public Transform lockedIcon;
	
	// front side 
	public Text spellName;
	public Text damageValue;
	
	
	//private
	private UB_SpellDetail baseSpell = null;
	private UB_Spell spell = null;

//	public void LoadSpell(UB_SavedCharacter saved)
//	{
//	
//	}
//	

	public void LoadSpell(string sName, UB_SpellDetail sp, int lvl)
	{
		this.baseSpell = sp;
		this.spell = new UB_Spell()
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
		
		this.spellIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(sp.Icon, IconManager.IconTypes.Spell);
		
		if(lvl > 0)
		{
			UpgradeSpell(lvl);
		}
		spellUpgradeBar.LoadBar(lvl,sp.UpgradeLevels);
		
		if(PF_PlayerData.activeCharacter.characterData.CharacterLevel < this.spell.LevelReq)
		{
			this.lockedIcon.gameObject.SetActive(true);
			this.lockedIcon.GetComponentInChildren<Text>().text =  "Unlocks at " + this.spell.LevelReq;
		}
		else
		{
			this.lockedIcon.gameObject.SetActive(false);
		}
		
		this.spellName.text = this.spell.SpellName;
		this.damageValue.text = "+" + this.spell.Dmg + " DMG";
	
	}
	
	void UpgradeSpell(int level)
	{
		for(int z = 0; z < level; z++)
		{
			this.spell.Dmg =  Mathf.CeilToInt( (1f + this.baseSpell.UpgradePower) * (float)this.spell.Dmg);
			
			if(this.spell.ApplyStatus != null)
			{
				this.spell.ApplyStatus.ChanceToApply *= 1.0f + this.baseSpell.UpgradePower;
				this.spell.ApplyStatus.ModifyAmount *= 1.0f + this.baseSpell.UpgradePower;
			}
		}
	}
}



