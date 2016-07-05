using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ActIntroController : MonoBehaviour {
	public Text ActName;
	public Text ActIntro;
	public Button StartButton;
	public DirectionCanvasController dirCanvasController;
	
	public TweenColor colorFader;
	public CanvasGroup cg;

	public TweenRot titleAnimator;
	public TweenAlpha descriptionAnimator;
	
	public float startTime = 0;
	public float waitTime = 5;
    public float debugSpinTime = float.MaxValue;
	
	public DisplayStates activeState = DisplayStates.ActIntro;
	public enum DisplayStates { ActIntro, ActOutro, BossIntro, PlayerDied }

    void OnEnable()
    {
        InterstitialController.OnInterstitialFinish += OnInterstitialFinish;
        debugSpinTime = Time.time + 60; // Automatically spin (eventually) if OnInterstitialFinish fails
    }

    void OnDisable()
    {
        InterstitialController.OnInterstitialFinish -= OnInterstitialFinish;
        debugSpinTime = float.MaxValue;
    }

	public void OnSkipClicked()
	{
		titleAnimator.PlayReverse();
		if(this.activeState == DisplayStates.BossIntro)
		{
			PF_GamePlay.OutroPane(this.gameObject, .5f, () =>
			{
				
			}); 
		}
		else if(this.activeState == DisplayStates.PlayerDied)
		{
			ContinueToGameOver();
		}
		else
		{
			if(PF_GamePlay.UseRaidMode)
			{
				GameplayController.RaiseGameplayEvent(GlobalStrings.QUEST_COMPLETE_EVENT, PF_GamePlay.GameplayEventTypes.OutroQuest);
			}
			else
			{
				StartQuest();
			}
			PF_GamePlay.OutroPane(this.gameObject, .5f); 
		}
		
	}

    void OnInterstitialFinish()
    {
		this.titleAnimator.ResetToBeginning();
		this.descriptionAnimator.ResetToBeginning();
		
         debugSpinTime = float.MaxValue;
        StartCoroutine(Spin());
    }

	public void ShowInfo(DisplayStates state)
	{
		this.activeState = state;
		
		if(state == DisplayStates.ActIntro)
		{
			UpdateFields_Act();
		}
		else if(state == DisplayStates.ActOutro)
		{
			UpdateFields_Act();
		}
		else if(state == DisplayStates.BossIntro)
		{			
			// hack to make sure that we do not stomp on already existing info slides
			if(this.cg.alpha == 0)
			{
				UpdateFields_Boss();
				
				debugSpinTime = float.MaxValue;
				StartCoroutine(Spin());
				PF_GamePlay.IntroPane(this.gameObject, .75f);
			}
		}	
		else if(state == DisplayStates.PlayerDied)
		{
			UpdateFields_Fail();
			PF_GamePlay.IntroPane(this.gameObject, .75f);
		}
	}
	
	
	public void UpdateFields_Act()
	{
		if(PF_GamePlay.ActiveQuest != null)
		{
			if(this.activeState == DisplayStates.ActIntro)
			{
				if(PF_GamePlay.ActiveQuest.levelData.Acts.Count > 0)
				{
					ActName.text = PF_GamePlay.QuestProgress.CurrentAct.Key;
					ActIntro.text = PF_GamePlay.QuestProgress.CurrentAct.Value.IntroMonolog;
				}
			}
		}
	}
	
	public IEnumerator Spin()
	{
		this.titleAnimator.PlayForward();
		this.descriptionAnimator.PlayForward();
		
		this.startTime = Time.time;
		while(this.startTime + this.waitTime > Time.time)
		{
			
			yield return new WaitForEndOfFrame();
		}
		
		descriptionAnimator.PlayReverse();
		
		
		
		StartCoroutine(PF_GamePlay.Wait(.55f, ()=> 
		{ 
			titleAnimator.PlayReverse();
			PF_GamePlay.OutroPane(this.gameObject, .5f); 
			
			if(this.activeState == DisplayStates.PlayerDied)
			{
				ContinueToGameOver();
			}
			else
			{
				if(PF_GamePlay.UseRaidMode)
				{
					GameplayController.RaiseGameplayEvent(GlobalStrings.QUEST_COMPLETE_EVENT, PF_GamePlay.GameplayEventTypes.OutroQuest);
				}
				else
				{
					StartQuest();
				}
			}
		}));
	}
	
	
	public void UpdateFields_Boss()
	{
		if(PF_GamePlay.ActiveQuest != null)
		{
			ActName.text = PF_GamePlay.encounters.First().DisplayName;
			ActIntro.text = PF_GamePlay.QuestProgress.CurrentAct.Value.IntroBossMonolog;
		}
	}
	
	public void UpdateFields_Fail()
	{
		if(PF_GamePlay.ActiveQuest != null)
		{
			var act = PF_GamePlay.QuestProgress.CurrentAct;
			ActName.text = ActName.text = string.Format("{0} Failed!",  PF_GamePlay.QuestProgress.CurrentAct.Key);
			ActIntro.text = act.Value.FailureMonolog;
		}
	}
	

	public void ContinueToNextAct()
	{
		dirCanvasController.gameplayController.turnController.AdvanceAct();
		
		UnityAction afterActAdvances = () => 
		{ 
			ShowInfo(DisplayStates.ActIntro);

		};
		PF_GamePlay.Wait(1.0f, afterActAdvances);
	}
	
	public void ContinueToQuestComplete()
	{
        GameplayController.RaiseGameplayEvent(GlobalStrings.QUEST_COMPLETE_EVENT, PF_GamePlay.GameplayEventTypes.OutroQuest);
		PF_GamePlay.OutroPane(this.gameObject, .75f);
	}
	
	public void ContinueToGameOver()
	{
        GameplayController.RaiseGameplayEvent(GlobalStrings.PLAYER_DIED_EVENT, PF_GamePlay.GameplayEventTypes.EndQuest);
		PF_GamePlay.OutroPane(this.gameObject, .75f);
	}
	
	public void StartQuest()
	{
		PF_GamePlay.OutroPane(this.gameObject, .75f);
		GameplayController.RaiseGameplayEvent(GlobalStrings.QUEST_START_EVENT, PF_GamePlay.GameplayEventTypes.StartQuest);
	}
}
