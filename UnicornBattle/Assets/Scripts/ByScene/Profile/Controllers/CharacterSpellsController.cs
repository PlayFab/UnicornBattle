using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CharacterSpellsController : MonoBehaviour {
	
	public SpellItem Spell1;
	public SpellItem Spell2;
	public SpellItem Spell3;
	
	// Use this for initialization
	void Start () {
		//TweenScale.Tween(this.heartArt.gameObject, .5f, new Vector3(1,1,1), new Vector3(1.1f,1.1f,1.1f), TweenMain.Style.PingPong, TweenMain.Method.EaseIn, null);
		//TweenScale.Tween(this.heartArt.gameObject, .5f, new Vector3(1,1,1), new Vector3(1.1f,1.1f,1.1f), TweenMain.Style.PingPong, TweenMain.Method.EaseIn, null);
		//TweenPos.Tween(gameObject, 1f, vector3.zero);
		//TweenRot.Tween(Spell1.panelBg.gameObject, .75f, Quaternion.Euler(new Vector3(-180,0,0)));
	}

	public void Init()
	{
		
		if(PF_PlayerData.activeCharacter != null)
		{
			// test spell 1
			if(PF_GameData.Spells.ContainsKey(PF_PlayerData.activeCharacter.baseClass.Spell1))
			{
				UB_SpellDetail spell1 = PF_GameData.Spells[PF_PlayerData.activeCharacter.baseClass.Spell1];
				this.Spell1.LoadSpell(PF_PlayerData.activeCharacter.baseClass.Spell1, spell1, PF_PlayerData.activeCharacter.characterData.Spell1_Level);
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
				this.Spell2.LoadSpell(PF_PlayerData.activeCharacter.baseClass.Spell2, spell2, PF_PlayerData.activeCharacter.characterData.Spell2_Level);
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
				this.Spell3.LoadSpell(PF_PlayerData.activeCharacter.baseClass.Spell3, spell3, PF_PlayerData.activeCharacter.characterData.Spell3_Level);
			}
			else
			{
				// something went wrong, could not find the spell
				Debug.Log("something went wrong, could not find spell 3");
			}
			
		}
		
	}
	
}
