using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAchievementsController : MonoBehaviour
{
    public List<Image> colorize = new List<Image>();
    public AchievementItem achievementItemPrefab;
    public CharacterPicker cPicker;
    public List<AchievementItem> items;
    public Transform listView;

    public void Init()
    {
        if (cPicker.selectedSlot.saved != null && PF_GameData.Achievements != null && PF_GameData.Achievements.Count > 0)
        {
            while (items.Count < PF_GameData.Achievements.Count)
            {
                var achvItem = Instantiate(achievementItemPrefab);
                achvItem.transform.SetParent(listView, false);
                items.Add(achvItem);
            }

            for (var z = 0; z < items.Count; z++)
            {
                var kvp = PF_GameData.Achievements.ElementAt(z);
                // check here for already awarded

                colorize.Add(items[z].coloredBar);

                items[z].icon.overrideSprite = GameController.Instance.iconManager.GetIconById(kvp.Value.Icon, IconManager.IconTypes.Misc);
                items[z].Name.text = kvp.Value.AchievementName;
                items[z].progressBar.maxValue = kvp.Value.Threshold;

                if (PF_PlayerData.DoesCharacterHaveAchievement(cPicker.selectedSlot.saved.characterDetails.CharacterId, kvp.Key))
                {
                    items[z].CompleteAchievement();
                    StartCoroutine(items[z].progressBar.UpdateBar(kvp.Value.Threshold));
                    items[z].display.text = GlobalStrings.COMPLETE_MSG;
                }
                else
                {
                    items[z].ResetImage();

                    int progress = CalcProgress(kvp.Value);
                    items[z].display.text = string.Format("{0:n0}/{1:n0} {2}", AchievementDisplayNumber(progress), AchievementDisplayNumber(kvp.Value.Threshold), kvp.Value.MatchingStatistic);
                    StartCoroutine(items[z].progressBar.UpdateBar(progress));
                }
            }
        }

        var classType = (PF_PlayerData.PlayerClassTypes)Enum.Parse(typeof(PF_PlayerData.PlayerClassTypes), cPicker.selectedSlot.saved.baseClass.CatalogCode);

        switch ((int)classType)
        {
            case 0:
                foreach (var item in colorize)
                    item.color = PF_GamePlay.ClassColor1;
                break;
            case 1:
                foreach (var item in colorize)
                    item.color = PF_GamePlay.ClassColor2;
                break;
            case 2:
                foreach (var item in colorize)
                    item.color = PF_GamePlay.ClassColor3;
                break;
            default:
                Debug.LogWarning("Unknown Class type detected...");
                break;
        }
    }

    private string AchievementDisplayNumber(int input)
    {
        if (input < 900)
            return input.ToString();
        else if (input < 1000)
            return "<1K";
        else if (input < 900000)
            return (input / 1000).ToString() + "K";
        else if (input < 1000000)
            return "<1M";
        else
            return (input / 1000000).ToString() + "M";
    }

    public int CalcProgress(UB_Achievement achvItem)
    {
        var charId = cPicker.selectedSlot.saved.characterDetails.CharacterId;
        Dictionary<string, int> stats;

        if (PF_PlayerData.characterStatistics != null && PF_PlayerData.characterStatistics.Count > 0
        && PF_GameData.Achievements != null && PF_GameData.Achievements.Count > 0
        && PF_PlayerData.characterAchievements != null)
        {
            PF_PlayerData.characterStatistics.TryGetValue(charId, out stats);
            if (stats == null)
                return 0;

            if (achvItem.SingleStat)

                return stats.ContainsKey(achvItem.MatchingStatistic) ? stats[achvItem.MatchingStatistic] : 0;

            var total = 0;
            foreach (var stat in stats)
                if (stat.Key.Contains(achvItem.MatchingStatistic))
                    total += stat.Value;
            return total;
        }
        return 0;
    }
}
