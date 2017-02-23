using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpellSlot : MonoBehaviour {
	public Button SpellButton;
	public Image SpellIcon;
	
	public Image Tint;
	public Text CooldownText;
	public Image LockedIcon;
	
	public UB_Spell SpellData;
	
	
	public bool isOnCD;
	public bool isLocked;
	
	private int cdTurns;
	private UB_SpellDetail baseSpell;
	
	public void Lock()
	{	
		this.SpellButton.interactable = false;
		this.isLocked = true;
		this.LockedIcon.gameObject.SetActive(true);
		EnableTint();
	}
	
	public void Unlock()
	{
		this.SpellButton.interactable = true;
		this.isLocked = false;
		this.LockedIcon.gameObject.SetActive(false);
		DisableTint();
	}
	
	public void EnableCD(int turns)
	{
		this.SpellButton.interactable = false;
		this.isOnCD = true;
		this.CooldownText.text = "" + turns;
		this.cdTurns = turns;
		this.CooldownText.gameObject.SetActive(true);
		EnableTint();
		
	}
	
	public void DisableCD()
	{
		this.SpellButton.interactable = true;
		this.isOnCD = false;
		this.CooldownText.text = string.Empty;
		this.CooldownText.gameObject.SetActive(false);
		DisableTint();
	}
	
	public void DecrementCD()
	{
		if(this.cdTurns > 0)
		{
			this.cdTurns--;
		}
		
		if(this.cdTurns == 0 && this.isOnCD)
		{
			DisableCD();
		}
		else if(this.isOnCD)
		{	
			this.CooldownText.text = "" + this.cdTurns;
		}
	}
	
	
	public void AddSpellData(string spName, UB_SpellDetail sp, int spLvl)
	{
		if(sp == null)
			return;
			
		this.baseSpell = sp;
		this.SpellData = new UB_Spell()
		{
			SpellName = spName,
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
		
		this.SpellIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(sp.Icon, IconManager.IconTypes.Spell);
		
		if(spLvl > 0)
		{
			UpgradeSpell(spLvl);
		}
		
		if(PF_PlayerData.activeCharacter.characterData.CharacterLevel < this.SpellData.LevelReq)
		{
			this.DisableCD();
			this.Lock ();
		}
		else
		{
			this.Unlock();
			this.DisableCD();
		}	
	}
	
	void UpgradeSpell(int level)
	{
		for(int z = 0; z < level; z++)
		{
			this.SpellData.Dmg = Mathf.CeilToInt( (1f + this.baseSpell.UpgradePower) * (float)this.SpellData.Dmg);
			
			if(this.SpellData.ApplyStatus != null)
			{
				this.SpellData.ApplyStatus.ChanceToApply *= 1f + this.baseSpell.UpgradePower;
				this.SpellData.ApplyStatus.ModifyAmount *= 1f + this.baseSpell.UpgradePower;
			}
		}
	}
	
	
	public void EnableTint()
	{
		this.Tint.gameObject.SetActive(true);
	}
	
	public void DisableTint()
	{
		this.Tint.gameObject.SetActive(false);
	}
}
