using UnityEngine;

namespace UnicornBattle.Controllers
{
	public class DirectionCanvasController : MonoBehaviour
	{
		public QuestIntroController QuestIntroObject;
		public QuestOutroController QuestCompleteObject;
		public ActIntroController ActIntroObject;
		public GameplayController gameplayController;

		void OnEnable()
		{
			GameplayController.OnGameplayEvent += OnGameplayEventReceived;
		}

		void OnDisable()
		{
			GameplayController.OnGameplayEvent -= OnGameplayEventReceived;
		}

		void OnGameplayEventReceived(string message, UBGamePlay.GameplayEventTypes type)
		{
			if (type == UBGamePlay.GameplayEventTypes.IntroAct)
			{
				ActIntroObject.ShowInfo(ActIntroController.DisplayStates.ActIntro);
				UBAnimator.OutroPane(QuestIntroObject.gameObject, .75f);
			}

			if (type == UBGamePlay.GameplayEventTypes.IntroQuest)
			{
				//QuestIntroObject.UpdateLevedetails();
				//PF_GamePlay.IntroPane(QuestIntroObject.gameObject, .75f);
			}

			if (type == UBGamePlay.GameplayEventTypes.StartBossBattle)
			{
				ActIntroObject.ShowInfo(ActIntroController.DisplayStates.BossIntro);

			}

			if (type == UBGamePlay.GameplayEventTypes.PlayerDied)
			{
				ActIntroObject.ShowInfo(ActIntroController.DisplayStates.PlayerDied);
				UBAnimator.IntroPane(ActIntroObject.gameObject, .75f);

				this.QuestCompleteObject.UpdateQuestStats();
				UBAnimator.IntroPane(this.QuestCompleteObject.gameObject, .75f);
			}

			if (type == UBGamePlay.GameplayEventTypes.OutroQuest)
			{
				this.QuestCompleteObject.UpdateQuestStats();
				UBAnimator.IntroPane(this.QuestCompleteObject.gameObject, .75f);
			}
		}
	}
}