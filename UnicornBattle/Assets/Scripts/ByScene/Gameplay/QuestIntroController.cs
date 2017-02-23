using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestIntroController : MonoBehaviour
{
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
    }

    public void UpdateLevedetails()
    {
        if (PF_GamePlay.ActiveQuest == null)
            return;

        levelImage.overrideSprite = PF_GamePlay.ActiveQuest.levelIcon;
        bgImage.overrideSprite = PF_GamePlay.ActiveQuest.levelIcon;
        levelName.text = PF_GamePlay.ActiveQuest.levelName;
        levelDescription.text = PF_GamePlay.ActiveQuest.levelData.Description;
        ParseWinConditions();
    }

    private void ParseWinConditions()
    {
        var conditions = PF_GamePlay.ActiveQuest.levelData.WinConditions;
        List<string> condionsList = new List<string>();

        if (conditions.CompleteAllActs)
            condionsList.Add("Complete All Acts");
        if (conditions.FindCount > 0 && !string.IsNullOrEmpty(conditions.FindTarget))
            condionsList.Add(string.Format("Find {0} {1}(s)", conditions.FindCount, conditions.FindTarget));
        if (conditions.KillCount > 0 && !string.IsNullOrEmpty(conditions.KillTarget))
            condionsList.Add(string.Format("Kill {0} {1}(s)", conditions.KillCount, conditions.KillTarget));
        if (conditions.SurvivalTime > 0)
            condionsList.Add(string.Format("Survive for {0} sec", conditions.SurvivalTime));
        if (conditions.TimeLimit > 0)
            condionsList.Add(string.Format("Finish within {0} sec", conditions.SurvivalTime));

        foreach (var item in WinConditions)
            item.gameObject.SetActive(false);

        for (var z = 0; z < condionsList.Count && z < 4; z++) // can only fit 4 items in the display.
        {
            WinConditions[z].gameObject.SetActive(true);
            var text = WinConditions[z].GetComponentInChildren<Text>();
            text.text = condionsList[z];
        }
    }
}
