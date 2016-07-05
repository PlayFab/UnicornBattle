using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ActOutroController : MonoBehaviour {
	public Text ActName;
	public Text ActIntro;
	public Button StartButton;

	public void UpdateFields()
	{
		
		if(PF_GamePlay.ActiveQuest != null)
		{
			if(string.IsNullOrEmpty(PF_GamePlay.QuestProgress.CurrentAct.Key) || PF_GamePlay.QuestProgress.CurrentAct.Value.IsActCompleted == false)
			{
				// FIRST ACT
				//TODO fix the bug here when re-entering scenes
				if(PF_GamePlay.ActiveQuest.levelData.Acts.Count > 0)
				{
					var firstAct = PF_GamePlay.ActiveQuest.levelData.Acts.First();
					ActName.text = firstAct.Key;
					ActIntro.text = firstAct.Value.IntroMonolog;
					PF_GamePlay.QuestProgress.CurrentAct = firstAct;
					PF_GamePlay.QuestProgress.ActIndex = 0;
				}
			}
			else
			{
				// NEXT ACT
				Debug.Log ("NEXT ACT -- TESTING LOGIC: " + (PF_GamePlay.ActiveQuest.levelData.Acts.Count > ++PF_GamePlay.QuestProgress.ActIndex).ToString());
				Debug.Log(PF_GamePlay.QuestProgress.ActIndex.ToString());
				
				if(PF_GamePlay.ActiveQuest.levelData.Acts.Count > ++PF_GamePlay.QuestProgress.ActIndex)
				{
					//TODO finish this code block
				}
			}
		}
	}
	
	public void StartQuest()
	{
		GameplayController.RaiseGameplayEvent("Starting Quest", PF_GamePlay.GameplayEventTypes.StartQuest);
	}
	
}
