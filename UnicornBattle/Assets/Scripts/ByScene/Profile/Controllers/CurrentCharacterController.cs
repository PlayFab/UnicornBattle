using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CurrentCharacterController : MonoBehaviour {
	public Text characterName;
	public Text characterLevel;
	public Image displayImage;
	public FillBarController expBar;
	public Text livesCount;
	
	public Text hp;
	public Text dp;
	public Text sp;

	public SpellItem spell1;
	public SpellItem spell2;
	public SpellItem spell3;

	public void Init()
	{
		// may want these things in an init function that can be called after certain calls complete
	
		if(PF_PlayerData.activeCharacter != null)
		{
            this.characterName.text = PF_PlayerData.activeCharacter.characterDetails.CharacterName;
			this.characterLevel.text = "" + PF_PlayerData.activeCharacter.characterData.CharacterLevel;
			this.hp.text = "" + PF_PlayerData.activeCharacter.PlayerVitals.MaxHealth;
			this.dp.text = "" + PF_PlayerData.activeCharacter.PlayerVitals.MaxDefense;
			this.sp.text = "" + PF_PlayerData.activeCharacter.PlayerVitals.MaxSpeed;

			Sprite icon = PF_PlayerData.activeCharacter.characterData.CustomAvatar == null ? GameController.Instance.iconManager.GetIconById(PF_PlayerData.activeCharacter.baseClass.Icon) : GameController.Instance.iconManager.GetIconById(PF_PlayerData.activeCharacter.characterData.CustomAvatar);
			this.displayImage.overrideSprite = icon;
			
			if(PF_GameData.CharacterLevelRamp.Count > 0)
			{
				string key = string.Format("{0}", PF_PlayerData.activeCharacter.characterData.CharacterLevel+1);
				if(PF_GameData.CharacterLevelRamp.ContainsKey(key))
				{
					this.expBar.maxValue = PF_GameData.CharacterLevelRamp[key];
				}
				this.expBar.currentValue = PF_PlayerData.activeCharacter.characterData.ExpThisLevel;
				
			}
			
			if(PF_PlayerData.characterVirtualCurrency.Count > 0)
			{
				if(PF_PlayerData.characterVirtualCurrency.ContainsKey(GlobalStrings.HEART_CURRENCY))
				{
					this.livesCount.text = "" + PF_PlayerData.characterVirtualCurrency[GlobalStrings.HEART_CURRENCY];
/*					if(PF_PlayerData.characterVirtualCurrency[GlobalStrings.HEART_CURRENCY] < 3)
					{
						TweenScale.Tween(this.heartArt.gameObject, .333f, new Vector3(1,1,1), new Vector3(1.1f,1.1f,1.1f), TweenMain.Style.PingPong, TweenMain.Method.EaseIn, null);
					}
					else
					{
						TweenScale.Tween(this.heartArt.gameObject, .5f, new Vector3(1,1,1), new Vector3(1.05f,1.05f,1.05f), TweenMain.Style.PingPong, TweenMain.Method.EaseIn, null);
					}*/
				}
			}
		}
	

		if(PF_GameData.Spells.ContainsKey(PF_PlayerData.activeCharacter.baseClass.Spell1))
		{
			UB_SpellDetail spell1 = PF_GameData.Spells[PF_PlayerData.activeCharacter.baseClass.Spell1];
			this.spell1.LoadSpell(PF_PlayerData.activeCharacter.baseClass.Spell1, spell1, PF_PlayerData.activeCharacter.characterData.Spell1_Level);
		}
		else
		{
			// something went wrong, could not find the spell
			Debug.Log("something went wrong, could not find spell 1");
		}
		
		// test spell 2
		if(PF_GameData.Spells.ContainsKey(PF_PlayerData.activeCharacter.baseClass.Spell2))
		{
			UB_SpellDetail spell2 = PF_GameData.Spells[PF_PlayerData.activeCharacter.baseClass.Spell2];
			this.spell2.LoadSpell(PF_PlayerData.activeCharacter.baseClass.Spell2, spell2, PF_PlayerData.activeCharacter.characterData.Spell2_Level);
		}
		else
		{
			// something went wrong, could not find the spell
			Debug.Log("something went wrong, could not find spell 2");
		}
		
		// test spell 3
		if(PF_GameData.Spells.ContainsKey(PF_PlayerData.activeCharacter.baseClass.Spell3))
		{
			UB_SpellDetail spell3 = PF_GameData.Spells[PF_PlayerData.activeCharacter.baseClass.Spell3];
			this.spell3.LoadSpell(PF_PlayerData.activeCharacter.baseClass.Spell3, spell3, PF_PlayerData.activeCharacter.characterData.Spell3_Level);
		}
		else
		{
			// something went wrong, could not find the spell
			Debug.Log("something went wrong, could not find spell 3");
		}

	}
}
