using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
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

        public void OnStartButtonPressed() { }

        public void UpdateLevedetails()
        {
            var l_activeQuest = GameController.Instance.ActiveLevel;

            if (l_activeQuest == null)
                return;

            levelImage.overrideSprite = l_activeQuest.levelIcon;
            bgImage.overrideSprite = l_activeQuest.levelIcon;
            levelName.text = l_activeQuest.levelName;
            levelDescription.text = l_activeQuest.levelData.Description;
            ParseWinConditions();
        }

        private void ParseWinConditions()
        {
            var conditions = GameController.Instance.ActiveLevel.levelData.WinConditions;
            List<string> l_conditionsList = new List<string>();

            if (conditions.CompleteAllActs)
                l_conditionsList.Add("Complete All Acts");
            if (conditions.FindCount > 0 && !string.IsNullOrEmpty(conditions.FindTarget))
                l_conditionsList.Add(string.Format("Find {0} {1}(s)", conditions.FindCount, conditions.FindTarget));
            if (conditions.KillCount > 0 && !string.IsNullOrEmpty(conditions.KillTarget))
                l_conditionsList.Add(string.Format("Kill {0} {1}(s)", conditions.KillCount, conditions.KillTarget));
            if (conditions.SurvivalTime > 0)
                l_conditionsList.Add(string.Format("Survive for {0} sec", conditions.SurvivalTime));
            if (conditions.TimeLimit > 0)
                l_conditionsList.Add(string.Format("Finish within {0} sec", conditions.SurvivalTime));

            foreach (var item in WinConditions)
                item.gameObject.SetActive(false);

            for (var z = 0; z < l_conditionsList.Count && z < 4; z++) // can only fit 4 items in the display.
            {
                WinConditions[z].gameObject.SetActive(true);
                var text = WinConditions[z].GetComponentInChildren<Text>();
                text.text = l_conditionsList[z];
            }
        }
    }
}