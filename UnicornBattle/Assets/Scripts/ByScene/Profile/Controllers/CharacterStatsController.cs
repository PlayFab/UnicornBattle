using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CharacterStatsController : MonoBehaviour {
	
	public StatItem HP_Stat;
	public StatItem MP_Stat;
	public StatItem DP_Stat;
	public StatItem SP_Stat;

	public void Init()
	{
		UB_SavedCharacter active = PF_PlayerData.activeCharacter;
		if(active != null)
		{
			HP_Stat.statValue.text = "" + active.characterData.Health;
			HP_Stat.levelBonus.text = "+" + active.baseClass.HPLevelBonus;
			
			MP_Stat.statValue.text = "" + active.characterData.Mana;
			MP_Stat.levelBonus.text = "+" +  active.baseClass.MPLevelBonus;
			
			DP_Stat.statValue.text = "" + active.characterData.Defense;
			DP_Stat.levelBonus.text = "+" + active.baseClass.DPLevelBonus;
			
			SP_Stat.statValue.text = "" + active.characterData.Speed;
			SP_Stat.levelBonus.text = "+" + active.baseClass.SPLevelBonus;
		}	
		
	}
	
	
}