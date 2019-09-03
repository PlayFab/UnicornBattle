using UnicornBattle.Controllers;
using UnicornBattle.Managers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.UI;

public class SpellItem : MonoBehaviour
{
	public SpellUpgradeController spellUpgradeBar;
	public Image spellIcon;

	public Transform lockedIcon;

	// front side 
	public Text spellName;
	public Text damageValue;

	//private
	private UBSpellDetail baseSpell = null;
	private UBSpell spell = null;

	public void LoadSpell(string sName, UBSpellDetail sp, int lvl)
	{
		if (null == sp)
		{
			Debug.LogError("Spell Detail is null");
			return;
		}

		this.baseSpell = sp;

		this.spell = new UBSpell()
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

		IconManager l_iconMgr = GameController.Instance.iconManager;
		if (null == l_iconMgr) return;
		this.spellIcon.overrideSprite = l_iconMgr.GetIconById(sp.Icon, IconManager.IconTypes.Spell);

		if (lvl > 0)
		{
			UpgradeSpell(lvl);
		}
		spellUpgradeBar.LoadBar(lvl, sp.UpgradeLevels);

		if (GameController.Instance.ActiveCharacter.characterData.CharacterLevel < this.spell.LevelReq)
		{
			this.lockedIcon.gameObject.SetActive(true);
			this.lockedIcon.GetComponentInChildren<Text>().text = "Unlocks at " + this.spell.LevelReq;
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
		for (int z = 0; z < level; z++)
		{
			this.spell.Dmg = Mathf.CeilToInt((1f + this.baseSpell.UpgradePower) * (float) this.spell.Dmg);

			if (this.spell.ApplyStatus != null)
			{
				this.spell.ApplyStatus.ChanceToApply *= 1.0f + this.baseSpell.UpgradePower;
				this.spell.ApplyStatus.ModifyAmount *= 1.0f + this.baseSpell.UpgradePower;
			}
		}
	}
}