using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class QuestIntroController : MonoBehaviour {
	public Image bgImage;
	public Image levelImage;
	public Text greeting;
	public Text levelName;
	public Text levelDescription;
	public Button StartButton;
	
	public Transform WinConditionPrefab;
	public Transform WinConditionsContainer;
	
	public List<Transform> WinConditions;

	public void OnStartButtonPressed()
	{
		//GameplayController.RaiseGameplayEvent("Intro Act", PF_GamePlay.GameplayEventTypes. IntroAct);
	}
	
	public void UpdateLevedetails()
	{
		if(PF_GamePlay.ActiveQuest != null)
		{
			this.levelImage.overrideSprite = PF_GamePlay.ActiveQuest.levelIcon;
			this.bgImage.overrideSprite = PF_GamePlay.ActiveQuest.levelIcon;
			this.levelName.text = PF_GamePlay.ActiveQuest.levelName;
			this.levelDescription.text = PF_GamePlay.ActiveQuest.levelData.Description;
			ParseWinConditions();
		}
	
	}
	
	void ParseWinConditions()
	{
		//PF_GamePlay.ActiveQuest.
		UB_LevelWinConditions conditions = PF_GamePlay.ActiveQuest.levelData.WinConditions;
		List<string> condionsList = new List<string>();
		
		if(conditions.CompleteAllActs)
		{
			condionsList.Add("Complete All Acts");
		}
		
		if(conditions.FindCount > 0 && !string.IsNullOrEmpty(conditions.FindTarget))
		{
			condionsList.Add(string.Format("Find {0} {1}(s)", conditions.FindCount, conditions.FindTarget ));
		}
		
		if(conditions.KillCount > 0 && !string.IsNullOrEmpty(conditions.KillTarget))
		{
			condionsList.Add(string.Format("Kill {0} {1}(s)", conditions.KillCount, conditions.KillTarget));
		}
		
		if(conditions.SurvivalTime > 0)
		{
			condionsList.Add(string.Format("Survive for {0} sec", conditions.SurvivalTime));
		}
		
		if(conditions.TimeLimit > 0)
		{
			condionsList.Add(string.Format("Finish within {0} sec", conditions.SurvivalTime));
		}
		

		foreach(var item in WinConditions)
		{
			item.gameObject.SetActive(false);
		}

		for(int z = 0; z < condionsList.Count; z++) 
		{	
			if(z < 4) // can only fit 4 items in the display.
			{
				WinConditions[z].gameObject.SetActive(true);
				var text = WinConditions[z].GetComponentInChildren<Text>();
				text.text = condionsList[z];
				//TODO add these to PF_GamePlay
			}
		}
		
	}
	
}
