using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

public class PlayerActionBarController : MonoBehaviour {
	public TweenPos tweener;

	public SpellSlot Spell1Button;
	public SpellSlot Spell2Button;
	public SpellSlot Spell3Button;
	public Button FleeButton;
	public Button UseItemButton;
	
	
	//private PF_GamePlay.PlayerSpellInputs playerSelectedInput;
	
	public PlayerUIEffectsController pUIController;
	
	
	void Awake()
	{	
		Spell1Button.SpellButton.onClick.AddListener(() => { SpellClicked(PF_GamePlay.PlayerSpellInputs.Spell1);	});
		Spell2Button.SpellButton.onClick.AddListener(() => { SpellClicked(PF_GamePlay.PlayerSpellInputs.Spell2);	});
		Spell3Button.SpellButton.onClick.AddListener(() => { SpellClicked(PF_GamePlay.PlayerSpellInputs.Spell3);	});
		FleeButton.onClick.AddListener(() => { SpellClicked(PF_GamePlay.PlayerSpellInputs.Flee);	});
		UseItemButton.onClick.AddListener(() => { SpellClicked(PF_GamePlay.PlayerSpellInputs.UseItem);	});
		
	}

	public void UpdateSpellBar()
	{
		if(PF_PlayerData.activeCharacter != null)
		{
			string spell1 = PF_PlayerData.activeCharacter.baseClass.Spell1;
			this.Spell1Button.AddSpellData(spell1, PF_GameData.Spells.ContainsKey(spell1) ? PF_GameData.Spells[spell1] :null, PF_PlayerData.activeCharacter.characterData.Spell1_Level);
			
			string spell2 = PF_PlayerData.activeCharacter.baseClass.Spell2;
			this.Spell2Button.AddSpellData(spell2, PF_GameData.Spells.ContainsKey(spell2) ? PF_GameData.Spells[spell2] :null, PF_PlayerData.activeCharacter.characterData.Spell2_Level);
			
			string spell3 = PF_PlayerData.activeCharacter.baseClass.Spell3;
			this.Spell3Button.AddSpellData(spell3, PF_GameData.Spells.ContainsKey(spell3) ? PF_GameData.Spells[spell3] :null, PF_PlayerData.activeCharacter.characterData.Spell3_Level);
		}
	}
	
	private void ClearInput()
	{
		//this.playerSelectedInput = PF_GamePlay.PlayerSpellInputs.Null;
	}
	
	public void SpellClicked(PF_GamePlay.PlayerSpellInputs input)
	{
		switch(input)
		{
			case PF_GamePlay.PlayerSpellInputs.Spell1:
				Debug.Log("Spell1");
				if(!Spell1Button.isOnCD && !Spell1Button.isLocked)
				{
					//playerSelectedInput = PF_GamePlay.PlayerSpellInputs.Spell1;
					pUIController.gameplayController.PlayerAttacks(Spell1Button);
				}
			break;
			
			case PF_GamePlay.PlayerSpellInputs.Spell2:
				Debug.Log("Spell2");
				//playerSelectedInput = PF_GamePlay.PlayerSpellInputs.Spell2;
				pUIController.gameplayController.PlayerAttacks(Spell2Button);
			break;
			
			case PF_GamePlay.PlayerSpellInputs.Spell3:
				Debug.Log("Spell3");
				//playerSelectedInput = PF_GamePlay.PlayerSpellInputs.Spell3;
				pUIController.gameplayController.PlayerAttacks(Spell3Button);
			break;
			
			case PF_GamePlay.PlayerSpellInputs.Flee:
				//playerSelectedInput = PF_GamePlay.PlayerSpellInputs.Flee;
				this.pUIController.StartEncounterInput(PF_GamePlay.PlayerEncounterInputs.Evade);
				Debug.Log("Flee");
			break;
			
			case PF_GamePlay.PlayerSpellInputs.UseItem:
				//playerSelectedInput = PF_GamePlay.PlayerSpellInputs.UseItem;
				pUIController.UseItem();
				Debug.Log("UseItem");
			break;
		}
		
	}
	
}



