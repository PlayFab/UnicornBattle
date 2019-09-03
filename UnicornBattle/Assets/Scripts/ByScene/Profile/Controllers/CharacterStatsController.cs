using UnicornBattle.Models;
using UnityEngine;

namespace UnicornBattle.Controllers
{

	public class CharacterStatsController : MonoBehaviour
	{
		public StatItem HP_Stat;
		public StatItem MP_Stat;
		public StatItem DP_Stat;
		public StatItem SP_Stat;

		public void Init()
		{
			UBSavedCharacter l_activeCharacter = GameController.Instance.ActiveCharacter;
			if (l_activeCharacter != null)
			{
				HP_Stat.statValue.text = "" + l_activeCharacter.characterData.Health;
				HP_Stat.levelBonus.text = "+" + l_activeCharacter.baseClass.HPLevelBonus;

				MP_Stat.statValue.text = "" + l_activeCharacter.characterData.Mana;
				MP_Stat.levelBonus.text = "+" + l_activeCharacter.baseClass.MPLevelBonus;

				DP_Stat.statValue.text = "" + l_activeCharacter.characterData.Defense;
				DP_Stat.levelBonus.text = "+" + l_activeCharacter.baseClass.DPLevelBonus;

				SP_Stat.statValue.text = "" + l_activeCharacter.characterData.Speed;
				SP_Stat.levelBonus.text = "+" + l_activeCharacter.baseClass.SPLevelBonus;
			}
		}
	}
}