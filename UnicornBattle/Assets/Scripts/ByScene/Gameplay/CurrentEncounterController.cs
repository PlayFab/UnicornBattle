using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class CurrentEncounterController : MonoBehaviour {
	public Image encounterIcon;
	public Image encounterBanner;
	public Text encounterName;
	public Text encounterLevel;
	public Image encounterFlag;
	
	public FillBarController lifeBar;

	public void UpdateCurrentEncounter(UB_GamePlayEncounter encounter)
	{
		// change the health bar
		this.lifeBar.maxValue = encounter.Data.Vitals.Health;
		StartCoroutine(this.lifeBar.UpdateBar(encounter.Data.Vitals.Health, true));
		
		// change banner color
        if (encounter.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP))
		{
			this.encounterBanner.color = Color.red;
		}
        else if (encounter.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_STORE))
		{
			this.encounterBanner.color = Color.green;
		}
        else if (encounter.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_HERO))
		{
			this.encounterBanner.color = Color.cyan;
		}
		
		// change icon
		this.encounterIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(encounter.Data.Icon);
		
		// change name
		this.encounterName.text = encounter.DisplayName;
		
		
		// change level and flag color
        if (encounter.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP))
		{
			if(encounter.Data.Vitals.CharacterLevel <= PF_PlayerData.activeCharacter.characterData.CharacterLevel)
			{
				this.encounterFlag.color = Color.green;
			}
			else if(encounter.Data.Vitals.CharacterLevel <= PF_PlayerData.activeCharacter.characterData.CharacterLevel + 3)
			{
				this.encounterFlag.color = Color.yellow;
			}
			else
			{
				this.encounterFlag.color = Color.red;	
			}
			this.encounterLevel.text = "" + encounter.Data.Vitals.CharacterLevel;
		}
		else
		{
			this.encounterFlag.color = new Color(188,0,255,255);
			this.encounterLevel.text = string.Empty;
		}
	}
	
}
