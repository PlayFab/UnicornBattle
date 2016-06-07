using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


public class LevelUpSpellItem : MonoBehaviour {
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
	private UB_SpellDetail baseSpell = null;
	private UB_Spell spell = null;

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
		
		this.SpellIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(sp.Icon);
		
		if(lvl > 0)
		{
			UpgradeSpell(lvl);
		}
		spellUpgradeBar.LoadBar(lvl,sp.UpgradeLevels);
		
		if(PF_PlayerData.activeCharacter.characterData.CharacterLevel < this.spell.LevelReq)
		{
			this.unlockedLevel.text = "Unlocked at level: " + this.spell.LevelReq;
			this.lockedIcon.gameObject.SetActive(true);
			this.lockedMessage.gameObject.SetActive(true);
			this.UpgradeButton.interactable = false;
		}
		else
		{
			this.lockedIcon.gameObject.SetActive(false);
			this.lockedMessage.gameObject.SetActive(false);
			this.UpgradeButton.interactable = true;
		}
		
		this.SpellName.text = this.spell.SpellName;
		this.Damage.text = "" + this.spell.Dmg;
		this.UpgradeDamage.text = "" + Mathf.CeilToInt( (1f + this.baseSpell.UpgradePower) * (float)this.spell.Dmg) ;
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
