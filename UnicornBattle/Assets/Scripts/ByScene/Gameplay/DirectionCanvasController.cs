using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DirectionCanvasController : MonoBehaviour {
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
    
    void OnGameplayEventReceived(string message,  PF_GamePlay.GameplayEventTypes type )
	{
		//Debug.Log(string.Format("{0} -- {1}",type.ToString(), message));
		
		if(type == PF_GamePlay.GameplayEventTypes.IntroAct)
		{
			ActIntroObject.ShowInfo(ActIntroController.DisplayStates.ActIntro);
			PF_GamePlay.OutroPane(QuestIntroObject.gameObject, .75f);
		}
		
//		if(type == PF_GamePlay.GameplayEventTypes.OutroAct)
//		{
//			//ActIntroObject.ShowInfo(ActIntroController.DisplayStates.ActOutro);
//			//PF_GamePlay.IntroPane(ActIntroObject.gameObject, .75f);
//		}
		
		if(type == PF_GamePlay.GameplayEventTypes.IntroQuest)
		{
			//QuestIntroObject.UpdateLevedetails();
			//PF_GamePlay.IntroPane(QuestIntroObject.gameObject, .75f);
		}
		
		if(type == PF_GamePlay.GameplayEventTypes.StartBossBattle)
		{
			ActIntroObject.ShowInfo(ActIntroController.DisplayStates.BossIntro);
			//PF_GamePlay.IntroPane(ActIntroObject.gameObject, .75f);
		}
		
		if(type == PF_GamePlay.GameplayEventTypes.PlayerDied)
		{
			ActIntroObject.ShowInfo(ActIntroController.DisplayStates.PlayerDied);
			PF_GamePlay.IntroPane(ActIntroObject.gameObject, .75f);
			
			this.QuestCompleteObject.UpdateQuestStats();
			PF_GamePlay.IntroPane(this.QuestCompleteObject.gameObject, .75f);
		}
		
		
		if(type == PF_GamePlay.GameplayEventTypes.OutroQuest)
		{
//			if(PF_GamePlay.UseRaidMode == false)
//			{
//				//ActIntroObject.ShowInfo(ActIntroController.DisplayStates.ActOutro);
//				PF_GamePlay.IntroPane(ActIntroObject.gameObject, .75f);
//			}
			
			this.QuestCompleteObject.UpdateQuestStats();
			PF_GamePlay.IntroPane(this.QuestCompleteObject.gameObject, .75f);
			
		}
	}
}
