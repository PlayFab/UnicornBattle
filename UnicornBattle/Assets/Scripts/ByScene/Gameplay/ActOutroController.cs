using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{

	public class ActOutroController : MonoBehaviour
	{
		public Text ActName;
		public Text ActIntro;
		public Button StartButton;

		public void UpdateFields()
		{
			GameController l_gc = GameController.Instance;

			if (l_gc.ActiveLevel != null)
			{
				if (string.IsNullOrEmpty(l_gc.QuestProgress.CurrentAct.Key) || l_gc.QuestProgress.CurrentAct.Value.IsActCompleted == false)
				{
					// FIRST ACT
					//TODO fix the bug here when re-entering scenes
					if (l_gc.ActiveLevel.levelData.Acts.Count > 0)
					{
						var firstAct = l_gc.ActiveLevel.levelData.Acts.First();
						ActName.text = firstAct.Key;
						ActIntro.text = firstAct.Value.IntroMonolog;
						l_gc.QuestProgress.CurrentAct = firstAct;
						l_gc.QuestProgress.ActIndex = 0;
					}
				}
				else
				{
					// NEXT ACT
					//Debug.Log("NEXT ACT -- TESTING LOGIC: " + (l_gc.ActiveLevel.levelData.Acts.Count > ++l_gc.QuestProgress.ActIndex).ToString());
					//Debug.Log(l_gc.QuestProgress.ActIndex.ToString());

					if (l_gc.ActiveLevel.levelData.Acts.Count > ++l_gc.QuestProgress.ActIndex)
					{
						//TODO finish this code block
					}
				}
			}
		}

		public void StartQuest()
		{
			GameplayController.RaiseGameplayEvent("Starting Quest", UBGamePlay.GameplayEventTypes.StartQuest);
		}
	}
}